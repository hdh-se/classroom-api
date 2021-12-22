using ManageCourse.Core.Constansts;
using ManageCourse.Core.Data;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Model.Args;
using ManageCourse.Core.Model.Responses;
using ManageCourse.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services.Implementation
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IGeneralModelRepository _generalModelRepository;

        public CourseService(AppDbContext appDbContext, IGeneralModelRepository generalModelRepository)
        {
            _appDbContext = appDbContext;
            _generalModelRepository = generalModelRepository;
        }

        public async Task<Course> CreateCourseAsync(CreateCourseArgs courseArgs)
        {
            var courses = new Course
            {
                SubjectId = courseArgs.SubjectId,
                Schedule = courseArgs.Schedule,
                Description = courseArgs.Description,
                GradeId = courseArgs.GradeId,
                CourseCode = StringHelper.GenerateCode(20),
                Title = courseArgs.Title,
                Credits = courseArgs.Credits
            };
            AuditHelper.CreateAudit(courses, courseArgs.CurrentUser);
            _ = await _appDbContext.Courses.AddAsync(courses);
            await _appDbContext.SaveChangesAsync();
            var courseUser = new Course_User
            {
                CourseId = courses.Id,
                UserId = courseArgs.UserId,
                Role = courseArgs.Role != Role.None ? courseArgs.Role : Role.Teacher
            };
            await _generalModelRepository.Create(courseUser);
            return courses;
        }

        public async Task<Course_User> AddMemberIntoCourseAsync(AddMemberIntoCourseArgs memberIntoCourseArgs)
        {
            var courseUser = new Course_User
            {
                CourseId = memberIntoCourseArgs.CourseId,
                UserId = memberIntoCourseArgs.UserId,
                Role = memberIntoCourseArgs.Role != Role.None ? memberIntoCourseArgs.Role : Role.Student
            };
            AuditHelper.CreateAudit(courseUser, memberIntoCourseArgs.CurrentUser);

            await _generalModelRepository.Create(courseUser);
            return courseUser;
        }

        public async Task<Course> GetByIdAsync(long id)
        {
            var courses = await _generalModelRepository.Get<Course>(id);
            return courses;
        }

        public async Task<Assignments> CreateNewAssignments(CreateNewAssignmentsArgs createNewAssignmentsArgs)
        {
            var numberOfAssignments = _generalModelRepository.GetQueryable<Assignments>().Where(c => c.CourseId == createNewAssignmentsArgs.CourseId).Count();
            var assignments = new Assignments
            {
                CourseId = createNewAssignmentsArgs.CourseId,
                Name = createNewAssignmentsArgs.Name,
                MaxGrade = createNewAssignmentsArgs.MaxGrade,
                Description = createNewAssignmentsArgs.Description,
                Order = numberOfAssignments + 1
            };
            AuditHelper.CreateAudit(assignments, createNewAssignmentsArgs.CurrentUser);
            await _generalModelRepository.Create(assignments);
            return assignments;
        }

        public async Task<Assignments> UpdateAssignments(UpdateAssignmentsArgs updateAssignmentsArgs)
        {
            var assignment = await _generalModelRepository.Get<Assignments>(updateAssignmentsArgs.Id);
            assignment.Name = String.IsNullOrEmpty(updateAssignmentsArgs.Name) ? assignment.Name : updateAssignmentsArgs.Name;
            assignment.Description = String.IsNullOrEmpty(updateAssignmentsArgs.Description) ? assignment.Description : updateAssignmentsArgs.Description;
            assignment.MaxGrade = assignment.MaxGrade > 0 && assignment.MaxGrade != updateAssignmentsArgs.MaxGrade ? updateAssignmentsArgs.MaxGrade : assignment.MaxGrade;
            AuditHelper.UpdateAudit(assignment, updateAssignmentsArgs.CurrentUser);
            await _generalModelRepository.Update(assignment);
            return assignment;
        }

        public ICollection<Assignments> SortAssignments(SortAssignmentsArgs sortAssignmentsArgs)
        {
            var listAssignments = _generalModelRepository.GetQueryable<Assignments>().AsEnumerable().Where(a => a.CourseId == sortAssignmentsArgs.CourseId).ToList();
            foreach (var assignments in listAssignments)
            {
                var order = sortAssignmentsArgs.assignmentSimples.AsEnumerable().Where(a => a.Id == assignments.Id).FirstOrDefault();
                if (order != null)
                {
                    assignments.Order = order.Order;
                }
            }

            _appDbContext.Assignments.UpdateRange(listAssignments);
            _appDbContext.SaveChanges();
            return listAssignments;
        }

        public async Task<bool> UpdateGrades(UpdateGradesArgs updateGradesArgs)
        {
            var assignment = await _generalModelRepository.Get<Assignments>(updateGradesArgs.AssignmentId);
            if (assignment == null || assignment.CourseId != updateGradesArgs.CourseId)
            {
                return false;
            }

            var studentIDs = updateGradesArgs.Grades.Select(g => g.MSSV);
            var listGradeExist = _generalModelRepository.GetQueryable<Grade>().Where(g => studentIDs.Contains(g.MSSV) && g.AssignmentId == assignment.Id).ToList();
            var listNewStudent = new List<Student>();
            foreach (var grade in updateGradesArgs.Grades)
            {
                var gradeExist = listGradeExist.Where(g => g.MSSV == grade.MSSV).FirstOrDefault();
                if (gradeExist != null)
                {
                    gradeExist.GradeAssignment = grade.GradeAssignment;
                    gradeExist.IsFinalized = false;
                    AuditHelper.UpdateAudit(gradeExist, updateGradesArgs.CurrentUser);
                    _appDbContext.Update(gradeExist);
                }
                else
                {
                    var student = _generalModelRepository.GetQueryable<Student>().Where(s => s.StudentID == grade.MSSV).FirstOrDefault();
                    if (student == null)
                    {
                        student = new Student
                        {
                            StudentID = grade.MSSV
                        };
                        AuditHelper.CreateAudit(student, updateGradesArgs.CurrentUser);
                        await _generalModelRepository.Create(student);
                        listNewStudent.Add(student);
                    }
                    grade.StudentId = student.Id;
                    grade.IsFinalized = false;
                    grade.AssignmentId = assignment.Id;
                    AuditHelper.CreateAudit(grade, updateGradesArgs.CurrentUser);
                    _appDbContext.Add(grade);
                }
            }

            await _appDbContext.SaveChangesAsync();
            await CreateCourseStudentAsync(listNewStudent, updateGradesArgs.CourseId, updateGradesArgs.CurrentUser);
            return true;
        }

        public async Task<bool> UpdateMemberInClass(UpdateMemberInClassArgs updateMembers)
        {
            var studentIDs = updateMembers.Students.Select(s => s.StudentID);
            var listStudentExist = _generalModelRepository.GetQueryable<Student>().Where(g => studentIDs.Contains(g.StudentID)).ToList();
            var listStudent = new List<Student>();
            foreach (var student in updateMembers.Students)
            {
                var studentExist = listStudentExist.Where(s => s.StudentID == student.StudentID).FirstOrDefault();
                if (studentExist != null)
                {
                    studentExist.FullName = student.FullName != studentExist.FullName ? student.FullName : studentExist.FullName;
                    AuditHelper.UpdateAudit(studentExist, updateMembers.CurrentUser);
                    _appDbContext.Update(studentExist);
                    listStudent.Add(studentExist);
                }
                else
                {
                    AuditHelper.CreateAudit(student, updateMembers.CurrentUser);
                    _appDbContext.Add(student);
                    listStudent.Add(student);
                }
            }

            await _appDbContext.SaveChangesAsync();
            await CreateCourseStudentAsync(listStudent, updateMembers.CourseId, updateMembers.CurrentUser);
            return true;
        }

        private async Task CreateCourseStudentAsync(ICollection<Student> students, int courseId, string currentUser)
        {
            var listCourseStudentExist = _generalModelRepository.GetQueryable<Course_Student>().Where(c => c.CourseId == courseId).ToList();

            foreach (var student in students)
            {
                if (!listCourseStudentExist.Where(c => c.StudentCode == student.StudentID).Any())
                {
                    var courseStudent = new Course_Student
                    {
                        StudentCode = student.StudentID,
                        CourseId = courseId,
                        StudentId = student.Id,
                    };
                    AuditHelper.CreateAudit(courseStudent, currentUser);
                    _appDbContext.Add(courseStudent);
                }
            }

            await _appDbContext.SaveChangesAsync();
        }

        public ICollection<GradeOfAssignmentResponse> GetAllGradeOfAssignment(int assignmentId)
        {
            var result = _appDbContext.Grades
                .Join(_appDbContext.Students,
                        Grade => Grade.StudentId,
                        Student => Student.Id,
                        (Grade, Student) => new{
                            Student = Student,
                            Grade = Grade,
                        }).Join(_appDbContext.Assignments,
                                data => data.Grade.AssignmentId,
                                Assignment => Assignment.Id,
                                (data, Assignment) => new
                                {
                                    Student = data.Student,
                                    Grade = data.Grade,
                                    Assignment = Assignment
                                }).Where(d => d.Assignment.Id == assignmentId).Select(x => new GradeOfAssignmentResponse(x.Student, x.Grade, x.Assignment)).ToList();
            return result;
        }

        public ICollection<GradeOfCourseResponse> GetAllGradeOfCourse(int courseId)
        {
            var result = _appDbContext.Students.Select(s => new GradeOfCourseResponse
            {
                Id = s.Id,
                Mssv = s.StudentID,
                Name = s.FullName,
                Grades = _appDbContext.Grades.Join(_appDbContext.Assignments, 
                Grade => Grade.AssignmentId, 
                Assignments => Assignments.Id, 
                (Grade, Assignments) => new {
                    Grade = Grade,
                    Assignment = Assignments}).Where(data => data.Grade.MSSV == s.StudentID && data.Assignment.CourseId == courseId).Select(d => new GradeSimpleResponse(d.Grade, d.Assignment)).ToList()
            }).ToList();
            return result;
        }

        public async Task<bool> UpdateGradeSpecific(UpdateGradeSpecificArgs updateGrade)
        {
            var assignment = await _generalModelRepository.Get<Assignments>(updateGrade.AssignmentsId);
            if (assignment == null || assignment.CourseId != updateGrade.CourseId)
            {
                return false;
            }

            var student = _generalModelRepository.GetQueryable<Student>().Where(s => s.StudentID == updateGrade.MSSV).FirstOrDefault();
            if (student==null)
            {
                return false;
            }
            var grade = _generalModelRepository.GetQueryable<Grade>().Where(s => s.AssignmentId == updateGrade.AssignmentsId && s.StudentId == student.Id).FirstOrDefault();
            if (grade == null)
            {
                return false;
            }
            grade.GradeAssignment = updateGrade.GradeAssignment;
            grade.IsFinalized = updateGrade.IsFinalized;
            await _generalModelRepository.Update(grade);
            return true;
        }
    }
}
