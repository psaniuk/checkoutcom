using System;

namespace checkoutcom.paymentgateway.Exceptons
{
    public class ProcessPaymentException : Exception
    {
        public ProcessPaymentException() { }
        public ProcessPaymentException(string message) : base(message) { }
        public ProcessPaymentException(string message, System.Exception inner) : base(message, inner) { }
    }
}