using EventStore.Domain.Core;
using EventStore.Messages.UserEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain
{
    public class User: AggregateBase, IRouteEvent<Created>
    {
        private User(string login, string password, IIdentityGenerator identityGenerator)
        {
            var @event = new Created(login, password, identityGenerator);
            RaiseEvent(@event);
        }

        public User()
        {
        }

        public string Login { get; set; }

        public string Password { get; set; }

        #region Domain Methods

        public static User CreateUser(string login, string password, IIdentityGenerator identityGenerator)
        {
            return new User(login, password, identityGenerator);
        }

        public bool CheckPassword(string password)
        {
            return this.Password == password;
        }

        #endregion

        #region Event Listeners

        void IRouteEvent<Created>.Apply(Created @event)
        {
            this.Id = @event.AggregateId;
            this.Login = @event.Login;
            this.Password = @event.Password;
        }

        #endregion
    }
}
