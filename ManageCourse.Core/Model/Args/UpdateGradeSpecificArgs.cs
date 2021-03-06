using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class UpdateGradeSpecificArgsBase
    {
        public string MSSV { get; set; }
        public bool IsFinalized { get; set; }
        public float GradeAssignment { get; set; }
    } 
    
    public class UpdateGradeSpecificArgs: UpdateGradeSpecificArgsBase
    {
        public int CourseId { get; set; }
        public int AssignmentsId { get; set; }
        public string CurrentUser { get; set; }
    }
}
