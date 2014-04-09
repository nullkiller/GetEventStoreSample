using EventStore.Domain;
using EventStore.Domain.Core;
using EventStore.Messages.UserEvents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.ReadModel.Users
{
    public class UsersView: IUsersView, IEventHandler<Created>, IProjection
    {
        private const string UsersViewKey = "users-by-login";

        private ConcurrentDictionary<string, Guid> usersByLogin;

        public UsersView()
        {
            usersByLogin = new ConcurrentDictionary<string, Guid>();
        }

        public void Handle(Created @event)
        {
            usersByLogin.GetOrAdd(@event.Login, @event.AggregateId);
        }

        public Guid? GetUserId(string login)
        {
            Guid id;
            return usersByLogin.TryGetValue(login, out id) ? (Guid?)id : null;
        }

        ProjectionData IProjection.Data
        {
            get
            {
                var data = usersByLogin.Select(i => new UserDto { AggregateRootId = i.Value, Login = i.Key }).ToList();
                return new ProjectionData<IEnumerable<UserDto>> { Data = data, Name = UsersViewKey };
            }
            set
            {
                var data = (ProjectionData<IEnumerable<UserDto>>)value;
                usersByLogin = new ConcurrentDictionary<string, Guid>(data.Data.ToDictionary(i => i.Login, i => i.AggregateRootId));
            }
        }

        string IProjection.Name
        {
            get
            {
                return UsersViewKey;
            }
        }
    }
}
