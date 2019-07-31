using System;

namespace checkoutcom.paymentgateway.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public CardNumber CardNumber { get; set; } = CardNumber.Empty;
        public decimal Amount { get; set; }
        public Currency Currency { get; set; } = Currency.Empty;
        public DateTime ExpireAt {get; set; }
        public string CVV { get; set; }
        public Guid TransactionId { get; set; }
    }
}