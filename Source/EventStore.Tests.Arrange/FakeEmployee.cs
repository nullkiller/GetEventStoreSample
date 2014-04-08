using EventStore.Domain;
using EventStore.Infrastructure.Misc;
using EventStore.Messages.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Tests.Arrange
{
    public class FakeEmployee
    {
        public static readonly string TestName = "Name";

        public static EmployeeCreated ArrangeCreated()
        {
            return new EmployeeCreated(TestName, new IdentityGenerator());
        }

        public static CompetenciesChanged ArrangeCompetenciesChanged(Guid employeeId, int amount = 3)
        {
            var competences = ArrangeCompetencies(amount).Select(i => new CompetenceDocumentInfo{ CompetenceId = i.CompetenceId});
            return new CompetenciesChanged(employeeId, competences);
        }

        public static IEnumerable<CompetenceDocument> ArrangeCompetencies(int amount = 3)
        {
            return Enumerable.Range(1, amount).Select(i => new CompetenceDocument { CompetenceId = Guid.NewGuid() }).ToList();
        }

        public static Employee ArrangeEmployee(int competenciesNumber = 0)
        {
            var employee = Employee.Create(TestName, new IdentityGenerator());

            if (competenciesNumber > 0)
            {
                var competences = ArrangeCompetencies(competenciesNumber);
                employee.AddCompetences(competences);
            }

            return employee;
        }
    }
}
