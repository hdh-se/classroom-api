using ManageCourse.Core.Helpers;
using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Repositories;
using ManageCourse.Core.Services;
using ManageCourse.Core.Utilities;
using ManageCourseAPI.Controllers.Common;
using ManageCourseAPI.Model.Queries;
using ManageCourseAPI.Model.Request;
using ManageCourseAPI.Model.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ExcelDataReader;
using ManageCourse.Core.Model.Responses;
using System.Data;
using ClosedXML.Excel;
using System.IO;

namespace ManageCourseAPI.Controllers
{
    [ApiController]
    [Route("course")]
    public class CourseController : ApiControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly AppUserManager _appUserManager;
        private readonly IEmailService _emailService;

        public CourseController(
            IGeneralModelRepository generalModelRepository,
            DbContextContainer dbContextContainer,
            ICourseService courseService,
            IEmailService emailService,
            AppUserManager appUserManager) : base(generalModelRepository, dbContextContainer)
        {
            _courseService = courseService;
            _appUserManager = appUserManager;
            _emailService = emailService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetCousersAsync([FromQuery] CourseQuery courseQuery)
        {
            var user = await _appUserManager.FindByNameAsync(courseQuery.CurrentUser);
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
            var courseUsers = new CourseUserQuery
            {
                UserId = user.Id,
                StartAt = courseQuery.StartAt,
                MaxResults = courseQuery.MaxResults,
            };
            var coursers = await GetSearchResult(courseUsers, c => new CourseResponse(c.Course));
            foreach (var course in coursers.Data)
            {
                course.Owner = (await _appUserManager.FindByNameAsync(course.Owner)).NormalizedDisplayName;
            }
            return Ok(
                new GeneralResponse<GeneralResultResponse<CourseResponse>>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = coursers,
                    Message = "Get Course Successfull"
                });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id, string currentUser)
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

