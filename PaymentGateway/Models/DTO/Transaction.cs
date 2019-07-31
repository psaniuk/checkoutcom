using System;

namespace checkoutcom.paymentgateway.Models
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.Empty;
        public TransactionStatus Status { get; set; } = TransactionStatus.Unknown;
    }
}
