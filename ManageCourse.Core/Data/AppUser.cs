using ManageCourse.Core.Constansts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class AppUser: IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string NormalizedDisplayName { get; set; }
        public string PersonalEmail { get; set; }
        public bool PersonalEmailConfirmed { get; set; }
        public string NormalizedPersonalEmail { get; set; }
        public string PersonalPhoneNumber { get; set; }
        public UserStatus UserStatus { get; set; }
    }
}
