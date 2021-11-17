using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class Subject
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long DepartmentId { get; set; }
        public int NumberOfCredits { get; set; }
        public virtual Department Department { get; set; }
    }
}
