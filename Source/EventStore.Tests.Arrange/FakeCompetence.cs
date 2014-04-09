using EventStore.Domain;
using EventStore.Infrastructure.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Tests.Arrange
{
    public class FakeCompetence
    {
        public const string TestName = "Test";

        public static Competence ArrangeCompetence()
        {
            return Competence.Create(TestName, new IdentityGenerator());
        }
    }
}
