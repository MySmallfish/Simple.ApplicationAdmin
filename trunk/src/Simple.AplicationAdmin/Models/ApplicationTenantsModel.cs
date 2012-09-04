using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simple.ApplicationAdmin.Contracts;

namespace Simple.AplicationAdmin.Models
{
    public class ApplicationTenantsModel
    {
        public int TotalCount { get; set; }
        public string ApplicationName { get; set; }
        public ApplicationTenantHeaderInfo[] Tenants { get; set; }
    }
}