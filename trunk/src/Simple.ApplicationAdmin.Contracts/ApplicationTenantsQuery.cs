using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.ApplicationAdmin.Contracts
{
    [DataContract(Namespace = Namespaces.Data)]
    public class ApplicationTenantsQuery
    {
        [DataMember]
        public string ApplicationName { get; set; }

        [DataMember]
        public int StartIndex { get; set; }

        [DataMember]
        public int PageSize { get; set; }
    }
}
