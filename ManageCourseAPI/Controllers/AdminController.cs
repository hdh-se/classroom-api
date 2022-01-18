using IdentityModel.Client;
using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DTOs;
using ManageCourse.Core.Helpers;
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
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ManageCourseAPI.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController: ApiControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly AppUserManager _appUserManager;
        private readonly AppSignInManager _appSignInManager;
        private readonly IEmailService _emailService;
        private readonly IAdminService _adminService;

        public AdminController(
            IGeneralModelRepository generalModelRepository,
            DbContextContainer dbContextContainer,
            ICourseService courseService,
            IEmailService emailService,
            IAdminService adminService,
            AppSignInManager appSignInManager,
            AppUserManager appUserManager) : base(generalModelRepository, dbContextContainer)
        {
            _courseService = courseService;
            _appUserManager = appUserManager;
            _appSignInManager = appSignInManager;
            _emailService = emailService;
            _adminService = adminService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAdminPageAsync([FromBody] LoginAgrs request)
        {
            Guards.NotNullOrEmpty(request.Username, nameof(request.Username));
            Guards.NotNullOrEmpty(request.Password, nameof(request.Password));
            var user = await _appUserManager.FindByNameAsync(request.Username);
            if (!(await _appSignInManager.CanSignInAsync(user)) || user.RoleAccount != RoleAccount.Admin)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "User is disabled.",
                    Message = "Login failed"
                });
            }
            var result = await _appSignInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Login failed"
                });

            }
            await _appSignInManager.SignInAsync(user, false);
            var tokenClient = new TokenClient(new HttpClient() { BaseAddress = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/connect/token") }, new TokenClientOptions { ClientId = "courseclient", ClientSecret = "CourseApi" });
            var tokenResponse = await tokenClient.RequestPasswordTokenAsync(request.Username, request.Password);
            return Ok(new GeneralResponse<LoginResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new LoginResponse
                {
                    FullName = user.NormalizedDisplayName,
                    Email = user.Email,
                    Username = user.UserName,
                    Role = user.RoleAccount,
                    Token = tokenResponse.AccessToken,
                    RefreshToken = ""
                },
                Message = "Login successfull"
            });
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAdminPageAsync([FromBody] RegisterNewUserRequest request)
        {
            var registerNewUserData = new RegisterNewUserData
            {
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PersonalEmail = request.PersonalEmail,
                PersonalPhoneNumber = request.PersonalPhoneNumber,
                StudentID = request.StudentID,
                Username = request.Username,
                Password = request.Password,
            };
            var user = await _adminService.Register(registerNewUserData);
            var token = HttpUtility.UrlEncode(await _appUserManager.GenerateEmailConfirmationTokenAsync(user));
            var confirmationLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Email/ConfirmEmail?token={token}&email={user.Email}";
            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendConfirmMail(user.Email, confirmationLink);
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
        [Route("admin-account")]
        public async Task<IActionResult> GetListAdminAccountAsync([FromQuery] AdminAccountQuery query)
        {
            var account = await GetSearchResult(query, c => new UserResponse(c));
            return Ok(new GeneralResponse<GeneralResultResponse<UserResponse>>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = account,
                Message = "Get admin account sucessfull"
            });
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("admin-account/{id}")]
        public async Task<IActionResult> GetAdminAccountAsync(int id)
        {
            var user = await GeneralModelRepository.GetQueryable<AppUser>().Where(u => u.Id == id && u.RoleAccount == RoleAccount.Admin).Select(u => new UserResponse(u)).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "User not found.",
                    Message = "User not found."
                });
            }
            return Ok(new GeneralResponse<UserResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = user,
                Message = "Get admin successfully"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("admin-account/create-account")]
        public async Task<IActionResult> CreateNewAdminAccountAsync([FromBody] RegisterNewUserRequest request)
        {
            var registerNewUserData = new RegisterNewUserData
            {
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PersonalEmail = request.PersonalEmail,
                PersonalPhoneNumber = request.PersonalPhoneNumber,
                Username = request.Username,
                Password = request.Password,
            };
            var user = await _adminService.Register(registerNewUserData);
            var token = HttpUtility.UrlEncode(await _appUserManager.GenerateEmailConfirmationTokenAsync(user));
            var confirmationLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Email/ConfirmEmail?token={token}&email={user.Email}";
            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendConfirmMail(user.Email, confirmationLink);
            if (emailResponse)
            {
                Console.WriteLine("Email");
            }
            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Create new comment review sucessfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("user-account")]
        public async Task<IActionResult> GetListUserAccountAsync([FromQuery] UserAccountQuery query)
        {
            var account = await GetSearchResult(query, c => new UserResponse(c));
            return Ok(new GeneralResponse<GeneralResultResponse<UserResponse>>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = account,
                Message = "Create new comment review sucessfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("user-account/change-student-id")]
        public async Task<IActionResult> ChangeStudentIdUserAccountAsync([FromBody] ChangeStudentIDRequest request)
        {
            var user = await _appUserManager.FindByNameAsync(request.CurrentUser);
            if (user == null || user.RoleAccount != RoleAccount.Admin )
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }
            var userAccount = await _appUserManager.FindByNameAsync(request.Username);
            if (userAccount == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = $"Not found account { request.Username }",
                    Message = $"Not found account {request.Username}"
                });
            }

            if (!String.IsNullOrEmpty(request.MSSV))
            {
                var studentExist = GeneralModelRepository.GetQueryable<AppUser>().Where(u => u.StudentID == request.MSSV).FirstOrDefault();
                if (studentExist != null)
                {
                    return BadRequest(new GeneralResponse<string>
                    {
                        Status = ApiResponseStatus.Error,
                        Result = ResponseResult.Error,
                        Content = $"Student has been exist",
                        Message = $"Student has been exist"
                    });
                }
            }

            await _adminService.UpdateStudentIDAsync(new ManageCourse.Core.Model.Args.UpdateStudentIDArgs { 
                MSSV = request.MSSV,
                Username = request.Username
            });
            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Create new comment review sucessfull"
            });
        } 
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("user-account/lock-account")]
        public async Task<IActionResult> LockUserAccountAsync(ApprovalAccountRequest request)
        {
            var user = await _appUserManager.FindByNameAsync(request.CurrentUser);
            if (user == null || user.RoleAccount != RoleAccount.Admin)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }
            var userAccount = await _appUserManager.FindByNameAsync(request.Username);
            if (userAccount == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = $"Not found account { request.Username }",
                    Message = $"Not found account {request.Username}"
                });
            }

            await _adminService.ManagerAccountAsync(new ManageCourse.Core.Model.Args.ApprovalAccountArgs { 
                Username = request.Username,
                UserStatus = UserStatus.Locked,
                CurrentUser = request.CurrentUser
            });

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Create new comment review sucessfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("user-account/unlock-account")]
        public async Task<IActionResult> UnlockUserAccountAsync(ApprovalAccountRequest request)
        {
            var user = await _appUserManager.FindByNameAsync(request.CurrentUser);
            if (user == null || user.RoleAccount != RoleAccount.Admin)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }
            var userAccount = await _appUserManager.FindByNameAsync(request.Username);
            if (userAccount == null)
            {
                return BadRequest(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = $"Not found account { request.Username }",
                    Message = $"Not found account {request.Username}"
                });
            }

            await _adminService.ManagerAccountAsync(new ManageCourse.Core.Model.Args.ApprovalAccountArgs
            {
                Username = request.Username,
                UserStatus = UserStatus.Active,
                CurrentUser = request.CurrentUser
            });
            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Create new comment review sucessfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("course")]
        public async Task<IActionResult> GetListCourseAsync([FromQuery] CourseQuery courseQuery)
        {
            var user = await _appUserManager.FindByNameAsync(courseQuery.CurrentUser);
            if (user == null || user.RoleAccount != RoleAccount.Admin)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }
            var coursers = await GetSearchResult(courseQuery, c => new CourseResponse(c));
            foreach (var course in coursers.Data)
            {
                course.Owner = (await _appUserManager.FindByNameAsync(course.Owner)).NormalizedDisplayName;
            }   
            return Ok(new GeneralResponse<GeneralResultResponse<CourseResponse>>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = coursers,
                Message = "Create new comment review sucessfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("course/{id}")]
        public async Task<IActionResult> GetCourseByIdAsync(int id, string currentUser)
        {
            var user = await _appUserManager.FindByNameAsync(currentUser);
            if (user == null || user.RoleAccount != RoleAccount.Admin)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }

            var coursers = (await _courseService.GetByIdAsync(id));
            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new
                {
                    course = new CourseResponse(coursers),
                },
                Message = "Get class sucessfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("course/assignments")]
        public async Task<IActionResult> GetAssignmentsAsync([FromQuery] AssignmentsQuery query)
        {
            var user = await _appUserManager.FindByNameAsync(query.CurrentUser);
            if (user == null || user.RoleAccount != RoleAccount.Admin)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }
            var result = await GetSearchResult(query, a => new AssignmentsResponse(a));
            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = result,
                Message = "Get assignments sucessfully"
            });
        }
    }
}
