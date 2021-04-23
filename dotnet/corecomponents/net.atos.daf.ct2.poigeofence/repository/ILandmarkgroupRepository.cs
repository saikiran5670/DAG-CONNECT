using net.atos.daf.ct2.poigeofence.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    interface ILandmarkgroupRepository
    {
        Task<LandmarkGroup> CreateGroup(LandmarkGroup landmarkgroup);

        Task<int> AddgroupReference(LandmarkgroupRef landmarkgroupref);
        Task<int> DeleteGroupref(int landmark_group_id);
        Task<LandmarkGroup> UpdateGroup(LandmarkGroup landmarkgroup);
    }
}
