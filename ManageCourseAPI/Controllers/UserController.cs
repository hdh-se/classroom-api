using ManageCourse.Core.DTOs;
using ManageCourse.Core.Services;
using ManageCourseAPI.Model.Request;
using ManageCourseAPI.Model.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("users")]
    public class UserController: ControllerBase
    {
        protected IUserService UserService { get; private set; }

        public UserController(IUserService userService)
        {
            UserService = userService;
        }

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
            var response = new SingularResponse<UserResponse>
            {
                Result = new UserResponse(user),
            };
            return Created($"{Request.Path}/{user.Id}", response);
        }
    }
}
