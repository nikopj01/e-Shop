using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Core.Contracts
{
    public interface IRepository<T>
    {
        IQueryable<T> Collection();
        void Commit();
        void Delete(string Id);
        T Find(Guid Id);
        void Insert(T t);
        void Update(T t);
    }
}
