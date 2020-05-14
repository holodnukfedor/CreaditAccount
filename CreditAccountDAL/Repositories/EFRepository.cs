using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CreditAccountDAL
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        public DbSet<T> _dbSet;

        public EFRepository(DbSet<T> dbSet)
        {
            _dbSet = dbSet;
        }

        public void Create(T item)
        {
            _dbSet.Add(item);
        }

        public ValueTask<T> Get(long key)
        {
            return _dbSet.FindAsync(key);
        }

        public void Update(T item)
        {
            _dbSet.Update(item);
        }
    }
}
