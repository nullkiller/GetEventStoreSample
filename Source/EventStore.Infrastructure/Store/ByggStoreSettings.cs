using EventStore.Infrastructure.Events;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Store
{
    public class ByggStoreSettings : IStoreSettings<IDbConnection>
    {
        [Inject]
        public IServiceBus ServiceBus { get; set; }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString);
        }
    }
}
