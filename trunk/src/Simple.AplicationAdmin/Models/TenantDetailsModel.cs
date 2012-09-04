using System;
using Simple.ApplicationAdmin.Contracts;

namespace Simple.AplicationAdmin.Models
{
    public class TenantDetailsModel
    {
        public string ApplicationName { get; set; }
        
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ContractStartTime { get; set; }
        public DatabaseConfigurationInfo[] Connections { get; set; }
    }

    

    public class UpdateTenantDetailsModel
    {
        public string ApplicationName { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime? ContractStartTime { get; set; }
    }
}