using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageCourseAPI.Model.Response
{
    public class GeneralResultResponse<T>
    {
        public bool HasMore { get; }
        public int Total { get; }
        public IList<T> Data { get; }
        public GeneralResultResponse(bool hasMore, int total, IEnumerable<T> data)
        {
            HasMore = hasMore;
            Total = total;
            Data = data.ToList().AsReadOnly();
        }
        
        public GeneralResultResponse( IEnumerable<T> data)
        {
            HasMore = false;
            Total = data.Count();
            Data = data.ToList().AsReadOnly();
        }
    }
}
