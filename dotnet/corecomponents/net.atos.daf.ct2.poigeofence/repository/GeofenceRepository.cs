using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
   
    public class GeofenceRepository :IGeofenceRepository
    {
        private readonly IDataAccess dataAccess;
        private static readonly log4net.ILog log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GeofenceRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        #region Geofence

        public async Task<Geofence> CreatePolygonGeofence(Geofence geofence)
        {
            try
            {
                geofence = await Exists(geofence);

                // duplicate Geofence
                if (geofence.Exists)
                {
                    return geofence;
                }
                var parameter = new DynamicParameters();
                if (geofence.OrganizationId > 0)
                    parameter.Add("@organization_id", geofence.OrganizationId);
                else
                    parameter.Add("@organization_id", null);
                parameter.Add("@category_id", geofence.CategoryId);
                parameter.Add("@sub_category_id", geofence.SubCategoryId);
                parameter.Add("@name", geofence.Name);
                parameter.Add("@address", geofence.Address);
                parameter.Add("@city", geofence.City);
                parameter.Add("@country", geofence.Country);
                parameter.Add("@zipcode", geofence.Zipcode);
                parameter.Add("@type", geofence.Type);
                if (geofence.Nodes.Count() > 0)
                {
                    parameter.Add("@latitude", geofence.Nodes[0].Latitude);
                    parameter.Add("@longitude", geofence.Nodes[0].Longitude);
                }
                parameter.Add("@distance", geofence.Distance);
                parameter.Add("@trip_id", geofence.TripId);
                parameter.Add("@state", 'A');
                parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@created_by", geofence.CreatedBy);

                string query = @"INSERT INTO master.landmark(organization_id, category_id, sub_category_id, name, address, city, country, zipcode, type, latitude, longitude, distance, trip_id, state, created_at, created_by)
	                              VALUES (@organization_id, @category_id, @sub_category_id, @name, @address, @city, @country, @zipcode, @type, @latitude, @longitude, @distance, @trip_id, @state, @created_at, @created_by) RETURNING id";

                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                geofence.Id = id;
                //if (geofence.Type.ToString() == ((char)LandmarkType.PolygonGeofence).ToString())
                //{
                if (geofence.Id > 0)
                {
                    foreach (var item in geofence.Nodes)
                    {
                        var nodeparameter = new DynamicParameters();
                        nodeparameter.Add("@landmark_id", geofence.Id);
                        nodeparameter.Add("@seq_no", item.SeqNo);
                        nodeparameter.Add("@latitude", item.Latitude);
                        nodeparameter.Add("@longitude", item.Longitude);
                        nodeparameter.Add("@state", "A");
                        nodeparameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                        nodeparameter.Add("@created_by", geofence.CreatedBy);
                        string nodeQuery = @"INSERT INTO master.nodes(landmark_id, seq_no, latitude, longitude, state, created_at, created_by)
	                                VALUES (@landmark_id, @seq_no, @latitude, @longitude, @state, @created_at, @created_by) RETURNING id";
                        var nodeId = await dataAccess.ExecuteScalarAsync<int>(nodeQuery, nodeparameter);
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return geofence;
        }

        public async Task<bool> DeleteGeofence(List<int> geofenceIds, int organizationID)
        {
            log.Info("Delete geofenceIds method called in repository");
            try
            {
                if (geofenceIds.Count > 0)
                {
                    foreach (var item in geofenceIds)
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("@organization_id", organizationID);
                        parameter.Add("@id", item);
                        var queryLandmark = @"update master.landmark set state='D' where id=@id and organization_id=@organization_id";
                        await dataAccess.ExecuteScalarAsync<int>(queryLandmark, parameter);

                        var queryNodes = @"update master.nodes set state='D' where landmark_id=@id";
                        await dataAccess.ExecuteScalarAsync<int>(queryNodes, parameter);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                log.Info("Delete geofenceIds method in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }
        }

        public async Task<IEnumerable<GeofenceEntityResponce>> GetAllGeofence(GeofenceEntityRequest geofenceEntityRequest)
        {
            log.Info("Get GetAllGeofence method called in repository");
            GeofenceEntityRequest geofenceEntityRequestList = new GeofenceEntityRequest();
            try
            {
                string query = string.Empty;
                query = @"select L.id geofenceID,
                                 L.name geofenceName, 
                                 case when C.type='P' then C.name end category,
                                 case when C.type='S' then C.name end subCategory 
                                 from master.landmark L
	                             left join master.category C on L.category_id=C.id
	                             where L.state='A'";
                var parameter = new DynamicParameters();
                if (geofenceEntityRequest.organization_id > 0)
                {
                    parameter.Add("@organization_id", geofenceEntityRequest.organization_id);
                    query = $"{query} and L.organization_id=@organization_id ";

                    if (geofenceEntityRequest.category_id > 0)
                    {
                        parameter.Add("@category_id", geofenceEntityRequest.category_id);
                        query = $"{query} and L.category_id=@category_id";
                    }

                    if (geofenceEntityRequest.sub_category_id > 0)
                    {
                        parameter.Add("@sub_category_id", geofenceEntityRequest.sub_category_id);
                        query = $"{query} and L.sub_category_id=@sub_category_id";
                    }
                    return await dataAccess.QueryAsync<GeofenceEntityResponce>(query, parameter);
                }
                //Handel Null Exception
            }
            catch (System.Exception ex)
            {
            }
            return null;
        }

        public async Task<IEnumerable<Geofence>> GetGeofenceByGeofenceID(int organizationId, int geofenceId)
        {
            log.Info("Get GetAllGeofence method called in repository");
            GeofenceEntityRequest geofenceEntityRequestList = new GeofenceEntityRequest();
            try
            {
                string query = string.Empty;
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@Id", geofenceId);
                query = @"select L.id Id,
                                 L.organization_id OrganizationId,
                                 L.name, 
                                 case when C.type='P' then C.name end categoryName,
                                 case when C.type='S' then C.name end subcategoryName,
                                 L.address,
                                 L.city,
                                 L.country,
                                 L.zipcode,
                                 L.latitude,
                                 L.longitude,
                                 L.distance,
                                 L.created_at,
                                 L.created_by,
                                 L.modified_at,
                                 L.modified_by
                                 from master.landmark L
	                             left join master.category C on L.category_id=C.id
	                             where L.id=@Id and L.organization_id=@organization_id and L.state='A'";

                return await dataAccess.QueryAsync<Geofence>(query, parameter);
            }
            catch (System.Exception ex)
            {
                log.Info("GetGeofenceByGeofenceID  method in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }
        }
        public async Task<List<Geofence>> CreateCircularGeofence(List<Geofence> geofence)
        {
            try
            {
                foreach (var item in geofence)
                {
                    var parameter = new DynamicParameters();
                    if (item.OrganizationId > 0)
                        parameter.Add("@organization_id", item.OrganizationId);
                    else
                        parameter.Add("@organization_id", null);
                    parameter.Add("@category_id", item.CategoryId);
                    parameter.Add("@sub_category_id", item.SubCategoryId);
                    parameter.Add("@name", item.Name);
                    parameter.Add("@address", item.Address);
                    parameter.Add("@city", item.City);
                    parameter.Add("@country", item.Country);
                    parameter.Add("@zipcode", item.Zipcode);
                    parameter.Add("@type", item.Type);
                    parameter.Add("@latitude", item.Latitude);
                    parameter.Add("@longitude", item.Longitude);
                    parameter.Add("@distance", item.Distance);
                    parameter.Add("@trip_id", item.TripId);
                    parameter.Add("@state", 'A');
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    parameter.Add("@created_by", item.CreatedBy);

                    string query = @"INSERT INTO master.landmark(organization_id, category_id, sub_category_id, name, address, city, country, zipcode, type, latitude, longitude, distance, trip_id, state, created_at, created_by)
	                              VALUES (@organization_id, @category_id, @sub_category_id, @name, @address, @city, @country, @zipcode, @type, @latitude, @longitude, @distance, @trip_id, @state, @created_at, @created_by) RETURNING id";

                    var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    item.Id = id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return geofence;
        }

        public async Task<Geofence> UpdatePolygonGeofence(Geofence geofence)
        {
            try
            {
                geofence = await Exists(geofence);

                // duplicate Geofence
                if (geofence.Exists)
                {
                    return geofence;
                }

                var parameter = new DynamicParameters();
                parameter.Add("@category_id", geofence.CategoryId);
                parameter.Add("@sub_category_id", geofence.SubCategoryId);
                parameter.Add("@name", geofence.Name);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@modified_by", geofence.ModifiedBy);
                parameter.Add("@id", geofence.Id);
                string query = @"UPDATE master.landmark
	                                SET category_id=@category_id
	                                   ,sub_category_id=@sub_category_id
	                                   ,name=@name
	                                   ,modified_at=@modified_at
	                                   ,modified_by=@modified_by
	                                WHERE id=@id
	                                returning id;";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                geofence.Id = id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return geofence;
        }


        //private async Task<bool> RemoveExistingNodes(int landmarkId)
        //{
        //    try
        //    {
        //        var parameter = new DynamicParameters();
        //        parameter.Add("@id", landmarkId);
        //        var query = @"delete from master.nodes where landmark_id = @id";
        //        var count = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private async Task<Geofence> Exists(Geofence geofenceRequest)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Geofence> groupList = new List<Geofence>();
                var query = @"select id from master.landmark where 1=1 ";
                if (geofenceRequest != null)
                {

                    // id
                    if (Convert.ToInt32(geofenceRequest.Id) > 0)
                    {
                        parameter.Add("@id", geofenceRequest.Id);
                        query = query + " and id!=@id";
                    }
                    // name
                    if (!string.IsNullOrEmpty(geofenceRequest.Name))
                    {
                        parameter.Add("@name", geofenceRequest.Name);
                        query = query + " and name=@name";
                    }
                    // organization id filter
                    if (geofenceRequest.OrganizationId > 0)
                    {
                        parameter.Add("@organization_id", geofenceRequest.OrganizationId);
                        query = query + " and organization_id=@organization_id ";
                    }
                }
                var groupid = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (groupid > 0)
                {
                    geofenceRequest.Exists = true;
                    geofenceRequest.Id = groupid;
                }
                return geofenceRequest;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }


}
