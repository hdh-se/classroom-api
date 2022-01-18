using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class ApprovalAccountArgs
    {
        public UserStatus UserStatus { get; set; }
        public string Username { get; set; }
        public string CurrentUser { get; set; }
    }
}
