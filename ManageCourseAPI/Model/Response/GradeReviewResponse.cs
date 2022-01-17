using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class GradeReviewResponse
    {
        public GradeReviewResponse(GradeReview gradeReview)
        {
            Id = gradeReview.Id;
            GradeExpect = gradeReview.GradeExpect;
            Message = gradeReview.Message;
            StudentId = gradeReview.Id;
            MSSV = gradeReview.Student.StudentID;
            GradeId = gradeReview.Id;
            Student = new StudentResponse(gradeReview.Student);
            Status = gradeReview.Status;
        }

        public int Id { get; set; }
        public float GradeExpect { get; set; }
        public string Message { get; set; }
        public int StudentId { get; set; }
        public string MSSV { get; set; }
        public int GradeId { get; set; }
        public StudentResponse Student { get; set; }
        public GradeReviewStatus Status { get; set; }
    }
}
