using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
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
    public class NotificationController: ApiControllerBase
    {
        private readonly AppUserManager _appUserManager;
        private readonly ICourseService _courseService;
        private readonly IGradeReviewService _gradeReviewService;
        private readonly INotitficationService _notitficationService;
        public NotificationController(
            IGeneralModelRepository generalModelRepository,
            DbContextContainer dbContextContainer) : base(generalModelRepository, dbContextContainer)
        {
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

            var student = GeneralModelRepository.GetQueryable<Student>().Where(c => c.StudentID == user.StudentID).FirstOrDefault();
            List<Notification> notifications = new List<Notification>();
            var teacherNotificationQuery = new TeacherNotificationQuery {
                IncludeCount = notificationQuery.IncludeCount,
                Includes = notificationQuery.Includes,
                MaxResults = notificationQuery.MaxResults,
                SortColumn = notificationQuery.SortColumn,
                StartAt = notificationQuery.StartAt,
                TeacherId = user.Id
            };
            var studentNotificationQuery = new StudentNotificationQuery {
                IncludeCount = notificationQuery.IncludeCount,
                Includes = notificationQuery.Includes,
                MaxResults = notificationQuery.MaxResults,
                SortColumn = notificationQuery.SortColumn,
                StartAt = notificationQuery.StartAt,
                StudentId = student.Id
            };
            var teacherNotifications = await GetSearchResult(teacherNotificationQuery, c => c);
            notifications.AddRange(teacherNotifications.Data);
            var studentNotifications = await GetSearchResult(studentNotificationQuery, c => c);
            notifications.AddRange(studentNotifications.Data);
            notifications.Sort((x, y) => x.CreateOn.CompareTo(y.CreateOn));

            return Ok(
                new GeneralResponse<object>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = new { 
                        Total = teacherNotifications.Total + studentNotifications.Total,
                        HasMore = teacherNotifications.HasMore || studentNotifications.HasMore,
                        Data = new {
                            AmountUnseen = notifications.Where(x => x.IsSeen == false).Count(),
                            Notifications = notifications
                        }
                    },
                    Message = "Get Course Successfull"
                });
        }
    }
}
