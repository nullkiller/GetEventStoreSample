using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Misc
{
    public class IdentityGenerator: IIdentityGenerator
    {
        public Guid NewId()
        {
            return Guid.NewGuid();
        }
    }
}
