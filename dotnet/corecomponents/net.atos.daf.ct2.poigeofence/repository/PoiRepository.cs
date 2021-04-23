using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public class PoiRepository : IPoiRepository
    {
        private readonly IDataAccess dataAccess;

        private static readonly log4net.ILog log =
      log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public PoiRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        public async Task<List<POIEntityResponce>> GetAllPOI(POIEntityRequest objPOIEntityRequest)
        {
            log.Info("Subscribe Subscription method called in repository");
            //POIEntityRequest objPOIEntityRequestList = new POIEntityRequest();
            try
            {
                string query = string.Empty;
                query = @"select 
                           l.id
                           ,l.name
                           ,l.latitude
                           ,l.longitude
                           ,c.name as category
                           ,l.city from MASTER.LANDMARK l
                     LEFT JOIN MASTER.CATEGORY c on l.category_id = c.id
                     WHERE 1=1";
                var parameter = new DynamicParameters();
                if (objPOIEntityRequest.organization_id > 0)
                {
                    parameter.Add("@organization_id", objPOIEntityRequest.organization_id);
                    query = $"{query} and l.organization_id=@organization_id ";

                    if (objPOIEntityRequest.category_id >0)
                    {
                        parameter.Add("@category_id", objPOIEntityRequest.category_id);
                        query = $"{query} and l.category_id=@category_id";
                    }

                    else if (objPOIEntityRequest.sub_category_id > 0)
                    {
                        parameter.Add("@sub_category_id", objPOIEntityRequest.sub_category_id);
                        query = $"{query} and l.sub_category_id=@sub_category_id";
                    }
                }
                var data = await _dataAccess.QueryAsync<POIEntityResponce>(query, parameter);
                List<POIEntityResponce> objPOIEntityResponceList = new List<POIEntityResponce>();
                return objPOIEntityResponceList = data.Cast<POIEntityResponce>().ToList();
                //Handel Null Exception
            }
            catch (System.Exception)
            {
            }
            return null;
        }


        #region Geofence

        public async Task<Geofence> CreateGeofence(Geofence geofence)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", geofence.Id);
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
       
        #endregion
    }
}
