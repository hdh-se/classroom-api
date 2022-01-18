using ManageCourse.Core.Data;
using ManageCourse.Core.DTOs;
using ManageCourse.Core.Model.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services
{
    public interface IAdminService
    {
        public Task<AppUser> Register(RegisterNewUserData data);
        public Task<AppUser> UpdateStudentIDAsync(UpdateStudentIDArgs args);
        public Task<AppUser> ManagerAccountAsync(ApprovalAccountArgs args)

    }
}
