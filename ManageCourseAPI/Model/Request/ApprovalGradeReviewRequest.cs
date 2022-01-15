using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class ApprovalGradeReviewRequest
    {
        public int CourseId { get; set; }
        public int GradeId { get; set; }
        public int GradeReviewId { get; set; }
        public GradeReviewStatus ApprovalStatus { get; set; }
        public string CurrentUser { get; set; }
    }
}
