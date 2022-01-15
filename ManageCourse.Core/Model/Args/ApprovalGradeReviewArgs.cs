using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class ApprovalGradeReviewArgs
    {
        public GradeReviewStatus ApprovalStatus { get; set; }
        public int GradeReviewId { get; set; }
        public string CurrentUser { get; set; }
    }
}
