using EventStore.Domain.Core;
using EventStore.Infrastructure.DataAccess;
using EventStore.Infrastructure.Ninject;
using EventStore.Infrastructure.Store;
using EventStore.Web.Ioc;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EventStore.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : Ninject.Web.Common.NinjectHttpApplication
    {
        public IEventStore Store { get; set; }

        public IRepositoryCache DataCache { get; set; }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.DependencyResolver = Bootstrapper.Instance.DependencyResolver;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            Store.FetchAllEvents();
            var elapsed = sw.Elapsed;
            sw.Stop();
        }

        protected override global::Ninject.IKernel CreateKernel()
        {
            Store = Bootstrapper.Instance.Get<IEventStore>();
            DataCache = Bootstrapper.Instance.Get<IRepositoryCache>();

            return Bootstrapper.Instance.Kernel;
        }
    }
}