using EventStore.Domain.Core;
using EventStore.Infrastructure.Events;
using NEventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Store
{
    public class NEventStore : IEventStore
    {
        private IStoreSettings<IStoreEvents> _settings;
        private IServiceBus _serviceBus;
        private IEventStream _eventStream;
        private IStoreEvents _store;

        public NEventStore(IStoreSettings<IStoreEvents> settings, IServiceBus serviceBus)
        {
            _settings = settings;
            _serviceBus = serviceBus;

            _store = _settings.GetConnection();
            _eventStream = _store.OpenStream("main");
        }

        public void FetchAllEvents()
        {
            foreach (var @event in _eventStream.CommittedEvents)
            {
                var domainEvent = (DomainEvent)@event.Body;
                _serviceBus.Send(domainEvent);
            }
        }

        public void SaveEvents(IEnumerable<DomainEvent> newEvents, Guid commitId)
        {
            lock (typeof(NEventStore))
            {
                foreach (var @event in newEvents)
                {
                    var eventMessage = new EventMessage
                    {
                        //Headers = new Dictionary<string, object> { { "Commit Id", commitId } },
                        Body = @event
                    };

                    _eventStream.Add(eventMessage);
                }

                _eventStream.CommitChanges(commitId);
            }

            foreach (var @event in newEvents)
            {
                _serviceBus.Send(@event);
            }
        }
    }
}
