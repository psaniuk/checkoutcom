using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using checkoutcom.paymentgateway.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using checkoutcom.paymentgateway.Contracts;
using checkoutcom.paymentgateway.Services;

namespace checkoutcom.paymentgateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PaymentGatewayDbContext>(options => options.UseInMemoryDatabase("PaymentGatewayDb"));
            services.AddControllers();
            services.AddScoped<IBankApiHttpClient, BankApiHttpClientMock>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IPaymentDetailsRepository, PaymentDetailsRepository>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IIdempotencyKeyRepository, IdempotencyKeyRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureRequestLocalization(app);
        }

        private void ConfigureRequestLocalization(IApplicationBuilder app)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("de-DE"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
        }
    }
}
