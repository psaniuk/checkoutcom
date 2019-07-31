using System;

namespace checkoutcom.paymentgateway.Models.DTO
{
    public class BankTransaction
    {
        public Guid Id { get; set; } = Guid.Empty;
        public TransactionStatus Status { get; set; } = TransactionStatus.Unknown;
    }
}
