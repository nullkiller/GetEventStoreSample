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

        public IAggregate GetOrAdd(Guid aggregateId, Func<IAggregate> aggregateFactory)
        {
            IAggregate existingAggregate;

            if (!_all.TryGetValue(aggregateId, out existingAggregate))
            {
                existingAggregate = aggregateFactory();
                _all.Add(aggregateId, existingAggregate);
            }

            return existingAggregate;
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
