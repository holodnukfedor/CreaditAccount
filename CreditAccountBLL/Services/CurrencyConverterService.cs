using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using CurrencyCodesResolver;
using Microsoft.Extensions.Logging;

namespace CreditAccountBLL
{
    public class CurrencyConverterService : ICurrencyConverterService
    {
        private string _apiAddress;
        private int _comparableCurrencyCode;
        private Dictionary<int, decimal> _rateDictionary;
        private TimeSpan _expirationTimeout;
        private TimeSpan _halfOfTheExpirationTimeout;
        private TimeSpan _updateCacheIntervalOnError;
        private DateTime _rateDictRebuildedTime;
        private ICurrencyCodesResolver _currencyCodesResolver;
        private ReaderWriterLockSlim _readerWriterLockSlim;
        private Thread _rebuildCacheThread;
        private bool _disposed;
        private bool _isWorking;
        ILogger<CurrencyConverterService> _logger;

        public CurrencyConverterService(CurrencyConverterConfiguration currencyConverterConfiguration, ICurrencyCodesResolver currencyCodesResolver, ILogger<CurrencyConverterService> logger)
        {
            if (currencyConverterConfiguration.UpdateCacheOnErrorIntervalSec > currencyConverterConfiguration.CacheExpirationTimeoutSec)
                throw new Exception($"UpdateCacheIntervalOnError has to be considerably lesser than ExpirationTimeout. UpdateCacheOnErrorIntervalSec: [{currencyConverterConfiguration.UpdateCacheOnErrorIntervalSec}], CacheExpirationTimeoutSec: [{currencyConverterConfiguration.CacheExpirationTimeoutSec}]");

            _apiAddress = currencyConverterConfiguration.ApiAddress;
            _comparableCurrencyCode = currencyCodesResolver.Resolve(currencyConverterConfiguration.ComparableCurrencyCode);
            _expirationTimeout = TimeSpan.FromSeconds(currencyConverterConfiguration.CacheExpirationTimeoutSec);
            _halfOfTheExpirationTimeout = TimeSpan.FromSeconds(currencyConverterConfiguration.CacheExpirationTimeoutSec / 2);
            _updateCacheIntervalOnError = TimeSpan.FromSeconds(currencyConverterConfiguration.UpdateCacheOnErrorIntervalSec);
            _currencyCodesResolver = currencyCodesResolver;
            _readerWriterLockSlim = new ReaderWriterLockSlim();
            _isWorking = true;
            _rebuildCacheThread = new Thread(RebuildCacheHandler);
            _rebuildCacheThread.Start();
            _logger = logger;
        }

        ~CurrencyConverterService()
        {
            if (!_disposed && _readerWriterLockSlim != null)
                _readerWriterLockSlim.Dispose();
        }

        private async void RebuildCacheHandler()
        {
            do
            {
                await RebuildCache();

                _readerWriterLockSlim.EnterReadLock();
                bool isCurrencyRatesDataAvailable = false;
                try
                {
                    isCurrencyRatesDataAvailable = IsCurrencyRatesDataAvailable();
                }
                finally
                {
                    _readerWriterLockSlim.ExitReadLock();
                }

                if (isCurrencyRatesDataAvailable)
                    Thread.Sleep(_halfOfTheExpirationTimeout);
                else
                    Thread.Sleep(_updateCacheIntervalOnError);
            } while (_isWorking);
        }

        private bool IsCurrencyRatesDataAvailable()
        {
            return _rateDictionary == null || DateTime.Now - _rateDictRebuildedTime > _expirationTimeout;
        }

        private async Task RebuildCache()
        {
            Dictionary<int, decimal> result = await ReadDataFromUrl();
            if (result == null)
                return;

            _readerWriterLockSlim.EnterWriteLock();

            try
            {
                _rateDictionary = result;
                _rateDictRebuildedTime = DateTime.Now;
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }
        }

        private async Task<Dictionary<int, decimal>> ReadDataFromUrl()
        {
            try
            {
                const string currencyNodeName = "Cube";
                const string currencyAttributeName = "currency";
                const string rateAttributeName = "rate";

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(_apiAddress);
                response.EnsureSuccessStatusCode();
                Stream responseBody = await response.Content.ReadAsStreamAsync();
                CancellationTokenSource source = new CancellationTokenSource();
                XDocument doc = await XDocument.LoadAsync(responseBody, LoadOptions.PreserveWhitespace, source.Token);
                XNamespace defaultNamespace = doc.Root.GetDefaultNamespace();
                Dictionary<int, decimal> rateDictionary = doc.Descendants(defaultNamespace + currencyNodeName).
                    Where(x => x.Attribute(currencyAttributeName) != null).
                    ToDictionary(x => _currencyCodesResolver.Resolve((string)x.Attribute(currencyAttributeName)), y => (decimal)y.Attribute(rateAttributeName));
                return rateDictionary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured by getting currency rates");
                //TODO FHolod: send email to administrator
                return null;
            }
        }

        public Result<decimal> Convert(decimal amount, int fromCurrency, int toCurrency)
        {
            _readerWriterLockSlim.EnterReadLock();

            try
            {
                if (IsCurrencyRatesDataAvailable())
                    return Result<decimal>.CreateError($"Information about currency rate is unavailable now. Try to use our service later");

                if (_comparableCurrencyCode != fromCurrency)
                    amount /= _rateDictionary[fromCurrency];

                return Result<decimal>.CreateSuccess(amount * _rateDictionary[toCurrency]);
            }
            finally 
            {
                _readerWriterLockSlim.ExitReadLock();
            }
        }

        public void Dispose()
        {
            _isWorking = false;
            _rebuildCacheThread.Join();
            _readerWriterLockSlim.Dispose();
            _disposed = true; ;
        }
    }
}
