using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services.Implementation
{
    public class NotitficationService : INotitficationService
    {
        private readonly AppDbContext _appDbContext;
        protected AppUserManager _userManager { get; private set; }
        private readonly IGeneralModelRepository _generalModelRepository;
        public NotitficationService(AppDbContext appDbContext, IGeneralModelRepository generalModelRepository, AppUserManager userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _generalModelRepository = generalModelRepository;
        }
        public async Task<Notification> CreateFinalDecisionGradeReviewNotification(CreateFinalDecisionGradeReviewNotificationArgs notificationArgs)
        {
            var notification = new Notification
            {
                Message = notificationArgs.Message,
                UserId = notificationArgs.UserId,
                CourseId = notificationArgs.CourseId,
                GradeId = notificationArgs.GradeId,
                GradeReviewId = notificationArgs.GradeReviewId,
                IsSeen = false,
                SenderName = notificationArgs.CurrentUser,
                TypeNotification = TypeNotification.ForStudent
            };
            AuditHelper.CreateAudit(notification, notificationArgs.CurrentUser);
            _ = await _generalModelRepository.Create(notification);
            return notification;
        }

        public async Task<ICollection<Notification>> CreateGradeFinallizeNotification(CreateGradeFinallizeNotificationArgs createGradeFinallizeNotificationArgs)
        {
            var grade = await _generalModelRepository.Get<Grade>(createGradeFinallizeNotificationArgs.GradeId);
            var course = await _appDbContext.Courses.Where(x => x.Id == grade.Assignments.CourseId).FirstOrDefaultAsync();
            var students = await _appDbContext.Course_Students.Where(x => x.CourseId == course.Id).Select(x => new { StudentId = x.StudentId, StudentCode = x.StudentCode }).ToListAsync();
            var studentCodes = students.Select(x => x.StudentCode).ToList();
            var userIds = await _generalModelRepository.GetQueryable<AppUser>().Where(x => studentCodes.Contains(x.StudentID)).Select(x => x.Id).ToListAsync();
            var notifications = new List<Notification>();
           
            foreach (var userId in userIds)
            {
                var notification = new Notification
                {
                    Message = createGradeFinallizeNotificationArgs.Message,
                    CourseId = createGradeFinallizeNotificationArgs.CourseId,
                    GradeId = createGradeFinallizeNotificationArgs.GradeId,
                    GradeReviewId = createGradeFinallizeNotificationArgs.GradeReviewId,
                    UserId = userId
                };
                AuditHelper.CreateAudit(notification, createGradeFinallizeNotificationArgs.CurrentUser);
                notifications.Add(notification);
            }
            await _appDbContext.Notifications.AddRangeAsync(notifications);
            await _appDbContext.SaveChangesAsync();
            return notifications;
        }

        public async Task<Notification> CreateStudentNotification(CreateStudentNotificationSingleArgs studentNotificationSingleArgs)
        {
            var student = _appDbContext.Students.Where(x => x.Id == studentNotificationSingleArgs.StudentId).FirstOrDefault();
            var userId = await _generalModelRepository.GetQueryable<AppUser>().Where(x => student.StudentID == x.StudentID).Select(x => x.Id).FirstOrDefaultAsync();
            var notification = new Notification
            {
                Message = studentNotificationSingleArgs.Message,
                UserId = userId,
                CourseId = studentNotificationSingleArgs.CourseId,
                GradeId = studentNotificationSingleArgs.GradeId,
                GradeReviewId = studentNotificationSingleArgs.GradeReviewId,
                IsSeen = false,
                SenderName = studentNotificationSingleArgs.CurrentUser,
                TypeNotification = TypeNotification.ForStudent
            };
            AuditHelper.CreateAudit(notification, studentNotificationSingleArgs.CurrentUser);
            _ = await _generalModelRepository.Create(notification);
            return notification;
        }

        public async Task<ICollection<Notification>> CreateRequestGradeReviewNotification(CreateRequestGradeReviewNotificationArgs notificationArgs)
        {
            var gradeReview = await _generalModelRepository.Get<GradeReview>(notificationArgs.GradeReviewId);
            var grade = await _generalModelRepository.Get<Grade>(gradeReview.GradeId);
            var student = await _generalModelRepository.Get<Student>(notificationArgs.StudentId);
            var assignment = await _generalModelRepository.Get<Assignments>(gradeReview.Grade.AssignmentId);
            var course = await _appDbContext.Courses.Where(x => x.Id == assignment.CourseId).FirstOrDefaultAsync();
            var teacherIds = await _appDbContext.Course_Users.Where(x => x.CourseId == course.Id && x.Role == Role.Teacher).Select(x => x.UserId).ToListAsync();
            var notifications = new List<Notification>();
            foreach (var teacherId in teacherIds)
            {
                var notification = new Notification
                {
                    Message = notificationArgs.Message,
                    UserId = teacherId,
                    CourseId = course.Id,
                    GradeId = grade.Id,
                    GradeReviewId = gradeReview.Id,                    
                    SenderName = student.FullName,
                    IsSeen = false,
                    TypeNotification = TypeNotification.ForTeacher
                };
                AuditHelper.CreateAudit(notification, notificationArgs.CurrentUser);
                notifications.Add(notification);
            }
            await _appDbContext.Notifications.AddRangeAsync(notifications);
            await _appDbContext.SaveChangesAsync();
            return notifications;
        }

        public async Task<ICollection<Notification>> CreateStudentNotifications(CreateStudentNotificationsArgs studentNotificationSingleArgs)
        {
            var students = await _appDbContext.Course_Students.Where(x => studentNotificationSingleArgs.StudentIds.Contains(x.StudentId)).Select(x => new { StudentId = x.StudentId, StudentCode = x.StudentCode }).ToListAsync();
            var studentCodes = students.Select(x => x.StudentCode).ToList();
            var userIds = await _generalModelRepository.GetQueryable<AppUser>().Where(x => studentCodes.Contains(x.StudentID)).Select(x => x.Id).ToListAsync();
            var notifications = new List<Notification>();

            foreach (var userId in userIds)
            {
                var notification = new Notification
                {
                    Message = studentNotificationSingleArgs.Message,
                    CourseId = studentNotificationSingleArgs.CourseId,
                    GradeId = studentNotificationSingleArgs.GradeId,
                    GradeReviewId = studentNotificationSingleArgs.GradeReviewId,
                    UserId = userId
                };
                AuditHelper.CreateAudit(notification, studentNotificationSingleArgs.CurrentUser);
                notifications.Add(notification);
            }
            await _appDbContext.Notifications.AddRangeAsync(notifications);
            await _appDbContext.SaveChangesAsync();
            return notifications;
        }
        public async Task<ICollection<Notification>> CreateStudentOfCourseNotifications(CreateStudentOfCourseNotificationsArgs notificationArgs)
        {
            var students = await _appDbContext.Course_Students.Where(x => notificationArgs.CourseId == x.CourseId).Select(x => new { StudentId = x.StudentId, StudentCode = x.StudentCode }).ToListAsync();
            var studentCodes = students.Select(x => x.StudentCode).ToList();
            var userIds = await _generalModelRepository.GetQueryable<AppUser>().Where(x => studentCodes.Contains(x.StudentID)).Select(x => x.Id).ToListAsync();
            var notifications = new List<Notification>();

            foreach (var userId in userIds)
            {
                var notification = new Notification
                {
                    Message = notificationArgs.Message,
                    CourseId = notificationArgs.CourseId,
                    GradeId = notificationArgs.GradeId,
                    GradeReviewId = notificationArgs.GradeReviewId,
                    UserId = userId
                };
                AuditHelper.CreateAudit(notification, notificationArgs.CurrentUser);
                notifications.Add(notification);
            }
            await _appDbContext.Notifications.AddRangeAsync(notifications);
            await _appDbContext.SaveChangesAsync();
            return notifications;
        }

        public Task<Notification> UpdateStudentNotification(CreateStudentNotificationSingleArgs studentNotificationSingleArgs)
        {
            throw new NotImplementedException();
        }
    }
}
