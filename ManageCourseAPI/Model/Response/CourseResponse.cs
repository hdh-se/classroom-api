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
            Name = course.Name;
            Description = course.Description;
            Schedule = course.Schedule;
        }

        public long Id { get; set; }
        public long SubjectId { get; set; }
        public long GradeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Schedule { get; set; }
    }
}
