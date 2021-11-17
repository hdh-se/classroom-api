using ManageCourse.Core.Data;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Services;
using ManageCourseAPI.Model.Request;
using ManageCourseAPI.Model.Response;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Controllers
{
    [ApiController]
    [Route("course")]
    public class CourseController: ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCousersAsync()
        {
            var coursers = (await _courseService.GetCourseAsync()).AsEnumerable();
            return Ok(new GeneralResultResponse<Course>(coursers));
        }
        
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var coursers = (await _courseService.GetByIdAsync(id));
            return Ok(coursers);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateCousersAsync([FromBody]CreateCourseRequest courseRequest)
        {
            var args = new CreateCourseArgs
            {
                SubjectId = courseRequest.SubjectId,
                Schedule = courseRequest.Schedule,
                Description = courseRequest.Description,
                GradeId = courseRequest.GradeId,
                Name = courseRequest.Name
            };
            var courser = await _courseService.CreateCourseAsync(args);
            return Ok(new CourseResponse(courser));
        }
    }
}
