using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class Assignments: Audit, IHasId
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxGrade { get; set; }
        public int Order { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Assignments_Student> Assignments_Students { get; set; }
    }
}
