using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services.Implementation
{
    public class GradeReviewService : IGradeReviewService
    {
        private readonly AppDbContext _appDbContext;
        protected AppUserManager _userManager { get; private set; }
        private readonly IGeneralModelRepository _generalModelRepository;

        public GradeReviewService(AppDbContext appDbContext, IGeneralModelRepository generalModelRepository, AppUserManager userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _generalModelRepository = generalModelRepository;
        }
        public async Task<GradeReview> CreateGradeReviewAsync(CreateGradeReviewArgs createGradeReviewArgs)
        {
            var gradeReview = new GradeReview
            {
                GradeExpect = createGradeReviewArgs.GradeExpect,
                Message = createGradeReviewArgs.Reason,
                StudentId = createGradeReviewArgs.StudentId,
                GradeId = createGradeReviewArgs.GradeId,
                Status = GradeReviewStatus.Pending
            };
            AuditHelper.CreateAudit(gradeReview, createGradeReviewArgs.CurrentUser);
           
            await _generalModelRepository.Create(gradeReview);
            return gradeReview;
        }

        public async Task<ReviewComment> CreateReviewCommentAsync(CreateReviewCommentArgs createReviewCommentArgs)
        {
            var reviewComment = new ReviewComment
            {
                GradeReviewId = createReviewCommentArgs.GradeReviewId,
                Message = createReviewCommentArgs.Message,
            };
            AuditHelper.CreateAudit(reviewComment, createReviewCommentArgs.CurrentUser);
            reviewComment.StudentId = createReviewCommentArgs.StudentId > 0 ? createReviewCommentArgs.StudentId : 0;
            reviewComment.TeacherId = createReviewCommentArgs.TeacherId > 0 ? createReviewCommentArgs.TeacherId : 0;
            await _generalModelRepository.Create(reviewComment);
            return reviewComment;
        }

        public async Task<ICollection<ReviewComment>> GetReviewComments(int gradeReviewId, string currentUser)
        {
            var gradeReview = await _generalModelRepository.Get<GradeReview>(gradeReviewId);
            return gradeReview.ReviewComments;
        }

        public async Task<GradeReview> UpdateGradeReviewAsync(UpdateGradeReviewArgs updateGradeReviewArgs)
        {
            var gradeReview = await _generalModelRepository.Get<GradeReview>(updateGradeReviewArgs.GradeReviewId);
            AuditHelper.UpdateAudit(gradeReview, updateGradeReviewArgs.CurrentUser);
            gradeReview.Message = updateGradeReviewArgs.Reason;
            gradeReview.GradeExpect = updateGradeReviewArgs.GradeExpect;
            await _generalModelRepository.Update(gradeReview);
            return gradeReview;
        }

        public async Task<ReviewComment> UpdateReviewCommentAsync(UpdateReviewCommentArgs updateReviewCommentArgs)
        {
            var reviewComment = await _generalModelRepository.Get<ReviewComment>(updateReviewCommentArgs.ReviewCommentId);
            AuditHelper.UpdateAudit(reviewComment, updateReviewCommentArgs.CurrentUser);
            reviewComment.Message = updateReviewCommentArgs.Message;
            await _generalModelRepository.Update(reviewComment);
            return reviewComment;
        }

        public async Task ApprovalGradeReviewAsync(ApprovalGradeReviewArgs approvalGradeReviewArgs)
        {
            var gradeReview = await _generalModelRepository.Get<GradeReview>(approvalGradeReviewArgs.GradeReviewId);
            AuditHelper.UpdateAudit(gradeReview, approvalGradeReviewArgs.CurrentUser);
            gradeReview.Status = approvalGradeReviewArgs.ApprovalStatus;
            var grade = await _generalModelRepository.Get<Grade>(gradeReview.GradeId);
            grade.GradeAssignment = gradeReview.GradeExpect;
            if (approvalGradeReviewArgs.ApprovalStatus == GradeReviewStatus.Approve)
            {
                await _generalModelRepository.Update(grade);
            }
            await _generalModelRepository.Update(gradeReview);
        }
    }
}
