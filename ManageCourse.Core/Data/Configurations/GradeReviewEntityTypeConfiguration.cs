using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data.Configurations
{
    public class GradeReviewEntityTypeConfiguration : IEntityTypeConfiguration<GradeReview>
    {
        public void Configure(EntityTypeBuilder<GradeReview> builder)
        {
            builder.ToTable("GradeReview");
            builder.Property("Id").ValueGeneratedOnAdd();
            builder.HasOne(x => x.Grade).WithMany(y => y.GradeReviews).HasForeignKey(x => x.GradeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Student).WithMany(y => y.GradeReviews).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
