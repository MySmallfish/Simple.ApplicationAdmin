using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ApplicationAdmin.Contracts
{
    [ServiceContract(Namespace=Namespaces.Services)]
    public interface IApplicationManager
    {
        [OperationContract(IsOneWay = true)]
        void CreateApplication(string name);

        [OperationContract]
        ApplicationInfo[] GetApplications();

        [OperationContract]
        ApplicationTenantsResult GetApplicationTenants(ApplicationTenantsQuery query);

        [OperationContract]
        ApplicationTenantHeaderInfo GetTenantDetails(string applicationName, string tenantName);

        [OperationContract]
        void ActivateTenant(string applicationName, string tenantName);

        [OperationContract]
        void DeactivateTenant(string applicationName, string tenantName);

        [OperationContract]
        void UpdateTenantUrl(string applicationName, string tenantName, string url);        
        
        [OperationContract]
        void StartTenantContract(string applicationName, string tenantName, double grossMonthly, double netMonthly, double setupFee);

        [OperationContract]
        void AddDatabaseConfiguration(string applicationName, string tenantName, DatabaseConfigurationInfo configurationInfo);
        
        [OperationContract]
        void RemoveDatabaseConfiguration(string applicationName, string tenantName, string configurationName);

        [OperationContract]
        DatabaseConfigurationInfo[] GetDatabaseConfiguration(string applicationName, string tenantName);

        [OperationContract]
        void UpdateDatabaseConfiguration(string applicationName, string tenantName,
                                         DatabaseConfigurationInfo configurationInfo);
    }
}
