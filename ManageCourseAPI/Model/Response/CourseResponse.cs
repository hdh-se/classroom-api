using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class CourseResponse
    {
        public CourseResponse(Course course)
        {
            Id = course.Id;
            SubjectId = course.SubjectId;
            GradeId = course.GradeId;
            Title = course.Title;
            Credits = course.Credits;
            Title = course.Title;
            Description = course.Description;
            ClassCode = course.CourseCode;
            Schedule = course.Schedule;
            Owner = course.CreateBy;
        }

        public long Id { get; set; }
        public long SubjectId { get; set; }
        public long GradeId { get; set; }
        public string Title { get; set; }
        public string ClassCode { get; set; }
        public int Credits { get; set; }
        public string Description { get; set; }
        public string Schedule { get; set; }
        public string Owner { get; set; }
    }
}
