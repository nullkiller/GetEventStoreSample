using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace EventStore.Infrastructure.Misc
{
    public class FormsAuthenticationWrapper: IFormsAuthentication
    {
        public void SetAuthentication(string user, bool isPersistent)
        {
            FormsAuthentication.SetAuthCookie(user, isPersistent);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}
