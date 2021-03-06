using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data.Configurations
{
    public class CourseStudentEntityTypeConfiguration : IEntityTypeConfiguration<Course_Student>
    {
        public void Configure(EntityTypeBuilder<Course_Student> builder)
        {
            builder.ToTable("CourseStudent");
            //Set PrimaryKey
            builder.Property("Id").ValueGeneratedOnAdd();
            builder.HasOne(x => x.Student).WithMany(y => y.Course_Students).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Course).WithMany(y => y.Course_Students).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
