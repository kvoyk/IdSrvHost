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


//using System.Runtime.Remoting.Contexts;
//using IdentityServer.Host.AspId;
//using Microsoft.AspNet.Identity;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Host.AspId;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityServer.AspNetIdentity;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;


namespace IdentityServer.Host.Config
{
    public static class UserServiceExtensions
    {
        public static void ConfigureUserService(this IdentityServerServiceFactory factory, string connString)
        {
            factory.UserService = new Registration<IUserService, UserService>();
            factory.Register(new Registration<UserManager>());
            factory.Register(new Registration<UserStore>());
            factory.Register(new Registration<Context>(resolver => new Context(connString)));
        }
    }

    public class UserService : AspNetIdentityUserService<User, string>
    {
        public static UserService _service = null;
        public static UserManager _userManager = null;

        public UserService(UserManager userMgr) : base(userMgr)
        {
            _service = this;
        }

        protected override async Task<AuthenticateResult> ProcessExistingExternalAccountAsync(string userID,
            string provider, string providerId, IEnumerable<Claim> claims)
        {
            await UpdateAccountFromExternalClaimsAsync(userID, provider, providerId, claims);
            return await SignInFromExternalProviderAsync(userID, provider);
        }

        protected override async Task<AuthenticateResult> UpdateAccountFromExternalClaimsAsync(string userID,
            string provider, string providerId, IEnumerable<Claim> claims)
        {
            List<Claim> claimsList = claims as List<Claim> ?? claims.ToList();
            if (string.Equals("Cosign", provider, StringComparison.InvariantCultureIgnoreCase))
            {
                string userId = "";
                const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";

                foreach (Claim claim in claimsList.Where(claim => claim.Type.Equals("UserId")))
                {
                    userId = claim.Value;
                    break;
                }

                //Add any custom claims to existing claims from Cosign
                //claimsList.AddRange(roles.Select(role => new Claim("role", role, XmlSchemaString, provider)));
            }


            var existingClaims = await userManager.GetClaimsAsync(userID);
            var intersection = existingClaims.Intersect(claimsList, new ClaimComparer());
            var newClaims = claimsList.Except(intersection, new ClaimComparer());
            var oldClaims = existingClaims.Except(intersection, new ClaimComparer());

            foreach (var claim in oldClaims)
            {

                //if (claim.Type.Equals("role", StringComparison.InvariantCultureIgnoreCase) && !(claim.Value.StartsWith("AD-") | claim.Value.StartsWith("MC-")))
                //    continue;

                var result = await userManager.RemoveClaimAsync(userID, claim);
                if (!result.Succeeded)
                    return new AuthenticateResult(result.Errors.First());
            }


            foreach (var claim in newClaims)
            {
                var result = await userManager.AddClaimAsync(userID, claim);
                if (!result.Succeeded)
                    return new AuthenticateResult(result.Errors.First());
            }

            return null;
        }

        public static UserService GetUserService()
        {
            return _service;
        }

        public static UserManager GetUserManager()
        {
            return _userManager;
        }
    }

    
}