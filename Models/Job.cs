using System.ComponentModel.DataAnnotations;

namespace SmartJobRecommender.Models
{
    
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
    
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(50)]
        public string JobType { get; set; } 

        
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}