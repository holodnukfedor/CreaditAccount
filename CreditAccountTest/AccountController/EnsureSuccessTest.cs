using System;
using Xunit;
using System.Net.Http;      
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using CreditAccount;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit.Extensions.Ordering;
using System.Text;
using System.Linq;

[assembly: TestFramework("Xunit.Extensions.Ordering.TestFramework", "Xunit.Extensions.Ordering")]

namespace CreditAccountTest.AccountController
{
    [Order(1)]
    public class EnsureSuccessTest : IClassFixture<AccountDbFixture>, IDisposable
    {
        private TestServer _server;
        private AccountDbFixture _accountDbFixture;
        private ActionsCaller _methodCaller;
        private long TestUserId => _accountDbFixture.TestUserId;

        private async Task EmptyBalance()
        {
            BalanceVM[] balances = await _methodCaller.GetBalanceEnsureSuccessAsync(TestUserId);
            Assert.Empty(balances);
        }

        private async Task BalanceIs(decimal money, string currency)
        {
            BalanceVM[] balances = await _methodCaller.GetBalanceEnsureSuccessAsync(_accountDbFixture.TestUserId);
            BalanceVM balance = balances.First(x => String.Equals(x.CurrencyCode, currency));
            Assert.Equal(money, balance.Balance);
        }

        private async Task BalanceNear(decimal money, string currency)
        {
            BalanceVM[] balances = await _methodCaller.GetBalanceEnsureSuccessAsync(_accountDbFixture.TestUserId); ;
            BalanceVM balance = balances.First(x => String.Equals(x.CurrencyCode, currency));
            Assert.True((double)Math.Abs(balance.Balance - money) < 0.1);
        }

        public EnsureSuccessTest(AccountDbFixture accountDbFixture)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            _accountDbFixture = accountDbFixture;
            _accountDbFixture.Initialize(configuration);

            IWebHostBuilder builder = new WebHostBuilder()
                                           .UseConfiguration(configuration)
                                           .UseStartup<Startup>();
            _server = new TestServer(builder);
            _methodCaller = new ActionsCaller(_server.CreateClient());
        }

        //TODO FHolod: Это важно предполагалось сделать каждый внутренний вызов отдельным тестом но я не нашел быстро способ задания порядка TestCase
        //xunit.extension.ordering не работает
        [Fact]
        public async Task TestScenarion()
        {
            await EmptyBalance();
            await _methodCaller.PutMoneyEnsureSuccess(TestUserId, 200, "RUB");
            await BalanceIs(200, "RUB");
            await _methodCaller.PutMoneyEnsureSuccess(TestUserId, 20, "RUB");
            await BalanceIs(220, "RUB");
            await _methodCaller.WithDrawMoneyEnsureSuccess(TestUserId, 50, "RUB");
            await BalanceIs(170, "RUB");
            await _methodCaller.PutMoneyEnsureSuccess(TestUserId, 550, "JPY");
            await BalanceIs(550, "JPY");
            await _methodCaller.ChangeCurrencyEnsureSuccess(TestUserId, 160, "RUB", "EUR");
            await BalanceNear(2, "EUR");
            await _methodCaller.PutMoneyEnsureSuccess(TestUserId, 5000, "RUB");
            await _methodCaller.ChangeCurrencyEnsureSuccess(TestUserId, 4000, "RUB", "EUR");
            await BalanceNear(52, "EUR");
            await BalanceIs(1010, "RUB");
            await _methodCaller.ChangeCurrencyEnsureSuccess(TestUserId, 80, "RUB", "JPY");
            await BalanceNear(665.4M, "JPY");
        }

        public void Dispose()
        {
            
        }
    }
}
