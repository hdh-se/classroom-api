using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data.Configurations
{
    public class CourseUserEntityTypeConfiguration : IEntityTypeConfiguration<Course_User>
    {
        public void Configure(EntityTypeBuilder<Course_User> builder)
        {
            builder.ToTable("CourseUser");
            builder.Property("Id").ValueGeneratedOnAdd();
            builder.HasOne(x => x.Course).WithMany(y => y.Course_Users).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
