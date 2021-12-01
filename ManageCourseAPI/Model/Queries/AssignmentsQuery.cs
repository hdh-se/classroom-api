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
    public class AssignmentsQuery : BaseEFQuery<Assignments>
    {
        public string Name { get; set; }
        public string CurrentUser { get; set; }


        public override List<string> GetIncludeNavigationPaths()
        {
            return base.GetIncludeNavigationPaths();
        }

        public override ExpressionStarter<Assignments> GetQueryConditions()
        {
            var predicate = base.GetQueryConditions();

            if (!string.IsNullOrEmpty(Name))
            {
                predicate.And(c => c.Name.Contains(Name));
            }
            return predicate;
        }

        public override Dictionary<string, Expression<Func<Assignments, object>>> GetSortColumnMappings()
        {
            return new Dictionary<string, Expression<Func<Assignments, object>>>
            {
                { nameof(Assignments.Id), c => c.Id },
                { nameof(Assignments.Order), c => c.Order },
            };
        }
    }
}
