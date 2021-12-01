using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class Course: Audit, IHasId
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int GradeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CourseCode { get; set; }
        public int Credits { get; set; }
        public string Schedule { get; set; }
        public ICollection<Course_User> Course_Users { get; set; }
        public virtual ICollection<Assignments> Assignments { get; set; }

    }
}
