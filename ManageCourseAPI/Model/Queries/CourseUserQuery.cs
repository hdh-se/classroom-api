using LinqKit;
using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.Model.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Queries
{
    public class CourseUserQuery : BaseEFQuery<Course_User>
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public Role Role { get; set; }

        public override List<string> GetIncludeNavigationPaths()
        {
            var includes = base.GetIncludeNavigationPaths();
            includes.Add(nameof(Course));
            return includes;
        }

        public override ExpressionStarter<Course_User> GetQueryConditions()
        {
            var predicate = base.GetQueryConditions();

            if (UserId > 0)
            {
                predicate.And(c => c.UserId == UserId);
            }

            if (CourseId > 0)
            {
                predicate.And(c => c.CourseId == CourseId);
            }
            if (Role != Role.None )
            {
                predicate.And(c => c.Role == Role);
            }
            return predicate;
        }

        public override Dictionary<string, Expression<Func<Course_User, object>>> GetSortColumnMappings()
        {
            return new Dictionary<string, Expression<Func<Course_User, object>>>
            {
                { nameof(Course_User.Id), c => c.Id },
            };
        }
    }
}
