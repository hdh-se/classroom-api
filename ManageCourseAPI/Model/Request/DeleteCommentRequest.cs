using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class DeleteCommentRequest
    {
        public int CourseId { get; set; }
        public int GradeId { get; set; }
        public int GradeReviewId { get; set; }
        public int ReviewCommentId { get; set; }
        public string Message { get; set; }
        public string CurrentUser { get; set; }
    }
}
