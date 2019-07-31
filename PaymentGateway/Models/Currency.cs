namespace checkoutcom.paymentgateway.Models
{
    public class Currency
    {
        private const string Unknown = "Unknown";
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public static Currency Empty = new Currency { Name = Unknown, Symbol = Unknown };      
    }
}