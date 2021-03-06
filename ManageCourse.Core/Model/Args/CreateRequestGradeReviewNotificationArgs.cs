using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class CreateRequestGradeReviewNotificationArgs
    {
        public string Message { get; set; }
        public int GradeReviewId { get; set; }
        public int CourseId { get; set; }
        public int GradeId { get; set; }
        public int StudentId { get; set; }
        public string CurrentUser { get; set; }
    }
}
