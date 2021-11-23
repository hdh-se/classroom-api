using ManageCourse.Core.Data.Common;

namespace ManageCourse.Core.Data
{
    public class Grade: Audit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
