using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class CreateStudentNotificationsArgs
    {
        public string Message { get; set; }
        public int GradeReviewId { get; set; }
        public List<int> StudentIds { get; set; }
        public string CurrentUser { get; set; }
    }
}
