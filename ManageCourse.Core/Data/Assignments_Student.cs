using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class Assignments_Student: Audit
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public int UserId { get; set; }

        public virtual Assignments Assignments { get; set; }
        public virtual Student Student { get; set; }
    }
}
