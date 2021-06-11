using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofence
{
    public interface ILandmarkGroupManager
    {
        Task<LandmarkGroup> CreateGroup(LandmarkGroup landmarkgroup);
        Task<LandmarkGroup> UpdateGroup(LandmarkGroup landmarkgroup);
        Task<int> DeleteGroup(int groupid, int modifiedby);
        Task<IEnumerable<LandmarkGroup>> GetlandmarkGroup(int organizationid, int groupid);
        Task<int> Exists(LandmarkGroup landmarkgroup);
    }
}
