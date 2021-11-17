using ManageCourse.Core.Data;
using ManageCourse.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services
{
    public interface IUserService
    {
        Task<ChangeUserPasswordResult> ChangePassword(ChangeUserPasswordData data);
        Task<FullAppUser> FindById(int id);
        Task<AppUser> Register(RegisterNewUserData data);
        Task<PagingResult<FullAppUser>> Search(SearchUserData data);
        Task<AppUser> UpdateProfile(int userId, UpdateUserProfileData data);
        Task<UpdateUserRoleResult> UpdateRole(int id, UpdateUserRoleData data);
        Task<UpdateUserStatusResult> UpdateStatus(int id, UpdateUserStatusData data);
    }
}
