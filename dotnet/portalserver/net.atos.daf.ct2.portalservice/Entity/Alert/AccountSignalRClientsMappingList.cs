using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AccountSignalRClientsMappingList
    {
        internal readonly List<AccountSignalRClientMapper> _accountClientMapperList = new List<AccountSignalRClientMapper>();
    }
    public class AccountSignalRClientMapper
    {
        public int OrganizationId { get; set; }
        public int AccountId { get; set; }
        public string HubClientId { get; set; }
        public IEnumerable<int> FeatureIds { get; set; }
        public int ContextOrgId { get; set; }
    }
}