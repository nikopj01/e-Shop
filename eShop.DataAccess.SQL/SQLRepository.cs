using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.DataAccess.SQL
{
    public class SQLRepository<T> : IRepository<T> where T : class
    {
        internal DbModel db;
        internal DbSet<T> dbSet;

        public SQLRepository(DbModel dbmodel)
        {
            this.db = dbmodel;
            db.Configuration.ProxyCreationEnabled = false;
            this.dbSet = db.Set<T>();
        }

        public IQueryable<T> Collection()
        {
            return dbSet;
        }

        public void Commit()
        {
            db.SaveChanges();
        }

        public void Delete(string Id)
        {
            var t = dbSet.Find(Id);
            if(t != null)
            {
                dbSet.Remove(t);
            }

        }   

        public T Find(Guid Id)
        {
            return dbSet.Find(Id);
        }

        public void Insert(T t)
        {
            dbSet.Add(t);
        }

        public void Update(T t)
        {
            dbSet.Attach(t);
            db.Entry(t).State = System.Data.Entity.EntityState.Modified;
        }
    }
}
