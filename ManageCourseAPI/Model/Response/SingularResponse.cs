using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class SingularResponse<TValue>
    {
        public TValue Result { get; set; }
    }
}
