﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.ReadModel.Users
{
    public interface IUsersView
    {
        Guid? GetUserId(string login);
    }
}
