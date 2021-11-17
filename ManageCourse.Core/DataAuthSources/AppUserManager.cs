using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.Frameworks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static ManageCourse.Core.DataAuthSources.AppUserStore;

namespace ManageCourse.Core.DataAuthSources
{
    public class AppUserManager : UserManager<AppUser>
    {
        private readonly IServiceProvider _services;
        public AppUserManager(IUserStore<AppUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AppUser> passwordHasher, IEnumerable<IUserValidator<AppUser>> userValidators, IEnumerable<IPasswordValidator<AppUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _services = services;
        }
        protected IAppUserStore UserStore
        {
            get
            {
                var cast = Store as IAppUserStore;
                if (cast == null)
                {
                    throw new NotSupportedException($"{nameof(UserStore)} is unavailable.");
                }
                return cast;
            }
        }

        protected IAppUserEmailStore UserEmailStore
        {
            get
            {
                var cast = Store as IAppUserEmailStore;
                if (cast == null)
                {
                    throw new NotSupportedException($"{nameof(UserEmailStore)} is unavailable.");
                }
                return cast;
            }
        }

        protected IUserLockoutStore<AppUser> UserLockoutStore
        {
            get
            {
                var cast = Store as IUserLockoutStore<AppUser>;
                if (cast == null)
                {
                    throw new NotSupportedException($"{nameof(UserLockoutStore)} is unavailable.");
                }
                return cast;
            }
        }

        protected IUserPasswordStore<AppUser> UserPasswordStore
        {
            get
            {
                var cast = Store as IUserPasswordStore<AppUser>;
                if (cast == null)
                {
                    throw new NotSupportedException($"{nameof(UserPasswordStore)} is unavailable.");
                }
                return cast;
            }
        }

        protected IUserSecurityStampStore<AppUser> UserSecurityStampStore
        {
            get
            {
                var cast = Store as IUserSecurityStampStore<AppUser>;
                if (cast == null)
                {
                    throw new NotSupportedException($"{nameof(UserSecurityStampStore)} is unavailable.");
                }
                return cast;
            }
        }
        public virtual async Task<IdentityResult> CreateUserAsync(AppUser user, string password)
        {
            ThrowIfDisposed();
            var passwordStore = UserPasswordStore;
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            var result = await UpdatePasswordHash(passwordStore, user, password);
            if (!result.Succeeded)
            {
                return result;
            }
            return await CreateUserAsync(user);
        }

        public virtual async Task<IdentityResult> CreateUserAsync(AppUser user)
        {
            ThrowIfDisposed();
            await UpdateSecurityStampInternal(user);
            var result = await ValidateUserAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }
            if (Options.Lockout.AllowedForNewUsers && SupportsUserLockout)
            {
                await UserLockoutStore.SetLockoutEnabledAsync(user, true, CancellationToken);
            }
            await UpdateNormalizedUserNameAsync(user);
            await UpdateNormalizedDisplayName(user);
            await UpdateNormalizedEmailAsync(user);
            await UpdateNormalizedPersonalEmailAsync(user);

            return await UserStore.CreateUserAsync(user, CancellationToken);
        }

        public virtual async Task<string> GetUserPersonalEmailAsync(AppUser user)
        {
            ThrowIfDisposed();
            var store = UserEmailStore;
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return await store.GetPersonalEmailAsync(user, CancellationToken);
        }

        public virtual async Task<IdentityResult> SetUserPasswordAsync(AppUser user, string newPassword)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await UpdatePasswordHash(UserPasswordStore, user, newPassword);
            if (!result.Succeeded)
            {
                return result;
            }
            return await UpdateUserInnerAsync(user, UserUpdateFlag.Password);
        }

        public virtual Task<IdentityResult> UpdateUserAsync(AppUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return UpdateUserInnerAsync(user, UserUpdateFlag.AuthData);
        }

        public virtual async Task UpdateNormalizedDisplayName(AppUser user)
        {
            var nameParts = new[]
            {
                user.FirstName,
                user.MiddleName,
                user.LastName,
            };
            var displayName = string.Join(" ", nameParts.Where(name => !string.IsNullOrEmpty(name)));
            var normalizedDisplayName = KeyNormalizer.NormalizeName(displayName);
            await UserStore.SetNormalizedDisplayNameAsync(user, normalizedDisplayName);
        }

        public virtual async Task UpdateNormalizedPersonalEmailAsync(AppUser user)
        {
            var store = UserEmailStore;
            if (store != null)
            {
                var email = await GetUserPersonalEmailAsync(user);
                await store.SetNormalizedPersonalEmailAsync(user, ProtectPersonalData(NormalizeEmail(email)), CancellationToken);
            }
        }

        protected async Task<IdentityResult> UpdateUserInnerAsync(AppUser user, UserUpdateFlag flag)
        {
            var result = await ValidateUserAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }
            await UpdateNormalizedUserNameAsync(user);
            await UpdateNormalizedDisplayName(user);
            await UpdateNormalizedEmailAsync(user);
            await UpdateNormalizedPersonalEmailAsync(user);

            return await UserStore.UpdateUserAsync(user, flag, CancellationToken);
        }

        private static string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
#if NETSTANDARD2_0 || NET461
            _rng.GetBytes(bytes);
#else
            RandomNumberGenerator.Fill(bytes);
#endif
            return Base32.ToBase32(bytes);
        }

        private string ProtectPersonalData(string data)
        {
            if (Options.Stores.ProtectPersonalData)
            {
                var keyRing = _services.GetService<ILookupProtectorKeyRing>();
                var protector = _services.GetService<ILookupProtector>();
                return protector.Protect(keyRing.CurrentKeyId, data);
            }
            return data;
        }

        private async Task<IdentityResult> UpdatePasswordHash(IUserPasswordStore<AppUser> passwordStore, AppUser user, string newPassword, bool validatePassword = true)
        {
            if (validatePassword)
            {
                var validate = await ValidatePasswordAsync(user, newPassword);
                if (!validate.Succeeded)
                {
                    return validate;
                }
            }
            var hash = newPassword != null ? PasswordHasher.HashPassword(user, newPassword) : null;
            await passwordStore.SetPasswordHashAsync(user, hash, CancellationToken);
            await UpdateSecurityStampInternal(user);
            return IdentityResult.Success;
        }

        private async Task UpdateSecurityStampInternal(AppUser user)
        {
            if (SupportsUserSecurityStamp)
            {
                await UserSecurityStampStore.SetSecurityStampAsync(user, NewSecurityStamp(), CancellationToken);
            }
        }
    }
}
