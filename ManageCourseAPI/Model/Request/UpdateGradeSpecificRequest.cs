using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Request
{
    public class UpdateGradeSpecificRequestBase
    {
        public string MSSV { get; set; }
        public float Grade { get; set; }
        public bool IsFinalized { get; set; }
    } 
    
    public class UpdateGradeSpecificRequest : UpdateGradeSpecificRequestBase
    {
        public string CurrentUser { get; set; }

    }
}
