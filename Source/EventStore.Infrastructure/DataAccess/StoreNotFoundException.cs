using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventStore.Infrastructure.DataAccess
{
    class StoreNotFoundException : Exception
    {
        public StoreNotFoundException(string message)
            :base("EventStream " + message + " not found in store")
        {
        }
    }
}
