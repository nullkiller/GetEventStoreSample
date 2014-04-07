using EventStore.Domain.Core;
using EventStore.Domain.Core.CommonDomain.Core;
using EventStore.Messages.CompetenceTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain
{
    public class Competence: AggregateBase
    {
        public Competence()
        {
        }

        public string Name { get; set; }

        public static Competence Create(string name, IIdentityGenerator identityGenerator)
        {
            var competenceCreated = new CompetenceCreated(name, identityGenerator);

            var competence = new Competence();
            competence.RaiseEvent(competenceCreated);

            return competence;
        }

        public void Apply(CompetenceCreated @event)
        {
            Id = @event.AggregateId;
            Name = @event.Name;
        }
    }
}
