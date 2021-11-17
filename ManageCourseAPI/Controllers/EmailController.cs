using ManageCourse.Core.DataAuthSources;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Controllers
{

    [Route("Email")]
    public class EmailController: ControllerBase
    {
        private AppUserManager appUserManager;

        public EmailController(AppUserManager appUserManager)
        {
            this.appUserManager = appUserManager;
        }
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await appUserManager.FindByEmailAsync(email);
            if (user == null)
                return Ok("Not found user");

            var result = await appUserManager.ConfirmEmailAsync(user, token);
            return Ok(result.Succeeded ? "ConfirmEmail" : "Error");
        }
    }
}
