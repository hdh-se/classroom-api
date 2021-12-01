using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class SortAssignmentsArgs
    {
        public int CourseId { get; set; }
        public ICollection<AssignmentSimple> assignmentSimples { get; set; }
        public string CurrentUser { get; set; }
    }
    public class AssignmentSimple
    {
        public int Id { get; set; }
        public int Order { get; set; }
    }
}
