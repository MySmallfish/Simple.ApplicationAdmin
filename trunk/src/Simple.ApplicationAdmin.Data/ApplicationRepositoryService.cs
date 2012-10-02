using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Simple.ApplicationAdmin.Contracts;
using Simple.Utilities;

namespace Simple.ApplicationAdmin.Data
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ApplicationRepositoryService : IApplicationRepository
    {
        public ApplicationInfo[] GetApplications()
        {
            var applications = new List<ApplicationInfo>();
            UseDataContext(
                    dataSource =>
                    {
                        applications.AddRange(dataSource.Applications.Select(app => new ApplicationInfo { Name = app.Name, CreatedAt = app.CreatedAt, TenantsCount = app.ApplicationTenants.Count() }));
                    });
            return applications.ToArray();
        }

        protected virtual void UseDataContext(Action<SimpleApplicationAdminDataContext> action)
        {
            using (var dataContext = new SimpleApplicationAdminDataContext())
            {
                action(dataContext);
            }
        }

        public void CreateApplication(ApplicationInfo applicationInfo)
        {
            UseDataContext(dataContext =>
            {
                var appInfo = dataContext.Applications.FirstOrDefault(a => a.Name == applicationInfo.Name);
                if (appInfo != null)
                {
                    throw new ArgumentException(string.Format("Application '{0}' already exist.", applicationInfo.Name));
                }

                appInfo = new Application();
                appInfo.Name = applicationInfo.Name;
                appInfo.CreatedAt = applicationInfo.CreatedAt;

                dataContext.Applications.InsertOnSubmit(appInfo);
                dataContext.SubmitChanges();
            });
        }


        protected virtual ApplicationTenantHeaderInfo MapDataApplicationTenant(ApplicationTenant dataTenant)
        {
            var result = new ApplicationTenantHeaderInfo
            {
                Name = dataTenant.TenantName,
                IsActive = dataTenant.IsActive,
                ContractStartedAt = dataTenant.ContractStartedAt,
                InContract = dataTenant.ContractStartedAt.HasValue,
                Url = dataTenant.Url
            };

            return result;
        }

        public ApplicationTenantsResult GetApplicationTenants(ApplicationTenantsQuery query)
        {
            var result = new ApplicationTenantsResult();

            UseDataContext(dataContext =>
            {
                dataContext.ObjectTrackingEnabled = false;

                var searchQuery = dataContext.ApplicationTenants.Where(tenant => tenant.Application == query.ApplicationName);
                result.TotalCount = searchQuery.Count();
                searchQuery = searchQuery.Skip(query.StartIndex).Take(query.PageSize);
                result.Items = searchQuery.Select(MapDataApplicationTenant).ToArray();
            });

            return result;
        }

        public ApplicationTenantHeaderInfo GetTenantHeaderInfo(string applicationName, string tenantName)
        {
            var result = default(ApplicationTenantHeaderInfo);

            UseDataContext(dataContext =>
                               {
                                   dataContext.ObjectTrackingEnabled = false;
                                   var dataTenant =
                                       dataContext.ApplicationTenants.FirstOrDefault(
                                           t => t.Application == applicationName && t.TenantName == tenantName);

                                   if (dataTenant != null)
                                   {
                                       result = MapDataApplicationTenant(dataTenant);
                                   }
                               });
            return result;
        }

        public void UpdateTenantHeaderInfo(ApplicationTenantHeaderUpdateInfo tenantUpdateInfo)
        {

            UseDataContext(dataContext =>
                               {
                                   var dataTenant =
                                       dataContext.ApplicationTenants.FirstOrDefault(
                                           t => t.Application == tenantUpdateInfo.ApplicationName && t.TenantName == tenantUpdateInfo.Name);
                                   if (dataTenant == null)
                                   {
                                       throw new ArgumentException(string.Format("Tenant {0} from application {1} could not be found.", tenantUpdateInfo.Name, tenantUpdateInfo.ApplicationName));
                                   }

                                   dataTenant.Url = tenantUpdateInfo.Url;
                                   dataTenant.IsActive = tenantUpdateInfo.IsActive;
                                   dataTenant.ContractStartedAt = tenantUpdateInfo.ContractStartTime;

                                   dataContext.SubmitChanges();
                               });
        }

        public void UpdateTenantDatabaseConfiguration(string applicationName, string tenantName, DatabaseConfigurationInfo[] configuration)
        {
            UseDataContext(dataContext =>
                               {
                                   var dataTenant =
                                       dataContext.ApplicationTenants.FirstOrDefault(
                                           t =>
                                           t.Application == applicationName &&
                                           t.TenantName == tenantName);
                                   if (dataTenant == null)
                                   {
                                       throw new ArgumentException(
                                           string.Format("Tenant {0} from application {1} could not be found.",
                                                         tenantName, applicationName));
                                   }

                                   dataTenant.DatabaseConfiguration = DataHelper.MapToXml(configuration);

                                   dataContext.SubmitChanges();
                               });
        }

        public DatabaseConfigurationInfo[] GetTenantDatabaseConfiguration(string applicationName, string tenantName)
        {
            var configuration = new List<DatabaseConfigurationInfo>();

            UseDataContext(dataContext =>
                               {
                                   var dataTenant =
                                       dataContext.ApplicationTenants.FirstOrDefault(
                                           t =>
                                           t.Application == applicationName &&
                                           t.TenantName == tenantName);
                                   if (dataTenant == null)
                                   {
                                       throw new ArgumentException(
                                           string.Format("Tenant {0} from application {1} could not be found.",
                                                         tenantName, applicationName));
                                   }

                                   var items =
                                       DataHelper.MapFromXml<DatabaseConfigurationInfo[]>(dataTenant.DatabaseConfiguration);

                                   if (items != null)
                                   {
                                       configuration.AddRange(items);
                                   }
                               });

            return configuration.ToArray();
        }


        public void DeleteApplication(string name)
        {
            UseDataContext(
                dataSource =>
                {
                    var application = dataSource.Applications.FirstOrDefault(app => app.Name == name);
                    if (application != null)
                    {
                        if (application.ApplicationTenants.Any())
                        {
                            throw new ApplicationException(string.Format("Application {0} still contains tenants.", name));
                        }
                        dataSource.Applications.DeleteOnSubmit(application);
                    }

                    dataSource.SubmitChanges();
                });
        }

        public void AddApplicationTenant(string applicationName, string name, string url)
        {
            UseDataContext(
                dataSource =>
                {
                    if (dataSource.ApplicationTenants.Any(t => t.Application == applicationName && t.TenantName == name))
                    {
                        throw new ArgumentException(string.Format("Tenant {0} already exist in application {1}.",
                                                                  name, applicationName));
                    }

                    var tenant = new ApplicationTenant();
                    tenant.TenantName = name;
                    tenant.Url = url;
                    tenant.Application = applicationName;
                    tenant.IsActive = true;

                    dataSource.ApplicationTenants.InsertOnSubmit(tenant);

                    dataSource.SubmitChanges();
                });
        }

        public void DeleteApplicationTenant(string applicationName, string name)
        {
            UseDataContext(dataSource =>
                               {
                                   var tenant =
                                       dataSource.ApplicationTenants.FirstOrDefault(
                                           t => t.Application == applicationName && t.TenantName == name);
                                   if (tenant == null)
                                   {
                                       throw new ArgumentException(string.Format("Tenant {0} does not exist on application {1}.", name, applicationName));
                                   }

                                   dataSource.ApplicationTenants.DeleteOnSubmit(tenant);

                                   dataSource.SubmitChanges();
                               });
        }
    }
}
