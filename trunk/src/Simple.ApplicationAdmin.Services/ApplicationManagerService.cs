using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceModelEx;
using Simple.ApplicationAdmin.Contracts;
using Simple.ApplicationAdmin.Data;

namespace Simple.ApplicationAdmin.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ApplicationManagerService : IApplicationManager
    {
        public void CreateApplication(string name)
        {
            var applicationInfo = new ApplicationInfo();
            applicationInfo.Name = name;
            applicationInfo.CreatedAt = DateTime.UtcNow;

            UseRepository(repositoryService=>repositoryService.CreateApplication(applicationInfo));
        }


        public ApplicationInfo[] GetApplications()
        {
            var repositoryService = InProcFactory.CreateInstance<ApplicationRepositoryService, IApplicationRepository>();
            try
            {
                return repositoryService.GetApplications();
            }
            finally
            {
                InProcFactory.CloseProxy(repositoryService);
            }
        }


        public ApplicationTenantsResult GetApplicationTenants(ApplicationTenantsQuery query)
        {
            var repositoryService = InProcFactory.CreateInstance<ApplicationRepositoryService, IApplicationRepository>();
            try
            {
                return repositoryService.GetApplicationTenants(query);
            }
            finally
            {
                InProcFactory.CloseProxy(repositoryService);
            }
        }


        protected void UpdateTenant(string applicationName, string tenantName, Action<ApplicationTenantHeaderUpdateInfo> updateInfo)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName");
            }
            if (string.IsNullOrEmpty(tenantName))
            {
                throw new ArgumentNullException("tenantName");
            }

            var repositoryService = InProcFactory.CreateInstance<ApplicationRepositoryService, IApplicationRepository>();
            try
            {
                var tenant = repositoryService.GetTenantHeaderInfo(applicationName, tenantName);
                if (tenant == null)
                {
                    throw new ArgumentException(string.Format("Tenant {0} from application {1} could not be found.", tenantName, applicationName));
                }

                var tenantUpdateInfo = new ApplicationTenantHeaderUpdateInfo()
                                           {
                                               ApplicationName = applicationName,
                                               Name = tenantName,
                                               IsActive = tenant.IsActive,
                                               ContractStartTime = tenant.ContractStartedAt,
                                               Url = tenant.Url
                                           };

                updateInfo(tenantUpdateInfo);

                repositoryService.UpdateTenantHeaderInfo(tenantUpdateInfo);
            }
            finally
            {
                InProcFactory.CloseProxy(repositoryService);
            }

        }

        public void ActivateTenant(string applicationName, string tenantName)
        {
            UpdateTenant(applicationName, tenantName, updateInfo => updateInfo.IsActive = true);
        }

        public void DeactivateTenant(string applicationName, string tenantName)
        {
            UpdateTenant(applicationName, tenantName, updateInfo => updateInfo.IsActive = false);
        }

        public void UpdateTenantUrl(string applicationName, string tenantName, string url)
        {
            UpdateTenant(applicationName, tenantName, updateInfo => updateInfo.Url = url);
        }

        public void StartTenantContract(string applicationName, string tenantName, double grossMonthly, double netMonthly, double setupFee)
        {
            UpdateTenant(applicationName, tenantName, updateInfo =>
                                                          {
                                                              if (updateInfo.ContractStartTime.HasValue)
                                                              {
                                                                  throw new ArgumentException("Contract already started");
                                                              }
                                                              updateInfo.ContractStartTime = DateTime.UtcNow;
                                                          });
        }


        private void UseRepository(Action<IApplicationRepository> action)
        {
            var repositoryService = InProcFactory.CreateInstance<ApplicationRepositoryService, IApplicationRepository>();
            try
            {
                action(repositoryService);
            }
            finally
            {
                InProcFactory.CloseProxy(repositoryService);
            }
        }

        public ApplicationTenantHeaderInfo GetTenantDetails(string applicationName, string tenantName)
        {
            var result = default(ApplicationTenantHeaderInfo);
            UseRepository(repositoryService => result = repositoryService.GetTenantHeaderInfo(applicationName, tenantName));
            return result;
        }


        public void UpdateDatabaseConfiguration(string applicationName, string tenantName, DatabaseConfigurationInfo configurationInfo)
        {
            UseRepository(
                repository =>
                    {
                        var configuration = repository.GetTenantDatabaseConfiguration(applicationName, tenantName);
                        if (configuration != null)
                        {
                            var item = configuration.FirstOrDefault(i => i.Name == configurationInfo.Name);
                            if (item == null)
                            {
                                throw new ArgumentException(string.Format("Configuration with name '{0}' could not be found for tenant {1}, application {2}.", configurationInfo.Name, tenantName, applicationName));
                            }

                            item.ProviderName = configurationInfo.ProviderName;
                            item.ConnectionString = configurationInfo.ConnectionString;

                            repository.UpdateTenantDatabaseConfiguration(applicationName, tenantName, configuration);
                        }
                    });
        }

        public void AddDatabaseConfiguration(string applicationName, string tenantName, DatabaseConfigurationInfo configurationInfo)
        {
            UseRepository(
                repository =>
                {
                    var updatedConfiguration = new List<DatabaseConfigurationInfo>();
                    var configuration = repository.GetTenantDatabaseConfiguration(applicationName, tenantName);
                    if (configuration != null)
                    {
                        updatedConfiguration.AddRange(configuration);
                    }

                    updatedConfiguration.Add(configurationInfo);

                    repository.UpdateTenantDatabaseConfiguration(applicationName, tenantName, updatedConfiguration.ToArray());
                });
        }

        public void RemoveDatabaseConfiguration(string applicationName, string tenantName, string configurationName)
        {
            UseRepository(
                repository =>
                {
                    
                    var configuration = repository.GetTenantDatabaseConfiguration(applicationName, tenantName);
                    if (configuration != null)
                    {
                        var matched = configuration.Where(item => item.Name != configurationName);
                        configuration = (matched ?? Enumerable.Empty<DatabaseConfigurationInfo>()).ToArray();

                        repository.UpdateTenantDatabaseConfiguration(applicationName, tenantName, configuration);
                    }

                });
        }

        public DatabaseConfigurationInfo[] GetDatabaseConfiguration(string applicationName, string tenantName)
        {
            var result = new List<DatabaseConfigurationInfo>();
            UseRepository(
                repository =>
                    {
                        var configuration = repository.GetTenantDatabaseConfiguration(applicationName, tenantName);
                        if (configuration != null)
                        {
                            result.AddRange(configuration);
                        }
                    });
            return result.ToArray();
        }


        public void DeleteApplication(string name)
        {
            UseRepository(repositoryService => repositoryService.DeleteApplication(name));
        }


        public void AddApplicationTenant(string applicationName, string name, string url)
        {
            UseRepository(repositoryService=>repositoryService.AddApplicationTenant(applicationName, name, url));
        }

        public void DeleteApplicationTenant(string applicationName, string name)
        {
            UseRepository(repositoryService => repositoryService.DeleteApplicationTenant(applicationName, name));
        }
    }
}
