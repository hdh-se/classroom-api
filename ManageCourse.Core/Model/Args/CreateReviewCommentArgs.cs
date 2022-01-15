﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model.Args
{
    public class CreateReviewCommentArgs
    {
        public string Message { get; set; }
        public int GradeReviewId { get; set; }
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public string CurrentUser { get; set; }
    }
}
