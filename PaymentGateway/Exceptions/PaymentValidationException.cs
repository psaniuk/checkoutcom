using System;
using System.Runtime.Serialization;

namespace checkoutcom.paymentgateway.Exceptons
{
    public class PaymentValidationException: Exception
    {
        public PaymentValidationException()
        {
        }

        public PaymentValidationException(string error): base(error)
        {

        }

        public PaymentValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}