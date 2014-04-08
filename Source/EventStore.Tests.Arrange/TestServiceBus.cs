using EventStore.Domain.Core;
using EventStore.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Tests.Arrange
{
    public class TestServiceBus: List<DomainEvent>, IServiceBus
    {
        public void Send(Domain.Core.DomainEvent @event)
        {
            Add(@event);
        }
    }
}
