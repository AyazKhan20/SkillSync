using Microsoft.AspNetCore.Identity;

namespace SmartJobRecommender.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        
        public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
        public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }

    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public virtual ICollection<JobSkill> RequiredSkills { get; set; } = new List<JobSkill>();
    }

    public class UserSkill
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int SkillId { get; set; }
        public int Level { get; set; } 
        public virtual Skill? Skill { get; set; }
        public virtual IdentityUser? User { get; set; }
    }

    public class JobSkill
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int SkillId { get; set; }
        public virtual Job? Job { get; set; }
        public virtual Skill? Skill { get; set; }
    }
}
