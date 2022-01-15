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
    public class StudentNotificationQuery: BaseEFQuery<StudentNotification>
    {
        public int StudentId { get; set; }
        public override List<Expression<Func<StudentNotification, object>>> GetIncludeExpressions()
        {
            return base.GetIncludeExpressions();
        }

        public override List<string> GetIncludeNavigationPaths()
        {
            var includes = base.GetIncludeNavigationPaths();
            includes.Add(nameof(StudentNotification.Student));
            return includes;
        }

        public override ExpressionStarter<StudentNotification> GetQueryConditions()
        {
            var predicate = base.GetQueryConditions();

            if (StudentId > 0)
            {
                predicate.And(c => c.StudentId == StudentId);
            }

            return predicate;
        }

        public override Dictionary<string, Expression<Func<StudentNotification, object>>> GetSortColumnMappings()
        {
            return new Dictionary<string, Expression<Func<StudentNotification, object>>>
            {
                { nameof(StudentNotification.CreateOn), c => c.CreateOn },
            };
        }
    }
}
