using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class ChangePasswordRequest
    {
        public string CurrentPassWord { get; set; }
        public string NewPassWord { get; set; }
        public string CurrentUser { get; set; }
    }
}
