using LinqKit;
using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Queries
{
    public class CourseQuery: BaseEFQuery<Course>
    {
        public string Name { get; set; }


        public override List<string> GetIncludeNavigationPaths()
        {
            return base.GetIncludeNavigationPaths();
        }

        public override ExpressionStarter<Course> GetQueryConditions()
        {
            var predicate = base.GetQueryConditions();
            if (!string.IsNullOrEmpty(Name))
            {
                predicate.And(c => c.Name.Contains(Name));
            }
            return predicate;
        }

        public override Dictionary<string, Expression<Func<Course, object>>> GetSortColumnMappings()
        {
            return new Dictionary<string, Expression<Func<Course, object>>>
            {
                { nameof(Course.Id), c => c.Id },
            };
        }
    }
}
