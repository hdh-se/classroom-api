using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class Course_Student: Audit, IHasId
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public string StudentCode { get; set; }
        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}
