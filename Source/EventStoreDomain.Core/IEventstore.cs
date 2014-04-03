using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain.Core
{
    public interface IEventStore
    {
        void FetchAllEvents();

        void SaveEvents(IAggregate aggregate, IEnumerable<DomainEvent> newEvents, Guid commitId);
    }
}
