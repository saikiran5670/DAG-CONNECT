using net.atos.daf.ct2.poigeofence.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public interface ILandmarkGroupManager
    {
        Task<LandmarkGroup> CreateGroup(LandmarkGroup landmarkgroup);      
        Task<LandmarkGroup> UpdateGroup(LandmarkGroup landmarkgroup);
        Task<int> DeleteGroup(int groupid);
        Task<int> GetlandmarkGroup(int organizationid, int groupid);
    }
}
