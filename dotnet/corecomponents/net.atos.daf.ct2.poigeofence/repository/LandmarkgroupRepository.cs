using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.utilities;
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
                parameter.Add("@description", landmarkgroup.description);
                //parameter.Add("@icon_id", landmarkgroup.icon_id);
                parameter.Add("@state", "A");
                parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@created_by", landmarkgroup.created_by);

                string query = @"insert into master.landmarkgroup(organization_id, name,description,  state, created_at, created_by)
	                              VALUES (@organization_id, @name,@description,  @state, @created_at, @created_by) RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                foreach (var item in landmarkgroup.PoiList)
                {
                    LandmarkgroupRef landmarkgroupRef = new LandmarkgroupRef();
                    landmarkgroupRef.landmark_group_id = id;
                    landmarkgroupRef.ref_id = item.Id;
                    landmarkgroupRef.type = (LandmarkType)Enum.Parse(typeof(LandmarkType), item.Type.ToString());
                    var refid = await AddgroupReference(landmarkgroupRef);
                }
                landmarkgroup.id = id;
                return landmarkgroup;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<int> AddgroupReference(LandmarkgroupRef landmarkgroupref)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@landmark_group_id", landmarkgroupref.landmark_group_id);
                parameter.Add("@type", (char)landmarkgroupref.type);
                parameter.Add("@ref_id", landmarkgroupref.ref_id);
                            

                string query = @"insert into master.landmarkgroupref(landmark_group_id, type, ref_id)
	                              VALUES (@landmark_group_id, @type, @ref_id) RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<LandmarkGroup> UpdateGroup(LandmarkGroup landmarkgroup)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", landmarkgroup.id);
               
                parameter.Add("@name", landmarkgroup.name);
                parameter.Add("@description", landmarkgroup.description); 
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@modified_by", landmarkgroup.modified_by);

                string query = @"update master.landmarkgroup set  name=@name,description=@description, modified_at=@modified_at, modified_by = @modified_by
	                              where id=@id RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                var deletegroupref = this.DeleteGroupref(landmarkgroup.id);
                foreach (var item in landmarkgroup.PoiList)
                {
                    LandmarkgroupRef landmarkgroupRef = new LandmarkgroupRef();
                    landmarkgroupRef.landmark_group_id = id;
                    landmarkgroupRef.ref_id = item.Id;
                    landmarkgroupRef.type = (LandmarkType)Enum.Parse(typeof(LandmarkType), item.Type.ToString());
                    var refid=  await AddgroupReference(landmarkgroupRef);
                }
                landmarkgroup.id = id;
                return landmarkgroup;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int DeleteGroupref(int landmark_group_id)
        {
            try
            {
                var parameter = new DynamicParameters();
                    parameter.Add("@landmark_group_id", landmark_group_id);

                string query = @"DELETE FROM master.landmarkgroupref
	                            WHERE landmark_group_id=@landmark_group_id;";
                var id =  dataAccess.ExecuteScalar<int>(query, parameter);
                                
                
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<int> DeleteGroup(int groupid,int modifiedby)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@groupid", groupid);
                parameter.Add("@state", "D");
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@modified_by", modifiedby);

                string query = @"update master.landmarkgroup set state=@state, modified_at=@modified_at, modified_by = @modified_by
	                              where id=@groupid RETURNING id";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);


                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IEnumerable<LandmarkGroup>> GetlandmarkGroup(int organizationid,int groupid)
        {
            try
            {
                var parameter = new DynamicParameters();


                string query = @"SELECT 
                                    lg.id,
                                    lg.name,
                                    lg.description,
                                    lg.organization_id,
                                    count(case when lgr.type in ('O','C') then 1 end) as geofenceCount, 
                                    count(case when lgr.type in ('P') then 1 end) as poiCount,
                                    lg.created_at,
                                    lg.modified_at
                                    FROM master.landmarkgroup lg                   
                                    LEFT JOIN MASTER.landmarkgroupref lgr on lgr.landmark_group_id = lg.id 
                                    LEFT JOIN MASTER.landmark lm on lm.id = lgr.ref_id
                                    WHERE 1=1 and lg.state in ('A','I') and lm.state in ('A','I') ";    
                                    

                if (organizationid > 0)
                {
                    parameter.Add("@organization_id", organizationid);
                    query = query + " and lg.organization_id=@organization_id";
                }
                if (groupid > 1)
                {
                    parameter.Add("@id", groupid);
                    query = query + " and lg.id=@id";
                }

                query = query + " group by lg.name,lgr.landmark_group_id,lg.organization_id,lg.description,lg.created_at,lg.modified_at,lg.id; ";
                IEnumerable <LandmarkGroup>  groups= await dataAccess.QueryAsync<LandmarkGroup>(query, parameter);


                return groups;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<List<LandmarkgroupRef>> GetlandmarkGroupref(int groupid)
        {
            try
            {
                List<LandmarkgroupRef> landmarkgroupRefs = new List<LandmarkgroupRef>();
                var parameter = new DynamicParameters();
                parameter.Add("@groupid", groupid);
              
                string query = @"SELECT l.id, 
                            i.icon,
                            i.id as iconid,                            
                            c.name as categoryname,                            
                            s.name as subcategoryname,
                            l.name as name,
                            l.address as address,
                            l.type as type,
                            l.state as state
                            FROM master.landmark l
                            LEFT JOIN MASTER.CATEGORY c on l.category_id = c.id
                            LEFT JOIN MASTER.CATEGORY s on l.sub_category_id = s.id
							LEFT JOIN master.icon i on c.icon_id = i.id
                            WHERE 1=1 and l.state in ('A','I') and l.id = any (select ref_id from master.landmarkgroupref where landmark_group_id= @groupid)";

                
                dynamic groups = await dataAccess.QueryAsync<dynamic>(query, parameter);
                foreach (var item in groups)
                {
                    landmarkgroupRefs.Add(this.Mapref(item));
                }

                return landmarkgroupRefs;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public LandmarkgroupRef Mapref(dynamic record)
        {
            LandmarkgroupRef obj = new LandmarkgroupRef();
            obj.landmarkid = record.id;
            //obj.landmark_group_id = record.landmark_group_id;
            //obj.ref_id = record.ref_id;
            obj.type = (LandmarkType)Convert.ToChar(record.type);
            obj.address = record.address;
            obj.categoryname = record.categoryname;
            obj.subcategoryname = record.subcategoryname;
            obj.landmarkname = record.name;
            obj.icon = record.icon;
            return obj;
        }

        public async Task<int> Exists(LandmarkGroup landmarkgroup)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Geofence> groupList = new List<Geofence>();
                var query = @"select id from master.landmarkgroup where 1=1 ";
               
                    if (Convert.ToInt32(landmarkgroup.id) > 0)
                    {
                        parameter.Add("@id", landmarkgroup.id);
                        query = query + " and id!=@id";
                    }
                    // name
                    if (!string.IsNullOrEmpty(landmarkgroup.name))
                    {
                        parameter.Add("@name", landmarkgroup.name);
                        query = query + " and name=@name";
                    }
                    // organization id filter
                    if (landmarkgroup.organization_id > 0)
                    {
                        parameter.Add("@organization_id", landmarkgroup.organization_id);
                        query = query + " and organization_id=@organization_id ";
                    }
                
                var groupid = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
               
                return groupid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
