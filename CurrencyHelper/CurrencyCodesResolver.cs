using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyCodesResolver
{
    public class CurrencyCodesResolver<T> : ICurrencyCodesResolver where T: Enum
    {
        private readonly Dictionary<string, int> _stringEnumCurrencyDict;
        private readonly Dictionary<int, string> _intStringCurrencyDict;

        private (Dictionary<string, int>, Dictionary<int, string>) GetCurrencyDictionaries()
        {
            Dictionary<string, int> stringEnumCurrencyDict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            Dictionary<int, string> enumStringCurrencyDict = new Dictionary<int, string>();
            Type enumType = typeof(T);

            foreach (int code in Enum.GetValues(enumType).Cast<int>())
            {
                string codeString = Enum.GetName(enumType, code);
                stringEnumCurrencyDict.Add(codeString, code);
                enumStringCurrencyDict.Add(code, codeString);
            }

            return (stringEnumCurrencyDict, enumStringCurrencyDict);
        }

        public CurrencyCodesResolver()
        {
            (Dictionary<string, int>, Dictionary<int, string>) dicts = GetCurrencyDictionaries();
            _stringEnumCurrencyDict = dicts.Item1;
            _intStringCurrencyDict = dicts.Item2;
        }

        public int Resolve(string code)
        {
            if (String.IsNullOrEmpty(code))
                throw new Exception($"Currency code can't an empty string");

            int result;
            if (!_stringEnumCurrencyDict.TryGetValue(code, out result))
                throw new Exception($"There is no currency of enum {typeof(T)} with string code {code}");

            return result;
        }

        public string Resolve(T code)
        {
            string result;
            if (!_intStringCurrencyDict.TryGetValue((int)(object)code, out result))
                throw new Exception($"There is no currency of enum {typeof(T)} with enum code {code}");

            return result;
        }

        public string Resolve(int code)
        {
            string result;
            if (!_intStringCurrencyDict.TryGetValue(code, out result))
                throw new Exception($"There is no currency of enum {typeof(T)} with numeric code {code}");

            return result;
        }

        public bool IsExists(string code)
        {
            return _stringEnumCurrencyDict.ContainsKey(code);
        }

        public bool IsExists(T code)
        {
            return _intStringCurrencyDict.ContainsKey((int)(object)code);
        }

        public bool IsExists(int code)
        {
            return _intStringCurrencyDict.ContainsKey(code);
        }
    }
}
