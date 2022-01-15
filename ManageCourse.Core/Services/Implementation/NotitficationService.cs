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
        public async Task<StudentNotification> CreateFinalDecisionGradeReviewNotification(CreateFinalDecisionGradeReviewNotificationArgs notificationArgs)
        {
            var studentNotification = new StudentNotification
            {
                Message = notificationArgs.Message,
                StudentId = notificationArgs.StudentId
            };
            AuditHelper.CreateAudit(studentNotification, notificationArgs.CurrentUser);

            _ = await _generalModelRepository.Create(studentNotification);
            return studentNotification;
        }

        public async Task<ICollection<StudentNotification>> CreateGradeFinallizeNotification(CreateGradeFinallizeNotificationArgs createGradeFinallizeNotificationArgs)
        {
            var course = await _appDbContext.Courses.Where(x => x.GradeId == createGradeFinallizeNotificationArgs.GradeId).FirstOrDefaultAsync();
            var studentIds = await _appDbContext.Course_Students.Where(x => x.CourseId == course.Id).Select(x => x.StudentId).ToListAsync();
            var studentNotifications = new List<StudentNotification>();
            foreach (var studentId in studentIds)
            {
                var studentNotification = new StudentNotification
                {
                    Message = createGradeFinallizeNotificationArgs.Message,
                    StudentId = studentId
                };
                AuditHelper.CreateAudit(studentNotification, createGradeFinallizeNotificationArgs.CurrentUser);
                studentNotifications.Add(studentNotification);
            }
            await _appDbContext.StudentNotifications.AddRangeAsync(studentNotifications);
            await _appDbContext.SaveChangesAsync();
            return studentNotifications;
        }

        public async Task<StudentNotification> CreateStudentNotification(CreateStudentNotificationSingleArgs studentNotificationSingleArgs)
        {
            var studentNotification = new StudentNotification
            {
                Message = studentNotificationSingleArgs.Message,
                StudentId = studentNotificationSingleArgs.StudentId
            };
            AuditHelper.CreateAudit(studentNotification, studentNotificationSingleArgs.CurrentUser);

            _ = await _generalModelRepository.Create(studentNotification);
            return studentNotification;
        }

        public async Task<ICollection<TeacherNotification>> CreateRequestGradeReviewNotification(CreateRequestGradeReviewNotificationArgs notificationArgs)
        {
            var gradeReview = await _generalModelRepository.Get<GradeReview>(notificationArgs.GradeReviewId);
            var course = await _appDbContext.Courses.Where(x => x.GradeId == gradeReview.GradeId).FirstOrDefaultAsync();
            var teacherIds = await _appDbContext.Course_Users.Where(x => x.CourseId == course.Id && x.Role == Role.Teacher).Select(x => x.UserId).ToListAsync();
            var teacherNotifications = new List<TeacherNotification>();
            foreach (var teacherId in teacherIds)
            {
                var teacherNotification = new TeacherNotification
                {
                    Message = notificationArgs.Message,
                    TeacherId = notificationArgs.StudentId
                };
                AuditHelper.CreateAudit(teacherNotification, notificationArgs.CurrentUser);
                teacherNotifications.Add(teacherNotification);
            }
            await _appDbContext.TeacherNotifications.AddRangeAsync(teacherNotifications);
            await _appDbContext.SaveChangesAsync();
            return teacherNotifications;
        }

        public async Task<ICollection<StudentNotification>> CreateStudentNotifications(CreateStudentNotificationsArgs studentNotificationSingleArgs)
        {
            var studentNotifications = new List<StudentNotification>();
            foreach (var studentId in studentNotificationSingleArgs.StudentIds)
            {
                var studentNotification = new StudentNotification
                {
                    Message = studentNotificationSingleArgs.Message,
                    StudentId = studentId
                };
                AuditHelper.CreateAudit(studentNotification, studentNotificationSingleArgs.CurrentUser);
                studentNotifications.Add(studentNotification);
            }
            await _appDbContext.StudentNotifications.AddRangeAsync(studentNotifications);
            await _appDbContext.SaveChangesAsync();
            return studentNotifications;
        }

        public Task<StudentNotification> UpdateStudentNotification(CreateStudentNotificationSingleArgs studentNotificationSingleArgs)
        {
            throw new NotImplementedException();
        }
    }
}
