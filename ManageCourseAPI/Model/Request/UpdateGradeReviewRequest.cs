﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class UpdateGradeReviewRequest
    {
        public int CourseId { get; set; }
        public int GradeId { get; set; }
        public int GradeReviewId { get; set; }
        public float GradeExpect { get; set; }
        public string Reason { get; set; }
        public string CurrentUser { get; set; }
    }
}
