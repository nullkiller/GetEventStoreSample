using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Messages.Employee
{
    public class CompetenciesChanged: DomainEvent
    {
        public CompetenciesChanged(Guid employeeId, IEnumerable<CompetenceDocumentInfo> competenceInfo)
        {
            this.AggregateId = employeeId;
            this.Competencies = competenceInfo;
        }

        public Guid AggregateId { get; set; }

        public IEnumerable<CompetenceDocumentInfo> Competencies { get; set; }
    }

    public class CompetenceDocumentInfo
    {
        public Guid CompetenceId { get; set; }
    }
}
