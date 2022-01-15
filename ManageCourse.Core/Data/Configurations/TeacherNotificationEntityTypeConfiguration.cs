using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data.Configurations
{
    public class TeacherNotificationEntityTypeConfiguration : IEntityTypeConfiguration<TeacherNotification>
    {
        public void Configure(EntityTypeBuilder<TeacherNotification> builder)
        {
            builder.ToTable("TeacherNotification");
            builder.Property("Id").ValueGeneratedOnAdd();
        }
    }
}
