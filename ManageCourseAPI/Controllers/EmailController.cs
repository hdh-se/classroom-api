using ManageCourse.Core.Constansts;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Controllers
{

    [Route("Email")]
    public class EmailController : ControllerBase
    {
        private AppUserManager appUserManager;
        private IGeneralModelRepository generalModelRepository;

        public EmailController(AppUserManager appUserManager, IGeneralModelRepository repository)
        {
            this.appUserManager = appUserManager;
            this.generalModelRepository = repository;
        }
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await appUserManager.FindByEmailAsync(email);
            if (user == null)
                return Ok("Not found user");

            var result = await appUserManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.UserStatus = UserStatus.Active;
                await generalModelRepository.Update(user);

            }
            return Ok(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var user = await appUserManager.FindByEmailAsync(email);
            if (user == null)
                return Ok("Not found user");

            var result = await appUserManager.ResetPasswordAsync(user, token, ConfigConstant.PASSWORD_DEFAULT);
            return Ok(result.Succeeded ?
                ConfigConstant.PASSWORD_DEFAULT : "Error");
        }
    }
}
