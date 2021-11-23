using LinqKit;
using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.DTOs;
using ManageCourse.Core.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services.Implementation
{
    public class UserService : IUserService
    {
        public const int DefaultPageIndex = 1;
        public const int DefaultPageSize = 10;

        protected AuthDbContext AuthDbContext { get; private set; }
        protected ILookupNormalizer LookupNormalizer { get; private set; }
        protected IPasswordHasher<AppUser> PasswordHasher { get; private set; }
        protected AppUserManager UserManager { get; private set; }
        protected IAppUserStore UserStore { get; private set; }
        public UserService(AuthDbContext authDbContext,
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
        public async Task<ChangeUserPasswordResult> ChangePassword(ChangeUserPasswordData data)
        {
            var user = await UserManager.FindByNameAsync(data.Username);
            if (user == null)
            {
                throw new NotImplementedException("Invalid username or password.");
            }
            if (data.CheckOldPassword)
            {
                var isOldPasswordValid = await CheckOldPassword(user, data.OldPassword);
                if (!isOldPasswordValid)
                {
                    throw new NotImplementedException("Invalid username or password.");
                }
            }

            var setPasswordResult = await UserManager.SetUserPasswordAsync(user, data.NewPassword);
            if (!setPasswordResult.Succeeded)
            {
                throw new NotImplementedException("Failed to set new password.");
            }

            return new ChangeUserPasswordResult
            {
                IsSuccess = true,
                NewPassword = data.NewPassword,
            };
        }

        private async Task<bool> CheckOldPassword(AppUser user, string password)
        {
            var checkOldPasswordResult = await UserManager.CheckPasswordAsync(user, password);
            return checkOldPasswordResult;
        }
        public async Task<FullAppUser> FindById(int id)
        {
            var user = await AuthDbContext.Users
                .SingleOrDefaultAsync(usr => usr.Id == id);
            if (user == null)
            {
                throw new NotImplementedException($"User with ID {id} is not found.");
            }
            var fullUser = new FullAppUser(user);

            return fullUser;
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

        public async Task<PagingResult<FullAppUser>> Search(SearchUserData data)
        {
            var pageIndex = data.PageIndex ?? DefaultPageIndex;
            var pageSize = data.PageSize ?? DefaultPageSize;

            var userRoleNameQuery = from usr in AuthDbContext.Users
                                    join usrRole in AuthDbContext.UserRoles.DefaultIfEmpty()
                                    on usr.Id equals usrRole.UserId into usrUsrRole
                                    from usrRole in usrUsrRole.DefaultIfEmpty()
                                    group new { usrRole.RoleId } by usr.Id into usrRoleGrp
                                    select new UserRoleName { UserId = usrRoleGrp.Key, RoleId = usrRoleGrp.Min(item => item.RoleId) };
            var predicate = PredicateBuilder.New<UserRoleNameJoin>(true);

            if (data.Query != null)
            {
                var normalizedQuery = LookupNormalizer.NormalizeName(data.Query);
                predicate.And(x => x.User.NormalizedDisplayName.Contains(normalizedQuery) || x.User.NormalizedUserName.Contains(normalizedQuery));
            }
            var searchUserQuery = AuthDbContext.Users
                .Join(userRoleNameQuery, usr => usr.Id, usrRoleName => usrRoleName.UserId, (usr, usrRoleName) => new UserRoleNameJoin { User = usr, RoleId = usrRoleName.RoleId })
                .Where(predicate);
            searchUserQuery = Sort(searchUserQuery, data.SortColumn, data.SortDirection);
            var users = await searchUserQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(join => new FullAppUser(join.User))
                .ToListAsync();
            var count = await searchUserQuery.CountAsync();
            var result = new PagingResult<FullAppUser>
            {
                Results = users,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
            return result;
        }

        private IQueryable<UserRoleNameJoin> Sort(IQueryable<UserRoleNameJoin> query, string columnName, SortDirection? direction)
        {
            var sortColumns = new Expression<Func<UserRoleNameJoin, object>>[] { usr => usr.User.Id };
            var sortDirection = SortDirection.Descending;
            var mappings = new[]
            {
                new KeyValuePair<string, Expression<Func<UserRoleNameJoin, object>>> ("login", join => join.User.NormalizedUserName ),
                new KeyValuePair<string, Expression<Func<UserRoleNameJoin, object>>> ("name", join => join.User.NormalizedDisplayName ),
                new KeyValuePair<string, Expression<Func<UserRoleNameJoin, object>>> ("role", join => join.RoleId ),
                new KeyValuePair<string, Expression<Func<UserRoleNameJoin, object>>> ("status", join => join.User.UserStatus ),
            };
            if (mappings.Any(mapping => mapping.Key == columnName))
            {
                sortColumns = mappings.Where(mapping => mapping.Key == columnName).Select(mapping => mapping.Value).ToArray();
                sortDirection = direction ?? SortDirection.Ascending;
            }
            return SortInner(query, sortColumns, sortDirection);
        }

        private IQueryable<UserRoleNameJoin> SortInner(IQueryable<UserRoleNameJoin> query, ICollection<Expression<Func<UserRoleNameJoin, object>>> sortColumns, SortDirection direction)
        {
            for (var i = 0; i < sortColumns.Count; i++)
            {
                var sortColumn = sortColumns.ElementAt(i);
                if (i == 0)
                {
                    query = direction == SortDirection.Ascending
                        ? query.OrderBy(sortColumn)
                        : query.OrderByDescending(sortColumn);
                    continue;
                }
                var orderQuery = (IOrderedQueryable<UserRoleNameJoin>)query;
                query = direction == SortDirection.Ascending
                    ? orderQuery.ThenBy(sortColumn)
                    : orderQuery.ThenByDescending(sortColumn);
            }
            return query;
        }

        public async Task<AppUser> UpdateProfile(int userId, UpdateUserProfileData data)
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotImplementedException($"User with ID {userId} is not found.");
            }

            user.Email = data.Email;
            user.PhoneNumber = data.PhoneNumber;
            user.FirstName = data.FirstName;
            user.MiddleName = data.MiddleName;
            user.LastName = data.LastName;
            user.PersonalEmail = data.PersonalEmail;
            user.PersonalPhoneNumber = data.PersonalPhoneNumber;
            user.UpdateBy = user.UserName;
            user.UpdateOn = DateTime.Now;
            var updateResult = await UserManager.UpdateUserAsync(user);
            if (updateResult.Succeeded)
            {
                return user;
            }

            var error = GetErrorMessages(updateResult.Errors);
            throw new NotImplementedException(error);
        }

        public Task<UpdateUserRoleResult> UpdateRole(int id, UpdateUserRoleData data)
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateUserStatusResult> UpdateStatus(int id, UpdateUserStatusData data)
        {
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotImplementedException($"User with ID {id} is not found.");
            }

            user.UserStatus = data.UserStatus;

            var updateResult = await UserManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                return new UpdateUserStatusResult
                {
                    UserId = id,
                    UserStatus = data.UserStatus,
                };
            }

            var error = GetErrorMessages(updateResult.Errors);
            throw new NotImplementedException(error);
        }
    }

    public class UserRoleName
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
    public class UserRoleNameJoin
    {
        public AppUser User { get; set; }
        public int RoleId { get; set; }
    }
}
