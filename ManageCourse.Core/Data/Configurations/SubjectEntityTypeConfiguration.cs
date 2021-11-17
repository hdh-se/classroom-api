using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ManageCourse.Core.Data.Configurations
{
    public class SubjectEntityTypeConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.ToTable("Subject");
            //Set PrimaryKey
            builder.Property("Id").ValueGeneratedOnAdd();
            //Set ManyToOne relationship
            builder.HasOne(x => x.Department).WithMany(y => y.Subjects).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
