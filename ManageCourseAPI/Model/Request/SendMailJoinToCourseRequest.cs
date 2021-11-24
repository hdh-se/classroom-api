using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCourse.Core.Constansts;

namespace ManageCourseAPI.Model.Request
{
    public class SendMailJoinToCourseRequest
    {
        public Role Role { get; set; }
        public string MailPersonReceive { get; set; }
        public string ClassCode { get; set; }
    }
}
