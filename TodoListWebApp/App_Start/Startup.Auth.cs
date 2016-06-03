//----------------------------------------------------------------------------------------------
//    Copyright 2014 Microsoft Corporation
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// The following using statements were added for this sample.
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Configuration;
using System.Globalization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using TodoListWebApp.Utils;
using System.Security.Claims;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols;

namespace TodoListWebApp
{
    public partial class Startup
    {
        //
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        // The App Key is a credential used to authenticate the application to Azure AD.  Azure AD supports password and certificate credentials.
        // The Metadata Address is used by the application to retrieve the signing keys used by Azure AD.
        // The AAD Instance is the instance of Azure, for example public Azure or Azure China.
        // The Authority is the sign-in URL of the tenant.
        // The Post Logout Redirect Uri is the URL where the user will be redirected after they sign out.
        //

        //Application
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:AppKey"];                
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        
        //API "Resource"
        string todoListResourceid = ConfigurationManager.AppSettings["ida:TodoListResourceid"];

        //AD FS
        private static string metadataAddress = ConfigurationManager.AppSettings["ida:ADFSDiscoveryDoc"];
        private static string ADFSService = ConfigurationManager.AppSettings["ida:ADFSService"];
        public static readonly string Authority = String.Format(CultureInfo.InvariantCulture, ADFSService);

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    MetadataAddress = metadataAddress,                    
                    PostLogoutRedirectUri = redirectUri,
                    RedirectUri = redirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        //
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        //
                        AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                        AuthenticationFailed = OnAuthenticationFailed
                    }

                });
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/Home/Error?message=" + context.Exception.Message);
            return Task.FromResult(0);
        }

        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification context)
        {
            var code = context.Code;

            ClientCredential credential = new ClientCredential(clientId, appKey);
            
            //Azure AD works with a guid as identifier
            //string userObjectID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            //For AD FS we can use UPN for instance
            string userObjectID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value;

            //{"Authority validation is not supported for this type of authority\r\nParameter name: validateAuthority"}
            //http://www.cloudidentity.com/blog/2013/09/12/active-directory-authentication-library-adal-v1-for-net-general-availability/
            //Azure AD does support Authority Validation, AD FS not (At least not in TP5)
            AuthenticationContext authContext = new AuthenticationContext(Authority, false, new NaiveSessionCache(userObjectID));      
            
            // If you create the redirectUri this way, it will contain a trailing slash.  
            // Make sure you've registered the same exact Uri in the Azure Portal (including the slash).
            Uri uri = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path));

            AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(code, uri, credential, todoListResourceid);
        }
    }
}