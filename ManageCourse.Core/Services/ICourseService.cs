using ManageCourse.Core.Data;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services
{
    public interface ICourseService
    {
        public Task<Course> CreateCourseAsync(CreateCourseArgs courseArgs);
        public Task<Course_User> AddMemberIntoCourseAsync(AddMemberIntoCourseArgs studentIntoCourseArgs);
        public Task<Course> GetByIdAsync(long id);
        public Task<Assignments> CreateNewAssignments(CreateNewAssignmentsArgs createNewAssignmentsArgs);
        public Task<Assignments> UpdateAssignments(UpdateAssignmentsArgs updateAssignmentsArgs);
        public ICollection<Assignments> SortAssignments(SortAssignmentsArgs sortAssignmentsArgs);
        public Task<bool> UpdateGrades(UpdateGradesArgs updateGradesArgs);
        public Task<bool> UpdateMemberInClass(UpdateMemberInClassArgs updateMembers);
        public ICollection<GradeOfAssignmentResponse> GetAllGradeOfAssignment(int assignmentId);
        public ICollection<GradeOfCourseResponse> GetAllGradeOfCourse(int courseId)
;
    }
}
