using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManageCourse.Core.DataAuthSources
{
    public interface IAppUserEmailStore : IUserEmailStore<AppUser>
    {
        Task<string> GetPersonalEmailAsync(AppUser user, CancellationToken cancellationToken = default(CancellationToken));
        Task SetNormalizedPersonalEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IAppUserStore : IUserStore<AppUser>
    {
        Task<IdentityResult> CreateUserAsync(AppUser user, CancellationToken cancellationToken = default(CancellationToken));
        Task SetNormalizedDisplayNameAsync(AppUser user, string normalizedDisplayName, CancellationToken cancellationToken = default(CancellationToken));
        Task<IdentityResult> UpdateUserAsync(AppUser user, UserUpdateFlag flag, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class AppUserStore : UserStore<AppUser, IdentityRole<int>, AuthDbContext, int>, IAppUserStore, IAppUserEmailStore
    {
        public AppUserStore(AuthDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }
        public virtual async Task<IdentityResult> CreateUserAsync(AppUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                Context.Add(user);
                await SaveChanges(cancellationToken);
                await transaction.CommitAsync();
            }
            return IdentityResult.Success;
        }
        public virtual Task<string> GetPersonalEmailAsync(AppUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PersonalEmail);
        }

        public virtual Task SetNormalizedPersonalEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedPersonalEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public virtual Task SetNormalizedDisplayNameAsync(AppUser user, string normalizedDisplayName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedDisplayName = normalizedDisplayName;
            return Task.CompletedTask;
        }

        public virtual async Task<IdentityResult> UpdateUserAsync(AppUser user, UserUpdateFlag flag, CancellationToken cancellationToken = default(CancellationToken))
        {
            IdentityResult result = null;
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                result = await UpdateAsync(user, cancellationToken);
                if (!result.Succeeded)
                {
                    return result;
                }
                //if (flag.HasFlag(UserUpdateFlag.Password))
                //{
                //    await SavePasswordHistoryInnerAsync(user, cancellationToken);
                //}
                //if (flag.HasFlag(UserUpdateFlag.AuthData))
                //{
                //    await SaveUserDesignationsInnerAsync(user, authData.DesignationIds, cancellationToken);
                //    await SaveRoleIdsInnerAsync(user, authData.RoleIds, cancellationToken);
                //    await SaveUserClaimsInnerAsync(user, authData, cancellationToken);
                //}
                await transaction.CommitAsync();
            }
            return result;
        }
    }
}
