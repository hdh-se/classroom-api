using ManageCourse.Core.Helpers;
using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Repositories;
using ManageCourse.Core.Services;
using ManageCourse.Core.Utilities;
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
        private readonly IEmailService _emailService;

        public CourseController(
            IGeneralModelRepository generalModelRepository,
            DbContextContainer dbContextContainer,
            ICourseService courseService,
            IEmailService emailService,
            AppUserManager appUserManager) : base(generalModelRepository, dbContextContainer)
        {
            _courseService = courseService;
            _appUserManager = appUserManager;
            _emailService = emailService;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id, string currentUser)
        {
            var user = await _appUserManager.FindByNameAsync(currentUser);
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

            var isParticipate = GeneralModelRepository.GetQueryable<Course_User>().Where(c => c.UserId == user.Id).Any();
            if (!isParticipate)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "You haven't been joined this class"
                });
            }
            var coursers = (await _courseService.GetByIdAsync(id));
            return Ok(new GeneralResponse<CourseResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new CourseResponse(coursers),
                Message = "You haven't been joined this class"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("send-mail")]
        public IActionResult SendMail([FromBody] SendMailJoinToCourseRequest sendMailJoinToCourseRequest)
        {

            Guards.ValidEmail(sendMailJoinToCourseRequest.MailPersonReceive);
            var token = StringHelper.GenerateHashString(sendMailJoinToCourseRequest.ClassCode);
            var inviteLink = $"{ConfigClient.URL_CLIENT}/class-detail/{token}";
            //EmailHelper emailHelper = new EmailHelper();
            //bool emailResponse = emailHelper.SendConfirmMail(sendMailJoinToCourseRequest.MailPersonReceive, inviteLink);
            _emailService.Send(sendMailJoinToCourseRequest.MailPersonReceive, token, inviteLink);
            //if (!emailResponse)
            //{
            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = $"Send mail to {sendMailJoinToCourseRequest.MailPersonReceive} failed"
            });
            //}

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = $"Send mail to {sendMailJoinToCourseRequest.MailPersonReceive} successfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{id}/everyone")]
        public async Task<IActionResult> GetMemberInCourse(int id)
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
            var listTeacher = await GeneralModelRepository.GetQueryable<AppUser>().Where(user => listTeacherIds.Contains(user.Id)).Select(user => new UserResponse(user)).ToListAsync();

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
                Role = courseRequest.Role,
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
        [HttpPost("add-member/invite-link")]
        public async Task<IActionResult> AddStudentIntoCousersByLinkAsync([FromBody] AddMemberIntoCourseByLinkRequest courseRequest)
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

            var course = await GeneralModelRepository.GetQueryable<Course>().Where(c => !String.IsNullOrEmpty(c.CourseCode) && StringHelper.GenerateHashString(c.CourseCode) == courseRequest.Token).FirstOrDefaultAsync();

            var courseUser = new Course_User
            {
                UserId = user.Id,
                CourseId = course.Id,
                Role = courseRequest.Role,
            };

            AuditHelper.CreateAudit(courseUser, user.UserName);

            var returnVal = await GeneralModelRepository.Create<Course_User>(courseUser);

            if (returnVal == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found Course"
                });
            }

            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new
                {
                    result = returnVal
                },
                Message = "OK"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add-member")]
        public async Task<IActionResult> AddStudentIntoCousersAsync([FromBody] AddMemberIntoCourseRequest courseRequest)
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

            var course = await GeneralModelRepository.Get<Course>(courseRequest.CourseId);
            if (course == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found Course"
                });
            }

            if (user.UserName != course.CreateBy)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }

            var newMember = await _appUserManager.FindByNameAsync(courseRequest.NewMember);
            if (newMember == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = $"{courseRequest.NewMember} found user"
                });
            }
            var args = new AddMemberIntoCourseArgs
            {
                CourseId = courseRequest.CourseId,
                CurrentUser = courseRequest.CurrentUser,
                Role = courseRequest.Role != Role.None ? courseRequest.Role : Role.Student,
                UserId = newMember.Id
            };

            await _courseService.AddMemberIntoCourseAsync(args);

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Add member classroom successfull"
            });
        }
    }
}
