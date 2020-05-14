using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CreditAccountDAL
{
    public interface IDbManager : IDisposable
    {
        AccountContext AccountContext { get; }
        IRepository<User> Users { get; }
        IRepository<Account> Accounts { get; }
        Task<int> SaveChanges();
    }
}
