using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class RemoveMemberInCourseRequest
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public string CurrentUser { get; set; }
    }
}
