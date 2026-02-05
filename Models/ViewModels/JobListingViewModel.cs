using System.Collections.Generic;

namespace SmartJobRecommender.Models.ViewModels
{
   
    public class JobListingViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string JobType { get; set; }
        public string Description { get; set; }
        public List<string> RequiredSkills { get; set; } = new List<string>();
    }
}