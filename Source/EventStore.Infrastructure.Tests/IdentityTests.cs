using EventStore.Infrastructure.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace EventStore.Infrastructure.Tests
{
    public class IdentityTests
    {
        [Fact]
        public void identity_should_be_unique()
        {
            var generator = new IdentityGenerator();
            HashSet<Guid> ids = new HashSet<Guid>();
            for (var i = 0; i < 100; i++)
            {
                var id = generator.NewId();
                id.Should().NotBe(Guid.Empty);
                ids.Contains(id).Should().BeFalse();
                ids.Add(id);
            }
        }
    }
}
