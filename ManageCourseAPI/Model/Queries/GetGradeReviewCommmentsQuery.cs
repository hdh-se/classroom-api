using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Queries
{
    public class GetGradeReviewCommmentsQuery: CommentGradeReviewQuery
    {
        public int CourseId { get; set; }
        public int GradeId { get; set; }
        public string CurrentUser { get; set; }
    }
}
