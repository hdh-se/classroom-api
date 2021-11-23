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
    public class CourseQuery : BaseEFQuery<Course>
    {
        public string Title { get; set; }
        public string CurrentUser { get; set; }


        public override List<string> GetIncludeNavigationPaths()
        {
            return base.GetIncludeNavigationPaths();
        }

        public override ExpressionStarter<Course> GetQueryConditions()
        {
            var predicate = base.GetQueryConditions();

            if (!string.IsNullOrEmpty(Title))
            {
                predicate.And(c => c.Title.Contains(Title));
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
