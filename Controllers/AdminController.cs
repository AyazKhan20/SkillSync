using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartJobRecommender.Data;
using SmartJobRecommender.Models;
using SmartJobRecommender.Models.ViewModels; // We will define this ViewModel below

namespace SmartJobRecommender.Controllers
{
    // [Authorize] will enforce that only logged-in users can access this controller.
    // In a production app, you would add Role-based authorization: [Authorize(Roles = "Admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. Admin Home / Dashboard ---
        public IActionResult Index()
        {
            return View();
        }

        // --- 2. Job Management (List All Jobs) ---
        public async Task<IActionResult> ManageJobs()
        {
            // Fetch all jobs, including their linked skills
            var jobs = await _context.Jobs
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .ToListAsync();

            return View(jobs);
        }

        // --- 3. Create New Job (GET) ---
        public async Task<IActionResult> CreateJob()
        {
            // We need to pass all available skills to the view for the checklist/selection box
            var viewModel = new JobViewModel
            {
                AllSkills = await _context.Skills.Select(s => new SkillListItem
                {
                    SkillId = s.Id,
                    SkillName = s.Name,
                    IsSelected = false
                }).ToListAsync()
            };
            return View(viewModel);
        }

        // --- 4. Create New Job (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJob(JobViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var job = new Job
                {
                    Title = viewModel.Title,
                    Company = viewModel.Company,
                    Description = viewModel.Description,
                    Location = viewModel.Location,
                    JobType = viewModel.JobType
                };

                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();

                // Link selected skills to the new job
                foreach (var selectedSkill in viewModel.AllSkills.Where(s => s.IsSelected))
                {
                    job.JobSkills.Add(new JobSkill { JobId = job.Id, SkillId = selectedSkill.SkillId });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageJobs));
            }

            // If model state is invalid, re-fetch skills and return the view
            viewModel.AllSkills = await _context.Skills.Select(s => new SkillListItem
            {
                SkillId = s.Id,
                SkillName = s.Name,
                IsSelected = false
            }).ToListAsync();

            return View(viewModel);
        }

        // --- 5. Skill Management (List and Create Skills) ---
        public async Task<IActionResult> ManageSkills()
        {
            var skills = await _context.Skills.OrderBy(s => s.Name).ToListAsync();
            return View(skills);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSkill(Skill skill)
        {
            if (ModelState.IsValid)
            {
                // Simple logic: check if skill exists (basic duplicate prevention)
                var existingSkill = await _context.Skills.FirstOrDefaultAsync(s => s.Name.ToLower() == skill.Name.ToLower());

                if (existingSkill == null)
                {
                    _context.Skills.Add(skill);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(ManageSkills));
            }

            // If validation failed, redirect back to list
            return RedirectToAction(nameof(ManageSkills));
        }

        // Note: You would add Edit and Delete actions later for full CRUD functionality
    }
}