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

        public async Task<Geofence> CreateGeofence(Geofence geofence)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", geofence.OrganizationId);
                parameter.Add("@category_id", geofence.CategoryId);
                parameter.Add("@sub_category_id", geofence.SubCategoryId);
                parameter.Add("@name", geofence.Name);
                parameter.Add("@address", geofence.Address);
                parameter.Add("@city", geofence.City);
                parameter.Add("@country", geofence.Country);
                parameter.Add("@zipcode", geofence.Zipcode);
                parameter.Add("@type", geofence.Type);
                parameter.Add("@latitude", geofence.Latitude);
                parameter.Add("@longitude", geofence.Longitude);
                parameter.Add("@distance", geofence.Distance);
                parameter.Add("@trip_id", geofence.TripId);
                parameter.Add("@state", 'A');
                parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@created_by", geofence.CreatedBy);

                string query = @"INSERT INTO master.landmark(organization_id, category_id, sub_category_id, name, address, city, country, zipcode, type, latitude, longitude, distance, trip_id, state, created_at, created_by)
	                              VALUES (@organization_id, @category_id, @sub_category_id, @name, @address, @city, @country, @zipcode, @type, @latitude, @longitude, @distance, @trip_id, @state, @created_at, @created_by) RETURNING id";

                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                geofence.Id = id;
                if (geofence.Type.ToString() == LandmarkType.PolygonGeofence.ToString())
                {
                    if (geofence.Id > 0)
                    {
                        foreach (var item in geofence.Nodes)
                        {
                            var nodeparameter = new DynamicParameters();
                            nodeparameter.Add("@landmark_id", item.LandmarkId);
                            nodeparameter.Add("@seq_no", item.SeqNo);
                            nodeparameter.Add("@latitude", item.Latitude);
                            nodeparameter.Add("@longitude", item.Longitude);
                            nodeparameter.Add("@state", "A");
                            nodeparameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                            nodeparameter.Add("@created_by", item.CreatedBy);
                            string nodeQuery = @"INSERT INTO master.nodes(landmark_id, seq_no, latitude, longitude, state, created_at, created_by)
	                                VALUES (@landmark_id, @seq_no, @latitude, @longitude, @state, @created_at, @created_by) RETURNING id";
                            var nodeId = await dataAccess.ExecuteScalarAsync<int>(nodeQuery, nodeparameter);
                        }
                    }
                }
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
                query = @"select L.id,
                                 L.name, 
                                 case when C.type='p' then C.name end category,
                                 case when C.type='s' then C.name end subcategory 
                                 from master.landmark L
	                             left join master.category C on L.category_id=C.id
	                             where 1=1 and state='A'";
                var parameter = new DynamicParameters();
                if (geofenceEntityRequest.organization_id > 0)
                {
                    parameter.Add("@organization_id", geofenceEntityRequest.organization_id);
                    query = $"{query} and l.organization_id=@organization_id ";

                    if (geofenceEntityRequest.category_id > 0)
                    {
                        parameter.Add("@category_id", geofenceEntityRequest.category_id);
                        query = $"{query} and l.category_id=@category_id";
                    }

                    if (geofenceEntityRequest.sub_category_id > 0)
                    {
                        parameter.Add("@sub_category_id", geofenceEntityRequest.sub_category_id);
                        query = $"{query} and l.sub_category_id=@sub_category_id";
                    }
                    return await dataAccess.QueryAsync<GeofenceEntityResponce>(query, parameter);
                }
                //Handel Null Exception
            }
            catch (System.Exception)
            {
            }
            return null;
        }
        #endregion
    }


}
