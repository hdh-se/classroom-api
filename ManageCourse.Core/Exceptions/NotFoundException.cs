using System;
using System.Runtime.Serialization;

namespace ManageCourse.Core.Exceptions
{
    [Serializable]
    public class NotFoundException: Exception
    {
        public NotFoundException() : base("Not Found Exception") { }

        public NotFoundException(string msg) : base(msg) { }

        public NotFoundException(string msg, Exception e) : base(msg, e) { }

        protected NotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
