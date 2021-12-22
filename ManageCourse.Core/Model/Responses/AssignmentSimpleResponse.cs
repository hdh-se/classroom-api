using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Responses
{
    public class AssignmentSimpleResponse
    {
        public AssignmentSimpleResponse(Assignments assignments)
        {
            Id = assignments.Id;
            Name = assignments.Name;
            MaxGrade = assignments.MaxGrade;
            GradeScale = assignments.GradeScale;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxGrade { get; set; }
        public float GradeScale { get; set; }
    }
}
