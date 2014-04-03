using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Events
{
    public interface IServiceBus
    {
        void Send(DomainEvent @event);
    }
}
