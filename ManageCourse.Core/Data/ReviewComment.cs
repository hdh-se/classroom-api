using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class ReviewComment: Audit, IHasId
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public int GradeReviewId { get; set; }
        public virtual GradeReview GradeReview { get; set; }
    }
}
