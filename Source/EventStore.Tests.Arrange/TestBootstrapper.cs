using EventStore.Infrastructure.Store;
using EventStore.Web.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEventStore;

namespace EventStore.Tests.Arrange
{
    public class TestBootstrapper
    {
        static TestBootstrapper()
        {
            Bootstrapper.Instance.Kernel.Rebind<IStoreSettings<IStoreEvents>>().ToConstant(FakeDatabase.ArrangeNSettings());
        }

        public static Bootstrapper Instance
        {
            get
            {
                return Bootstrapper.Instance;
            }
        }
    }
}
