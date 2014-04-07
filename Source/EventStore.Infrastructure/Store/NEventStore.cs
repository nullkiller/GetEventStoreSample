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
    public class NEventStore: IEventStore
    {
        private IStoreSettings<IStoreEvents> _settings;
        private IServiceBus _serviceBus;

        public NEventStore(IStoreSettings<IStoreEvents> settings, IServiceBus serviceBus)
        {
            _settings = settings;
            _serviceBus = serviceBus;
        }

        public void FetchAllEvents()
        {
            using (var store = _settings.GetConnection())
            {
                using (var stream = store.OpenStream("main"))
                {
                    foreach (var @event in stream.CommittedEvents)
                    {
                        var domainEvent = (DomainEvent)@event.Body;
                        _serviceBus.Send(domainEvent);
                    }
                }
            }
        }

        public void SaveEvents(IAggregate aggregate, IEnumerable<DomainEvent> newEvents, Guid commitId)
        {
            using (var store = _settings.GetConnection())
            {
                using (var stream = store.CreateStream("main"))
                {

                    var uncommitedEvents = aggregate.GetUncommittedEvents();
                    foreach (var @event in uncommitedEvents)
                    {
                        var eventMessage = new EventMessage
                        {
                            //Headers = new Dictionary<string, object> { { "Commit Id", commitId } },
                            Body = @event
                        };

                        stream.Add(eventMessage);
                    }

                    stream.CommitChanges(commitId);
                }
            }
        }
    }
}
