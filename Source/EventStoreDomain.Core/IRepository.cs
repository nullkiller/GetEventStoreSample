using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain.Core
{
    public interface IRepository
    {
        IQueryable<T> GetAll<T>() where T : class, IAggregate;

        T GetById<T>(Guid id) where T : class, IAggregate;

        void Save(IAggregate aggregate, Guid commitId);
    }
}
