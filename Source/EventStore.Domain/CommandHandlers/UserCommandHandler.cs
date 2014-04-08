using EventStore.Commands.User;
using EventStore.Domain.Core;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain.CommandHandlers
{
    public class UserCommandHandler: CommandHandler<CreateNewUserCommand>
    {
        private IRepository _repository;
        private IIdentityGenerator _identityGenerator;

        public UserCommandHandler(IRepository repository, IIdentityGenerator identityGenerator)
        {
            _repository = repository;
            _identityGenerator = identityGenerator;
        }

        public void Execute(CreateNewUserCommand command)
        {
            var user = User.CreateUser(command.Login, command.Password, _identityGenerator);
            _repository.Save(user, Guid.NewGuid());
        }
    }
}
