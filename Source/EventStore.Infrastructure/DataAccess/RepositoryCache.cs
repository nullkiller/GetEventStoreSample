using EventStore.Domain.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.DataAccess
{
    public class RepositoryCache: IRepositoryCache, IProjection
    {
        private const string CacheKey = "entities";

        private ConcurrentDictionary<Guid, IAggregate> _all;

        public RepositoryCache()
        {
            _all = new ConcurrentDictionary<Guid, IAggregate>();
        }

        public void Add(IAggregate aggregate)
        {
            var result = _all.GetOrAdd(aggregate.Id, aggregate);
            
            if(!Object.ReferenceEquals(aggregate, result))
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
            IAggregate aggregate;

            if (!_all.TryGetValue(id, out aggregate))
            {
                aggregate = null;
            }

            return aggregate;
        }

        public IEnumerable<IAggregate> GetAll()
        {
            return _all.Values;
        }

        ProjectionData IProjection.Data
        {
            get
            {
                var data = _all.Values;
                return new ProjectionData<IEnumerable<IAggregate>> { Data = data, Name = CacheKey };
            }
            set
            {
                var data = (ProjectionData<IEnumerable<IAggregate>>)value;
                _all = new ConcurrentDictionary<Guid, IAggregate>(data.Data.ToDictionary(i => i.Id));
            }
        }

        string IProjection.Name
        {
            get
            {
                return CacheKey;
            }
        }
    }
}
