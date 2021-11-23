using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class AddMemberIntoCourseArgs
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public Role Role { get; set; }
        public string CurrentUser { get; set; }
    }
}
