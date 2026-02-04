using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartJobRecommender.Data;
using SmartJobRecommender.Models;
using SmartJobRecommender.Models.ViewModels; 
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SmartJobRecommender.Services
{
    public class ResumeService : IResumeService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ResumeService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- 1. Resume Parsing and AI Skill Extraction ---
        // Simulates the reading of a file and sending the text to an AI for extraction.
        public async Task<List<string>> ExtractSkillsFromResume(IFormFile resumeFile)
        {
            // NOTE: In the real world, you would use a library here (e.g., DocX, iTextSharp) 
            // to extract plain text from the 'resumeFile' stream.

            // --- MOCK TEXT FOR DEMO ---
            string resumeText = @"
                Jane Doe - Junior Data Analyst
                Education: M.S. Data Science, 2024. 

                Experience: Internship at FinTech Insights (2023) - Built ETL pipelines.

                Skills:
                Programming: Python, R, SQL
                Tools: Pandas, NumPy, Scikit-learn, Tableau, Power BI
                Concepts: Statistical Modeling, Machine Learning, Data Visualization, Agile
            ";

            // --- SIMULATE AI RESPONSE ---
            // This is what the Gemini API would return after processing the text:
            List<string> simulatedSkills = new List<string>
            {
                "Python", "R", "SQL", "Pandas", "NumPy", "Scikit-learn", "Tableau",
                "Power BI", "Statistical Modeling", "Machine Learning", "Data Visualization",
                "Agile"
            };

            return simulatedSkills;
        }

        // --- 2. Saving Extracted Skills to the User's Database Profile ---
        public async Task SaveUserSkills(string userId, List<string> extractedSkills)
        {
            // 1. Normalize and clean the extracted skills
            var normalizedSkills = extractedSkills.Select(s => s.ToLower().Trim()).ToList();

            // 2. Ensure all extracted skills exist in the Skills master table.
            foreach (var skillName in normalizedSkills)
            {
                var existingSkill = await _context.Skills.FirstOrDefaultAsync(s => s.Name.ToLower() == skillName);
                if (existingSkill == null)
                {
                    // If the skill is new, add it to the master Skills table
                    _context.Skills.Add(new Skill { Name = skillName });
                }
            }
            await _context.SaveChangesAsync(); // Save new skills to prevent FK errors

            // 3. Get the IDs of the skills we are interested in
            var masterSkills = await _context.Skills
                .Where(s => normalizedSkills.Contains(s.Name.ToLower()))
                .ToListAsync();

            // 4. Clear any existing UserSkills for this user
            var existingUserSkills = _context.UserSkills.Where(us => us.UserId == userId);
            _context.UserSkills.RemoveRange(existingUserSkills);

            // 5. Add new links (UserSkill records)
            foreach (var skill in masterSkills)
            {
                _context.UserSkills.Add(new UserSkill { UserId = userId, SkillId = skill.Id });
            }

            await _context.SaveChangesAsync();
        }

        // --- 3. Job Recommendation Logic ---
        public async Task<List<RecommendedJobViewModel>> GetJobRecommendations(string userId)
        {
            // 1. Get the user's saved skills (from the UserSkills join table)
            var userSkillIds = await _context.UserSkills
                .Where(us => us.UserId == userId)
                .Select(us => us.SkillId)
                .ToListAsync();

            if (userSkillIds == null || userSkillIds.Count == 0)
            {
                return new List<RecommendedJobViewModel>(); // No skills, no recommendations
            }

            // 2. Fetch all jobs and their required skills
            var jobsWithSkills = await _context.Jobs
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .ToListAsync();

            var recommendations = new List<RecommendedJobViewModel>();

            // 3. Calculate Match Score for each job
            foreach (var job in jobsWithSkills)
            {
                var requiredSkillIds = job.JobSkills.Select(js => js.SkillId).ToList();
                int totalRequired = requiredSkillIds.Count;

                if (totalRequired == 0) continue; 

                
                int matchedCount = requiredSkillIds.Intersect(userSkillIds).Count();

                double matchPercentage = (double)matchedCount / totalRequired * 100;

               
                recommendations.Add(new RecommendedJobViewModel
                {
                    JobId = job.Id,
                    Title = job.Title,
                    Company = job.Company,
                    Location = job.Location,
                    MatchScore = (int)Math.Round(matchPercentage),
                    RequiredSkills = job.JobSkills.Select(js => js.Skill.Name).ToList(),
                    MissingSkills = job.JobSkills
                        .Where(js => !userSkillIds.Contains(js.SkillId)) // Skill ID is NOT in the user's list
                        .Select(js => js.Skill.Name)
                        .ToList()
                });
            }

            // 4. Sort and return top recommendations (e.g., top 10, descending score)
            return recommendations.OrderByDescending(r => r.MatchScore).Take(10).ToList();
        }
    }
}