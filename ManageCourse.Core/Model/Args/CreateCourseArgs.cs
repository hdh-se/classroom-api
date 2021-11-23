using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class CreateCourseArgs
    {
        public int SubjectId { get; set; }
        public int GradeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public string Schedule { get; set; }
        public string CurrentUser { get; set; }
        public int UserId { get; set; }
    }
}
