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
            return base.GetQueryConditions();
        }

        public override Dictionary<string, Expression<Func<Notification, object>>> GetSortColumnMappings()
        {
            return base.GetSortColumnMappings();
        }
    }
}
