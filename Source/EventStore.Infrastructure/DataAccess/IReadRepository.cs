using CommonDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.DataAccess
{
    public interface IReadRepository
    {
        IQueryable<T> GetAll<T>() where T : class, IAggregate;

        T GetById<T>(Guid id) where T : class, IAggregate;

        void Load();
    }
}
