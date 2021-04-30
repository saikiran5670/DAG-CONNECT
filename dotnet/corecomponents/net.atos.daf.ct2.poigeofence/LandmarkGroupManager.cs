﻿using net.atos.daf.ct2.poigeofence.entity;
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
            if (groupid >0)
            {
                foreach (var group in groups)
                {
                    var groupref = await _landmarkgroupRepository.GetlandmarkGroupref(groupid);
                    group.PoiList = new List<POI>();
                    //group.GeofenceList = new List<Geofence>();
                    foreach (var item in groupref)
                    {
                       
                            POI obj = new POI();
                            obj.Id = item.landmarkid;
                            obj.Name = item.landmarkname;
                            obj.CategoryName = item.categoryname;
                            obj.SubCategoryName = item.subcategoryname;
                            obj.Address = item.address;
                            obj.icon = item.icon;
                            obj.Type = Convert.ToString((char)item.type);
                            group.PoiList.Add(obj);
                                             
                    }
                }
                
                
            }

            return groups;
        }

        public async Task<int> Exists(LandmarkGroup landmarkgroup)
        {
            return await _landmarkgroupRepository.Exists(landmarkgroup);
        }
    }
}
