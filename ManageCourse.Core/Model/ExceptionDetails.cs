using ManageCourse.Core.Constansts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Model
{
    public class ExceptionDetails
    {
        public ExceptionDetails(ApiResultCodes code, string errorMessage, object details = null)
        {
            Code = code;
            Message = errorMessage;
            Details = details ?? string.Empty;
        }

        [JsonProperty("code")]
        public ApiResultCodes Code { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
        public object Details { get; set; }

        public override string ToString()
        {
            var str = $"({(int)Code} {Code}) {Message}";
            if (Details != null)
                str += Environment.NewLine + Details;
            return str;
        }
    }
}
