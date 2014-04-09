using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Tests.Arrange
{
    public class TestRepository: IRepository
    {
        public IRepositoryCache Cache { get; set; }

        public TestRepository()
        {
            Cache = new RepositoryCache();
        }

        public IQueryable<T> GetAll<T>() where T : class, IAggregate
        {
            return Cache.GetAll().OfType<T>().AsQueryable();
        }

        public T GetById<T>(Guid id) where T : class, IAggregate
        {
            return (T)Cache.Get(id);
        }

        public void Save(IAggregate aggregate, Guid commitId)
        {
            Cache.Add(aggregate);
        }
    }
}
