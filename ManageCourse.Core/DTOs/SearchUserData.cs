using ManageCourse.Core.Constansts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.DTOs
{
    public class SearchUserData
    {
        public string Query { get; set; }
        public string SortColumn { get; set; }
        public SortDirection? SortDirection { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }
}
