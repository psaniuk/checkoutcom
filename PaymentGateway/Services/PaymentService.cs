using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Exceptons;
using checkoutcom.paymentgateway.Models;
using checkoutcom.paymentgateway.Models.DTO;

namespace checkoutcom.paymentgateway.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly Regex _cardNumberRegex = new Regex("^(\\d{4}[- ]){3}\\d{4}|\\d{16}$"); 
        private readonly Regex _expireAtRegex = new Regex(@"^\d{1,2}\/\d{2,4}$");
        private readonly Regex _cvvRegex = new Regex(@"^(?!000)\d{3,4}$");
        private readonly IBankApiHttpClient _bankApiClient;
        private readonly IPaymentDetailsRepository _paymentDetailsRepository;

        public PaymentService(IBankApiHttpClient bankApiHttpClient, IPaymentDetailsRepository paymentDetailsRepository)
        {
            _bankApiClient = bankApiHttpClient;
            _paymentDetailsRepository = paymentDetailsRepository;
        }

        public Task<Guid> ProcessPaymentAsync(PaymentDetails payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            Validate(payment);

            return Task.FromResult(Guid.NewGuid());
        }

        private void Validate(PaymentDetails payment)
        {
            if (!IsCardNumberValid(payment.CardNumber))
                throw new PaymentValidationException("Card number is not valid");
            
            if (!decimal.TryParse(payment.Amount, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out decimal result))
                throw new PaymentValidationException("Amount is not valid");

            if (string.IsNullOrWhiteSpace(payment.Currency))
                throw new PaymentValidationException("Currency is required");
            
            if (!IsExpireAtValid(payment.ExpireAt))
                throw new PaymentValidationException("ExpireAt is not valid");
            
            if (!IsCvvNumberValid(payment.CVV))
                throw new PaymentValidationException("CVV is not valid");
        }
        
        private bool IsCardNumberValid(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            if (!_cardNumberRegex.IsMatch(cardNumber))
                return false;

            return true;
        }

        private bool IsExpireAtValid(string expireAt)
        {
            if (string.IsNullOrWhiteSpace(expireAt))
                return false;

            if (!_expireAtRegex.IsMatch(expireAt))
                return false;

            var dateParts = expireAt.Split('/'); 
            var month = int.Parse(dateParts[0]);     
            var year = DateTime.Now.Year - DateTime.Now.Year % 100 + int.Parse(dateParts[1]);   

            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); 
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);  

            return cardExpiry > DateTime.Now;
        }

        private bool IsCvvNumberValid(string cvv)
        {
            if (string.IsNullOrWhiteSpace(cvv) || !_cvvRegex.IsMatch(cvv))
                return false;
            
            return true;
        }
    }
}