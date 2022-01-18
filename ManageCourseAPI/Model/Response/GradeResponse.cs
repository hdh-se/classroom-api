using ManageCourse.Core.Data;

namespace ManageCourseAPI.Model.Response
{
    public class GradeResponse
    {
        public GradeResponse(Grade grade)
        {
            Id = grade.AssignmentId;
            GradeId = grade.Id;
            Grade = grade.GradeAssignment;
            MaxGrade = grade.Assignments.MaxGrade;
            GradeReviewId = 0;
            GradeScale = grade.Assignments.GradeScale;
        }

        public int Id { get; set; }
        public int GradeId { get; set; }
        public float Grade { get; set; }
        public int MaxGrade { get; set; }
        public int GradeReviewId { get; set; }
        public float GradeScale { get; set; }

    }
}