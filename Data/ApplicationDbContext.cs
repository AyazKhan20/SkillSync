using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SmartJobRecommender.Models; // Import custom models

namespace SmartJobRecommender.Data
{
    // Inherits from IdentityDbContext<IdentityUser>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- 1. Define DbSets for the new tables ---
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        // ------------------------------------------

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- 2. Configure the composite primary key for the JobSkill join table ---
            builder.Entity<JobSkill>()
                .HasKey(js => new { js.JobId, js.SkillId });

            // --- 3. Define the relationships for JobSkill ---

            // Configure the relationship from JobSkill back to Job
            builder.Entity<JobSkill>()
                .HasOne(js => js.Job) // JobSkill has one Job
                .WithMany(j => j.JobSkills) // Job has many JobSkills
                .HasForeignKey(js => js.JobId); // Foreign key is JobId

            // Configure the relationship from JobSkill back to Skill
            builder.Entity<JobSkill>()
                .HasOne(js => js.Skill) // JobSkill has one Skill
                .WithMany(s => s.JobSkills) // Skill has many JobSkills
                .HasForeignKey(js => js.SkillId); // Foreign key is SkillId


            // --- 4. Configure the composite primary key and relationships for UserSkill ---
            // (You would repeat the same process here for the UserSkill join table)
            builder.Entity<UserSkill>()
                .HasKey(us => new { us.UserId, us.SkillId });

            // ... (Other UserSkill configuration for relationships)
        }
    }
}