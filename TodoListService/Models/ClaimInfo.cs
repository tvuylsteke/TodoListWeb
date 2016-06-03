using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TodoListService.Models
{
    public class ClaimInfo
    {
        public ClaimInfo() { }

        public String ClaimType { get; set; }
        public String Value { get; set; }
        public String ValueType { get; set; }
        public String SubjectName { get; set; }
        public String IssuerName { get; set; }
    }
}