using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CreditAccountDAL;
using CurrencyCodesResolver;
using CreditAccountBLL;

namespace CreditAccount
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
            services.AddControllers();
            string connectionString =  Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AccountContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<IDbManager, EFDbManager>();
            CurrencyCodesResolver<ECurrencyCodeISO4127> currencyCodesResolver = new CurrencyCodesResolver<ECurrencyCodeISO4127>();
            services.AddSingleton<ICurrencyCodesResolver>(currencyCodesResolver);
            CurrencyConverterConfiguration currencyConverterConfiguration = new CurrencyConverterConfiguration();
            services.AddTransient<CurrencyConverterConfiguration>(x => currencyConverterConfiguration);
            Configuration.GetSection("CurrencyConverterConfiguration").Bind(currencyConverterConfiguration);
            services.AddSingleton<ICurrencyConverterService, CurrencyConverterService>();
            services.AddScoped<IAccountService, EFAccountService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/CreaditAccount-{Date}.txt");

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
        }
    }
}
