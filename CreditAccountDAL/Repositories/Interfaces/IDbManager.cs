using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CreditAccountDAL
{
    public interface IDbManager : IDisposable
    {
        IAccountRepository AccountRepository  { get; }
        IDisposable OpenConnection();
        void CloseConnection();
    }
}
