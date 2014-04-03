using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventStore.Domain.Core
{
    public interface IRouteEvents
    {
        void Register<T>(Action<T> handler);
        void Register(IAggregate aggregate);

        void Dispatch(DomainEvent eventMessage);
    }
}
