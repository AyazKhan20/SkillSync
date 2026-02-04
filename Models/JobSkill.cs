using System.ComponentModel.DataAnnotations.Schema;

namespace SmartJobRecommender.Models
{
    // Join table for the many-to-many relationship between Job and Skill
    public class JobSkill
    {
        // Foreign Keys forming the composite primary key
        public int JobId { get; set; }
        public int SkillId { get; set; }

        // Navigation properties
        // These link back to the main entities
        public Job Job { get; set; }
        public Skill Skill { get; set; }
    }
}