using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class UpdateGradeReviewArgs
    {
        public int GradeReviewId { get; set; }
        public float GradeExpect { get; set; }
        public string Reason { get; set; }
        public string CurrentUser { get; set; }
    }
}
