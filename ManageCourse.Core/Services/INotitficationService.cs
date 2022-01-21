using ManageCourse.Core.Data;
using ManageCourse.Core.Model.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services
{
    public interface INotitficationService
    {
        public Task<Notification> CreateStudentNotification(CreateStudentNotificationSingleArgs studentNotificationSingleArgs);
        public Task<Notification> UpdateStudentNotification(CreateStudentNotificationSingleArgs studentNotificationSingleArgs);
        public Task<ICollection<Notification>> CreateStudentNotifications(CreateStudentNotificationsArgs studentNotificationSingleArgs);
        public Task<ICollection<Notification>> CreateGradeFinallizeNotification(CreateGradeFinallizeNotificationArgs createGradeFinallizeNotificationArgs);
        public Task<Notification> CreateFinalDecisionGradeReviewNotification(CreateFinalDecisionGradeReviewNotificationArgs notificationArgs);
        public Task<ICollection<Notification>> CreateRequestGradeReviewNotification(CreateRequestGradeReviewNotificationArgs notificationArgs);
        public Task<ICollection<Notification>> CreateStudentOfCourseNotifications(CreateStudentOfCourseNotificationsArgs notificationArgs);
    }
}
