using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DTOs;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Services;
using ManageCourseAPI.Model.Request;
using ManageCourseAPI.Model.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ManageCourseAPI.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("users")]
    public class UserController : ControllerBase
    {
        public UserController(IUserService userService, AppUserManager appUserManager)
        {
            UserService = userService;
            AppUserManager = appUserManager;
        }

        protected IUserService UserService { get; private set; }
        protected AppUserManager AppUserManager { get; private set; }

        [HttpPost, Route("")]
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
    }
}
