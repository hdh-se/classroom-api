using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Responses
{
    public class GradeOfAssignmentResponse
    {
        public GradeOfAssignmentResponse(Student student, Grade grade, Assignments assignments)
        {
            Mssv = student.StudentID;
            Name = student.FullName;
            Grade = grade.GradeAssignment;
            MaxGrade = assignments.MaxGrade;
        }

        public string Mssv { get; set; }
        public string Name { get; set; }
        public float Grade { get; set; }
        public string Username { get; set; }
        public int MaxGrade { get; set; }
    }
}
