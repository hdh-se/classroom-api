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
                Role = Role.Teacher
            };
            await _generalModelRepository.Create(courseUser);
            return courses;
        }

        public async Task<Course_User> AddStudentIntoCourseAsync(AddStudentIntoCourseArgs studentIntoCourseArgs)
        {
            var courseUser = new Course_User
            {
                CourseId = studentIntoCourseArgs.CourseId,
                UserId = studentIntoCourseArgs.UserId,
                Role = Role.Student
            };
            AuditHelper.CreateAudit(courseUser, studentIntoCourseArgs.CurrentUser);
            
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
