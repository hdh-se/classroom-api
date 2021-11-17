using IdentityServer4.AspNetIdentity;
using ManageCourse.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ManageCourse.Core.DataAuthSources
{
    public class IdentityServerResourceOwnerPasswordValidator : ResourceOwnerPasswordValidator<AppUser>
    {
        public IdentityServerResourceOwnerPasswordValidator(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<ResourceOwnerPasswordValidator<AppUser>> logger) : base(userManager, signInManager, logger)
        {
        }
    }
}
