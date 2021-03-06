﻿using EventStore.Domain;
using EventStore.Infrastructure;
using EventStore.Infrastructure.Ninject;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventStore.Web.Ioc
{
    public class Bootstrapper
    {
        public StandardKernel Kernel { get; private set; }

        public static readonly Bootstrapper Instance = new Bootstrapper();

        private Bootstrapper()
        {
            CreateKernel();
        }

        private void CreateKernel()
        {
            Kernel = new StandardKernel(new InfrastructureModule(), new ServiceBusModule(), new CommandModule());
        }

        public T Get<T>()
        {
            return Kernel.Get<T>();
        }
    }
}