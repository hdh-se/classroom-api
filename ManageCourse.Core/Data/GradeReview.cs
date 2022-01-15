using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class GradeReview: Audit, IHasId
    {
        public int Id { get; set; }
        public float GradeExpect { get; set; }
        public string Message { get; set; }
        public int StudentId { get; set; }
        public int GradeId { get; set; }
        public GradeReviewStatus Status { get; set; }
        public Grade Grade { get; set; }
        public Student Student { get; set; }
        public ICollection<ReviewComment> ReviewComments { get; set; }
    }
}
