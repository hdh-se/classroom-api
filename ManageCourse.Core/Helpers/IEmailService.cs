using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourse.Core.Helpers
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html);
    }
}
