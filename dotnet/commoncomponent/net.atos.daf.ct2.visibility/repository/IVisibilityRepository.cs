using System;
using System.Collections.Generic;
using net.atos.daf.ct2.visibility.entity;
using net.atos.daf.ct2.data;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.visibility.repository
{
    public interface IVisibilityRepository
    {
        //IEnumerable<FeatureSet> GetFeatureSet(int userid, int orgid );
        Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibility(int accountId, int OrganizationId);
    }
}
