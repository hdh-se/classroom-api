using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Constansts
{
    public enum ApiResultCodes
    {
        General = 100000,
        NotFound = 100001,
        InvalidProperty = 100002,
        Duplicated = 100003,

        RefundFailed = 100004,
        PaymentFailed = 100005,
        CheckoutFailed = 100006,
        UpdateSeatStatusFailed = 100007,

        InvalidAction = 100009,
        ExpiredRequest = 100010,
        PaymentPending = 100011,
        InvalidGrant = 100012,
        PasswordChange = 100013,

        InvalidObject = 100014,
        SendingEmail = 100015,

        Unknown = 999999
    }
}
