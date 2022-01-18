using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class ChangeStudentIDRequest
    {
        public string MSSV { get; set; }
        public string Username { get; set; }
        public string CurrentUser { get; set; }
    }
}
