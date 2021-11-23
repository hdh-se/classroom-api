using ManageCourse.Core.Constansts;
using Microsoft.AspNetCore.Identity;
using System;

namespace ManageCourse.Core.Data
{
    public class AppUser: IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string StudentID { get; set; }
        public string Address { get; set; }
        public string NormalizedDisplayName { get; set; }
        public string PersonalEmail { get; set; }
        public bool PersonalEmailConfirmed { get; set; }
        public string NormalizedPersonalEmail { get; set; }
        public string PersonalPhoneNumber { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateOn { get; set; }
        public UserStatus UserStatus { get; set; }
    }
}
