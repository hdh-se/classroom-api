using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class AddStudentIntoCourseArgs
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public string CurrentUser { get; set; }
    }
}
