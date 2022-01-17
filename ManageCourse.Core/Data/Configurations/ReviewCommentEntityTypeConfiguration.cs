using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageCourse.Core.Data.Configurations
{
    public class ReviewCommentEntityTypeConfiguration : IEntityTypeConfiguration<ReviewComment>
    {
        public void Configure(EntityTypeBuilder<ReviewComment> builder)
        {
            builder.ToTable("ReviewComment");
            builder.Property("Id").ValueGeneratedOnAdd();
            builder.HasOne(x => x.GradeReview).WithMany(y => y.ReviewComments).HasForeignKey(x => x.GradeReviewId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
