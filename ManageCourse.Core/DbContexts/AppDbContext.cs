using ManageCourse.Core.Data;
using ManageCourse.Core.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ManageCourse.Core.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Course_User> Course_Users { get; set; }
        public DbSet<Course_Student> Course_Students { get; set; }
        public DbSet<Assignments> Assignments { get; set; }
        public DbSet<Assignments_Student> Assignments_Students { get; set; }
        public DbSet<GradeReview> GradeReviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StudentNotification> StudentNotifications { get; set; }
        public DbSet<TeacherNotification> TeacherNotifications { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SubjectEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GradeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourseEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourseUserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentsEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AssignmentsStudentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourseStudentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GradeReviewEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewCommentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StudentNotificationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherNotificationEntityTypeConfiguration());
        }
    }
}
