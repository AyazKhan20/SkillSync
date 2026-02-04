using Microsoft.AspNetCore.Identity;

namespace SmartJobRecommender.Models
{
    // Join table for the many-to-many relationship between IdentityUser and Skill
    public class UserSkill
    {
        // The Identity system uses string IDs for users
        public string UserId { get; set; }
        public int SkillId { get; set; }

        // Navigation properties
        public IdentityUser User { get; set; }
        public Skill Skill { get; set; }
    }
}