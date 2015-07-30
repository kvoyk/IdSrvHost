using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.Host.Config
{
    public static class ClientServiceExtensions
    {
        public static void ConfigureClientService(this IdentityServerServiceFactory factory, string connString)
        {
            var clientStore = new clientMgr.Stores.ClientStore(connString);
            factory.ClientStore = new Registration<IClientStore>(clientStore);

        }

    }
}
