using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CreditAccountDAL
{
    public interface IRepository<T>
    {
        void Create(T item);
        void Update(T item);
        ValueTask<T> Get(long key);
    }
}
