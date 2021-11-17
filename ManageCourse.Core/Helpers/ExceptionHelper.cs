using ManageCourse.Core.Constansts;
using ManageCourse.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Core.Helpers
{
    public class ExceptionHelper
    {
        public static BusinessRuleException CreateBusinessRuleException(string message)
        {
            throw new BusinessRuleException(ApiResultCodes.InvalidProperty, message);
        }

        public static BusinessRuleException CreateCheckoutFailedException(string message)
        {
            throw new BusinessRuleException(ApiResultCodes.CheckoutFailed, message);
        }

        public static BusinessRuleException CreateInvalidActionException(string message)
        {
            throw new BusinessRuleException(ApiResultCodes.InvalidAction, message);
        }

        public static BusinessRuleException CreateNotFoundException(string message)
        {
            throw new BusinessRuleException(ApiResultCodes.NotFound, message);
        }

        public static BusinessRuleException CreatePaymentFailedException(string message)
        {
            throw new BusinessRuleException(ApiResultCodes.PaymentFailed, message);
        }

        public static BusinessRuleException CreateValidationException(string fieldName)
        {
            throw new BusinessRuleException(ApiResultCodes.InvalidProperty, $"{fieldName} is invalid");
        }
    }
}
