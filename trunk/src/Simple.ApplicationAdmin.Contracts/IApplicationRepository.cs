using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ApplicationAdmin.Contracts
{
    [ServiceContract(Namespace = Namespaces.Services)]
    public interface IApplicationRepository
    {
        [OperationContract]
        void AddApplicationTenant(string applicationName, string name, string url);

        [OperationContract]
        void DeleteApplicationTenant(string applicationName, string name);

        [OperationContract]
        ApplicationInfo[] GetApplications();

        [OperationContract]
        void CreateApplication(ApplicationInfo applicationInfo);

        [OperationContract]
        ApplicationTenantHeaderInfo GetTenantHeaderInfo(string applicationName, string tenantName);

        [OperationContract]
        void UpdateTenantHeaderInfo(ApplicationTenantHeaderUpdateInfo tenantUpdateInfo);
        
        [OperationContract]
        ApplicationTenantsResult GetApplicationTenants(ApplicationTenantsQuery query);

        [OperationContract]
        DatabaseConfigurationInfo[] GetTenantDatabaseConfiguration(string applicationName, string tenantName);

        [OperationContract]
        void UpdateTenantDatabaseConfiguration(string applicationName, string tenantName, DatabaseConfigurationInfo[] configuration);

        [OperationContract]
        void DeleteApplication(string name);
    }
}
