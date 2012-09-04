using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ApplicationAdmin.Contracts
{
    [DataContract(Namespace = Namespaces.Data)]
    public class DatabaseConfigurationInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ConnectionString { get; set; }
        
        [DataMember]
        public string ProviderName { get; set; }
    }
}
