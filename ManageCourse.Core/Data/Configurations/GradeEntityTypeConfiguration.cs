using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data.Configurations
{
    public class GradeEntityTypeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.ToTable("Grade");
            builder.Property("Id").ValueGeneratedOnAdd();
            builder.HasOne(x => x.Student).WithMany(y => y.Grades).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Assignments).WithMany(y => y.Grades).HasForeignKey(x => x.AssignmentId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
