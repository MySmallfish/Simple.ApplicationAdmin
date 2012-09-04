using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ApplicationAdmin.Contracts
{
    [DataContract(Namespace=Namespaces.Data)]
    public class ApplicationInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime CreatedAt { get; set; }

        [DataMember]
        public int TenantsCount { get; set; }
    }
}
