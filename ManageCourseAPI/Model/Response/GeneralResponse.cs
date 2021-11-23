using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class GeneralResponse <TResult>
    {
        public ApiResponseStatus Status { get; set; }
        public ResponseResult Result { get; set; }
        public string Message { get; set; }
        public TResult Content { get; set; }    
    }
}
