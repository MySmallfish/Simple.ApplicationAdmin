using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Simple.AplicationAdmin.Models;
using Simple.ApplicationAdmin.Client;
using Simple.ApplicationAdmin.Contracts;
using Simple.Utilities;

namespace Simple.AplicationAdmin.Controllers
{
    public class ApplicationController : Controller
    {
        //
        // GET: /Application/

        public ActionResult ActivateTenant(string applicationName, string name)
        {
            var client = new ApplicationManagerClient();
            client.ActivateTenant(applicationName, name);
            return RedirectToAction("Tenants", new {applicationName});
        }
        public ActionResult DeactivateTenant(string applicationName, string name)
        {
            var client = new ApplicationManagerClient();
            client.DeactivateTenant(applicationName, name);
            return RedirectToAction("Tenants", new { applicationName });
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tenants(string applicationName, int startIndex = 0, int pageSize = 50)
        {
            var applicationTenantsModel = new ApplicationTenantsModel
            {
                ApplicationName = applicationName
            };

            var client = new ApplicationManagerClient();
            var query = new ApplicationTenantsQuery() { ApplicationName = applicationName, PageSize = pageSize, StartIndex = startIndex };
            var results = client.GetApplicationTenants(query);

            applicationTenantsModel.TotalCount = results.TotalCount;
            applicationTenantsModel.Tenants = results.Items ?? new ApplicationTenantHeaderInfo[]{};
            
            return View(applicationTenantsModel);
        }

        [HttpPost]
        public ActionResult UpdateTenantDetails(TenantDetailsModel details)
        {
            var client = new ApplicationManagerClient();
            client.UpdateTenantUrl(details.ApplicationName, details.Name, details.Url);
            
            return RedirectToAction("TenantDetails" , new { details.ApplicationName, details.Name});
        }
        public ActionResult TenantDetails(string applicationName, string name)
        {
            var tenantDetailsModel = GetTenantDetailsModel(applicationName, name);
            return View(tenantDetailsModel);
        }

        private TenantDetailsModel GetTenantDetailsModel(string applicationName, string name)
        {
            var tenantDetailsModel = default(TenantDetailsModel);
            var client = new ApplicationManagerClient();
            var headerInfo = client.GetTenantDetails(applicationName, name);
            if (headerInfo == null)
            {
                ModelState.AddModelError("TenantNotFound",
                                         string.Format("Tenant {0} in application {1} could not be found.", name,
                                                       applicationName));
            }
            else
            {
                tenantDetailsModel = new TenantDetailsModel();
                tenantDetailsModel.ApplicationName = applicationName;
                tenantDetailsModel.Name = name;
                tenantDetailsModel.Url = headerInfo.Url;
                tenantDetailsModel.ContractStartTime = headerInfo.ContractStartedAt;
                tenantDetailsModel.IsActive = headerInfo.IsActive;
                tenantDetailsModel.Connections = client.GetDatabaseConfiguration(applicationName, name);
            }
            return tenantDetailsModel;
        }

        public ActionResult TenantDatabaseConfiguration(string applicationName, string name)
        {

            var tenantDetailsModel = GetTenantDetailsModel(applicationName, name);
            return View(tenantDetailsModel);
        }

        [HttpPost]
        public ActionResult UpdateTenantDatabaseConfiguration(string applicationName, string name, DatabaseConfigurationInfo configurationInfo)
        {
            Requires.ArgumentNotNullOrEmptyString(applicationName, "applicationName");
            Requires.ArgumentNotNullOrEmptyString(name, "name");
            Requires.ArgumentNotNull(configurationInfo, "configurationInfo");
            Requires.ArgumentNotNull(configurationInfo.Name, "configurationInfo.Name");
            Requires.ArgumentNotNull(configurationInfo.ConnectionString, "configurationInfo.ConnectionString");

            var client = new ApplicationManagerClient();
            client.UpdateDatabaseConfiguration(applicationName, name, configurationInfo);
            return RedirectToAction("TenantDatabaseConfiguration", new { applicationName, name });
        }

        [HttpPost]
        public ActionResult AddTenantDatabaseConfiguration(string applicationName, string name, DatabaseConfigurationInfo configurationInfo)
        {
            Requires.ArgumentNotNullOrEmptyString(applicationName, "applicationName");
            Requires.ArgumentNotNullOrEmptyString(name, "name");
            Requires.ArgumentNotNull(configurationInfo, "configurationInfo");
            Requires.ArgumentNotNull(configurationInfo.Name, "configurationInfo.Name");
            Requires.ArgumentNotNull(configurationInfo.ConnectionString, "configurationInfo.ConnectionString");

            var client = new ApplicationManagerClient();
            client.AddDatabaseConfiguration(applicationName, name, configurationInfo);

            return RedirectToAction("TenantDatabaseConfiguration", new {applicationName, name});
        }

        [HttpPost]
        public ActionResult DeleteTenantDatabaseConfiguration(string applicationName, string name, string configurationname)
        {
            Requires.ArgumentNotNullOrEmptyString(applicationName, "applicationName");
            Requires.ArgumentNotNullOrEmptyString(name, "name");
            Requires.ArgumentNotNullOrEmptyString(configurationname, "configurationname");

            var client = new ApplicationManagerClient();
            client.RemoveDatabaseConfiguration(applicationName, name, configurationname);

            return RedirectToAction("TenantDatabaseConfiguration", new { applicationName, name });
        }

        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(string name)
        {
            Requires.ArgumentNotNullOrEmptyString(name, "name");

            var client = new ApplicationManagerClient();
            client.DeleteApplication(name);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            Requires.ArgumentNotNullOrEmptyString(name, "name");

            var client = new ApplicationManagerClient();
            client.CreateApplication(name);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult DeleteTenant(string applicationName, string name)
        {
            Requires.ArgumentNotNullOrEmptyString(applicationName, "applicationName");
            Requires.ArgumentNotNullOrEmptyString(name, "name");

            var client = new ApplicationManagerClient();
            client.DeleteApplicationTenant(applicationName, name);
            return RedirectToAction("Tenants", new { applicationName });
        }

        [HttpPost]
        public ActionResult AddTenant(string applicationName, string name, string url)
        {
            Requires.ArgumentNotNullOrEmptyString(applicationName, "applicationName");
            Requires.ArgumentNotNullOrEmptyString(name, "name");
            Requires.ArgumentNotNullOrEmptyString(url, "url");

            var client = new ApplicationManagerClient();
            client.AddApplicationTenant(applicationName, name, url);
            return RedirectToAction("Tenants", new { applicationName });
        }
    }
}
