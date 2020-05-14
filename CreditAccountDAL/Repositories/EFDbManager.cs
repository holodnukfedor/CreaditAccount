using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CreditAccountDAL
{
    public class EFDbManager : IDbManager
    {
        private bool _disposed = false;
        private AccountContext _accountContext;
        public IRepository<User> Users { get; }
        public IRepository<Account> Accounts { get; }
        public AccountContext AccountContext => _accountContext;

        public EFDbManager(DbContextOptions<AccountContext> options)
        {
            _accountContext = new AccountContext(options);
            Users = new EFRepository<User>(_accountContext.Users);
            Accounts = new EFRepository<Account>(_accountContext.Accounts);
        }

        public Task<int> SaveChanges()
        {
            return _accountContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _accountContext.Dispose();

            this._disposed = true;
        }
    }
}
