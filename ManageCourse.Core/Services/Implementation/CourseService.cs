using ManageCourse.Core.Data;
using ManageCourse.Core.DbContexts;
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
            var courses = new Course {
                SubjectId = courseArgs.SubjectId,
                Schedule = courseArgs.Schedule,
                Description = courseArgs.Description,
                GradeId = courseArgs.GradeId,
                Name = courseArgs.Name
            };
            _ = await _appDbContext.Courses.AddAsync(courses);
            await _appDbContext.SaveChangesAsync();
            return courses;
        }

        public async Task<Course> GetByIdAsync(long id)
        {
            var courses = await _generalModelRepository.Get<Course>(id);
            return courses;
        }

        public async Task<List<Course>> GetCourseAsync()
        {
            var courses = await _appDbContext.Courses.OrderByDescending(x => x.Id).ToListAsync();
            return courses;
        }
    }
}
