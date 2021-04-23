using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public class LandmarkGroupManager : ILandmarkGroupManager
    {
        private readonly ILandmarkgroupRepository _landmarkgroupRepository;
        public LandmarkGroupManager(ILandmarkgroupRepository landmarkgroupRepository)
        {
            _landmarkgroupRepository = landmarkgroupRepository;
        }

        public async Task<LandmarkGroup> CreateGroup(LandmarkGroup landmarkgroup)
        {
            return await _landmarkgroupRepository.CreateGroup(landmarkgroup);
        }

        public async Task<LandmarkGroup> UpdateGroup(LandmarkGroup landmarkgroup)
        {
            return await _landmarkgroupRepository.UpdateGroup(landmarkgroup);
        }

        public async Task<int> DeleteGroup(int groupid)
        {
            return await _landmarkgroupRepository.DeleteGroup(groupid);
        }

        public async Task<int> GetlandmarkGroup(int organizationid, int groupid)
        {
            return await _landmarkgroupRepository.GetlandmarkGroup(organizationid,groupid);
        }
    }
}
