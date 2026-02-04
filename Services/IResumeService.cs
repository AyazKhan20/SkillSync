using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic; // Added previously for List<string>
using SmartJobRecommender.Models.ViewModels; // <-- FIX: Necessary to find RecommendedJobViewModel

namespace SmartJobRecommender.Services
{
    // Interface defining the operations for resume processing and skill analysis
    public interface IResumeService
    {
        // 1. Core function: Extracts skills from the uploaded file's content
        Task<List<string>> ExtractSkillsFromResume(IFormFile resumeFile);

        // 2. Saves the extracted skills to the user's profile in the database
        Task SaveUserSkills(string userId, List<string> extractedSkills);

        // 3. Gets a list of jobs recommended for the user based on their saved skills
        Task<List<RecommendedJobViewModel>> GetJobRecommendations(string userId);
    }
}