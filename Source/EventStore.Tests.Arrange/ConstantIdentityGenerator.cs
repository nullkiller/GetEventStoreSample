using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Tests.Arrange
{
    public class ConstantIdentityGenerator: IIdentityGenerator
    {
        public Guid Identity { get; set; }

        public ConstantIdentityGenerator()
        {
            Identity = Guid.NewGuid();
        }

        public Guid NewId()
        {
            return Identity;
        }
    }
}
