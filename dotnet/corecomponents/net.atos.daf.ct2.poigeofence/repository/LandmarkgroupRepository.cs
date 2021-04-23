using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public class LandmarkgroupRepository : ILandmarkgroupRepository
    {
        private readonly IDataAccess dataAccess;
        private static readonly log4net.ILog log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LandmarkgroupRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        public async Task<LandmarkGroup> CreateGroup(LandmarkGroup landmarkgroup)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", landmarkgroup.organization_id);
                parameter.Add("@name", landmarkgroup.name);
                parameter.Add("@icon_id", landmarkgroup.icon_id);
                parameter.Add("@state", (char)landmarkgroup.state);
                parameter.Add("@created_at", landmarkgroup.created_at);
                parameter.Add("@created_by", landmarkgroup.created_by);

                string query = @"insert into master.landmarkgroup(organization_id, name, icon_id, state, created_at, created_by)
	                              VALUES (@organization_id, @name, @icon_id, @state, @created_at, @created_by) RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                foreach (var item in landmarkgroup.poilist)
                {
                    LandmarkgroupRef landmarkgroupRef = new LandmarkgroupRef();
                    landmarkgroupRef.landmark_group_id = id;
                    landmarkgroupRef.ref_id = item.Id;
                    landmarkgroupRef.type = (LandmarkType)Enum.Parse(typeof(LandmarkType), item.Type.ToString());
                    var refid = AddgroupReference(landmarkgroupRef);
                }
                landmarkgroup.id = id;
                return landmarkgroup;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> AddgroupReference(LandmarkgroupRef landmarkgroupref)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@landmark_group_id", landmarkgroupref.landmark_group_id);
                parameter.Add("@type", landmarkgroupref.type);
                parameter.Add("@ref_id", landmarkgroupref.ref_id);
                            

                string query = @"insert into master.landmarkgroupref(landmark_group_id, type, ref_id)
	                              VALUES (@landmark_group_id, @type, @ref_id) RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return id;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<LandmarkGroup> UpdateGroup(LandmarkGroup landmarkgroup)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", landmarkgroup.id);
                parameter.Add("@organization_id", landmarkgroup.organization_id);
                parameter.Add("@name", landmarkgroup.name);
                parameter.Add("@icon_id", landmarkgroup.icon_id);
                parameter.Add("@state", (char)landmarkgroup.state);
                parameter.Add("@created_at", landmarkgroup.created_at);
                parameter.Add("@created_by", landmarkgroup.created_by);

                string query = @"update master.landmarkgroup set organization_id=@organization_id, name=@name, state=@state)
	                              where id=@id RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                foreach (var item in landmarkgroup.poilist)
                {
                    LandmarkgroupRef landmarkgroupRef = new LandmarkgroupRef();
                    landmarkgroupRef.landmark_group_id = id;
                    landmarkgroupRef.ref_id = item.Id;
                    landmarkgroupRef.type = (LandmarkType)Enum.Parse(typeof(LandmarkType), item.Type.ToString());
                    var refid = AddgroupReference(landmarkgroupRef);
                }
                landmarkgroup.id = id;
                return landmarkgroup;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> DeleteGroupref(int landmark_group_id)
        {
            try
            {
                var parameter = new DynamicParameters();
                    parameter.Add("@landmark_group_id", landmark_group_id);

                string query = @"DELETE FROM master.landmarkgroupref
	                            WHERE landmark_group_id=@landmark_group_id;";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                                
                
                return id;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
