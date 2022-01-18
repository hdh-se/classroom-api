using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Responses
{
    public class GradeSimpleResponse
    {
        public GradeSimpleResponse(Grade grade, Assignments assignments)
        {
            Id = grade.AssignmentId;
            GradeId = grade.Id;
            Grade = grade.GradeAssignment;
            MaxGrade = assignments.MaxGrade;
        }

        public int Id { get; set; }
        public int GradeId { get; set; }
        public int GradeReviewId { get; set; }
        public float Grade { get; set; }
        public int MaxGrade { get; set; }
    }
}
