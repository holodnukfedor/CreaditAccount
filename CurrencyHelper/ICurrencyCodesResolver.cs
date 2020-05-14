using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyCodesResolver
{
    public interface ICurrencyCodesResolver
    {
        int Resolve(string code);
        string Resolve(int code);
        bool IsExists(string code);
        bool IsExists(int code);
    }
}
