using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class SendMailJoinToCourseRequest
    {
        public string MailPersonReceive { get; set; }
        public int CourseId { get; set; }
        public string ClassCode { get; set; }
    }
}
