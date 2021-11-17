using System.Collections.Generic;

namespace ManageCourse.Core.DTOs
{
    public class PagingResult<TResult>
    {
        public IList<TResult> Results { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
