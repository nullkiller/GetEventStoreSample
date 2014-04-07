using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Messages.CompetenceTree
{
    public class CompetenceCreated: DomainEvent
    {
        public Guid AggregateId { get; set; }

        public string Name { get; set; }

        public CompetenceCreated()
        {
        }

        public CompetenceCreated(string name, IIdentityGenerator identityGenerator)
        {
            this.Name = name;
            this.AggregateId = identityGenerator.NewId();
        }
    }
}
