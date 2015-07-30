using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Host.Config
{
    public static class ScopeService
    {
        public static void ConfigureScopeService(this IdentityServerServiceFactory factory, string connString)
        {
            var scopeStore = new clientMgr.Stores.ScopeStore(connString);
            factory.ScopeStore = new Registration<IScopeStore>(scopeStore);
        }
    }
}
