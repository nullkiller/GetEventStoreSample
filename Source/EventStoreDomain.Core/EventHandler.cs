using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain.Core
{
    public interface IEventHandler<T>
        where T: DomainEvent
    {
        void Handle(T @event);
    }
}
