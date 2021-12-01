using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class AssignmentsResponse
    {
        public AssignmentsResponse(Assignments course)
        {
            Id = course.Id;
            CourseId = course.CourseId;
            Name = course.Name;
            Description = course.Description;
            MaxGrade = course.MaxGrade;
            Order = course.Order;
        }

        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxGrade { get; set; }
        public int Order { get; set; }
    }
}
