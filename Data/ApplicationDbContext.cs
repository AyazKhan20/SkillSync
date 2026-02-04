using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SmartJobRecommender.Models; 

namespace SmartJobRecommender.Data
{
    
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        // ------------------------------------------

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<JobSkill>()
                .HasKey(js => new { js.JobId, js.SkillId });

            
            builder.Entity<JobSkill>()
                .HasOne(js => js.Job) // JobSkill has one Job
                .WithMany(j => j.JobSkills) // Job has many JobSkills
                .HasForeignKey(js => js.JobId); // Foreign key is JobId

            
            builder.Entity<JobSkill>()
                .HasOne(js => js.Skill) // JobSkill has one Skill
                .WithMany(s => s.JobSkills) // Skill has many JobSkills
                .HasForeignKey(js => js.SkillId); // Foreign key is SkillId


           
            // (You would repeat the same process here for the UserSkill join table)
            builder.Entity<UserSkill>()
                .HasKey(us => new { us.UserId, us.SkillId });

            
        }
    }
}