using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data.Configurations
{
    public class AssignmentsEntityTypeConfiguration : IEntityTypeConfiguration<Assignments>
    {
        public void Configure(EntityTypeBuilder<Assignments> builder)
        {
            builder.ToTable("Assignments");
            builder.Property("Id").ValueGeneratedOnAdd();
            builder.HasOne(x => x.Course).WithMany(y => y.Assignments).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
