using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services.Implementation
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IGeneralModelRepository _generalModelRepository;

        public CourseService(AppDbContext appDbContext, IGeneralModelRepository generalModelRepository)
        {
            _appDbContext = appDbContext;
            _generalModelRepository = generalModelRepository;
        }

        public async Task<Course> CreateCourseAsync(CreateCourseArgs courseArgs)
        {
            var courses = new Course
            {
                SubjectId = courseArgs.SubjectId,
                Schedule = courseArgs.Schedule,
                Description = courseArgs.Description,
                GradeId = courseArgs.GradeId,
                CourseCode = StringHelper.GenerateCode(20),
                Title = courseArgs.Title,
                Credits = courseArgs.Credits
            };
            AuditHelper.CreateAudit(courses, courseArgs.CurrentUser);
            _ = await _appDbContext.Courses.AddAsync(courses);
            await _appDbContext.SaveChangesAsync();
            var courseUser = new Course_User
            {
                CourseId = courses.Id,
                UserId = courseArgs.UserId,
                Role = courseArgs.Role != Role.None ? courseArgs.Role : Role.Teacher
            };
            await _generalModelRepository.Create(courseUser);
            return courses;
        }

        public async Task<Course_User> AddMemberIntoCourseAsync(AddMemberIntoCourseArgs memberIntoCourseArgs)
        {
            var courseUser = new Course_User
            {
                CourseId = memberIntoCourseArgs.CourseId,
                UserId = memberIntoCourseArgs.UserId,
                Role = memberIntoCourseArgs.Role != Role.None ? memberIntoCourseArgs.Role : Role.Student
            };
            AuditHelper.CreateAudit(courseUser, memberIntoCourseArgs.CurrentUser);
            
            await _generalModelRepository.Create(courseUser);
            return courseUser;
        }

        public async Task<Course> GetByIdAsync(long id)
        {
            var courses = await _generalModelRepository.Get<Course>(id);
            return courses;
        }

        public async Task<Assignments> CreateNewAssignments(CreateNewAssignmentsArgs createNewAssignmentsArgs)
        {
            var numberOfAssignments = _generalModelRepository.GetQueryable<Assignments>().Where(c => c.CourseId == createNewAssignmentsArgs.CourseId).Count();
            var assignments = new Assignments
            {
                CourseId = createNewAssignmentsArgs.CourseId,
                Name = createNewAssignmentsArgs.Name,
                MaxGrade = createNewAssignmentsArgs.MaxGrade,
                Description = createNewAssignmentsArgs.Description,
                Order = numberOfAssignments + 1
            };
            AuditHelper.CreateAudit(assignments, createNewAssignmentsArgs.CurrentUser);
            await _generalModelRepository.Create(assignments);
            return assignments;
        }

        public async Task<Assignments> UpdateAssignments(UpdateAssignmentsArgs updateAssignmentsArgs)
        {
            var assignment = await _generalModelRepository.Get<Assignments>(updateAssignmentsArgs.Id);
            assignment.Name = String.IsNullOrEmpty(updateAssignmentsArgs.Name) ? assignment.Name : updateAssignmentsArgs.Name; 
            assignment.Description = String.IsNullOrEmpty(updateAssignmentsArgs.Description) ? assignment.Description : updateAssignmentsArgs.Description; 
            assignment.MaxGrade = assignment.MaxGrade > 0 && assignment.MaxGrade != updateAssignmentsArgs.MaxGrade ? updateAssignmentsArgs.MaxGrade : assignment.MaxGrade;
            AuditHelper.UpdateAudit(assignment, updateAssignmentsArgs.CurrentUser);
            await _generalModelRepository.Update(assignment);
            return assignment;
        }

        public ICollection<Assignments> SortAssignments(SortAssignmentsArgs sortAssignmentsArgs)
        {
            var listAssignments = _generalModelRepository.GetQueryable<Assignments>().AsEnumerable().Where(a => a.CourseId == sortAssignmentsArgs.CourseId).ToList();
            foreach (var assignments in listAssignments)
            {
                var order = sortAssignmentsArgs.assignmentSimples.AsEnumerable().Where(a => a.Id == assignments.Id).FirstOrDefault();
                if (order != null)
                {
                    assignments.Order = order.Order;
                }
            }

            _appDbContext.Assignments.UpdateRange(listAssignments);
            _appDbContext.SaveChanges();
            return listAssignments;
        }
    }
}
