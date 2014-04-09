using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Misc
{
    public interface IFormsAuthentication
    {
        void SetAuthentication(string user, bool isPersistent);

        void SignOut();
    }
}
