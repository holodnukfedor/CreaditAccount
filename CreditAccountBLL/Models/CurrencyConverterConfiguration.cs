
namespace CreditAccountBLL
{
    public class CurrencyConverterConfiguration
    {
        public string ApiAddress { get; set; }
        public string ComparableCurrencyCode { get; set; }
        public int CacheExpirationTimeoutSec { get; set; }
        public int UpdateCacheOnErrorIntervalSec { get; set; }
        public CurrencyConverterConfiguration()
        {

        }
    }
}
