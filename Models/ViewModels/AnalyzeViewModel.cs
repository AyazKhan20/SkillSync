using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SmartJobRecommender.Models.ViewModels
{
    public class AnalyzeViewModel
    {
        // Property for the file upload (input)
        [Required(ErrorMessage = "Please upload your resume.")]
        [Display(Name = "Upload Resume (PDF only)")]
        public IFormFile ResumeFile { get; set; }

        // Property for the dream job title (input)
        [Required(ErrorMessage = "Please enter your dream job title.")]
        [Display(Name = "Dream Job Title")]
        public string DreamJobTitle { get; set; }

        // Properties for the results (output)
        public bool AnalysisCompleted { get; set; } = false;
        public List<string> ExtractedSkills { get; set; } = new List<string>();
        public List<RecommendedJobViewModel> RecommendedJobs { get; set; } = new List<RecommendedJobViewModel>();

        // Output for the Dream Job Gap Analysis (AI response content)
        public string DreamJobAnalysisResult { get; set; }
    }
}