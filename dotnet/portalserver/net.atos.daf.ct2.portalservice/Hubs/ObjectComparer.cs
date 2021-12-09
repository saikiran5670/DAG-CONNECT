using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.pushnotificationservice;

namespace net.atos.daf.ct2.portalservice.Hubs
{
    internal class ObjectComparer : IEqualityComparer<AccountSignalRClientList>
    {
        public bool Equals(AccountSignalRClientList x, AccountSignalRClientList y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }
            if (x is null || y is null)
            {
                return false;
            }
            return x.OrganizationId == y.OrganizationId && x.ContextOrgId == y.ContextOrgId && x.OTAFeatureId == y.OTAFeatureId;
        }

        public int GetHashCode([DisallowNull] AccountSignalRClientList obj)
        {
            if (obj == null)
            {
                return 0;
            }

            int orgIdHashCode = obj.OrganizationId.GetHashCode();
            int c = obj.ContextOrgId.GetHashCode();
            int otaFeatureIdHashCode = obj.OTAFeatureId.GetHashCode();
            return orgIdHashCode ^ orgIdHashCode ^ otaFeatureIdHashCode;
        }
    }
}
