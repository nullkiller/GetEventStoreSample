using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Commands.User
{
    public class CreateNewUserCommand: Command
    {
        public CreateNewUserCommand(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public Guid AggregateId { get; set; }

        public int Version { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }
    }
}
