using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.DTOs
{
    public class UpdateUserStatusResult
    {
        public int UserId { get; set; }
        public UserStatus UserStatus { get; set; }
    }
}
