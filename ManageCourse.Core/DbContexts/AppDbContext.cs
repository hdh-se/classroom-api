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
        public DbSet<Assignments> Assignments { get; set; }
        public DbSet<Assignments_Student> Assignments_Students { get; set; }

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
        }
    }
}
