using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class CreateCourseArgs
    {
        public long SubjectId { get; set; }
        public long GradeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Schedule { get; set; }
    }
}
