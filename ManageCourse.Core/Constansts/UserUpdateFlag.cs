using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Constansts
{
    [Flags]
    public enum UserUpdateFlag
    {
        None = 0,
        Password = 1,
        AuthData = 2,
    }
}
