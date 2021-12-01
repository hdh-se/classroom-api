using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data.Configurations
{
    public class AssignmentsStudentEntityTypeConfiguration : IEntityTypeConfiguration<Assignments_Student>
    {
        public void Configure(EntityTypeBuilder<Assignments_Student> builder)
        {
            builder.ToTable("Assignments_Student");
            builder.Property("Id").ValueGeneratedOnAdd();
            builder.HasOne(x => x.Assignments).WithMany(y => y.Assignments_Students).HasForeignKey(x => x.AssignmentId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Student).WithMany(y => y.Assignments_Students).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
