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
        public Task<StudentNotification> CreateStudentNotification(CreateStudentNotificationSingleArgs studentNotificationSingleArgs);
        public Task<StudentNotification> UpdateStudentNotification(CreateStudentNotificationSingleArgs studentNotificationSingleArgs);
        public Task<ICollection<StudentNotification>> CreateStudentNotifications(CreateStudentNotificationsArgs studentNotificationSingleArgs);
        public Task<ICollection<StudentNotification>> CreateGradeFinallizeNotification(CreateGradeFinallizeNotificationArgs createGradeFinallizeNotificationArgs);
        public Task<StudentNotification> CreateFinalDecisionGradeReviewNotification(CreateFinalDecisionGradeReviewNotificationArgs notificationArgs);
        public Task<ICollection<TeacherNotification>> CreateRequestGradeReviewNotification(CreateRequestGradeReviewNotificationArgs notificationArgs);
    }
}
