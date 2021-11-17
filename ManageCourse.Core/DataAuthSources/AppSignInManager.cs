using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ManageCourse.Core.DataAuthSources
{
    public class AppSignInManager : SignInManager<AppUser>
    {
        private readonly IUserConfirmation<AppUser> _confirmation;
        public AppSignInManager(UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<AppUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<AppUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<AppUser> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _confirmation = confirmation;
        }
        public override async Task<bool> CanSignInAsync(AppUser user)
        {
            if (user.UserStatus != UserStatus.Active)
            {
                Logger.LogWarning(5, "User is disabled.");
                return false;
            }
            if (Options.SignIn.RequireConfirmedEmail && !(await UserManager.IsEmailConfirmedAsync(user)))
            {
                Logger.LogWarning(0, "User cannot sign in without a confirmed email.");
                return false;
            }
            if (Options.SignIn.RequireConfirmedPhoneNumber && !(await UserManager.IsPhoneNumberConfirmedAsync(user)))
            {
                Logger.LogWarning(1, "User cannot sign in without a confirmed phone number.");
                return false;
            }
            if (Options.SignIn.RequireConfirmedAccount && !(await _confirmation.IsConfirmedAsync(UserManager, user)))
            {
                Logger.LogWarning(4, "User cannot sign in without a confirmed account.");
                return false;
            }
            return true;
        }
    }
}
