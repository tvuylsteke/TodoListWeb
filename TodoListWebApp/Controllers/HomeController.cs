using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Security.Claims;
using TodoListWebApp.Models;
using TodoListWebApp.Utils;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;

namespace TodoListWebApp.Controllers
{
    public class HomeController : Controller
    {
        //API
        private static string todoListResourceId = ConfigurationManager.AppSettings["todo:TodoListResourceId"];
        private static string todoListBaseAddress = ConfigurationManager.AppSettings["todo:TodoListBaseAddress"];

        //Application
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:AppKey"];

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Claims()
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            List<ClaimInfo> idClaims = new List<ClaimInfo>();

            foreach (var claim in cp.Claims)
            {
                ClaimInfo ci = new ClaimInfo();
                ci.ClaimType = claim.Type;
                ci.Value = claim.Value;
                ci.ValueType = claim.ValueType.Substring(claim.ValueType.IndexOf('#') + 1);
                ci.SubjectName = (claim.Subject == null || string.IsNullOrEmpty(claim.Subject.Name)) ? "Null" : claim.Subject.Name;
                ci.IssuerName = !String.IsNullOrEmpty(claim.Issuer) ? claim.Issuer : "Null";

                idClaims.Add(ci);
            }

            ViewBag.IDclaims = idClaims;

            List<ClaimInfo> atClaims = new List<ClaimInfo>();

            List<TodoItem> itemList = new List<TodoItem>();

            if (ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn") != null)
            {
                try
                {
                    string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value;
                    AuthenticationContext authContext = new AuthenticationContext(Startup.Authority, false, new NaiveSessionCache(userObjectID));
                    ClientCredential credential = new ClientCredential(clientId, appKey);
                    AuthenticationResult result = await authContext.AcquireTokenSilentAsync(todoListResourceId, credential, new UserIdentifier(userObjectID, UserIdentifierType.RequiredDisplayableId));

                    //
                    // Retrieve the user's To Do List.
                    //
                    HttpClient client = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, todoListBaseAddress + "/api/claims");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    HttpResponseMessage response = await client.SendAsync(request);

                    //
                    // Return the To Do List in the view.
                    //
                    if (response.IsSuccessStatusCode)
                    {
                        List<Dictionary<String, String>> responseElements = new List<Dictionary<String, String>>();
                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        String responseString = await response.Content.ReadAsStringAsync();
                        // [jelled] Untested but something along these lines should be better:
                        atClaims = JsonConvert.DeserializeObject<List<ClaimInfo>>(responseString, settings);
                        //responseElements = JsonConvert.DeserializeObject<List<Dictionary<String, String>>>(responseString, settings);

                        //foreach (var claim in responseElements)
                        //{
                        //    ClaimInfo ci = new ClaimInfo();
                        //    ci.ClaimType = claim["ClaimType"];
                        //    ci.Value = claim["Value"];
                        //    ci.ValueType = claim["ValueType"].Substring(claim["ValueType"].IndexOf('#') + 1);
                        //    ci.SubjectName = ((claim["SubjectName"]) != null && (String.IsNullOrEmpty(claim["SubjectName"])) == false) ? claim["SubjectName"] : "Null";
                        //    ci.IssuerName = !String.IsNullOrEmpty(claim["IssuerName"]) ? claim["IssuerName"] : "Null";

                        //    atClaims.Add(ci);
                        //}

                    }
                    else
                    {
                        // [jelled] Show the error somehow.
                    }
                }
                catch (AdalSilentTokenAcquisitionException)
                {
                    // [jelled] In case the exception is because acquiring the token silently failed, then the cached token is probably invalid
                    // (and cannot be refreshed using the refresh token). Force the user to sign in again.
                }
                catch (AdalException)
                {
                    // [jelled] Completely ignoring exceptions is always a bad idea. It could be interesting to show the exception on the page somehow.
                }
            }
            ViewBag.ATclaims = atClaims;

            //
            // If the call failed for any other reason, show the user an error.
            //
            //return View("Error");

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "TodoListWeb purpose:";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contoso Contact Information.";

            return View();
        }

        public ActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View("Error");
        }
    }
}