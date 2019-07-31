using System;

namespace checkoutcom.paymentgateway.Models
{
    public class Idempotency
    {
        public Guid Key { get; set; }
        public Payment Payment { get; set; }   
    }
}
