using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class RegisterNewUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string NormalizedDisplayName { get; set; }
        public string PersonalEmail { get; set; }
        public bool PersonalEmailConfirmed { get; set; }
        public string NormalizedPersonalEmail { get; set; }
        public string PersonalPhoneNumber { get; set; }
    }
}
