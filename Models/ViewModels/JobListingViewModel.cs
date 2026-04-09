using System.Collections.Generic;

namespace SmartJobRecommender.Models.ViewModels
{
   
    public class JobListingViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> RequiredSkills { get; set; } = new List<string>();
    }
}