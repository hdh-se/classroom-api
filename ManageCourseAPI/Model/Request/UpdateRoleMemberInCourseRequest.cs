using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class UpdateRoleMemberInCourseRequest
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public Role Role { get; set; }
        public string CurrentUser { get; set; }

    }
}
