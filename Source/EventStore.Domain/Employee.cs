using EventStore.Domain.Core;
using EventStore.Domain.Core.CommonDomain.Core;
using EventStore.Messages.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain
{
    public class Employee: AggregateBase
    {
        public Employee()
        {
            Competencies = new List<CompetenceDocument>();
        }

        public string Name { get; set; }

        public ICollection<CompetenceDocument> Competencies { get; set; }

        public static Employee Create(string name, IIdentityGenerator identityGenerator)
        {
            var competenceCreated = new EmployeeCreated(name, identityGenerator);

            var competence = new Employee();
            competence.RaiseEvent(competenceCreated);

            return competence;
        }

        public void Apply(EmployeeCreated @event)
        {
            Id = @event.AggregateId;
            Name = @event.Name;
        }

        public void Apply(CompetenciesChanged @event)
        {
            Competencies = @event.Competencies.Select(i => new CompetenceDocument { CompetenceId = i.CompetenceId }).ToList();
        }

        public void AddCompetences(IEnumerable<CompetenceDocument> set)
        {
            var competenceInfo = Competencies.Concat(set).Select(i => new CompetenceDocumentInfo { CompetenceId = i.CompetenceId }).ToList();
            var @event = new CompetenciesChanged(this.Id, competenceInfo);

            this.RaiseEvent(@event);
        }
    }
}
