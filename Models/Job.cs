using System.ComponentModel.DataAnnotations;

namespace SmartJobRecommender.Models
{
    // Represents a job posting
    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Company { get; set; }

        [Required]
        // Full job description, will be used for skill matching
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(50)]
        public string JobType { get; set; } // e.g., Full-time, Internship

        // Navigation property to link this job to its required skills
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}