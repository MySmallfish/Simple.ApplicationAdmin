using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ApplicationAdmin.Contracts
{
    [DataContract(Namespace = Namespaces.Data)]
    public class ApplicationTenantsResult
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public ApplicationTenantHeaderInfo[] Items { get; set; }
    }
}
