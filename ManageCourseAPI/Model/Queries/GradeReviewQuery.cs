using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Queries
{
    public class GradeReviewQuery
    {
        public int CourseId { get; set; }
        public int GradeId { get; set; }
        public int GradeReviewId { get; set; }
        public string CurrentUser { get; set; }
    }
}
