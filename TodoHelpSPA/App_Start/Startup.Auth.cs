using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using System.Configuration;
using System.IdentityModel.Tokens;

namespace TodoSPA
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseActiveDirectoryFederationServicesBearerAuthentication(
                new ActiveDirectoryFederationServicesBearerAuthenticationOptions
                {
                    MetadataEndpoint = ConfigurationManager.AppSettings["ida:AdfsMetadataEndpoint"],
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudience = ConfigurationManager.AppSettings["ida:Audience"],
                        ValidIssuer = ConfigurationManager.AppSettings["ida:Issuer"]
                    }
                }
            );
            
        }

    }    
}
