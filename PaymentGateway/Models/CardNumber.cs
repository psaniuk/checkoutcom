namespace checkoutcom.paymentgateway.Models
{
    public class CardNumber
    {
        public string Value { get; set; } = string.Empty;
        public static CardNumber Empty = new CardNumber { Value = "Unknown" };
    }
}