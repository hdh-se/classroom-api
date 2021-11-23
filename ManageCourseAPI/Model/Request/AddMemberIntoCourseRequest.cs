using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class AddMemberIntoCourseRequest
    {
        public int CourseId { get; set; }
        public Role Role { get; set; }
        public string NewMember { get; set; }
        public string CurrentUser { get; set; }
    }
}
