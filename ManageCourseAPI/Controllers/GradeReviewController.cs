using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Repositories;
using ManageCourse.Core.Services;
using ManageCourseAPI.Controllers.Common;
using ManageCourseAPI.Model.Queries;
using ManageCourseAPI.Model.Request;
using ManageCourseAPI.Model.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageCourseAPI.WebSocket;

namespace ManageCourseAPI.Controllers
{
    [ApiController]
    [Route("grade-review")]
    public class GradeReviewController : ApiControllerBase
    {
        private readonly AppUserManager _appUserManager;
        private readonly ICourseService _courseService;
        private readonly IGradeReviewService _gradeReviewService;
        private readonly INotitficationService _notitficationService;
        private readonly IWebSocket _socket;

        public GradeReviewController(
            ICourseService courseService,
            IGradeReviewService gradeReviewService,
            IGeneralModelRepository generalModelRepository,
            INotitficationService notitficationService,
            AppUserManager appUserManager,
            DbContextContainer dbContextContainer, IWebSocket socket) : base(generalModelRepository, dbContextContainer)
        {
            _courseService = courseService;
            _appUserManager = appUserManager;
            _gradeReviewService = gradeReviewService;
            _notitficationService = notitficationService;
            _socket = socket;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetGradeReviewAsync([FromQuery] GradeReviewQuery gradeReviewQuery)
        {
            var user = await _appUserManager.FindByNameAsync(gradeReviewQuery.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(gradeReviewQuery.CurrentUser, gradeReviewQuery.CourseId,
                Role.None, gradeId: gradeReviewQuery.GradeId, gradeReviewId: gradeReviewQuery.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create new grade review failed!!"
                });
            }

            var gradeReview = GeneralModelRepository.GetQueryable<GradeReview>()
                .Where(g => g.GradeId == gradeReviewQuery.GradeId).FirstOrDefault();
            var response = new GradeReviewResponse(gradeReview);
            var grade = await GeneralModelRepository.Get<Grade>(gradeReviewQuery.GradeId,
                includeNavigationPaths: "Assignments");
            var student = GeneralModelRepository.GetQueryable<Student>().Where(c => c.Id == grade.StudentId)
                .FirstOrDefault();
            response.Grade = new GradeResponse(grade);
            response.ExerciseName = grade.Assignments.Name;
            response.Student = new StudentResponse(student);

            return Ok(new GeneralResponse<GradeReviewResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = response,
                Message = "Create new comment review sucessfull"
            });
        }

        [HttpGet]
        [Route("comments")]
        public async Task<IActionResult> GetGradeReviewCommmentsAsync(
            [FromQuery] GetGradeReviewCommmentsQuery commmentsQuery)
        {
            var user = await _appUserManager.FindByNameAsync(commmentsQuery.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(commmentsQuery.CurrentUser, commmentsQuery.CourseId, Role.None,
                gradeId: commmentsQuery.GradeId, gradeReviewId: commmentsQuery.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Get comment grade review failed!!"
                });
            }

            var comments = await GetSearchResult(commmentsQuery, c => new ReviewCommentResponse(c));
            foreach (var comment in comments.Data)
            {
                if (comment.TeacherId > 0)
                {
                    var teacher = await _appUserManager.FindByIdAsync(comment.TeacherId.ToString());
                    comment.Teacher = new UserResponse(teacher);
                }

                if (comment.StudentId > 0)
                {
                    var student = await GeneralModelRepository.Get<Student>(comment.StudentId);
                    comment.Student = new StudentResponse(student);
                }
            }

