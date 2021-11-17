using IdentityModel;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using ManageCourse.Core.Data;
using ManageCourse.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.DataAuthSources
{
    public class IdentityServerProfileService : ProfileService<AppUser>
    {
        protected IUserService UserService { get; set; }

        public IdentityServerProfileService(IUserService userService,
            UserManager<AppUser> userManager,
            IUserClaimsPrincipalFactory<AppUser> claimsFactory,
            ILogger<ProfileService<AppUser>> logger)
            : base(userManager, claimsFactory, logger)
        {
            UserService = userService;
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.Identity.GetSubjectId();
            if (int.TryParse(subjectId, out var userId))
            {
                var user = await UserService.FindById(userId);

                if (!user.FirstName.IsNullOrEmpty())
                {
                    context.IssuedClaims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
                }
                if (!user.MiddleName.IsNullOrEmpty())
                {
                    context.IssuedClaims.Add(new Claim(JwtClaimTypes.MiddleName, user.MiddleName));
                }
                if (!user.LastName.IsNullOrEmpty())
                {
                    context.IssuedClaims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
                }

                var emailClaim = new Claim(JwtClaimTypes.Email, user.Email ?? user.PersonalEmail);
                context.IssuedClaims.Add(emailClaim);
                var usernameClaim = new Claim(JwtClaimTypes.PreferredUserName, user.UserName);
                context.IssuedClaims.Add(usernameClaim);
            }
            await base.GetProfileDataAsync(context);
        }

        public override Task IsActiveAsync(IsActiveContext context)
        {
            return base.IsActiveAsync(context);
        }
    }

}
