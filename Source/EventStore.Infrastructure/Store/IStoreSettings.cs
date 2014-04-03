using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventStore.Infrastructure.Store
{
    public interface IStoreSettings
    {
        IEventStoreConnection GetConnection();
    }
}
