using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class Student: Audit
    {
        public int Id { get; set; }
        public string StudentID { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBird { get; set; }
        public int UserId { get; set; }
        public string Phone { get; set; }
        public virtual ICollection<Assignments_Student> Assignments_Students { get; set; }
        public virtual ICollection<Course_Student> Course_Students { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<GradeReview> GradeReviews { get; set; }
        public virtual ICollection<ReviewComment> ReviewComments { get; set; }
        public virtual ICollection<StudentNotification> StudentNotification { get; set; }
    }
}
