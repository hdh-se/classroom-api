using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.Repositories;
using ManageCourse.Core.Services;
using ManageCourseAPI.Controllers.Common;
using ManageCourseAPI.Model.Queries;
using ManageCourseAPI.Model.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Controllers
{
    [ApiController]
    [Route("notification")]
    public class NotificationController : ApiControllerBase
    {
        private readonly AppUserManager _appUserManager;
        private readonly AppDbContext _appDbContext;
        private readonly ICourseService _courseService;
        private readonly IGradeReviewService _gradeReviewService;
        private readonly INotitficationService _notitficationService;
        public NotificationController(
            ICourseService courseService,
            IGradeReviewService gradeReviewService,
            INotitficationService notitficationService,
            IGeneralModelRepository generalModelRepository,
            AppDbContext appDbContext,
            AppUserManager appUserManager,
            DbContextContainer dbContextContainer) : base(generalModelRepository, dbContextContainer)
        {
            _appUserManager = appUserManager;
            _courseService = courseService;
            _appDbContext = appDbContext;
            _gradeReviewService = gradeReviewService;
            _notitficationService = notitficationService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetNotificationAsync([FromQuery] NotificationQuery notificationQuery)
        {
            var user = await _appUserManager.FindByNameAsync(notificationQuery.CurrentUser);
            if (user == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }

            notificationQuery.UserId = user.Id;

            var notifications = await GetSearchResult(notificationQuery, c => c);

            return Ok(
                new GeneralResponse<object>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = new
                    {
                        Total = notifications.Total,
                        HasMore = notifications.HasMore,
                        Data = new
                        {
                            AmountUnseen = notifications.Data.Where(x => x.IsSeen == false).Count(),
                            Notifications = notifications.Data
                        }
                    },
                    Message = "Get Course Successfull"
                });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("mark-seen/{id}")]
        public async Task<IActionResult> MarkSeenNotificationAsync(int id, string currentUser)
        {
            var user = await _appUserManager.FindByNameAsync(currentUser);
            if (user == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }

            var notificationExist = GeneralModelRepository.GetQueryable<Notification>().Where(c => c.UserId == user.Id && c.Id == id).FirstOrDefault();
            if (notificationExist == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Mark seen notification failed!!"
                });
            }

            notificationExist.IsSeen = true;
            await GeneralModelRepository.Update(notificationExist);

            return Ok(
                new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = "",
                    Message = "Mark seen notification successfull"
                });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("mark-seen")]
        public async Task<IActionResult> MarkSeenAllNotificationAsync(string currentUser)
        {
            var user = await _appUserManager.FindByNameAsync(currentUser);
            if (user == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user"
                });
            }

            var notificationExists = GeneralModelRepository.GetQueryable<Notification>().Where(c => c.UserId == user.Id && c.IsSeen == false).ToList();
            foreach (var notification in notificationExists)
            {
                notification.IsSeen = true;
            }
            _appDbContext.Notifications.UpdateRange(notificationExists);
            await _appDbContext.SaveChangesAsync();
            return Ok(
                new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = "",
                    Message = "Mark seen notification successfull"
                });
        }
    }
}
