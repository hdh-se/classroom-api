using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.DTOs
{
    public class ChangeUserPasswordData
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public bool CheckOldPassword { get; set; }
        public string CurrentUser { get; set; }
    }
}
