using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Messages.Employee
{
    public class EmployeeCreated: DomainEvent
    {
        public Guid AggregateId { get; set; }

        public string Name { get; set; }

        public EmployeeCreated()
        {
        }

        public EmployeeCreated(string name, IIdentityGenerator identityGenerator)
        {
            this.Name = name;
            this.AggregateId = identityGenerator.NewId();
        }
    }
}
