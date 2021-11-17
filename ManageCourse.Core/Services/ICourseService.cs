﻿using ManageCourse.Core.Data;
using ManageCourse.Core.Model.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Services
{
    public interface ICourseService
    {
        public Task<Course> CreateCourseAsync(CreateCourseArgs courseArgs);
        public Task<List<Course>> GetCourseAsync();
        public Task<Course> GetByIdAsync(long id);
    }
}
