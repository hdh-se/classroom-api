using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class SendMailJoinToCourseRequest
    {
        public string PersonReceive { get; set; }
        public int CourseId { get; set; }
    }
}
