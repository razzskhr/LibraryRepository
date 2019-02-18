using System;
using System.Threading.Tasks;
using Common.EncryptionRepository;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;


[assembly: OwinStartup(typeof(LibraryManagement.Startup))]

namespace LibraryManagement
{
    public class Startup
    {
        private IPasswordRepository passwordRepository;
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            OAuthAuthorizationServerOptions option = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new ApplicationOAuthProvider(passwordRepository),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
                AllowInsecureHttp = true
            };
            app.UseOAuthAuthorizationServer(option);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
