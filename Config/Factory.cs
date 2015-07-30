
/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.Core.Services.InMemory;

namespace IdentityServer.Host.Config
{
    class Factory
    {
        public static IdentityServerServiceFactory Configure(string connString)
        {
            var factory = new IdentityServerServiceFactory();



            //var scopeStore = new InMemoryScopeStore(Config.Scopes.Get());
            //factory.ScopeStore = new Registration<IScopeStore>(scopeStore);
            var scopeStore = new clientMgr.Stores.ScopeStore(connString);
            factory.ScopeStore = new Registration<IScopeStore>(scopeStore);



            //var clientStore = new InMemoryClientStore(Config.Clients.Get());
            //factory.ClientStore = new Registration<IClientStore>(clientStore);
            var clientStore = new clientMgr.Stores.ClientStore(connString);
            factory.ClientStore = new Registration<IClientStore>(clientStore);



            return factory;
        }
    }
}
