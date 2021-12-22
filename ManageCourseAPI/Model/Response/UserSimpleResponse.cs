using ManageCourse.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class UserSimpleResponse
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string StudentID { get; set; }
        public UserSimpleResponse(AppUser user)
        {
            Username = user.UserName;
            ProfileImageUrl = user.ProfileImageUrl;
            StudentID = user.StudentID;
            FirstName = user.FirstName;
            MiddleName = user.MiddleName;
            LastName = user.LastName;
        }
    }
}
