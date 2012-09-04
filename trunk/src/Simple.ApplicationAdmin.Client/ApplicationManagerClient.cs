using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Simple.ApplicationAdmin.Contracts;

namespace Simple.ApplicationAdmin.Client
{
    public class ApplicationManagerClient : ClientBase<IApplicationManager>, IApplicationManager
    {
        public void CreateApplication(string name)
        {
            Channel.CreateApplication(name);
        }


        public ApplicationInfo[] GetApplications()
        {
            return Channel.GetApplications();
        }


        public ApplicationTenantsResult GetApplicationTenants(ApplicationTenantsQuery query)
        {
            return Channel.GetApplicationTenants(query);
        }


        public ApplicationTenantHeaderInfo GetTenantDetails(string applicationName, string tenantName)
        {
            return Channel.GetTenantDetails(applicationName, tenantName);
        }

        public void ActivateTenant(string applicationName, string tenantName)
        {
            Channel.ActivateTenant(applicationName, tenantName);
        }

        public void DeactivateTenant(string applicationName, string tenantName)
        {
            Channel.DeactivateTenant(applicationName, tenantName);
        }

        public void UpdateTenantUrl(string applicationName, string tenantName, string url)
        {
            Channel.UpdateTenantUrl(applicationName, tenantName,url);
        }

        public void StartTenantContract(string applicationName, string tenantName, double grossMonthly, double netMonthly, double setupFee)
        {
            Channel.StartTenantContract(applicationName, tenantName, grossMonthly, netMonthly, setupFee);
        }


        public void AddDatabaseConfiguration(string applicationName, string tenantName, DatabaseConfigurationInfo configurationInfo)
        {
            Channel.AddDatabaseConfiguration(applicationName, tenantName, configurationInfo);
        }

        public void RemoveDatabaseConfiguration(string applicationName, string tenantName, string configurationName)
        {
            Channel.RemoveDatabaseConfiguration(applicationName, tenantName, configurationName);
        }

        public DatabaseConfigurationInfo[] GetDatabaseConfiguration(string applicationName, string tenantName)
        {
            return Channel.GetDatabaseConfiguration(applicationName, tenantName);
        }


        public void UpdateDatabaseConfiguration(string applicationName, string tenantName, DatabaseConfigurationInfo configurationInfo)
        {
            Channel.UpdateDatabaseConfiguration(applicationName, tenantName, configurationInfo);
        }
    }
}
