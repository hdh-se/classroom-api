using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data.Common
{
    public class Audit
    {
        public string CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateOn { get; set; }
    }
}
