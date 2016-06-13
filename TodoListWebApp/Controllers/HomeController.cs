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
        private string todoListResourceId = ConfigurationManager.AppSettings["todo:TodoListResourceId"];
        private string todoListBaseAddress = ConfigurationManager.AppSettings["todo:TodoListBaseAddress"];

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
            List<ClaimInfo> results = new List<ClaimInfo>();

            foreach (var claim in cp.Claims)
            {
                ClaimInfo ci = new ClaimInfo();
                ci.ClaimType = claim.Type;
                ci.Value = claim.Value;
                ci.ValueType = claim.ValueType.Substring(claim.ValueType.IndexOf('#') + 1);
                ci.SubjectName = ((claim.Subject) != null && (String.IsNullOrEmpty(claim.Subject.Name)) == false) ? claim.Subject.Name : "Null";
                ci.IssuerName = !String.IsNullOrEmpty(claim.Issuer) ? claim.Issuer : "Null";

                results.Add(ci);
            }

            ViewBag.IDclaims = results;     


            AuthenticationResult result = null;
            List<TodoItem> itemList = new List<TodoItem>();

            try
            {
                if (ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn") == null) {
                       ViewBag.ATclaims = new List<ClaimInfo>();
                       return View();
                }

                string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn").Value;
                AuthenticationContext authContext = new AuthenticationContext(Startup.Authority, false, new NaiveSessionCache(userObjectID));
                ClientCredential credential = new ClientCredential(clientId, appKey);                
                result = await authContext.AcquireTokenSilentAsync(todoListResourceId, credential, new UserIdentifier(userObjectID, UserIdentifierType.RequiredDisplayableId));

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
                    responseElements = JsonConvert.DeserializeObject<List<Dictionary<String, String>>>(responseString, settings);

                    List<ClaimInfo> ATresults = new List<ClaimInfo>();

                    foreach (var claim in responseElements)
                    {
                        ClaimInfo ci = new ClaimInfo();
                        ci.ClaimType = claim["ClaimType"];
                        ci.Value = claim["Value"];
                        ci.ValueType = claim["ValueType"].Substring(claim["ValueType"].IndexOf('#') + 1);
                        ci.SubjectName = ((claim["SubjectName"]) != null && (String.IsNullOrEmpty(claim["SubjectName"])) == false) ? claim["SubjectName"] : "Null";
                        ci.IssuerName = !String.IsNullOrEmpty(claim["IssuerName"]) ? claim["IssuerName"] : "Null";

                        ATresults.Add(ci);
                    }

                    ViewBag.ATclaims = ATresults;
                }
                else
                {
                    //empty list
                    ViewBag.ATclaims = new List<ClaimInfo>();
                }
            }
            catch (AdalException ee)
            {
                //empty list
                ViewBag.ATclaims = new List<ClaimInfo>();
            }

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