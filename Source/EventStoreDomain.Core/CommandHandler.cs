using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain.CommandHandlers
{
    public interface CommandHandler<T>
    {
        void Execute(T command);
    }
}
