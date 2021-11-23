using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class Course_User: Audit, IHasId
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public Role Role { get; set; }
        public virtual Course Course { get; set; }
    }
}
