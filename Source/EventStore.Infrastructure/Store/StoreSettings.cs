using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Store
{
    public class StoreSettings : IStoreSettings
    {
        public IEventStoreConnection GetConnection()
        {
            var ip = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 1113);
            return EventStoreConnection.Create(ip);
        }
    }
}
