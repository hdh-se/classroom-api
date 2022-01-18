using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class ApprovalAccountRequest
    {
        public string CurrentUser { get; set; }
        public string Username { get; set; }
    }
}
