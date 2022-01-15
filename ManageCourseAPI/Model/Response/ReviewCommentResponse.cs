using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class ReviewCommentResponse
    {
        public ReviewCommentResponse(ReviewComment reviewComment)
        {
            Id = reviewComment.Id;
            Message = reviewComment.Message;
            StudentId = reviewComment.StudentId;
            TeacherId = reviewComment.TeacherId;
            GradeReviewId = reviewComment.GradeReviewId;
            Student = reviewComment.Student;
        }

        public int Id { get; set; }
        public string Message { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public int GradeReviewId { get; set; }
        public Student Student { get; set; }
        public UserResponse Teacher { get; set; }
    }
}
