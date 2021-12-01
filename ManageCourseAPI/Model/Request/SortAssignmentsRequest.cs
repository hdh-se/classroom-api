using ManageCourse.Core.Model.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class SortAssignmentsRequest
    {
        public ICollection<AssignmentSimple> assignmentSimples { get; set; }
        public string CurrentUser { get; set; }
    }
    
}
