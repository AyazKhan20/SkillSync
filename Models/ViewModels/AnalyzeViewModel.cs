using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SmartJobRecommender.Models.ViewModels
{
    public class AnalyzeViewModel
    {
       
        [Required(ErrorMessage = "Please upload your resume.")]
        [Display(Name = "Upload Resume (PDF only)")]
        public IFormFile ResumeFile { get; set; } = default!;

       
        [Required(ErrorMessage = "Please enter your dream job title.")]
        [Display(Name = "Dream Job Title")]
        public string DreamJobTitle { get; set; } = string.Empty;

      
        public bool AnalysisCompleted { get; set; } = false;
        public List<string> ExtractedSkills { get; set; } = new List<string>();
        public List<RecommendedJobViewModel> RecommendedJobs { get; set; } = new List<RecommendedJobViewModel>();

        
        public string DreamJobAnalysisResult { get; set; } = string.Empty;
    }
}