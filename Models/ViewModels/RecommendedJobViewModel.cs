using System.Collections.Generic;

namespace SmartJobRecommender.Models.ViewModels
{
    // ViewModel used to display personalized job match data to the user
    public class RecommendedJobViewModel
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public int MatchScore { get; set; } // Percentage match
        public List<string> RequiredSkills { get; set; } = new List<string>();
        public List<string> MissingSkills { get; set; } = new List<string>();
    }
}