using EventStore.Domain;
using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventStore.Infrastructure.DataAccess
{
    public class AggregateFactory: IAggregateFactory
    {
        private static Dictionary<Type, Func<IAggregate>> factoryMethods;

        static AggregateFactory()
        {
            factoryMethods = new Dictionary<Type, Func<IAggregate>>
            {
                { typeof(EventStore.Messages.UserEvents.Created), () => new User() },
                { typeof(EventStore.Messages.Employee.EmployeeCreated), () => new Employee() },
                { typeof(EventStore.Messages.CompetenceTree.CompetenceCreated), () => new Competence() }
            };
        }

        public IAggregate CreateFrom(DomainEvent @event)
        {
            IAggregate aggregate = null;
            Func<IAggregate> factory;

            var isCreationEvent = factoryMethods.TryGetValue(@event.GetType(), out factory);

            if (isCreationEvent)
            {
                aggregate = factory();
                aggregate.ApplyEvent(@event);
            }

            return aggregate;
        }

        public bool IsCreationEvent(DomainEvent @event)
        {
            return factoryMethods.ContainsKey(@event.GetType());
        }
    }
}
