using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SmartJobRecommender.Models.ViewModels
{
    // Inner class to represent a single skill in a checklist item used by the view.
    public class SkillListItem
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public bool IsSelected { get; set; }
    }

    // Main ViewModel for the Create/Edit Job page.
    // This class is used to transfer data between the AdminController and CreateJob.cshtml.
    public class JobViewModel
    {
        // 1. Direct Form Properties
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Job Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Company { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(50)]
        [Display(Name = "Job Type")]
        public string JobType { get; set; }

        // 2. Collection Property (The bridge to the checklist UI)
        public List<SkillListItem> AllSkills { get; set; } = new List<SkillListItem>();
    }
}