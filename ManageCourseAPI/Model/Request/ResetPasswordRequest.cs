﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class ResetPasswordRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }

    }
}
