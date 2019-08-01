using System;

namespace checkoutcom.paymentgateway.Exceptons
{
    public class PaymentValidationException: System.Exception
    {
        public PaymentValidationException(string error): base(error)
        {

        }
    }
}