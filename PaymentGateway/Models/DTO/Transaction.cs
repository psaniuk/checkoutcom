using System;

namespace checkoutcom.paymentgateway.Models.DTO
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.Empty;
        public int Status { get; set; } = 0;
    }
}
