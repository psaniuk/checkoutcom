using System;

namespace checkoutcom.paymentgateway.Exceptons
{
    public class PaymentValidationException: Exception
    {
        public PaymentValidationException(string error): base(error)
        {

        }
    }
}