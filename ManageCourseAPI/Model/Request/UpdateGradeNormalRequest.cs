using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class UpdateGradeNormalRequest
    {
        public bool IsFinalized { get; set; }
        public List<UpdateGradeSpecificRequestBase> Scores { get; set; }
        public string CurrentUser { get; set; }
    }
}
