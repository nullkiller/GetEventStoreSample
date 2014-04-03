using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain.Core
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Version { get; }

        void ApplyEvent(DomainEvent @event);
        ICollection<DomainEvent> GetUncommittedEvents();
        void ClearUncommittedEvents();
    }
}
