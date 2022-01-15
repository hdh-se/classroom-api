using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace ManageCourse.Core.Data.Configurations
{
    public class StudentNotificationEntityTypeConfiguration : IEntityTypeConfiguration<StudentNotification>
    {
        public void Configure(EntityTypeBuilder<StudentNotification> builder)
        {
            builder.ToTable("StudentNotification");
            builder.Property("Id").ValueGeneratedOnAdd();
            builder.HasOne(x => x.Student).WithMany(y => y.StudentNotification).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
