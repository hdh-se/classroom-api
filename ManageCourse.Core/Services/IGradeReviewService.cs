using ManageCourse.Core.Data;
using ManageCourse.Core.Model.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services
{
    public interface IGradeReviewService
    {
        public Task<GradeReview> CreateGradeReviewAsync(CreateGradeReviewArgs createGradeReviewArgs);
        public Task<GradeReview> UpdateGradeReviewAsync(UpdateGradeReviewArgs updateGradeReviewArgs);
        public Task<ReviewComment> CreateReviewCommentAsync(CreateReviewCommentArgs createReviewCommentArgs);
        public Task<ReviewComment> UpdateReviewCommentAsync(UpdateReviewCommentArgs updateReviewCommentArgs);
        public Task ApprovalGradeReviewAsync(ApprovalGradeReviewArgs approvalGradeReviewArgs);
        public Task<ICollection<ReviewComment>> GetReviewComments(int gradeReviewId, string currentUser);
    }
}
