
namespace checkoutcom.paymentgateway.Models.DTO
{
    public class PaymentDetails
    {
        public string CardNumber { get; set; }

        public string Amount { get; set; }

        public string Currency { get; set; }

        public string ExpiryAt { get; set; } 

        public string CVV { get; set; }
    }
}
