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

        public PoiRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        public async Task<List<POIEntityResponce>> GetAllPOI(POIEntityRequest objPOIEntityRequest)
        {
            
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
                var data = await dataAccess.QueryAsync<POIEntityResponce>(query, parameter);
                List<POIEntityResponce> objPOIEntityResponceList = new List<POIEntityResponce>();
                return objPOIEntityResponceList = data.Cast<POIEntityResponce>().ToList();
                //Handel Null Exception
            }
            catch (System.Exception)
            {
            }
            return null;
        }

        public async Task<POI> CreatePOI(POI poi)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", poi.Id);
                parameter.Add("@category_id", poi.CategoryId);
                parameter.Add("@sub_category_id", poi.SubCategoryId);
                parameter.Add("@name", poi.Name);
                parameter.Add("@address", poi.Address);
                parameter.Add("@city", poi.City);
                parameter.Add("@country", poi.Country);
                parameter.Add("@zipcode", poi.Zipcode);
                parameter.Add("@type", poi.Type);
                parameter.Add("@latitude", poi.Latitude);
                parameter.Add("@longitude", poi.Longitude);
                parameter.Add("@distance", poi.Distance);
                parameter.Add("@trip_id", poi.TripId);
                parameter.Add("@state", 'A');
                parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@created_by", poi.CreatedBy);

                string query = @"INSERT INTO master.landmark(organization_id, category_id, sub_category_id, name, address, city, country, zipcode, type, latitude, longitude, distance, trip_id, state, created_at, created_by)
	                              VALUES (@organization_id, @category_id, @sub_category_id, @name, @address, @city, @country, @zipcode, @type, @latitude, @longitude, @distance, @trip_id, @state, @created_at, @created_by) RETURNING id";

                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                poi.Id = id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return poi;
        }


        public Task<List<POI>> GetAllPOI(POI poi)
        {
            throw new NotImplementedException();
        }
        
        public async Task<bool> UpdatePOI(POI poi)
        {
            bool result = false;
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", poi.Id);
                parameter.Add("@category_id", poi.CategoryId);
                parameter.Add("@sub_category_id", poi.SubCategoryId);
                parameter.Add("@name", poi.Name);
                parameter.Add("@address", poi.Address);
                parameter.Add("@city", poi.City);
                parameter.Add("@country", poi.Country);
                parameter.Add("@zipcode", poi.Zipcode);
                parameter.Add("@type", poi.Type);
                parameter.Add("@latitude", poi.Latitude);
                parameter.Add("@longitude", poi.Longitude);
                parameter.Add("@distance", poi.Distance);
                parameter.Add("@trip_id", poi.TripId);
                parameter.Add("@state", 'U');

                string query = @"Update master.landmark
                                SET 	organization_id=@organization_id,
		                                category_id=@category_id,
		                                sub_category_id=@sub_category_id, 
		                                name=@name, 
		                                address=@address,
		                                city=@city, 
		                                country=@country, 
		                                zipcode=@zipcode, 
		                                [type]=@type, 
		                                latitude=@latitude, 
		                                longitude=@longitude, 
		                                distance@distance, 
		                                trip_id=@trip_id, 
		                                state=@state
		                                ) RETURNING id;";

                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (id > 0)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return await Task.FromResult(result);
        }

        public Task<bool> DeletePOI(int poiId)
        {
            throw new NotImplementedException();
        }

     
    }
}
