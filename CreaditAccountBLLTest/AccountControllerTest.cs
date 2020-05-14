using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreditAccount.Controllers;
using CreditAccountDAL;
using CurrencyCodesResolver;
using CreditAccountBLL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Logging;
using CreditAccountBLL;

namespace CreaditAccountBLLTest
{
    [TestClass]
    public class AccountControllerTest
    {
        private IDbManager _dbManager;
        private CurrencyCodesResolver<ECurrencyCodeISO4127> _currencyCodesResolver;
        private ICurrencyConverterService _currencyConverterService;
        public AccountControllerTest()
        {
            //TODO FHolod: later use dependency injection container
            string connectionString = "Data Source=DESKTOP-UUK4JM4\\SQLEXPRESS;Initial Catalog=CreditAccount;Integrated Security=True";
            CurrencyConverterConfiguration converterConfiguration = new CurrencyConverterConfiguration() { ApiAddress = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml", ComparableCurrencyCode = "EUR", CacheExpirationTimeoutSec = 600, UpdateCacheOnErrorIntervalSec = 30 };

            _dbManager = new EFDbManager(new DbContextOptionsBuilder<AccountContext>().UseSqlServer(connectionString).Options);
            _currencyCodesResolver = new CurrencyCodesResolver<ECurrencyCodeISO4127>();
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
            ILogger<CurrencyConverterService> logger = loggerFactory.CreateLogger<CurrencyConverterService>();
            _currencyConverterService = new CurrencyConverterService(converterConfiguration, _currencyCodesResolver, logger);
        }

        [TestMethod]
        public void d()
        {

        }
    }
}
