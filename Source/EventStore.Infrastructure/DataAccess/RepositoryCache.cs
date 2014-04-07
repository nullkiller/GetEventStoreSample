using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.DataAccess
{
    public class RepositoryCache: IRepositoryCache
    {
        private Dictionary<Guid, IAggregate> _all;

        public RepositoryCache()
        {
            _all = new Dictionary<Guid, IAggregate>();
        }

        public void Add(IAggregate aggregate)
        {
            try
            {
                _all.Add(aggregate.Id, aggregate);
            }
            catch (ArgumentException)
            {
                throw new AggregateVersionException(aggregate.Id, aggregate.GetType(), aggregate.Version, 0);
            }
        }

        public void Reset()
        {
            _all.Clear();
        }

        public IAggregate Get(Guid id)
        {
            return _all[id];
        }

        public IEnumerable<IAggregate> GetAll()
        {
            return _all.Values;
        }
    }
}
