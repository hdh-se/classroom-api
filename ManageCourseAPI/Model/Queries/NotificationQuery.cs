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
    public class NotificationQuery : BaseEFQuery<Notification>
    {
        public int UserId { get; set; }
        public string CurrentUser { get; set; }
        public override List<Expression<Func<Notification, object>>> GetIncludeExpressions()
        {
            return base.GetIncludeExpressions();
        }

        public override List<string> GetIncludeNavigationPaths()
        {
            return base.GetIncludeNavigationPaths();
        }

        public override ExpressionStarter<Notification> GetQueryConditions()
        {
            var predicate = base.GetQueryConditions();

            if (UserId > 0)
            {
                predicate.And(c => c.UserId == UserId);
            }

            return predicate;
        }

        public override Dictionary<string, Expression<Func<Notification, object>>> GetSortColumnMappings()
        {
            return new Dictionary<string, Expression<Func<Notification, object>>>
            {
                { nameof(Notification.CreateOn), c => c.CreateOn },
            };
        }
    }
}
