using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.DTOs;
using ManageCourse.Core.Model.Args;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services.Implementation
{
    public class AdminService: IAdminService
    {
        public const int DefaultPageIndex = 1;
        public const int DefaultPageSize = 10;

        protected AuthDbContext AuthDbContext { get; private set; }
        protected ILookupNormalizer LookupNormalizer { get; private set; }
        protected IPasswordHasher<AppUser> PasswordHasher { get; private set; }
        protected AppUserManager UserManager { get; private set; }
        protected IAppUserStore UserStore { get; private set; }
        public AdminService(AuthDbContext authDbContext,
            ILookupNormalizer lookupNormalizer,
            IPasswordHasher<AppUser> passwordHasher,
            AppUserManager userManager,
            IAppUserStore userStore)
        {
            AuthDbContext = authDbContext;
            LookupNormalizer = lookupNormalizer;
            PasswordHasher = passwordHasher;
            UserManager = userManager;
            UserStore = userStore;
        }

        public async Task<AppUser> Register(RegisterNewUserData data)
        {
            var now = DateTime.Now;
            var user = new AppUser
            {
                UserName = data.Username,
                FirstName = data.FirstName,
                MiddleName = data.MiddleName,
                LastName = data.LastName,
                Email = data.Email,
                PersonalEmail = data.PersonalEmail,
                PhoneNumber = data.PhoneNumber,
                PersonalPhoneNumber = data.PersonalPhoneNumber,
                RoleAccount = RoleAccount.Admin,
                CreateBy = data.Username,
                CreateOn = DateTime.Now,
                UpdateBy = data.Username,
                UpdateOn = DateTime.Now
            };

            var createResult = await UserManager.CreateUserAsync(user, data.Password);
            if (createResult.Succeeded)
            {
                return user;
            }
            var error = GetErrorMessages(createResult.Errors);
            throw new NotImplementedException(error);
        }
        private string GetErrorMessages(IEnumerable<IdentityError> identityErrors)
        {
            var messages = identityErrors.Select(x => x.Description);
            return string.Join(",", messages);
        }

        public async Task<AppUser> UpdateStudentIDAsync(UpdateStudentIDArgs args)
        {
            var user = await UserManager.FindByNameAsync(args.Username);
            user.StudentID = args.MSSV;
            AuthDbContext.Update(user);
            await AuthDbContext.SaveChangesAsync();
            return user;
        }
        public async Task<AppUser> ManagerAccountAsync(ApprovalAccountArgs args)
        {
            var user = await UserManager.FindByNameAsync(args.Username);
            user.UserStatus = args.UserStatus;
            user.UpdateBy = args.CurrentUser;
            user.UpdateOn = DateTime.Now;
            AuthDbContext.Update(user);
            await AuthDbContext.SaveChangesAsync();
            return user;
        }
    }
}
