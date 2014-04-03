using EventStore.Infrastructure.DataAccess;
using EventStore.Infrastructure.Ninject;
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
        [Inject]
        public IReadRepository ReadRepository { get; set; }

        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.DependencyResolver = Bootstrapper.Instance.DependencyResolver;

            ReadRepository.Load();
        }

        protected override global::Ninject.IKernel CreateKernel()
        {
            ReadRepository = Bootstrapper.Instance.Get<IReadRepository>();

            return Bootstrapper.Instance.Kernel;
        }
    }
}