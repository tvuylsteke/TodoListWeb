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
        //private string todoListResourceId = ConfigurationManager.AppSettings["todo:TodoListResourceId"];
        //private string todoListBaseAddress = ConfigurationManager.AppSettings["todo:TodoListBaseAddress"];

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