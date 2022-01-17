using LinqKit;
using ManageCourse.Core.Data;
using ManageCourse.Core.Model.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Queries
{
    public class CommentGradeReviewQuery : BaseEFQuery<ReviewComment>
    {
        public int GradeReviewId { get; set; }
        public override List<string> GetIncludeNavigationPaths()
        {
            var includes = base.GetIncludeNavigationPaths();
            return includes;
        }

        public override ExpressionStarter<ReviewComment> GetQueryConditions()
        {
            var predicate = base.GetQueryConditions();

            if (GradeReviewId > 0)
            {
                predicate.And(c => c.GradeReviewId == GradeReviewId);
            }

            return predicate;
        }

        public override Dictionary<string, Expression<Func<ReviewComment, object>>> GetSortColumnMappings()
        {
            return new Dictionary<string, Expression<Func<ReviewComment, object>>>
            {
                { nameof(ReviewComment.CreateOn), c => c.CreateOn },
            };
        }
    }
}
