using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.DTOs
{
    public class ChangeUserPasswordResult
    {
        public bool IsSuccess { get; set; }
        public string NewPassword { get; set; }
    }
}
