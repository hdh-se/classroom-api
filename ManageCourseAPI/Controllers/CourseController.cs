using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Repositories;
using ManageCourse.Core.Services;
using ManageCourseAPI.Controllers.Common;
using ManageCourseAPI.Model.Queries;
using ManageCourseAPI.Model.Request;
using ManageCourseAPI.Model.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Controllers
{
    [ApiController]
    [Route("course")]
    public class CourseController : ApiControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly AppUserManager _appUserManager;

        public CourseController(
            IGeneralModelRepository generalModelRepository,
            DbContextContainer dbContextContainer,
            ICourseService courseService,
            AppUserManager appUserManager) : base(generalModelRepository, dbContextContainer)
        {
            _courseService = courseService;
            _appUserManager = appUserManager;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetCousersAsync([FromQuery] CourseQuery courseQuery)
        {
            var user = await _appUserManager.FindByNameAsync(courseQuery.CurrentUser);
            if (user == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }
            var courseUsers = new CourseUserQuery
            {
                UserId = user.Id,
                StartAt = courseQuery.StartAt,
                MaxResults = courseQuery.MaxResults,
            };
            var coursers = await GetSearchResult(courseUsers, c => new CourseResponse(c.Course));
            foreach (var course in coursers.Data)
            {
                course.Owner = (await _appUserManager.FindByNameAsync(course.Owner)).NormalizedDisplayName;
            }
            return Ok(
                new GeneralResponse<GeneralResultResponse<CourseResponse>>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = coursers,
                    Message = "Get Course Successfull"
                });
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var coursers = (await _courseService.GetByIdAsync(id));
            return Ok(coursers);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("send-mail")]
        public async Task<IActionResult> SendMail([FromBody] SendMailJoinToCourseRequest sendMailJoinToCourseRequest)
        {

            var user = await _appUserManager.FindByNameAsync(sendMailJoinToCourseRequest.PersonReceive);
            if (user == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }
            var inviteLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Email/JoinClass";
            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendConfirmMail(user.Email, inviteLink);
            if (emailResponse)
            {
                Console.WriteLine("Email");
            }
            var response = new SingularResponse<UserResponse>
            {
                Result = new UserResponse(user),
            };
            return Created($"{Request.Path}/{user.Id}", response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{id}/everyone")]
        public async Task<IActionResult> GetMemberInCourse( int id)
        {
            var course = await GeneralModelRepository.GetAndCheckExisting<Course>(id);
           
            var getTeachersQuery = new CourseUserQuery
            {
                CourseId = id,
                Role = Role.Teacher,
                StartAt = 0,
                MaxResults = 100,
            };
            
            var getStudentsQuery = new CourseUserQuery
            {
                CourseId = id,
                Role = Role.Student,
                StartAt = 0,
                MaxResults = 100,
            };
            var listTeacherIds = (await GetSearchResult(getTeachersQuery, c => c.UserId)).Data;
            var listTeacher = await GeneralModelRepository.GetQueryable<AppUser>().Where(user => listTeacherIds.Contains(user.Id)).Select(user=> new UserResponse(user)).ToListAsync();
                
            var listStudentIds = (await GetSearchResult(getStudentsQuery, c => c.UserId)).Data;
            var listStudent = await GeneralModelRepository.GetQueryable<AppUser>().Where(user => listStudentIds.Contains(user.Id)).Select(user => new UserResponse(user)).ToListAsync();
            var memberCourseResponse = new MemberCourseResponse
            {
                Total = listStudent.Count + listTeacher.Count,
                Teachers = listTeacher,
                Students = listStudent,
            };
            return Ok(
                new GeneralResponse<MemberCourseResponse>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = memberCourseResponse,
                    Message = "Get Member Successfull"
                });

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateCousersAsync([FromBody] CreateCourseRequest courseRequest)
        {
            var user = await _appUserManager.FindByNameAsync(courseRequest.CurrentUser);
            if (user == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }
            var args = new CreateCourseArgs
            {
                SubjectId = courseRequest.SubjectId,
                Schedule = courseRequest.Schedule,
                Description = courseRequest.Description,
                GradeId = courseRequest.GradeId,
                Title = courseRequest.Title,
                Credits = courseRequest.Credits,
                CurrentUser = courseRequest.CurrentUser,
                UserId = user.Id
            };
            var courser = await _courseService.CreateCourseAsync(args);

            return Ok(new GeneralResponse<CourseResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new CourseResponse(courser),
                Message = "Created new course successfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add-student")]
        public async Task<IActionResult> AddStudentIntoCousersAsync([FromBody] AddStudentIntoCourseRequest courseRequest)
        {
            var user = await _appUserManager.FindByNameAsync(courseRequest.CurrentUser);
            if (user == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }
            var args = new AddStudentIntoCourseArgs
            {
                CourseId = courseRequest.CourseId,
                CurrentUser = courseRequest.CurrentUser,
                UserId = user.Id
            };
            var courser = await _courseService.AddStudentIntoCourseAsync(args);

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Join classroom successfull"
            });
        }
    }
}
