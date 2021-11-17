using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.DTOs
{
    public class FullAppUser: AppUser
    {
        public FullAppUser(AppUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            NormalizedEmail = user.NormalizedEmail;
            Email = user.Email;
            NormalizedEmail = user.NormalizedEmail;
            EmailConfirmed = user.EmailConfirmed;
            PasswordHash = user.PasswordHash;
            SecurityStamp = user.SecurityStamp;
            ConcurrencyStamp = user.ConcurrencyStamp;
            PhoneNumber = user.PhoneNumber;
            PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            TwoFactorEnabled = user.TwoFactorEnabled;
            LockoutEnd = user.LockoutEnd;
            LockoutEnabled = user.LockoutEnabled;
            AccessFailedCount = user.AccessFailedCount;
            FirstName = user.FirstName;
            MiddleName = user.MiddleName;
            LastName = user.LastName;
            PersonalEmail = user.PersonalEmail;
            NormalizedPersonalEmail = user.NormalizedPersonalEmail;
            PersonalEmailConfirmed = user.PersonalEmailConfirmed;
            PersonalPhoneNumber = user.PersonalPhoneNumber;
            UserStatus = user.UserStatus;
        }
    }
}
