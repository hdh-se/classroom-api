using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public string ProfileImageUrl { get; set; }
        public string PersonalEmail { get; set; }
        public string StudentID { get; set; }
        public string PhoneNumber { get; set; }
        public string PersonalPhoneNumber { get; set; }
        public UserStatus UserStatus { get; set; }
        public UserResponse(AppUser user)
        {
            Id = user.Id;
            Username = user.UserName;
            ProfileImageUrl = user.ProfileImageUrl;
            Email = user.Email;
            Gender = user.Gender;
            StudentID = user.StudentID;
            PhoneNumber = user.PhoneNumber;
            FirstName = user.FirstName;
            MiddleName = user.MiddleName;
            LastName = user.LastName;
            PersonalEmail = user.PersonalEmail;
            PersonalPhoneNumber = user.PersonalPhoneNumber;
            UserStatus = user.UserStatus;
        }
    }
}
