using net.atos.daf.ct2.poigeofence.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public interface ILandmarkgroupRepository
    {
        Task<LandmarkGroup> CreateGroup(LandmarkGroup landmarkgroup);
        Task<int> AddgroupReference(LandmarkgroupRef landmarkgroupref);
        int DeleteGroupref(int landmark_group_id);
        Task<LandmarkGroup> UpdateGroup(LandmarkGroup landmarkgroup);
        Task<IEnumerable<LandmarkGroup>> GetlandmarkGroup(int organizationid, int groupid);
        Task<int> DeleteGroup(int groupid,int modifiedby);

        Task<List<LandmarkgroupRef>> GetlandmarkGroupref(int groupid);
    }
}
