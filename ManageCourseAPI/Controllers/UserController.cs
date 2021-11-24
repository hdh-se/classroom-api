using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using IdentityServer4.Services;
using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.DTOs;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Services;
using ManageCourse.Core.Utilities;
using ManageCourseAPI.Model.Request;
using ManageCourseAPI.Model.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ManageCourseAPI.Controllers
{
    [Route("users")]
    public class UserController : ControllerBase
    {
        public UserController(
            IUserService userService, 
            AppUserManager appUserManager, 
            SignInManager<AppUser> signInManager, 
            IIdentityServerInteractionService interactionService,
            AuthDbContext authDbContext
            )
        {
            UserService = userService;
            AppUserManager = appUserManager;
            SignInManager = signInManager;
            InteractionService = interactionService;
            AuthDbContext = authDbContext;

        }

        protected IUserService UserService { get; private set; }
        protected AppUserManager AppUserManager { get; private set; }
        protected SignInManager<AppUser> SignInManager { get; private set; }
        protected IIdentityServerInteractionService InteractionService { get; private set; }
        protected AuthDbContext AuthDbContext { get; private set; }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Create([FromBody] RegisterNewUserRequest request)
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
            var user = await UserService.Register(registerNewUserData);
            var token = HttpUtility.UrlEncode(await AppUserManager.GenerateEmailConfirmationTokenAsync(user));
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

        [HttpPost, Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginAgrs request)
        {
            Guards.NotNullOrEmpty(request.Username, nameof(request.Username));
            Guards.NotNullOrEmpty(request.Password, nameof(request.Password));
            var user = await AppUserManager.FindByNameAsync(request.Username);
            var result = await SignInManager.CheckPasswordSignInAsync(user, request.Password, false);
            await SignInManager.SignInAsync(user, false);
            var tokenClient = new TokenClient(new HttpClient() { BaseAddress = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/connect/token") }, new TokenClientOptions { ClientId = "courseclient", ClientSecret = "CourseApi" });
            var tokenResponse = await tokenClient.RequestPasswordTokenAsync(request.Username, request.Password);
            return Ok(new GeneralResponse<LoginResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new LoginResponse { 
                    FullName = user.NormalizedDisplayName,
                    Email = user.Email,
                    Username = user.UserName,
                    Token = tokenResponse.AccessToken , 
                    RefreshToken=""},
                Message = "Login successfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost, Route("update")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateProfileUser userArgs)
        {
            var user = await AppUserManager.FindByNameAsync(userArgs.CurrentUser);
            if (user==null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "User not found"
                });
            }
            var userData = new UpdateUserProfileData();
            userArgs.CopyPropertiesTo(userData);
            await UserService.UpdateProfile(user.Id, userData);
            return Ok(new GeneralResponse<UserResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new UserResponse(user),
                Message = "Update profile successfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost, Route("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            var user = await AppUserManager.FindByNameAsync(changePasswordRequest.CurrentUser);
            if (user==null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "User not found"
                });
            }
            var result = await AppUserManager.ChangePasswordAsync(user,changePasswordRequest.CurrentPassWord,changePasswordRequest.NewPassWord);
            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Update password successfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet, Route("profile")]
        public async Task<IActionResult> GetProfileAsync (string username)
        {
            var user = await AppUserManager.FindByNameAsync(username);
            if (user==null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "User not found"
                });
            }

            return Ok(new GeneralResponse<UserResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new UserResponse(user),
                Message = "Get profile successfull"
            });
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            return Ok("LoggedOut");
        }

        [HttpGet, Route("login")]
        public async Task<IActionResult> Login()
        {
            var listprovider = (await SignInManager.GetExternalAuthenticationSchemesAsync()).FirstOrDefault();
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(listprovider.Name, "https://localhost:44344/users/login-external-callback");
            return Challenge(properties, listprovider.Name);
        }

        [HttpGet, Route("login-external-callback")]
        public async Task<IActionResult> LoginExternalCallback(string returnUrl)
        {
            var listprovider = (await SignInManager.GetExternalAuthenticationSchemesAsync()).FirstOrDefault();
            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest();
            }

            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                await SignInManager.UpdateExternalAuthenticationTokensAsync(info);
                var userExist = AuthDbContext.Users.Where(t => t.Email == info.Principal.FindFirstValue(ClaimTypes.Email)).FirstOrDefault();
                var tokenClient = new TokenClient(new HttpClient() { BaseAddress = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/connect/token") }, new TokenClientOptions { ClientId = "courseclient", ClientSecret = "CourseApi" });
                var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync("courseapi.read");

                return Redirect($"{ConfigClient.URL_CLIENT}/login?token={tokenResponse.AccessToken}&email={userExist.Email}&username={userExist.UserName}"); 
                //return Ok(new GeneralResponse<LoginResponse>
                //{
                //    Status = ApiResponseStatus.Success,
                //    Result = ResponseResult.Successfull,
                //    Content = new LoginResponse {
                //        FullName = userExist.NormalizedDisplayName,
                //        Email = userExist.Email,
                //        Username = userExist.UserName,
                //        Token = tokenResponse.AccessToken , 
                //        RefreshToken=""},
                //    Message = "Login successfull"
                //});
            }
            string email = info.Principal.FindFirstValue(ClaimTypes.Email);
            string FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? info.Principal.FindFirstValue(ClaimTypes.Name);
            string LastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
            var user = new AppUser
            {
                UserName = StringHelper.GenerateCode(5),
                Email = email,
                FirstName = FirstName,
                LastName = LastName,
                UserStatus = UserStatus.Active,
                EmailConfirmed = true
            };
            var resultRegister = await AppUserManager.CreateUserAsync(user);
            if (resultRegister.Succeeded)
            {
                await SignInManager.SignInAsync(user, false);
                await AppUserManager.AddLoginAsync(user, info);
                var claimsPrincipal = await SignInManager.CreateUserPrincipalAsync(user);
                var tokenClient = new TokenClient(new HttpClient() { BaseAddress = new Uri($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/connect/token") }, new TokenClientOptions { ClientId = "courseclient", ClientSecret = "CourseApi" });
                var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync("courseapi.read");
                return Redirect($"{ConfigClient.URL_CLIENT}/login?token={tokenResponse.AccessToken}&email={user.Email}&username={user.UserName}"); 
                //Ok(new GeneralResponse<LoginResponse>
                //{
                //    Status = ApiResponseStatus.Success,
                //    Result = ResponseResult.Successfull,
                //    Content = new LoginResponse {
                //        FullName = user.NormalizedDisplayName,
                //        Email = user.Email,
                //        Username = user.UserName,
                //        Token = tokenResponse.AccessToken, 
                //        RefreshToken = "" },
                //    Message = "Login successfull"
                //});
            }

            return Redirect($"{ConfigClient.URL_CLIENT}/login");
        }
    }
}