            return Ok(new GeneralResponse<GeneralResultResponse<ReviewCommentResponse>>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = comments,
                Message = "Get comment review sucessfull"
            });
        }

        [HttpPost]
        [Route("approval")]
        public async Task<IActionResult> ApprovalGradeReviewAsync(
            [FromBody] ApprovalGradeReviewRequest approvalGradeReview)
        {
            var user = await _appUserManager.FindByNameAsync(approvalGradeReview.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(approvalGradeReview.CurrentUser, approvalGradeReview.CourseId,
                Role.Teacher, gradeId: approvalGradeReview.GradeId, gradeReviewId: approvalGradeReview.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create new grade review failed!!"
                });
            }

            var student = GeneralModelRepository.GetQueryable<Student>().Where(c => c.StudentID == user.StudentID)
                .FirstOrDefault();
            var approvalGradeReviewArgs = new ApprovalGradeReviewArgs
            {
                ApprovalStatus = approvalGradeReview.ApprovalStatus,
                GradeReviewId = approvalGradeReview.GradeReviewId,
                CurrentUser = approvalGradeReview.CurrentUser
            };
            await _gradeReviewService.ApprovalGradeReviewAsync(approvalGradeReviewArgs);

            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new
                {
                    GradeReviewId = approvalGradeReview.GradeReviewId,
                    Status = approvalGradeReviewArgs.ApprovalStatus
                },
                Message = $"{approvalGradeReview.ApprovalStatus} grade review sucessfull"
            });
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateGradeReviewAsync([FromBody] CreateGradeReviewRequest gradeReviewRequest)
        {
            if (!(await ValidateGradeOfUserAsync(gradeReviewRequest.CurrentUser, gradeReviewRequest.CourseId,
                Role.Student, gradeId: gradeReviewRequest.GradeId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create grade review failed!!"
                });
            }

            var user = await _appUserManager.FindByNameAsync(gradeReviewRequest.CurrentUser);
            var student = GeneralModelRepository.GetQueryable<Student>()
                .FirstOrDefault(c => c.StudentID == user.StudentID);
            var gradeReviewArgs = new CreateGradeReviewArgs
            {
                GradeId = gradeReviewRequest.GradeId,
                GradeExpect = gradeReviewRequest.GradeExpect,
                StudentId = student.Id,
                Reason = gradeReviewRequest.Reason,
                CurrentUser = gradeReviewRequest.CurrentUser
            };
            var gradeReview = await _gradeReviewService.CreateGradeReviewAsync(gradeReviewArgs);
            //TODO create-notice
            var noticeArgs = new CreateRequestGradeReviewNotificationArgs
            {
                CurrentUser = gradeReviewRequest.CurrentUser,
                GradeReviewId = gradeReview.Id,
                Message =
                    $"{gradeReview.Student?.FullName} create new grade review for {gradeReview?.Grade?.Assignments?.Name}",
                StudentId = gradeReview.Student != null ? gradeReview.Student.Id : 0
            };
            await _notitficationService.CreateRequestGradeReviewNotification(noticeArgs);
            var response = new GradeReviewResponse(gradeReview);
            var grade = await GeneralModelRepository.Get<Grade>(gradeReviewRequest.GradeId,
                includeNavigationPaths: "Assignments");
            response.Grade = new GradeResponse(grade);
            response.ExerciseName = grade.Assignments.Name;
            response.Student = new StudentResponse(student);
            response.MSSV = student.StudentID;

            return Ok(new GeneralResponse<GradeReviewResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = response,
                Message = "Create new grade review successfull"
            });
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> UpdateGradeReviewAsync([FromBody] UpdateGradeReviewRequest gradeReviewRequest)
        {
            var user = await _appUserManager.FindByNameAsync(gradeReviewRequest.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(gradeReviewRequest.CurrentUser, gradeReviewRequest.CourseId,
                Role.Teacher, gradeId: gradeReviewRequest.GradeId, gradeReviewId: gradeReviewRequest.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Update grade review failed!!"
                });
            }

            var gradeReviewExist = GeneralModelRepository
                .GetQueryable<GradeReview>().FirstOrDefault(c =>
                    c.Id == gradeReviewRequest.GradeReviewId && c.CreateBy == user.UserName && c.Status == GradeReviewStatus.Pending);
            if (gradeReviewExist == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Update grade review failed!!"
                });
            }

            var gradeReviewArgs = new UpdateGradeReviewArgs
            {
                GradeReviewId = gradeReviewRequest.GradeReviewId,
                GradeExpect = gradeReviewRequest.GradeExpect,
                Reason = gradeReviewRequest.Reason,
                CurrentUser = gradeReviewRequest.CurrentUser
            };
            var gradeReview = await _gradeReviewService.UpdateGradeReviewAsync(gradeReviewArgs);
            //TODO create-notice
            var response = new GradeReviewResponse(gradeReview);
            var grade = await GeneralModelRepository.Get<Grade>(gradeReviewRequest.GradeId,
                includeNavigationPaths: "Assignments");
            var student = GeneralModelRepository.GetQueryable<Student>()
                .FirstOrDefault(c => c.Id == grade.StudentId);
            response.Grade = new GradeResponse(grade);
            response.ExerciseName = grade.Assignments.Name;
            response.Student = new StudentResponse(student);
            response.MSSV = student.StudentID;

            return Ok(new GeneralResponse<GradeReviewResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = response,
                Message = "Update grade review successfull"
            });
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteGradeReviewAsync([FromBody] DeleteGradeReviewRequest gradeReviewRequest)
        {
            var user = await _appUserManager.FindByNameAsync(gradeReviewRequest.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(gradeReviewRequest.CurrentUser, gradeReviewRequest.CourseId,
                Role.Teacher, gradeId: gradeReviewRequest.GradeId, gradeReviewId: gradeReviewRequest.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Delete grade review failed!!"
                });
            }

            var gradeReviewExist = GeneralModelRepository
                .GetQueryable<GradeReview>().FirstOrDefault(c =>
                    c.Id == gradeReviewRequest.GradeReviewId && c.CreateBy == user.UserName);
            if (gradeReviewExist == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Delete grade review failed!!"
                });
            }

            await GeneralModelRepository.Delete<GradeReview>(gradeReviewRequest.GradeReviewId);
            //TODO create-notice
            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Delete grade review sucessfull"
            });
        }

        [HttpPost]
        [Route("teacher-comment")]
        public async Task<IActionResult> CreateTeacherCommentAsync(
            [FromBody] CreateTeacherCommentRequest createTeacherComment)
        {
            var user = await _appUserManager.FindByNameAsync(createTeacherComment.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(createTeacherComment.CurrentUser, createTeacherComment.CourseId,
                Role.Teacher, gradeId: createTeacherComment.GradeId,
                gradeReviewId: createTeacherComment.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create new comment failed!!"
                });
            }

            var reviewCommentArgs = new CreateReviewCommentArgs
            {
                GradeReviewId = createTeacherComment.GradeReviewId,
                Message = createTeacherComment.Message,
                TeacherId = user.Id,
                StudentId = 0,
                CurrentUser = createTeacherComment.CurrentUser
            };
            var reviewComment = await _gradeReviewService.CreateReviewCommentAsync(reviewCommentArgs);
            var grade = await GeneralModelRepository.Get<Grade>(createTeacherComment.GradeId);
            var assignment = await GeneralModelRepository.Get<Assignments>(grade.AssignmentId);
            var gradeReview = await GeneralModelRepository.Get<GradeReview>(createTeacherComment.GradeReviewId);
            var student = await GeneralModelRepository.Get<Student>(gradeReview.StudentId);
            //TODO create-notice
            var noticeArgs = new CreateStudentNotificationSingleArgs
            {
                GradeReviewId = reviewCommentArgs.GradeReviewId,
                CourseId = createTeacherComment.CourseId,
                GradeId = grade.Id,
                StudentId = student.Id,
                Message =
                    $"{user.NormalizedDisplayName} comment in your request grade review for assignment {assignment.Name}",
                CurrentUser = createTeacherComment.CurrentUser
            };
            await _notitficationService.CreateStudentNotification(noticeArgs);

            var response = new ReviewCommentResponse(reviewComment);
            response.Teacher = new UserResponse(user);
            var studentUser = GeneralModelRepository
                .GetQueryable<AppUser>().FirstOrDefault(u => u.StudentID == student.StudentID);
            if (studentUser != null)
            {
                _socket.SendComment(studentUser.Id, response);
            }


            foreach (var receiver in QueryTeacherListFrom(createTeacherComment.TeacherId))
            {
                _socket.SendComment(receiver, response);
            }

            return Ok(new GeneralResponse<ReviewCommentResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = response,
                Message = "Create new comment review sucessfull"
            });
        }

        private List<int> QueryTeacherListFrom(int courseId)
        {
            var teachers = GeneralModelRepository
                .GetQueryable<Course>()
                .Where(u => u.Id == courseId)
                .SelectMany(c => c.Course_Users)
                .Where(u => u.Role == Role.Teacher)
                .Select(u => u.UserId).ToList();
            return teachers;
        }

        [HttpPut]
        [Route("teacher-comment/update")]
        public async Task<IActionResult> UpdateTeacherCommentAsync([FromBody] UpdateCommentRequest updateCommentRequest)
        {
            var user = await _appUserManager.FindByNameAsync(updateCommentRequest.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(updateCommentRequest.CurrentUser, updateCommentRequest.CourseId,
                Role.Teacher, gradeId: updateCommentRequest.GradeId,
                gradeReviewId: updateCommentRequest.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create new comment failed!!"
                });
            }

            var reviewCommentExist = GeneralModelRepository
                .GetQueryable<ReviewComment>()
                .FirstOrDefault(c => c.Id == updateCommentRequest.ReviewCommentId && c.CreateBy == user.UserName);
            if (reviewCommentExist == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create new comment failed!!"
                });
            }

            var reviewCommentArgs = new UpdateReviewCommentArgs
            {
                ReviewCommentId = reviewCommentExist.Id,
                Message = updateCommentRequest.Message,
                CurrentUser = updateCommentRequest.CurrentUser
            };
            var reviewComment = await _gradeReviewService.UpdateReviewCommentAsync(reviewCommentArgs);
            var response = new ReviewCommentResponse(reviewComment);
            response.Teacher = new UserResponse(user);
            //TODO create-notice

            var res = Ok(new GeneralResponse<ReviewCommentResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = response,
                Message = "Create new comment review sucessfull"
            });

            var studentId = GeneralModelRepository.GetQueryable<GradeReview>()
                .Where(g => g.Id == updateCommentRequest.GradeReviewId)
                .Select(g => g.Grade)
                .Select(g => g.MSSV)
                .Join(GeneralModelRepository
                    .GetQueryable<AppUser>().ToList(), m => m, s => s.StudentID, (m, s) => s.Id)
                .FirstOrDefault();
            MessagesService.UpdateComment(studentId, res);
            foreach (var teacherId in QueryTeacherListFrom(updateCommentRequest.CourseId))
            {
                MessagesService.UpdateComment(teacherId, res);
            }
            
            return res;
        }

        [HttpDelete]
        [Route("teacher-comment/delete")]
        public async Task<IActionResult> DeleteTeacherCommentAsync([FromBody] DeleteCommentRequest deleteCommentRequest)
        {
            var user = await _appUserManager.FindByNameAsync(deleteCommentRequest.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(deleteCommentRequest.CurrentUser, deleteCommentRequest.CourseId,
                Role.Teacher, gradeId: deleteCommentRequest.GradeId,
                gradeReviewId: deleteCommentRequest.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Delete comment failed!!"
                });
            }

            var reviewCommentExist = GeneralModelRepository
                .GetQueryable<ReviewComment>()
                .FirstOrDefault(c => c.Id == deleteCommentRequest.ReviewCommentId && c.CreateBy == user.UserName);
            if (reviewCommentExist == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Delete comment failed!!"
                });
            }

            await GeneralModelRepository.Delete<ReviewComment>(reviewCommentExist.Id);
            //TODO create-notice
            var res = Ok(new GeneralResponse<int>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = deleteCommentRequest.ReviewCommentId,
                Message = "Delete comment review sucessfull"
            });
            var student = GeneralModelRepository.GetQueryable<ReviewComment>()
                .Where(r=> r.GradeReviewId == deleteCommentRequest.GradeReviewId)
                .Select(r => r.GradeReview)
                .Select(g => g.Student)
                .Join(GeneralModelRepository.GetQueryable<AppUser>().ToList(), s => s.StudentID, u => u.StudentID,
                    (s, u) => s.Id).FirstOrDefault();
            MessagesService.DeleteComment(student, res);

            var teachers = QueryTeacherListFrom(GeneralModelRepository.GetQueryable<GradeReview>()
                .Where(g => g.Id == deleteCommentRequest.GradeReviewId)
                .Select(g => g.Grade)
                .Select(g => g.Assignments)
                .Select(a => a.Course).Select(c => c.Id).FirstOrDefault());

            foreach (var teacher in teachers)
            {
                MessagesService.DeleteComment(teacher, res);
            }

            return res;
        }

        [HttpPost]
        [Route("student-comment")]
        public async Task<IActionResult> CreateStudentCommentAsync(
            [FromBody] CreateStudentCommentRequest gradeReviewRequest)
        {
            var user = await _appUserManager.FindByNameAsync(gradeReviewRequest.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(gradeReviewRequest.CurrentUser, gradeReviewRequest.CourseId,
                Role.Student, gradeId: gradeReviewRequest.GradeId, gradeReviewId: gradeReviewRequest.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create new grade review failed!!"
                });
            }

            var student = GeneralModelRepository.GetQueryable<Student>()
                .FirstOrDefault(c => c.StudentID == user.StudentID);
            var reviewCommentArgs = new CreateReviewCommentArgs
            {
                GradeReviewId = gradeReviewRequest.GradeReviewId,
                Message = gradeReviewRequest.Message,
                StudentId = student.Id,
                TeacherId = 0,
                CurrentUser = gradeReviewRequest.CurrentUser
            };
            var reviewComment = await _gradeReviewService.CreateReviewCommentAsync(reviewCommentArgs);
            var grade = await GeneralModelRepository.Get<Grade>(gradeReviewRequest.GradeId);
            var assignment = await GeneralModelRepository.Get<Assignments>(grade.AssignmentId);
            var noticeArgs = new CreateRequestGradeReviewNotificationArgs
            {
                GradeReviewId = reviewCommentArgs.GradeReviewId,
                StudentId = student.Id,
                CourseId = gradeReviewRequest.CourseId,
                GradeId = grade.Id,
                Message = $"{student.FullName} comment in request grade review for assignment {assignment.Name}",
                CurrentUser = gradeReviewRequest.CurrentUser
            };
            await _notitficationService.CreateRequestGradeReviewNotification(noticeArgs);
            var response = new ReviewCommentResponse(reviewComment);
            response.Student = new StudentResponse(student);
            var teachers = QueryTeacherListFrom(gradeReviewRequest.CourseId);
            foreach (var teacher in teachers)
            {
                _socket.SendComment(teacher, response);
            }

            return Ok(new GeneralResponse<ReviewCommentResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = response,
                Message = "Create new comment review sucessfull"
            });
        }

        [HttpPut]
        [Route("student-comment/update")]
        public async Task<IActionResult> UpdateStudentCommentAsync([FromBody] UpdateCommentRequest gradeReviewRequest)
        {
            var user = await _appUserManager.FindByNameAsync(gradeReviewRequest.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(gradeReviewRequest.CurrentUser, gradeReviewRequest.CourseId,
                Role.Student, gradeId: gradeReviewRequest.GradeId, gradeReviewId: gradeReviewRequest.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create new comment failed!!"
                });
            }

            var student = GeneralModelRepository.GetQueryable<Student>().Where(c => c.StudentID == user.StudentID)
                .FirstOrDefault();
            var reviewCommentExist = GeneralModelRepository.GetQueryable<ReviewComment>()
                .Where(c => c.Id == gradeReviewRequest.ReviewCommentId && c.CreateBy == user.UserName).FirstOrDefault();
            if (reviewCommentExist == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Delete comment failed!!"
                });
            }

            var reviewCommentArgs = new UpdateReviewCommentArgs
            {
                ReviewCommentId = reviewCommentExist.Id,
                Message = gradeReviewRequest.Message,
                CurrentUser = gradeReviewRequest.CurrentUser
            };
            var reviewComment = await _gradeReviewService.UpdateReviewCommentAsync(reviewCommentArgs);
            //TODO create-notice

            var response = new ReviewCommentResponse(reviewComment);
            response.Student = new StudentResponse(student);

            var res = Ok(new GeneralResponse<ReviewCommentResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = response,
                Message = "Create new comment review sucessfull"
            });

            foreach (var id in QueryTeacherListFrom(gradeReviewRequest.CourseId))
            {
                MessagesService.UpdateComment(id, res);
            }

            return res;
        }

        [HttpDelete]
        [Route("student-comment/delete")]
        public async Task<IActionResult> DeleteStudentCommentAsync([FromBody] DeleteCommentRequest deleteCommentRequest)
        {
            var user = await _appUserManager.FindByNameAsync(deleteCommentRequest.CurrentUser);
            if (!(await ValidateGradeReviewOfUserAsync(deleteCommentRequest.CurrentUser, deleteCommentRequest.CourseId,
                Role.Student, gradeId: deleteCommentRequest.GradeId,
                gradeReviewId: deleteCommentRequest.GradeReviewId)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create new grade review failed!!"
                });
            }

            var student = GeneralModelRepository.GetQueryable<Student>()
                .FirstOrDefault(c => c.StudentID == user.StudentID);
            var reviewCommentExist = GeneralModelRepository
                .GetQueryable<ReviewComment>()
                .FirstOrDefault(c => c.Id == deleteCommentRequest.ReviewCommentId && c.CreateBy == user.UserName);
            if (reviewCommentExist == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Delete comment failed!!"
                });
            }

            await GeneralModelRepository.Delete<ReviewComment>(reviewCommentExist.Id);

            //TODO create-notice
            var res = Ok(new GeneralResponse<int>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = deleteCommentRequest.ReviewCommentId,
                Message = "Create new comment review sucessfull"
            });
            foreach (var id in QueryTeacherListFrom(deleteCommentRequest.CourseId))
            {
                MessagesService.DeleteComment(id, res);
            }

            return res;
        }

        private async Task<bool> ValidateGradeOfUserAsync(string username, int courseId, Role role = Role.None,
            bool isCheckOwner = false, int gradeId = 0)
        {
            var result = true;
            var user = await _appUserManager.FindByNameAsync(username);
            if (user == null)
            {
                return false;
            }

            var courser = (await _courseService.GetByIdAsync(courseId));
            if (courser == null)
            {
                return false;
            }

            var courseUser = GeneralModelRepository.GetQueryable<Course_User>()
                .Where(c => c.UserId == user.Id && c.CourseId == courseId).FirstOrDefault();
            if (courseUser == null)
            {
                return false;
            }

            if (role != Role.None)
            {
                result = courseUser.Role == role;
            }

            var grade = GeneralModelRepository.GetQueryable<Grade>()
                .Where(c => c.Id == gradeId && c.Assignments.CourseId == courseId).FirstOrDefault();
            if (grade != null)
            {
                return grade.MSSV == user.StudentID;
            }

            if (isCheckOwner)
            {
                return courser.CreateBy == username;
            }

            return result;
        }

        private async Task<bool> ValidateGradeReviewOfUserAsync(string username, int courseId, Role role = Role.None,
            bool isCheckOwner = false, int gradeId = 0, int gradeReviewId = 0)
        {
            var user = await _appUserManager.FindByNameAsync(username);
            if (user == null)
            {
                return false;
            }

            var courser = (await _courseService.GetByIdAsync(courseId));
            if (courser == null)
            {
                return false;
            }

            var student = GeneralModelRepository.GetQueryable<Student>().Where(c => c.StudentID == user.StudentID)
                .FirstOrDefault();

            switch (role)
            {
                case Role.None:
                {
                    var courseUser = GeneralModelRepository.GetQueryable<Course_User>()
                        .Where(c => c.UserId == user.Id && c.CourseId == courseId).FirstOrDefault();
                    if (courseUser == null)
                    {
                        return false;
                    }

                    var gradeReview = GeneralModelRepository.GetQueryable<GradeReview>()
                        .Where(c => c.Id == gradeReviewId).FirstOrDefault();
                    if (gradeReview == null && (courseUser.Role == Role.Student && student.Id != gradeReview.StudentId))
                    {
                        return false;
                    }

                    break;
                }
                case Role.Teacher:
                {
                    var courseUser = GeneralModelRepository.GetQueryable<Course_User>()
                        .Where(c => c.UserId == user.Id && c.CourseId == courseId).FirstOrDefault();
                    if (courseUser == null && courseUser.Role != Role.Teacher)
                    {
                        return false;
                    }

                    break;
                }
                case Role.Student:
                {
                    var courseUser = GeneralModelRepository.GetQueryable<Course_User>()
                        .Where(c => c.UserId == user.Id && c.CourseId == courseId).FirstOrDefault();
                    if (courseUser == null && courseUser.Role != Role.Student)
                    {
                        return false;
                    }

                    var grade = GeneralModelRepository.GetQueryable<Grade>()
                        .Where(c => c.Id == gradeId && c.Assignments.CourseId == courseId).FirstOrDefault();
                    if (grade != null)
                    {
                        return grade.MSSV == user.StudentID;
                    }

                    break;
                }
                default:
                    break;
            }

            if (isCheckOwner)
            {
                return courser.CreateBy == username;
            }

            return true;
        }
    }
}