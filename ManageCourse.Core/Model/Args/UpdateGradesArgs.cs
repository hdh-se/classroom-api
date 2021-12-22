using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class UpdateGradesArgs
    {
        public ICollection<Grade> Grades { get; set; }
        public int CourseId { get; set; }
        public int AssignmentId { get; set; }
        public string CurrentUser { get; set; }
    }
}
