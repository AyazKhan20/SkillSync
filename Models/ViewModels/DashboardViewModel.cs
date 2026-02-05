using System;
using System.Collections.Generic;

namespace SmartJobRecommender.Models.ViewModels
{

    public class DashboardViewModel
    {
        public string UserName { get; set; } 
        public int TotalSkills { get; set; }
        public DateTime? LastAnalyzed { get; set; } 

        
        public List<string> UserSkills { get; set; } = new List<string>();

        
        public List<RecommendedJobViewModel> TopRecommendations { get; set; } = new List<RecommendedJobViewModel>();

        // List of the most critical missing skills identified across top jobs
        public List<string> CriticalMissingSkills { get; set; } = new List<string>();
    }
}