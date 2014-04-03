using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Events
{
    public class InProcessServiceBus: IServiceBus
    {
        [Inject]
        public IKernel Kernel { get; set; }

        public InProcessServiceBus()
        {
        }

        public void Send(Domain.Core.DomainEvent @event)
        {
            SendInternal<DomainEvent>(@event);
            Send(@event.GetType(), @event);
        }

        private void Send(Type type, DomainEvent @event)
        {
            var genericMethod = this.GetType().GetMethod("SendInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var method = genericMethod.MakeGenericMethod(type);
            method.Invoke(this, new object[]{ @event });
        }

        private void SendInternal<T>(T @event)
            where T: DomainEvent
        {
            var handlers = Kernel.GetAll<IEventHandler<T>>();
            foreach (var handler in handlers)
            {
                handler.Handle(@event);
            }
        }
    }
}
