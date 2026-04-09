using System.Collections.Generic;

namespace SmartJobRecommender.Models.ViewModels
{
    
    public class RecommendedJobViewModel
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int MatchScore { get; set; } 
        public List<string> RequiredSkills { get; set; } = new List<string>();
        public List<string> MissingSkills { get; set; } = new List<string>();
    }
}