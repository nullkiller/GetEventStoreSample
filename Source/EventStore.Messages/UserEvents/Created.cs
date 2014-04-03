using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Messages.UserEvents
{
    public class Created: DomainEvent
    {
        public Created(string login, string password)
        {
            Login = login;
            Password = password;

            AggregateId = Guid.NewGuid();
        }
        
        public string Password { get; set; }

        public string Login { get; set; }

        public Guid AggregateId { get; set; }

        public int Version { get; set; }
    }
}
