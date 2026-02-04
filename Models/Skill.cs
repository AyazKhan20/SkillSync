using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartJobRecommender.Models
{
    // Represents a unique skill (e.g., "Python", "React", "AWS")
    public class Skill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        // Skill name, unique to prevent duplication
        public string Name { get; set; }

        // Navigation property for jobs that require this skill
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

        // Navigation property for users who possess this skill
        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }
}