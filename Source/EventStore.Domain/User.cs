using EventStore.Domain.Core.CommonDomain.Core;
using EventStore.Messages.UserEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain
{
    public class User: AggregateBase
    {
        private User(string login, string password)
        {
            var @event = new Created(login, password);
            RaiseEvent(@event);
        }

        public User()
        {
        }

        public string Login { get; set; }

        public string Password { get; set; }

        #region Event Listeners

        public void Apply(Created @event)
        {
            this.Id = @event.AggregateId;
            this.Login = @event.Login;
            this.Password = @event.Password;
        }

        #endregion

        #region Domain Methods

        public static User CreateUser(string login, string password)
        {
            return new User(login, password);
        }

        public bool Validate(string password)
        {
            return this.Password == password;
        }

        #endregion
    }
}
