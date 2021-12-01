﻿using ManageCourse.Core.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Data
{
    public class Student: Audit
    {
        public int Id { get; set; }
        public string StudentID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBird { get; set; }
        public string Phone { get; set; }
        public virtual ICollection<Assignments_Student> Assignments_Students { get; set; }
    }
}
