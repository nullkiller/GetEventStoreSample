using EventStore.Infrastructure.Misc;
using EventStore.Tests.Arrange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using EventStore.Domain.Core;
using EventStore.Messages.Employee;
using Xunit;

namespace EventStore.Domain.Tests
{
    public class EmployeeTests
    {
        [Fact]
        public void employee_should_be_creatable()
        {
            var generator = new ConstantIdentityGenerator();
            var employee = Employee.Create(FakeEmployee.TestName, generator);
            var events = employee.As<IAggregate>().GetUncommittedEvents();

            var creationEvent = events.Single().As<EmployeeCreated>();
            
            creationEvent.Should().NotBeNull();
            creationEvent.Name.Should().Be(FakeEmployee.TestName);
            creationEvent.AggregateId.Should().Be(generator.Identity);
        }

        [Fact]
        public void employee_should_add_competencies()
        {
            var employee = FakeEmployee.ArrangeEmployee(0);
            var newCompetences = FakeEmployee.ArrangeCompetencies(3);

            employee.AddCompetences(newCompetences);

            employee.Competencies.Should().BeEquivalentTo(newCompetences);

            var events = employee.As<IAggregate>().GetUncommittedEvents();
            var changeEvnet = events.Last().As<CompetenciesChanged>();
            changeEvnet.Competencies.Select(i => i.CompetenceId).Should().BeEquivalentTo(newCompetences.Select(i => i.CompetenceId));
        }

        [Fact]
        public void employee_should_apply_to_change_competencies()
        {
            var employee = FakeEmployee.ArrangeEmployee(5);
            var newCompetences = FakeEmployee.ArrangeCompetenciesChanged(employee.Id, 3);

            employee.As<IRouteEvent<CompetenciesChanged>>().Apply(newCompetences);

            employee.Competencies.Select(i => i.CompetenceId).Should().BeEquivalentTo(newCompetences.Competencies.Select(i => i.CompetenceId));
        }
    }
}
