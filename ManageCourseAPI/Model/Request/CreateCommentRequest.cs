using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class CreateCommentRequest
    {
        public string Message { get; set; }
        public int CourseId { get; set; }
        public int GradeId { get; set; }
        public int GradeReviewId { get; set; }
        public string CurrentUser { get; set; }
    }

    public class CreateStudentCommentRequest : CreateCommentRequest
    {
        public int StudentId { get; set; }
    }
    public class CreateTeacherCommentRequest : CreateCommentRequest
    {
        public int TeacherId { get; set; }
    }
}
