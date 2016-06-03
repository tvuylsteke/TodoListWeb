using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using TodoListService.Models;
using System.Security.Claims;

namespace TodoListService.Controllers
{
    [Authorize]
    public class ClaimsController : ApiController
    {
        public IEnumerable<ClaimInfo> Get()
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

            return results;
        }           
        
    }
}
