using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class UpdateMemberInClassArgs
    {
        public ICollection<Student> Students { get; set; }
        public int CourseId { get; set; }
        public string CurrentUser { get; set; }
    }
}
