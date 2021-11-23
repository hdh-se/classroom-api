using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class AddStudentIntoCourseRequest
    {
        public int CourseId { get; set; }
        public string CurrentUser { get; set; }
    }
}
