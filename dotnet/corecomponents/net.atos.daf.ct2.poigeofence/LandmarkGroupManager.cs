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

        public async Task<int> DeleteGroup(int groupid, int modifiedby)
        {
            return await _landmarkgroupRepository.DeleteGroup(groupid,modifiedby);
        }
        // Task<IEnumerable<LandmarkgroupRef>> GetlandmarkGroupref(int groupid)
        public async Task<IEnumerable<LandmarkGroup>> GetlandmarkGroup(int organizationid, int groupid)
        {
            var groups = await _landmarkgroupRepository.GetlandmarkGroup(organizationid,groupid);
            foreach (var item in groups)
            {
                var groupref = await _landmarkgroupRepository.GetlandmarkGroupref(item.id);
                item.poilist = new List<POI>();                
                foreach (var pois in groupref)
                {
                    POI pOI = new POI();
                    pOI.Id = pois.ref_id;
                    pOI.Type = pois.type.ToString();
                    item.poilist.Add(pOI);
                }
            }
            return groups;
        }
    }
}
