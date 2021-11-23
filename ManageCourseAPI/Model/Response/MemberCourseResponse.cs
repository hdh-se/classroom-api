using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class MemberCourseResponse
    {
        public int Total { get; set; }
        public string Owner { get; set; }
        public ICollection<UserResponse> Teachers { get; set; }
        public ICollection<UserResponse> Students { get; set; }
    }
}
