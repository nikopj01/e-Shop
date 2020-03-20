using eShop.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.DataAccess.InMemory
{
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        public InMemoryRepository()
        {

        }

        public IQueryable<T> Collection()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Delete(string Id)
        {
            throw new NotImplementedException();
        }

        public T Find(Guid Id)
        {
            throw new NotImplementedException();
        }

        public void Insert(T t)
        {
            throw new NotImplementedException();
        }

        public void Update(T t)
        {
            throw new NotImplementedException();
        }
    }
}
