using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class CreateGradeFinallizeNotificationArgs
    {
        public string Message { get; set; }
        public int GradeId { get; set; }
        public string CurrentUser { get; set; }
    }
}
