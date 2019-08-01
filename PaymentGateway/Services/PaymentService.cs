using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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
        private readonly ICurrencyRepository _currencyRepository;

        public PaymentService(IBankApiHttpClient bankApiHttpClient, IPaymentDetailsRepository paymentDetailsRepository, ICurrencyRepository currencyRepository)
        {
            _bankApiClient = bankApiHttpClient;
            _paymentDetailsRepository = paymentDetailsRepository;
            _currencyRepository = currencyRepository;
        }

        public async Task<Guid> ProcessPaymentAsync(PaymentDetails paymentDetails)
        {
            if (paymentDetails == null)
                throw new ArgumentNullException(nameof(paymentDetails));

            try
            {
                Payment payment = await ConvertToPaymentIfValid(paymentDetails);
                BankTransaction transaction = await _bankApiClient.SubmitPaymentAsync(paymentDetails);
                if (transaction == null || transaction.Status != TransactionStatus.Success)
                    throw new ProcessPaymentException("Failed to submit a payment to a bank");
            
                payment.TransactionId = transaction.Id;
                await _paymentDetailsRepository.AddAsync(payment);
                
                return payment.Id;
            }
            catch (Exception exp) when (!(exp is PaymentValidationException))
            {
                throw new ProcessPaymentException("An error has occured while processing a payment", exp);
            }
        }

        private async Task<Payment> ConvertToPaymentIfValid(PaymentDetails payment)
        {
            if (!IsCardNumberValid(payment.CardNumber))
                throw new PaymentValidationException("Card number is not valid");
            
            if (!decimal.TryParse(payment.Amount, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out decimal amount))
                throw new PaymentValidationException("Amount is not valid");

            if (string.IsNullOrWhiteSpace(payment.Currency))
                throw new PaymentValidationException("Currency is not valid");
            
            var currency = await _currencyRepository.FindBy(payment.Currency);
            if (currency == null)
                throw new PaymentValidationException("Currency is not valid");

            var expireAt = ParseExpireAt(payment.ExpireAt);
            if (expireAt < DateTime.UtcNow)
                throw new PaymentValidationException("ExpireAt is not valid");
            
            if (!IsCvvNumberValid(payment.CVV))
                throw new PaymentValidationException("CVV is not valid");

            var cardNumber = new CardNumber() { Value = payment.CardNumber };

            return new Payment() 
            {
                CardNumber = cardNumber,
                Amount = amount,
                Currency = currency,
                ExpireAt = expireAt,
                CVV = payment.CVV,
            };
        }
        
        private bool IsCardNumberValid(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            if (!_cardNumberRegex.IsMatch(cardNumber))
                return false;

            return true;
        }

        private DateTime ParseExpireAt(string expireAt)
        {
            if (string.IsNullOrWhiteSpace(expireAt))
                return DateTime.MinValue;

            if (!_expireAtRegex.IsMatch(expireAt))
                return DateTime.MinValue;

            var dateParts = expireAt.Split('/'); 
            var month = int.Parse(dateParts[0]);     
            var year = DateTime.UtcNow.Year - DateTime.UtcNow.Year % 100 + int.Parse(dateParts[1]);   

            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); 
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);  

            return cardExpiry;
        }

        private bool IsCvvNumberValid(string cvv)
        {
            if (string.IsNullOrWhiteSpace(cvv) || !_cvvRegex.IsMatch(cvv))
                return false;
            
            return true;
        }
    }
}