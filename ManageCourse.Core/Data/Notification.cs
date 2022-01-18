using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class Notification: Audit, IHasId
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int GradeId { get; set; }
        public int GradeReviewId { get; set; }
        public bool IsSeen { get; set; }
        public string SenderName { get; set; }
        public TypeNotification TypeNotification { get; set; }
        public string Message { get; set; }
    }
    public class StudentNotification: Notification
    {
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
    public class TeacherNotification: Notification
    {
        public int TeacherId { get; set; }
    }

}
