using ManageCourse.Core.Data.Common;
using System.Collections.Generic;

namespace ManageCourse.Core.Data
{
    public class Grade: Audit, IHasId
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string MSSV { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float GradeAssignment { get; set; }
        public bool IsFinalized { get; set; }
        public virtual Assignments Assignment { get; set; }
        public virtual Student Student { get; set; }
        public ICollection<GradeReview> GradeReviews { get; set; }

    }
}
