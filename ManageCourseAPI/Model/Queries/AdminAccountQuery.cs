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
    public class AdminAccountQuery : BaseEFQuery<AppUser>
    {
        public string Username { get; set; }
        public override List<Expression<Func<AppUser, object>>> GetIncludeExpressions()
        {
            return base.GetIncludeExpressions();
        }

        public override List<string> GetIncludeNavigationPaths()
        {
            return base.GetIncludeNavigationPaths();
        }

        public override ExpressionStarter<AppUser> GetQueryConditions()
        {
            var predicate = base.GetQueryConditions();

            if (!String.IsNullOrEmpty(Username))
            {
                predicate.And(c => c.UserName.Contains(Username));
            }

            predicate.And(c => c.RoleAccount == ManageCourse.Core.Constansts.RoleAccount.Admin);

            return predicate;
        }

        public override Dictionary<string, Expression<Func<AppUser, object>>> GetSortColumnMappings()
        {
            return new Dictionary<string, Expression<Func<AppUser, object>>>
            {
                { nameof(AppUser.CreateOn), c => c.CreateOn },
            };
        }
    }
}
