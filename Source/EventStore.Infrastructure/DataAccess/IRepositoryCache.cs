using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.DataAccess
{
    public interface IRepositoryCache
    {
        void Reset();
        IAggregate Get(Guid id);
        void Add(IAggregate aggregate);
        IEnumerable<IAggregate> GetAll();
    }
}
