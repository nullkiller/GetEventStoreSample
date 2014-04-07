using EventStore.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Messages.UserEvents
{
    public class Created: DomainEvent
    {
        public Created()
        {
        }

        public Created(string login, string password, IIdentityGenerator identityGenerator)
        {
            Login = login;
            Password = password;

            AggregateId = identityGenerator.NewId();
        }
        
        public string Password { get; set; }

        public string Login { get; set; }

        public Guid AggregateId { get; set; }
    }
}
