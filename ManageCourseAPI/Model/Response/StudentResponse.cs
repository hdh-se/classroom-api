using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class StudentResponse
    {
        public StudentResponse(Student student)
        {
            Id = student.Id;
            StudentID = student.StudentID;
            FullName = student.FullName;
            FirstName = student.FirstName;
            MiddleName = student.MiddleName;
            LastName = student.LastName;
            DateOfBird = student.DateOfBird;
            UserId = student.UserId;
            Phone = student.Phone;
        }

        public int Id { get; set; }
        public string StudentID { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBird { get; set; }
        public int UserId { get; set; }
        public string Phone { get; set; }
    }
}
