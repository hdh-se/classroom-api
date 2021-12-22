using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class UpdateMemberByFileRequest
    {
        public IFormFile file { get; set; }
        public string CurrentUser { get; set; }
    }
}
