using System.ComponentModel.DataAnnotations;

namespace checkoutcom.paymentgateway.Models
{
    public class Currency
    {
        public Currency(): this (string.Empty, string.Empty)
        {

        }
        public Currency(string code, string symbol)
        {
            Code = code;
            Symbol = symbol;
        }

        [Key]
        public string Code { get; set; }
        public string Symbol { get; set; } 
    }
}