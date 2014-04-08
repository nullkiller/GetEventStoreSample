﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Domain.Core
{
    public interface IRouteEvent<T>
        where T: DomainEvent
    {
        void Apply(T @event);
    }
}
