using IdentityServer4.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ManageCourse.Core.Utilities
{
    public static class UtilityMethods
    {
        public static bool IsValidEmail(string email)
        {
            var rx = new Regex(@"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            return rx.IsMatch(email);
        }

    }
    public static class Guards
    {
        public static void ValidateTimeFrame(DateTime startTime, DateTime endTime, string startTimeFieldName, string endTimeFieldName)
        {
            if (startTime > endTime)
            {
                throw new ArgumentException($"'{endTimeFieldName}' must be greater than '{startTimeFieldName}'.");
            }
        }

        public static void NotNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentException($"'{name}' can not be null or empty.");
        }

        public static void NotNullOrEmpty<T>(IEnumerable<T> obj, string name)
        {
            if (obj.IsNullOrEmpty()) throw new ArgumentException($"'{name}' can not be null or empty.");
        }

        public static void PositiveNumber(int value, string name)
        {
            if (value < 0) throw new ArgumentException($"'{name}' can not be a negative number.");
        }

        public static void ValidEmail(string emailAddress)
        {
            NotNullOrEmpty(emailAddress, nameof(emailAddress));
            if (!UtilityMethods.IsValidEmail(emailAddress)) throw new ArgumentException($"'{emailAddress}' is not a valid email.");
        }

        public static void ValidId(long id, string name)
        {
            if (id <= 0) throw new ArgumentException($"'{name}' is not valid id.");
        }

        public static void ValidAmount(decimal value, string name)
        {
            if (value < 0) throw new ArgumentException($"'{name}' must be a positive number.");
        }

        public static void NotNull(object obj, string name)
        {
            if (obj == null) throw new ArgumentException($"'{name}' can not be null.");
        }

        public static void ValidateEnum<TEnum>(int value, string fieldName)
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new ArgumentException($"Invalid value for the type '{fieldName}'.");
        }
    }

    public static class Alias
    {
        public static T For<T>()
        {
            return default;
        }
    }
}
