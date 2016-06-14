using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using System.IdentityModel.Tokens;

namespace TodoListService
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            /*app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Audience = ConfigurationManager.AppSettings["ida:Audience"],
                    Tenant = ConfigurationManager.AppSettings["ida:Tenant"],
                    
                });
                */
            //http://www.cloudidentity.com/blog/2013/10/25/securing-a-web-api-with-adfs-on-ws2012-r2-got-even-easier/

            String validAudience = ConfigurationManager.AppSettings["ida:Audience"];
            String adfsDiscoveryDoc = ConfigurationManager.AppSettings["ida:ADFSDiscoveryDoc"];

            app.UseActiveDirectoryFederationServicesBearerAuthentication(
                new ActiveDirectoryFederationServicesBearerAuthenticationOptions
                {
                    // [jelled] Why is Audience not being checked via this mechanism?
                    //Audience = ConfigurationManager.AppSettings["ida:Audience"],                  
                    MetadataEndpoint = adfsDiscoveryDoc,
                    TokenValidationParameters = new TokenValidationParameters() { ValidAudience = validAudience },
                });
        }
    }
}
