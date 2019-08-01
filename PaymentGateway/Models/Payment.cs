using System;

namespace checkoutcom.paymentgateway.Models
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public CardNumber CardNumber { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public DateTime ExpireAt {get; set; }
        public string CVV { get; set; } = string.Empty;
        public Guid TransactionId { get; set; } = Guid.Empty;
    }
}