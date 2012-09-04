using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ApplicationAdmin.Contracts
{
    [DataContract(Namespace = Namespaces.Data)]
    public class ApplicationTenantHeaderUpdateInfo
    {
        [DataMember]
        public string ApplicationName { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public DateTime? ContractStartTime { get; set; }
    }
}
