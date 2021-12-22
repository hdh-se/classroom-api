using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Responses
{
    public class GradeOfCourseResponse
    {
        public string Mssv { get; set; }
        public string Name { get; set; }
        public ICollection<GradeSimpleResponse> Grades { get; set; }
    }
}
