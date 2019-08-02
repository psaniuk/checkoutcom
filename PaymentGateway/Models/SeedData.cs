using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace checkoutcom.paymentgateway.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new PaymentGatewayDbContext(serviceProvider.GetRequiredService<DbContextOptions<PaymentGatewayDbContext>>()))
            {
                if (dbContext.Currencies.Any())
                    return;
                
                dbContext.Currencies.AddRange(new Currency("EUR", "€"), new Currency("USD", "$"));
                dbContext.SaveChanges();
            }
        }
    }
}