            var courseUser = GeneralModelRepository.GetQueryable<Course_User>().Where(c => c.UserId == user.Id && c.CourseId == id).FirstOrDefault();
            if (courseUser == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "You haven't been joined this class"
                });
            }
            var coursers = (await _courseService.GetByIdAsync(id));
            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new
                {
                    course = new CourseResponse(coursers),
                    role = courseUser.Role
                },
                Message = "Get class sucessfull"
            });
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("{id}/assignments")]
        public async Task<IActionResult> CreateNewAssignmentAsync([FromBody] CreateNewAssignmentsRequest createNewAssignmentsRequest)
        {
            if (!(await ValidateUserInClassAsync(createNewAssignmentsRequest.CurrentUser, createNewAssignmentsRequest.CourseId, Role.Teacher)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Create new asssignments failed!!"
                });
            }
            var args = createNewAssignmentsRequest.MapTo<CreateNewAssignmentsRequest, CreateNewAssignmentsArgs>();
            var result = await _courseService.CreateNewAssignments(args);

            return Ok(new GeneralResponse<AssignmentsResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new AssignmentsResponse(result),
                Message = "Get class sucessfull"
            });
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{id}/assignments/{assignmentsId}")]
        public async Task<IActionResult> UpdateAssignmentAsync(int id,int assignmentsId, [FromBody]UpdateAssignmentsRequest updateAssignmentsRequest)
        {
            if (!(await ValidateUserInClassAsync(updateAssignmentsRequest.CurrentUser, id, Role.Teacher)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Update asssignments failed!!"
                });
            }
            var args = updateAssignmentsRequest.MapTo<UpdateAssignmentsRequest, UpdateAssignmentsArgs>();
            args.Id = assignmentsId;
            var result = await _courseService.UpdateAssignments(args);
            return Ok(new GeneralResponse<AssignmentsResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new AssignmentsResponse(result),
                Message = "Update asssignments sucessfully"
            });
        }

        private async Task<bool> ValidateUserInClassAsync(string username, int courseId, Role role = Role.None, bool isCheckOwner = false)
        {
            var user = await _appUserManager.FindByNameAsync(username);
            if (user == null)
            {
                return false;
            }
            var coursers = (await _courseService.GetByIdAsync(courseId));
            if (coursers == null)
            {
                return false;
            }
            var courseUser = GeneralModelRepository.GetQueryable<Course_User>().Where(c => c.UserId == user.Id && c.CourseId == courseId).FirstOrDefault();
            if (courseUser == null)
            {
                return false;
            }

            if (role != Role.None)
            {
                return courseUser.Role == role;
            }
            
            if (isCheckOwner)
            {
                return coursers.CreateBy == username;
            }

            return true;
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        [Route("{id}/assignments/{assignmentsId}")]
        public async Task<IActionResult> DeleteAssignmentsAsync(int id, string currentUser, long assignmentsId)
        {
            if (!(await ValidateUserInClassAsync(currentUser, id, Role.Teacher)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Delete asssignments failed!!"
                });
            }
            
            await GeneralModelRepository.Delete<Assignments>(assignmentsId);
            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Delete asssignments sucessfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{id}/assignments")]
        public async Task<IActionResult> GetAssignmentsAsync(int id, [FromQuery] AssignmentsQuery query)
        {
            if (!(await ValidateUserInClassAsync(query.CurrentUser, id)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Get asssignments failed!!"
                });
            }
            query.CourseId = id;
            var result =await GetSearchResult(query, a => new AssignmentsResponse(a));
            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = result,
                Message = "Get assignments sucessfully"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("{id}/assignments-sort")]
        public async Task<IActionResult> SortAssignmentsAsync(int id, [FromBody]SortAssignmentsRequest sortAssignmentsRequest)
        {
            if (!(await ValidateUserInClassAsync(sortAssignmentsRequest.CurrentUser, id)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Sort asssignments failed!!"
                });
            }
            var args = sortAssignmentsRequest.MapTo<SortAssignmentsRequest, SortAssignmentsArgs>();
            args.CourseId = id;
            var result = _courseService.SortAssignments(args);
            var response = result.Select(a => new AssignmentsResponse(a));
            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = response,
                Message = "Sort asssignments sucessfully"
            });
        }

        //TODO
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{id}/all-grades")]
        public async Task<IActionResult> GetAllGradeAsync(int id, string currentUser)
        {
            if (!(await ValidateUserInClassAsync(currentUser, id, Role.Teacher)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Get all grades asssignments failed!!"
                });
            }
            var result =  _courseService.GetAllGradeOfCourse(id);
            var listAssignment = GeneralModelRepository.GetQueryable<Assignments>().Where(a => a.CourseId == id).Select(a => new AssignmentSimpleResponse(a)).ToList();
            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new { 
                    header = listAssignment,
                    scores = result,
                    total = result.Count
                },
                Message = "Get assignments sucessfully"
            });
        }
        
        //TODO
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{id}/assignments/{assignmentsId}/all-grades")]
        public async Task<IActionResult> GetAllGradeAsync(int id, long assignmentsId, string currentUser)
        {
            if (!(await ValidateUserInClassAsync(currentUser, id, Role.Teacher)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Get all grades asssignments failed!!"
                });
            }
            var result =  _courseService.GetAllGradeOfAssignment((int)assignmentsId);
            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new { 
                    data = result,
                    total = result.Count
                },
                Message = "Get assignments sucessfully"
            });
        }
        
        //TODO
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("{id}/assignments/{assignmentsId}/update-grade")]
        public async Task<IActionResult> UpdateGradeAsync(int id, long assignmentsId, [FromForm] UpdateGradeRequest updateGrade)
        {
            if (!(await ValidateUserInClassAsync(updateGrade.CurrentUser, id, Role.Teacher)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Get asssignments failed!!"
                });
            }

            var listGrade = ConvertFileToListGrade(updateGrade.file, (int)assignmentsId);
            var args = new UpdateGradesArgs { 
                AssignmentId = (int)assignmentsId,
                CourseId = id,
                CurrentUser = updateGrade.CurrentUser,
                Grades = listGrade
            };
            await _courseService.UpdateGrades(args);

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Get assignments sucessfully"
            });
        }
        
        //TODO
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("{id}/update-student")]
        public async Task<IActionResult> UpdateMemberAsync(int id, [FromForm] UpdateMemberByFileRequest updateMember)
        {
            if (!(await ValidateUserInClassAsync(updateMember.CurrentUser, id, isCheckOwner: true)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Update member failed!!"
                });
            }

            var listStudent = ConvertFileToListStudent(updateMember.file);
            var args = new UpdateMemberInClassArgs
            { 
                CourseId = id,
                CurrentUser = updateMember.CurrentUser,
                Students = listStudent
            };
            var result = await _courseService.UpdateMemberInClass(args);

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Get assignments sucessfully"
            });
        }

        private ICollection<Grade> ConvertFileToListGrade (IFormFile file, int assignmentId)
        {
            var grades = new List<Grade>();
            if (file != null)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var sreader = ExcelReaderFactory.CreateReader(file.OpenReadStream()))
                {
                    sreader.Read();
                    do
                    {
                        while (sreader.Read()) //Each ROW
                        {
                            var gradeAssignment = 0f;
                            var mssv = "";
                            try
                            {
                                gradeAssignment = Convert.ToSingle(sreader.GetValue(1));
                                mssv = Convert.ToString(sreader.GetValue(0));
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            var grade = new Grade {
                                AssignmentId = assignmentId,
                                GradeAssignment = gradeAssignment,
                                MSSV = mssv
                            };
                            grades.Add(grade);
                        }
                    } while (sreader.NextResult());
                }
            }
            return grades;
        }

        private ICollection<Student> ConvertFileToListStudent (IFormFile file)
        {
            var students = new List<Student>();
            if (file != null)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var sreader = ExcelReaderFactory.CreateReader(file.OpenReadStream()))
                {
                    sreader.Read();
                    do
                    {
                        while (sreader.Read()) //Each ROW
                        {
                            var studentID = "";
                            var fullName = "";
                            try
                            {
                                studentID = Convert.ToString(sreader.GetValue(0));
                                fullName = Convert.ToString(sreader.GetValue(1));
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            var student = new Student
                            {
                                StudentID = studentID,
                                FullName = fullName
                            };
                            students.Add(student);
                        }
                    } while (sreader.NextResult());
                }
            }
            return students;
        }
        
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("{id}/assignments/{assignmentsId}/update-grade-normal")]
        public async Task<IActionResult> UpdateGradeForStudentSpecificAsync(int id, long assignmentsId, [FromBody] UpdateGradeNormalRequest request)
        {
            if (!(await ValidateUserInClassAsync(request.CurrentUser, id, Role.Teacher)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Get asssignments failed!!"
                });
            }
            var args = new UpdateGradeNormalArgs {
                AssignmentId = (int)assignmentsId,
                CourseId = id,
                CurrentUser = request.CurrentUser,
                Grades = new List<UpdateGradeSpecificArgsBase>()
            };

            for (int i = 0; i < request.Scores.Count; i++)
            {
                var grade = new UpdateGradeSpecificArgsBase {
                    MSSV = request.Scores[i].MSSV,
                    GradeAssignment = request.Scores[i].Grade,
                    IsFinalized = request.Scores[i].IsFinalized
                };
                args.Grades.Add(grade);
            }

            var result = await _courseService.UpdateGradeNormal(args);
            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = result,
                Message = "Get assignments sucessfully"
            });
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("{id}/assignments/{assignmentsId}/update-grade-finalized")]
        public async Task<IActionResult> UpdateGradeForStudentSpecificAsync(int id, long assignmentsId, [FromBody] UpdateGradeSpecificRequest request)
        {
            if (!(await ValidateUserInClassAsync(request.CurrentUser, id, Role.Teacher)))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Get asssignments failed!!"
                });
            }
            
            var result = await _courseService.UpdateGradeSpecific(new UpdateGradeSpecificArgs { 
                    AssignmentsId = (int)assignmentsId, 
                    CourseId = id, 
                    GradeAssignment = request.Grade,
                    MSSV = request.MSSV,
                    CurrentUser = request.CurrentUser});
            return Ok(new GeneralResponse<object>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = result,
                Message = "Get assignments sucessfully"
            });
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("send-mail")]
        public IActionResult SendMail([FromBody] SendMailJoinToCourseRequest sendMailJoinToCourseRequest)
        {

            Guards.ValidEmail(sendMailJoinToCourseRequest.MailPersonReceive);
            var tokenClassCode = StringHelper.GenerateHashString(sendMailJoinToCourseRequest.ClassCode);
            var tokenEmail = StringHelper.GenerateHashString(sendMailJoinToCourseRequest.ClassCode);
            var inviteLink = $"{ConfigClient.URL_CLIENT}/class-join?classToken={tokenClassCode}&role={(int)sendMailJoinToCourseRequest.Role}&email={tokenEmail}";
            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendConfirmMail(sendMailJoinToCourseRequest.MailPersonReceive, inviteLink);
            //_emailService.Send(sendMailJoinToCourseRequest.MailPersonReceive, tokenClassCode, inviteLink);
            if (!emailResponse)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = "",
                    Message = $"Send mail to {sendMailJoinToCourseRequest.MailPersonReceive} failed"
                });
            }

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = $"Send mail to {sendMailJoinToCourseRequest.MailPersonReceive} successfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("{id}/everyone")]
        public async Task<IActionResult> GetMemberInCourse(int id)
        {
            var course = await GeneralModelRepository.GetAndCheckExisting<Course>(id);

            var getTeachersQuery = new CourseUserQuery
            {
                CourseId = id,
                Role = Role.Teacher,
                StartAt = 0,
                MaxResults = 100,
            };

            var getStudentsQuery = new CourseUserQuery
            {
                CourseId = id,
                Role = Role.Student,
                StartAt = 0,
                MaxResults = 100,
            };
            var listTeacherIds = (await GetSearchResult(getTeachersQuery, c => c.UserId)).Data;
            var listTeacher = await GeneralModelRepository.GetQueryable<AppUser>().Where(user => listTeacherIds.Contains(user.Id)).Select(user => new UserResponse(user)).ToListAsync();

            var listStudentIds = (await GetSearchResult(getStudentsQuery, c => c.UserId)).Data;
            var listStudent = await GeneralModelRepository.GetQueryable<AppUser>().Where(user => listStudentIds.Contains(user.Id)).Select(user => new UserResponse(user)).ToListAsync();
            var listStudentCode = listStudent.Select(s => s.StudentID).ToList();
            var listStudentCodeNotHasAccount = await GeneralModelRepository.GetQueryable<Course_Student>().Where(cs => !listStudentCode.Contains(cs.StudentCode) && cs.CourseId == id).Select(cs => cs.StudentCode).ToListAsync();
            listStudent.AddRange(await GeneralModelRepository.GetQueryable<Student>().Where(s => listStudentCodeNotHasAccount.Contains(s.StudentID)).Select(s => new UserResponse(s)).ToListAsync());
            var memberCourseResponse = new MemberCourseResponse
            {
                Total = listStudent.Count + listTeacher.Count,
                Owner = course.CreateBy,
                Teachers = listTeacher,
                Students = listStudent,
            };
            return Ok(
                new GeneralResponse<MemberCourseResponse>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = memberCourseResponse,
                    Message = "Get Member Successfull"
                });

        }

        [HttpGet]
        [Route("{id}/download-grade-board")]
        public FileResult DownloadGradeBoard(int id)
        {
            var result = _courseService.GetAllGradeOfCourse(id);
            var listAssignment = GeneralModelRepository.GetQueryable<Assignments>().Where(a => a.CourseId == id).Select(a => new AssignmentSimpleResponse(a)).ToList();
            DataTable dt = new DataTable("GradeBoard");
            dt.Columns.Add(new DataColumn("Họ và Tên"));
            dt.Columns.Add(new DataColumn("MSSV"));
            foreach (var assignment in listAssignment)
            {
                dt.Columns.Add(new DataColumn(assignment.Name));
            }

            foreach (var item in result)
            {
                var row = dt.NewRow();
                row[0] = item.Name;
                row[1] = item.Mssv;
                var index = 2;
                foreach (var score in item.Grades)
                {
                    row[index] = score.Grade.ToString() +"/" + score.MaxGrade.ToString();
                    index++;
                }
                dt.Rows.Add(row);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GradeBoard.xlsx");
                }
            }
        }

        [HttpGet]
        [Route("download-template-update-member")]
        public FileResult DownloadTemplateUpdateMember()
        {
            DataTable dt = new DataTable("TemplateUpdateMember");
            dt.Columns.Add(new DataColumn("StudentId"));
            dt.Columns.Add(new DataColumn("FullName"));

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TemplateUpdateMember.xlsx");
                }
            }
        }

        [HttpGet]
        [Route("download-template-update-grade")]
        public FileResult DownloadTemplateUpdateGrade()
        {
            DataTable dt = new DataTable("TemplateUpdateGrade");
            dt.Columns.Add(new DataColumn("StudentId"));
            dt.Columns.Add(new DataColumn("Grade"));

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TemplateUpdateGrade.xlsx");
                }
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateCousersAsync([FromBody] CreateCourseRequest courseRequest)
        {
            var user = await _appUserManager.FindByNameAsync(courseRequest.CurrentUser);
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
            var args = new CreateCourseArgs
            {
                SubjectId = courseRequest.SubjectId,
                Schedule = courseRequest.Schedule,
                Description = courseRequest.Description,
                GradeId = courseRequest.GradeId,
                Title = courseRequest.Title,
                Role = courseRequest.Role,
                Credits = courseRequest.Credits,
                CurrentUser = courseRequest.CurrentUser,
                UserId = user.Id
            };
            var courser = await _courseService.CreateCourseAsync(args);

            return Ok(new GeneralResponse<CourseResponse>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = new CourseResponse(courser),
                Message = "Created new course successfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add-member/invite-link")]
        public async Task<IActionResult> AddStudentIntoCousersByLinkAsync([FromBody] AddMemberIntoCourseByLinkRequest courseRequest)
        {
            var user = await _appUserManager.FindByNameAsync(courseRequest.CurrentUser);
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
            
            var course = GeneralModelRepository.GetQueryable<Course>().AsEnumerable()
                .Where(c => !String.IsNullOrEmpty(c.CourseCode) &&
                StringHelper.Check(courseRequest.Token,c.CourseCode))
                .FirstOrDefault();
            if (course == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found course"
                });
            }

            if (String.IsNullOrEmpty(courseRequest.Invitee))
            {
                if (await AddMemberIntoCourseAsync(user, courseRequest.Role, course.Id))
                {
                    return Ok(new GeneralResponse<string>
                    {
                        Status = ApiResponseStatus.Success,
                        Result = ResponseResult.Successfull,
                        Content = "",
                        Message = "Add member successfull"
                    });
                }

                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Add member failed"
                });
            }

            var invitee = await _appUserManager.FindByNameAsync(courseRequest.Invitee);
            if (invitee == null)
            {

                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found inviteed user"
                });
            }

            if (await AddMemberIntoCourseAsync(invitee.Id, user.UserName, courseRequest.Role, course.Id))
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Success,
                    Result = ResponseResult.Successfull,
                    Content = "",
                    Message = "Add member successfull"
                });
            }

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Error,
                Result = ResponseResult.Error,
                Content = "",
                Message = "Add member failed"
            });
        }
        private async Task<bool> AddMemberIntoCourseAsync(int newMemberId, string currentUser, Role role, int courseId)
        {
            var courseExist = GeneralModelRepository.GetQueryable<Course_User>().Where(c => c.Role == role && c.UserId == newMemberId && c.CourseId == courseId).Any();
            if (courseExist)
            {
                return false;
            }
            var courseUser = new Course_User
            {
                UserId = newMemberId,
                CourseId = courseId,
                Role = role,
            };

            AuditHelper.CreateAudit(courseUser, currentUser);

            _ = await GeneralModelRepository.Create<Course_User>(courseUser);
            return true;
        }
        private async Task<bool> AddMemberIntoCourseAsync(AppUser appUser, Role role, int courseId)
        {
            var courseExist = GeneralModelRepository.GetQueryable<Course_User>().Where(c => c.Role == role && c.UserId == appUser.Id && c.CourseId == courseId).Any();
            if (courseExist)
            {
                return false;
            }
            var courseUser = new Course_User
            {
                UserId = appUser.Id,
                CourseId = courseId,
                Role = role,
            };

            AuditHelper.CreateAudit(courseUser, appUser.UserName);

            _ = await GeneralModelRepository.Create<Course_User>(courseUser);
            return true;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("update-role-member")]
        public async Task<IActionResult> RemoveStudentInCousersAsync([FromBody] UpdateRoleMemberInCourseRequest courseRequest)
        {
            var user = await _appUserManager.FindByNameAsync(courseRequest.CurrentUser);
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

            var course = await GeneralModelRepository.Get<Course>(courseRequest.CourseId);
            if (course == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found Course"
                });
            }

            if (user.UserName != course.CreateBy)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "You are not owner classroom"
                });
            }

            var courseUser = GeneralModelRepository.GetQueryable<Course_User>().Where(c => c.CourseId == courseRequest.CourseId && c.UserId == courseRequest.UserId).FirstOrDefault();
            if (courseUser == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user in class"
                });
            }
            courseUser.Role = courseRequest.Role;
            await GeneralModelRepository.Update<Course_User>(courseUser);

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Add member classroom successfull"
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("remove-member")]
        public async Task<IActionResult> RemoveStudentInCousersAsync([FromBody] RemoveMemberInCourseRequest courseRequest)
        {
            var user = await _appUserManager.FindByNameAsync(courseRequest.CurrentUser);
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

            var course = await GeneralModelRepository.Get<Course>(courseRequest.CourseId);
            if (course == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found Course"
                });
            }

            if (user.UserName != course.CreateBy)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "You are not owner classroom"
                });
            }

            var courseUser = GeneralModelRepository.GetQueryable<Course_User>().Where(c => c.CourseId == courseRequest.CourseId && c.UserId == courseRequest.UserId).FirstOrDefault();
            if (courseUser == null)
            {
                return Ok(new GeneralResponse<string>
                {
                    Status = ApiResponseStatus.Error,
                    Result = ResponseResult.Error,
                    Content = "",
                    Message = "Not found user in class"
                });
            }

            await GeneralModelRepository.Delete<Course_User>(courseUser.Id);

            return Ok(new GeneralResponse<string>
            {
                Status = ApiResponseStatus.Success,
                Result = ResponseResult.Successfull,
                Content = "",
                Message = "Add member classroom successfull"
            });
        }
    }
}
