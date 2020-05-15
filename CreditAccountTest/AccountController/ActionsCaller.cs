using System.Net.Http;
using System.Threading.Tasks;
using CreditAccount;
using Newtonsoft.Json;
using System.Text;
using Xunit;

namespace CreditAccountTest.AccountController
{
    public class ActionsCaller
    {
        private HttpClient _client;
        public ActionsCaller(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<HttpResponseMessage> GetBalanceAsync(long userId)
        {
            return await _client.GetAsync($"/account/GetBalance?userId={userId}");
        }

        public async Task<BalanceVM[]> GetBalanceEnsureSuccessAsync(long userId)
        {
            HttpResponseMessage response = await GetBalanceAsync(userId);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            BalanceVM[] balances = JsonConvert.DeserializeObject<BalanceVM[]>(responseString);
            return balances;
        }

        public async Task<HttpResponseMessage> PutMoney(long userId, decimal money, string currency)
        {
            ChangeBalanceVM changeBalanceVM = new ChangeBalanceVM() { Amount = money, CurrencyCode = currency, UserId = userId };
            string json = JsonConvert.SerializeObject(changeBalanceVM);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/account/PutMoney", data);
        }

        public async Task PutMoneyEnsureSuccess(long userId, decimal money, string currency)
        {
            HttpResponseMessage response = await PutMoney(userId, money, currency);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.Empty(responseString);
        }

        public async Task<HttpResponseMessage> ChangeCurrency(long userId, decimal money, string fromCurrency, string toCurrency)
        {
            ChangeCurrencyVM changeCurrencyVM = new ChangeCurrencyVM() { Amount = money, FromCurrencyCode = fromCurrency, ToCurrencyCode = toCurrency, UserId = userId };
            string json = JsonConvert.SerializeObject(changeCurrencyVM);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/account/ChangeCurrency", data);
        }

        public async Task ChangeCurrencyEnsureSuccess(long userId, decimal money, string fromCurrency, string toCurrency)
        {
            HttpResponseMessage response = await ChangeCurrency(userId, money, fromCurrency, toCurrency);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.Empty(responseString);
        }

        public async Task<HttpResponseMessage> WithDrawMoney(long userId, decimal money, string currency)
        {
            ChangeBalanceVM changeBalanceVM = new ChangeBalanceVM() { Amount = money, CurrencyCode = currency, UserId = userId };
            string json = JsonConvert.SerializeObject(changeBalanceVM);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PutAsync($"/account/WithDrawMoney", data);
        }

        public async Task WithDrawMoneyEnsureSuccess(long userId, decimal money, string currency)
        {
            HttpResponseMessage response = await WithDrawMoney(userId, money, currency);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            Assert.Empty(responseString);
        }
    }
}
