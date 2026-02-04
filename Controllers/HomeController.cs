using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartJobRecommender.Models;
using SmartJobRecommender.Services;
using Microsoft.AspNetCore.Identity;
using SmartJobRecommender.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmartJobRecommender.Data;

namespace SmartJobRecommender.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IResumeService _resumeService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        // Constructor now includes all necessary dependencies for Identity, Services, and Data.
        public HomeController(ILogger<HomeController> logger,
                              IResumeService resumeService,
                              UserManager<IdentityUser> userManager,
                              ApplicationDbContext context)
        {
            _logger = logger;
            _resumeService = resumeService;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // --- Analyze (GET): Shows the resume upload form ---
        [Authorize]
        public IActionResult Analyze()
        {
            return View(new AnalyzeViewModel());
        }

        // --- Analyze (POST): Handles form submission, file upload, and initial analysis ---
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Analyze(AnalyzeViewModel viewModel)
        {
            // Input Validation: Check for file existence first
            if (viewModel.ResumeFile == null || viewModel.ResumeFile.Length == 0)
            {
                ModelState.AddModelError("ResumeFile", "Please upload a resume file.");
                return View(viewModel);
            }

            // Input Validation: Check for other required fields (Dream Job Title)
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Get the current logged-in user's ID
            var userId = _userManager.GetUserId(User);

            try
            {
                // 1. Extract Skills (Service handles the file reading/AI simulation)
                var extractedSkills = await _resumeService.ExtractSkillsFromResume(viewModel.ResumeFile);

                // 2. Save skills to the database (links skills to the user)
                await _resumeService.SaveUserSkills(userId, extractedSkills);

                // 3. Generate Job Recommendations (Used to fetch current user skills for gap analysis)
                var recommendations = await _resumeService.GetJobRecommendations(userId);

                // 4. Populate ViewModel for the results view
                viewModel.ExtractedSkills = extractedSkills;
                viewModel.RecommendedJobs = recommendations;
                viewModel.AnalysisCompleted = true; // Flag to show results section

                // Return the view with the populated ViewModel to display results
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resume analysis for user {UserId}", userId);
                ModelState.AddModelError("", "An unexpected error occurred during analysis. Please try again.");
                return View(viewModel);
            }
        }

        // --- Dashboard Action: Fetches and displays personalized user data ---
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var userName = User.Identity.Name;

            var viewModel = new DashboardViewModel
            {
                UserName = userName
            };

            try
            {
                // 1. Get Job Recommendations (This fetches and calculates matches based on skills)
                var recommendations = await _resumeService.GetJobRecommendations(userId);
                viewModel.TopRecommendations = recommendations.Take(3).ToList(); // Show top 3 on dashboard

                // 2. Extract User Skills
                var userSkills = await _context.UserSkills
                    .Where(us => us.UserId == userId)
                    .Include(us => us.Skill)
                    .Select(us => us.Skill.Name)
                    .ToListAsync();

                viewModel.UserSkills = userSkills;
                viewModel.TotalSkills = userSkills.Count;

                
                var allRequiredSkills = viewModel.TopRecommendations.SelectMany(r => r.RequiredSkills).Distinct().ToList();

               
                var missingSkills = allRequiredSkills
                    .Except(userSkills, StringComparer.OrdinalIgnoreCase)
                    .OrderByDescending(s =>
                        
                        viewModel.TopRecommendations.Count(r => r.MissingSkills.Contains(s))
                    )
                    .Take(5)
                    .ToList();

                viewModel.CriticalMissingSkills = missingSkills;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data for user {UserId}", userId);
               
            }

            return View(viewModel);
        }

       
        [Authorize]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GenerateDreamJobAnalysis([FromBody] DreamJobAnalysisRequest request)
        {
            if (string.IsNullOrEmpty(request.DreamJobTitle))
            {
                return BadRequest(new { success = false, message = "Dream job title is required." });
            }

            var userId = _userManager.GetUserId(User);

            var recommendations = await _resumeService.GetJobRecommendations(userId);
            var userSkills = recommendations.FirstOrDefault()?.RequiredSkills.Select(s => s.ToLower()).ToList()
                             ?? new List<string>();

            if (userSkills.Any())
            {
                try
                {
                    // Mocking AI response logic
                    var mockAnalysis = await Task.Run(() =>
                    {
                        // Mocking a different response based on a keyword for demonstration
                        if (request.DreamJobTitle.ToLower().Contains("manager"))
                        {
                            return $@"
### Missing Skills & Rationale
1. **Budgeting & Financial Planning:** Crucial for managing project resources and stakeholders.
2. **Advanced Conflict Resolution:** Necessary for leading diverse teams and handling client disputes.
3. **Product Roadmap Software (Aha!, Jira):** Industry standard tools for planning and tracking product development.

### Recommended Courses to Fill the Gap
* **Budgeting & Planning:** **[Financial Analysis and Decision Making]** on Coursera
* **Conflict Resolution:** **[Leading Teams: Conflict Resolution]** on LinkedIn Learning
* **Product Roadmap:** **[Jira Roadmapping Essentials]** on Atlassian University
";
                        }
                        // Default mock response
                        return $@"
### Missing Skills & Rationale
1. **Cloud Certification (AWS/Azure):** Essential for virtually all modern technical roles.
2. **Advanced Algorithms & Data Structures:** Required for technical interviews (LeetCode style).
3. **CI/CD Tools (Jenkins/GitHub Actions)::** Needed to deploy and maintain code bases efficiently.

### Recommended Courses to Fill the Gap
* **Cloud Certification:** **[AWS Certified Cloud Practitioner Essentials]** on AWS Skill Builder
* **Advanced Algorithms:** **[Data Structures and Algorithms Masterclass]** on Udemy
* **CI/CD Tools:** **[DevOps Fundamentals: CI/CD with Jenkins]** on Pluralsight
";
                    });

                    return Json(new { success = true, result = mockAnalysis });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Gemini AI call failed for Dream Job Analysis.");
                    return StatusCode(500, new { success = false, message = "AI analysis service is temporarily unavailable." });
                }
            }

            return BadRequest(new { success = false, message = "Please upload and analyze your resume first to establish your skill profile." });
        }

        
        public class DreamJobAnalysisRequest
        {
            public string DreamJobTitle { get; set; }
        }


        public IActionResult Jobs()
        {
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}