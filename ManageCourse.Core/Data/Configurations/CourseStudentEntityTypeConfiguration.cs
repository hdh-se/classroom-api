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
            builder.Property("Id").ValueGeneratedOnAdd();
        }
    }
}
