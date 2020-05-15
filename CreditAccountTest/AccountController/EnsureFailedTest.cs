using Xunit;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using CreditAccount;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CreditAccountTest.AccountController
{
    public class EnsureFailedTest
    {
        private const string NotExistsCurrency = "ZZZ";
        private TestServer _server;
        private AccountDbFixture _accountDbFixture;
        private ActionsCaller _methodCaller;
        private long TestUserId => _accountDbFixture.TestUserId;
        private long NotExistedUserId => _accountDbFixture.NotExistUserId;

        public EnsureFailedTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            _accountDbFixture = new AccountDbFixture();
            _accountDbFixture.Initialize(configuration);

            IWebHostBuilder builder = new WebHostBuilder()
                                           .UseConfiguration(configuration)
                                           .UseStartup<Startup>();
            _server = new TestServer(builder);
            _methodCaller = new ActionsCaller(_server.CreateClient());
        }

        [Fact]
        public async Task GetBalanceUserNotExists()
        {
            HttpResponseMessage response = await _methodCaller.GetBalanceAsync(NotExistedUserId);
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutMoneyUserNotExists()
        {
            HttpResponseMessage response = await _methodCaller.PutMoney(NotExistedUserId, 10, "RUB");
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutMoneyCurrencyNotExists()
        {
            HttpResponseMessage response = await _methodCaller.PutMoney(TestUserId, 10, NotExistsCurrency);
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task WithdrawMoneyUserNotExists()
        {
            HttpResponseMessage response = await _methodCaller.WithDrawMoney(TestUserId, NotExistedUserId, "RUB");
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task WithdrawMoneyCurrencyNotExists()
        {
            HttpResponseMessage response = await _methodCaller.WithDrawMoney(TestUserId, 10, NotExistsCurrency);
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task WithdrawMoneyNoCurrencyAccount()
        {
            HttpResponseMessage response = await _methodCaller.WithDrawMoney(TestUserId, 10, "RUB");
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task WithdrawMoneyLackOfMoney()
        {
            await _methodCaller.PutMoney(TestUserId, 100, "RUB");
            HttpResponseMessage response = await _methodCaller.WithDrawMoney(TestUserId, 200, "RUB");
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangeCurrencySourceUserNotExists()
        {
            HttpResponseMessage response = await _methodCaller.ChangeCurrency(NotExistedUserId, 10, "EUR", "RUB");
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangeCurrencySourceCurrencyNotExists()
        {
            HttpResponseMessage response = await _methodCaller.ChangeCurrency(TestUserId, 10, NotExistsCurrency, "RUB");
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangeCurrencyDestinationCurrencyNotExists()
        {
            await _methodCaller.PutMoney(TestUserId, 100, "RUB");
            HttpResponseMessage response = await _methodCaller.ChangeCurrency(TestUserId, 10, "RUB", NotExistsCurrency);
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangeCurrencySameCurrencies()
        {
            await _methodCaller.PutMoney(TestUserId, 100, "RUB");
            HttpResponseMessage response = await _methodCaller.ChangeCurrency(TestUserId, 10, "RUB", "RUB");
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangeCurrencyNoCurrencyAccount()
        {
            HttpResponseMessage response = await _methodCaller.ChangeCurrency(TestUserId, 10, "RUB", "RUB");
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangeCurrencyLackOfMoney()
        {
            await _methodCaller.PutMoney(TestUserId, 100, "RUB");
            HttpResponseMessage response = await _methodCaller.ChangeCurrency(TestUserId, 150, "RUB", "RUB");
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(responseString);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
