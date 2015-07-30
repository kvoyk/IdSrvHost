using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Entity.Infrastructure.Annotations;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;
using Thinktecture.IdentityServer.Core.Services;
using IdentityServer.Host;
using IdentityServer.Host.Config;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Owin.Security.Providers.Cosign;
using Owin.Security.Providers.GitHub;
using Thinktecture.IdentityServer.Core.Models;
using AuthenticationOptions = Thinktecture.IdentityServer.Core.Configuration.AuthenticationOptions;


[assembly: OwinStartup("IdentityServerHostStartup", typeof (IdentityServer.Host.Startup))]

namespace IdentityServer.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
#if DEBUG
            const string serverName = "devservername";
#else
           const string serverName = "prodservername";
#endif

            LogProvider.SetCurrentLogProvider(new TraceSourceLogProvider());


            const string connectionName = "AspId";

            var factory = new IdentityServerServiceFactory();
            factory.ConfigureClientService(connectionName);
            factory.ConfigureScopeService(connectionName);
            factory.ConfigureUserService(connectionName);
            //factory.ConfigureClientStoreCache();
            //factory.ConfigureScopeStoreCache();
            //factory.ConfigureUserServiceCache();

            app.Map("/core1", coreApp =>
            {
                var options = new IdentityServerOptions
                {
                    IssuerUri = "https://" + serverName + "/identityserver",
                    SiteName = "Identity Server Name",
                    SigningCertificate = Cert.Load(serverName),
                    Factory = factory,
                    CorsPolicy = CorsPolicy.AllowAll,
                    RequireSsl = true,
                    EnableWelcomePage = true,
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnableLoginHint = true,
                        EnableSignOutPrompt = false,
                        EnableLocalLogin = false,
                        EnablePostSignOutAutoRedirect = true,
                        PostSignOutAutoRedirectDelay = 0,
                        RequireAuthenticatedUserForSignOutMessage = false,
                        RememberLastUsername = false,
                        SignInMessageThreshold = 3,
                        IdentityProviders = ConfigureAdditionalIdentityProviders1,
                        CookieOptions = new Thinktecture.IdentityServer.Core.Configuration.CookieOptions()
                        {
                            Prefix = "Core1",
                            SecureMode = CookieSecureMode.Always
                        }
                    },
                    LoggingOptions = new LoggingOptions
                    {
                        EnableHttpLogging = true,
                        EnableWebApiDiagnostics = true,
                        IncludeSensitiveDataInLogs = true,
                        WebApiDiagnosticsIsVerbose = false
                    },
                    EventsOptions = new EventsOptions
                    {
                        RaiseSuccessEvents = true,
                        RaiseErrorEvents = true,
                        RaiseFailureEvents = true,
                        RaiseInformationEvents = true
                    }
                };


                coreApp.UseIdentityServer(options);
            });

            app.Map("/core2", coreApp =>
            {
                var options = new IdentityServerOptions
                {
                    IssuerUri = "https://" + serverName + "/identityserver",
                    SiteName = "Identity Server Name",
                    SigningCertificate = Cert.Load(serverName),
                    Factory = factory,
                    CorsPolicy = CorsPolicy.AllowAll,
                    RequireSsl = true,
                    EnableWelcomePage = false,
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnableLoginHint = true,
                        EnableSignOutPrompt = false,
                        EnableLocalLogin = false,
                        EnablePostSignOutAutoRedirect = true,
                        PostSignOutAutoRedirectDelay = 0,
                        RequireAuthenticatedUserForSignOutMessage = false,
                        RememberLastUsername = false,
                        SignInMessageThreshold = 3,
                        IdentityProviders = ConfigureAdditionalIdentityProviders2,
                        CookieOptions = new Thinktecture.IdentityServer.Core.Configuration.CookieOptions()
                        {
                            Prefix = "Core2",
                            SecureMode = CookieSecureMode.Always
                        }
                    },
                    LoggingOptions = new LoggingOptions
                    {
                        EnableHttpLogging = true,
                        EnableWebApiDiagnostics = true,
                        IncludeSensitiveDataInLogs = true
                    },
                    EventsOptions = new EventsOptions
                    {
                        RaiseSuccessEvents = true,
                        RaiseErrorEvents = true,
                        RaiseFailureEvents = true,
                        RaiseInformationEvents = true
                    }
                };
                coreApp.UseIdentityServer(options);
            });
        }

        public static void ConfigureAdditionalIdentityProviders1(IAppBuilder app, string signInAsType)
        {
            var cosign1 = new CosignAuthenticationOptions
            {
                AuthenticationType = "Cosign",
                SignInAsAuthenticationType = signInAsType,
                CosignServer = "weblogin.umich.edu",
                CosignServicePort = 6663,
                IdentityServerHostInstance = "core1",
#if DEBUG
                ClientServer = "devservername"
#else
                ClientServer = "prodservername"
#endif
            };
            app.UseCosignAuthentication(cosign1);
        }

        public static void ConfigureAdditionalIdentityProviders2(IAppBuilder app, string signInAsType)
        {
            var gitHub = new GitHubAuthenticationOptions
            {
                AuthenticationType = "GitHub",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "XXXXXXXXX",
                ClientSecret = "XXXXXXXXX",
            };
            app.UseGitHubAuthentication(gitHub);

            var fb = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "XXXXXXXXX",
                AppSecret = "XXXXXXXXX",
                Scope = {"email"}
            };
            app.UseFacebookAuthentication(fb);

            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "XXXXXXXXX",
                ClientSecret = "XXXXXXXXX",
                Scope = {"email"}
            };
            app.UseGoogleAuthentication(google);

            var cosign2 = new CosignAuthenticationOptions
            {
                AuthenticationType = "Cosign",
                SignInAsAuthenticationType = signInAsType,
                CosignServer = "weblogin.umich.edu",
                CosignServicePort = 6663,
                IdentityServerHostInstance = "core2",
#if DEBUG
                ClientServer = "devservername"
#else
                ClientServer = "prodservernameu"
#endif
            };
            app.UseCosignAuthentication(cosign2);
        }
    }
}