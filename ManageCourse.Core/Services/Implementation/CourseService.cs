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

    }
}
