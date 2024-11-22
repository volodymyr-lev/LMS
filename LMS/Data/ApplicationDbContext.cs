using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LMS.Models;

namespace LMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Thesis> Theses { get; set; }
        public DbSet<CourseWork> CourseWorks { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<RuleParameter> RuleParameters { get; set; }
        public DbSet<ThesisVerification> ThesisVerifications { get; set; }
        public DbSet<Violation> Violations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Course конфігурація
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Lecturer)
                .WithMany()
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Groups)
                .WithOne(g => g.Course)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Group конфігурація
            modelBuilder.Entity<Group>()
                .HasMany(g => g.Students)
                .WithMany(s => s.Groups)
                .UsingEntity<Enrollment>(
                    j => j
                        .HasOne(e => e.Student)
                        .WithMany()
                        .HasForeignKey(e => e.StudentId)
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j
                        .HasOne(e => e.Group)
                        .WithMany()
                        .HasForeignKey(e => e.GroupId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.ToTable("GroupStudents");
                        j.HasKey(gs => new { gs.GroupId, gs.StudentId });
                    });

            // Enrollment конфігурація
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // CourseWork конфігурація
            modelBuilder.Entity<CourseWork>()
                .HasOne(cw => cw.Student)
                .WithMany()
                .HasForeignKey(cw => cw.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thesis конфігурація
            modelBuilder.Entity<Thesis>()
                .HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ThesisVerification конфігурація
            modelBuilder.Entity<ThesisVerification>()
                .HasOne(tv => tv.Thesis)
                .WithMany()
                .HasForeignKey(tv => tv.ThesisId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThesisVerification>()
                .HasMany(tv => tv.Violations)
                .WithOne(v => v.ThesisVerification)
                .HasForeignKey(v => v.ThesisVerificationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rule і RuleParameter конфігурація
            modelBuilder.Entity<Rule>()
                .HasMany(r => r.Parameters)
                .WithOne(rp => rp.Rule)
                .HasForeignKey(rp => rp.RuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
