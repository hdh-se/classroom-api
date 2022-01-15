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
    public class TeacherNotificationQuery : BaseEFQuery<TeacherNotification>
    {
        public int TeacherId { get; set; }
        public override List<Expression<Func<TeacherNotification, object>>> GetIncludeExpressions()
        {
            return base.GetIncludeExpressions();
        }

        public override List<string> GetIncludeNavigationPaths()
        {
            var includes = base.GetIncludeNavigationPaths();
            return includes;
        }

        public override ExpressionStarter<TeacherNotification> GetQueryConditions()
        {
            var predicate = base.GetQueryConditions();

            if (TeacherId > 0)
            {
                predicate.And(c => c.TeacherId == TeacherId);
            }

            return predicate;
        }

        public override Dictionary<string, Expression<Func<TeacherNotification, object>>> GetSortColumnMappings()
        {
            return new Dictionary<string, Expression<Func<TeacherNotification, object>>>
            {
                { nameof(TeacherNotification.CreateOn), c => c.CreateOn },
            };
        }
    }
}
