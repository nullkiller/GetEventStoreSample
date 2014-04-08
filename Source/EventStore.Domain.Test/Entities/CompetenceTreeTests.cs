using EventStore.Domain.Core;
using EventStore.Messages.CompetenceTree;
using EventStore.Tests.Arrange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace EventStore.Domain.Tests
{
    public class CompetenceTreeTests
    {
        [Fact]
        public void competence_should_be_creatable()
        {
            var generator = new ConstantIdentityGenerator();
            var competence = Competence.Create(FakeCompetence.TestName, generator);
            var events = competence.As<IAggregate>().GetUncommittedEvents();

            var creationEvent = events.Single().As<CompetenceCreated>();

            creationEvent.Should().NotBeNull();
            creationEvent.Name.Should().Be(FakeCompetence.TestName);
            creationEvent.AggregateId.Should().Be(generator.Identity);
        }
    }
}
