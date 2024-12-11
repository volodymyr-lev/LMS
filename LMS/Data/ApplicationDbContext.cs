using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LMS.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupCourse> GroupCourses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Thesis> Theses { get; set; }
        public DbSet<CourseWork> CourseWorks { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<RuleParameter> RuleParameters { get; set; }
        public DbSet<ThesisVerification> ThesisVerifications { get; set; }
        public DbSet<Violation> Violations { get; set; }
        public DbSet<RuleRuleParameter> RuleRuleParameters { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Course Configuration
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Lecturer)
                .WithMany()
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment Configuration
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany()
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Rule)
                .WithMany()
                .HasForeignKey(a => a.RuleId)
                .OnDelete(DeleteBehavior.Restrict);

            // New Assignment-CourseWork Configuration
            modelBuilder.Entity<CourseWork>()
                .HasOne(cw => cw.Assignment)
                .WithMany(a => a.CourseWorks)
                .HasForeignKey(cw => cw.AssignmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // New Assignment-Thesis Configuration
            modelBuilder.Entity<Thesis>()
                .HasOne(t => t.Assignment)
                .WithMany(a => a.Theses)
                .HasForeignKey(t => t.AssignmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Group-Course Configuration (Many-to-Many)
            modelBuilder.Entity<GroupCourse>()
                .HasKey(gc => new { gc.GroupId, gc.CourseId });

            modelBuilder.Entity<GroupCourse>()
                .HasOne(gc => gc.Group)
                .WithMany(g => g.GroupCourses)
                .HasForeignKey(gc => gc.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupCourse>()
                .HasOne(gc => gc.Course)
                .WithMany(c => c.GroupCourses)
                .HasForeignKey(gc => gc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Group-Student Configuration (One-to-Many)
            modelBuilder.Entity<Group>()
                .HasMany(g => g.Students)
                .WithOne()
                .HasForeignKey("GroupId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Enrollment Configuration
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

            // CourseWork Configuration
            modelBuilder.Entity<CourseWork>()
                .HasOne(cw => cw.Student)
                .WithMany()
                .HasForeignKey(cw => cw.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseWork>()
                .HasOne(cw => cw.Advisor)
                .WithMany()
                .HasForeignKey(cw => cw.AdvisorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseWork>()
                .HasMany(cw => cw.Rules)
                .WithMany()
                .UsingEntity(j => j.ToTable("CourseWorkRules"));

            // Thesis Configuration
            modelBuilder.Entity<Thesis>()
                .HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Thesis>()
                .HasMany(t => t.Rules)
                .WithMany()
                .UsingEntity(j => j.ToTable("ThesisRules"));

            modelBuilder.Entity<Thesis>()
                .HasOne(t => t.Mentor)
                .WithMany()
                .HasForeignKey(t => t.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ThesisVerification Configuration
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

            // Rule and RuleParameter Configuration
            modelBuilder.Entity<RuleRuleParameter>()
                .HasKey(rp => new { rp.RuleId, rp.RuleParameterId });

            modelBuilder.Entity<RuleRuleParameter>()
                .HasOne(rp => rp.Rule)
                .WithMany(r => r.RuleParameters)
                .HasForeignKey(rp => rp.RuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RuleRuleParameter>()
                .HasOne(rp => rp.RuleParameter)
                .WithMany(rp => rp.Rules)
                .HasForeignKey(rp => rp.RuleParameterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure table names explicitly
            modelBuilder.Entity<Course>().ToTable("Courses");
            modelBuilder.Entity<Group>().ToTable("Groups");
            modelBuilder.Entity<GroupCourse>().ToTable("GroupCourses");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollments");
            modelBuilder.Entity<Thesis>().ToTable("Theses");
            modelBuilder.Entity<CourseWork>().ToTable("CourseWorks");
            modelBuilder.Entity<Rule>().ToTable("Rules");
            modelBuilder.Entity<RuleParameter>().ToTable("RuleParameters");
            modelBuilder.Entity<ThesisVerification>().ToTable("ThesisVerifications");
            modelBuilder.Entity<Violation>().ToTable("Violations");
            modelBuilder.Entity<Assignment>().ToTable("Assignments");

            // Update indexes
            modelBuilder.Entity<Assignment>()
                .HasIndex(a => a.CourseId);

            modelBuilder.Entity<Assignment>()
                .HasIndex(a => a.RuleId);

            // New indexes for Assignment relationship with CourseWork and Thesis
            modelBuilder.Entity<CourseWork>()
                .HasIndex(cw => cw.AssignmentId);

            modelBuilder.Entity<Thesis>()
                .HasIndex(t => t.AssignmentId);

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.LecturerId);

            modelBuilder.Entity<CourseWork>()
                .HasIndex(cw => cw.StudentId);

            modelBuilder.Entity<Thesis>()
                .HasIndex(t => t.StudentId);

            modelBuilder.Entity<ThesisVerification>()
                .HasIndex(tv => tv.ThesisId);

            modelBuilder.Entity<Violation>()
                .HasIndex(v => v.ThesisVerificationId);

            // Configure cascade delete behavior
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}