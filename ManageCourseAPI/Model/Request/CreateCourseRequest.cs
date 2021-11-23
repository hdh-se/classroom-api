using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class CreateCourseRequest
    {
        public int SubjectId { get; set; }
        public int GradeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public string Schedule { get; set; }
        public string CurrentUser { get; set; }
    }
}
