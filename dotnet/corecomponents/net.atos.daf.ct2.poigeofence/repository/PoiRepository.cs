using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.ENUM;
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
        public async Task<List<POIEntityResponse>> GetAllGobalPOI(POIEntityRequest objPOIEntityRequest)
        {
            List<POIEntityResponse> objPOIEntityResponceList = new List<POIEntityResponse>();
            try
            {
                string query = string.Empty;
                query = @"select 
                            l.id as GlobalPOIId
                           ,l.name as POIName
                           ,l.latitude
                           ,l.longitude
                           ,c.name as category
						   ,l.category_id as CategoryId
						   ,l.sub_category_id as SubCategoryId
                           ,l.city from MASTER.LANDMARK l
                     LEFT JOIN MASTER.CATEGORY c on l.category_id = c.id
                     WHERE l.organization_id is null";

                var parameter = new DynamicParameters();
                if (objPOIEntityRequest.CategoryId > 0)
                {
                    parameter.Add("@category_id", objPOIEntityRequest.CategoryId);
                    query = $"{query} and l.category_id=@category_id";
                }

                if (objPOIEntityRequest.SubCategoryId > 0)
                {
                    parameter.Add("@sub_category_id", objPOIEntityRequest.SubCategoryId);
                    query = $"{query} and l.sub_category_id=@sub_category_id";
                }

                var data = await dataAccess.QueryAsync<POIEntityResponse>(query, parameter);
                return objPOIEntityResponceList = data.Cast<POIEntityResponse>().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<POI>> GetAllPOI(POI poiFilter)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<POI> pois = new List<POI>();
                string query = string.Empty;
                query = @"SELECT l.id, 
                            l.organization_id,
                            l.category_id,
                            c.name,                            
                            l.sub_category_id, 
                            s.name,
                            l.name,
                            l.address,
                            l.city,
                            l.country,
                            l.zipcode,
                            l.type,
                            l.latitude,
                            l.longitude,
                            l.distance,
                            l.trip_id,
                            l.state,
                            l.created_at,
                            l.created_by,
                            l.modified_at,
                            l.modified_by
                            FROM master.landmark l
                            LEFT JOIN MASTER.CATEGORY c on l.category_id = c.id
                            LEFT JOIN MASTER.CATEGORY s on l.sub_category_id = s.id
                            WHERE 1=1 and l.state in ('A','I') ";

                if (poiFilter.Id > 0)
                {
                    parameter.Add("@id", poiFilter.Id);
                    query = query + " and l.id=@id ";
                }
                if (poiFilter.OrganizationId > 0)
                {
                    parameter.Add("@organization_id", poiFilter.OrganizationId);
                    query = query + " and l.organization_id = @organization_id ";
                }
                if (poiFilter.CategoryId > 0)
                {
                    parameter.Add("@category_id", poiFilter.CategoryId);
                    query = query + " and l.category_id= @category_id ";
                }
                if (poiFilter.SubCategoryId > 0)
                {
                    parameter.Add("@category_id", poiFilter.CategoryId);
                    query = query + " and l.category_id= @category_id ";
                }
                if (!string.IsNullOrEmpty(poiFilter.Name))
                {
                    parameter.Add("@name", poiFilter.Name.ToLower());
                    query = query + " and LOWER(l.name) = @name ";
                }
                if (!string.IsNullOrEmpty(poiFilter.Address))
                {
                    parameter.Add("@address", poiFilter.Address.ToLower());
                    query = query + " and LOWER(l.address) = @address ";
                }
                if (!string.IsNullOrEmpty(poiFilter.City))
                {
                    parameter.Add("@city", poiFilter.City.ToLower());
                    query = query + " and LOWER(l.city) = @city ";
                }
                if (!string.IsNullOrEmpty(poiFilter.Country))
                {
                    parameter.Add("@country", poiFilter.Country.ToLower());
                    query = query + " and LOWER(l.country) = @country ";
                }
                if (!string.IsNullOrEmpty(poiFilter.Zipcode))
                {
                    parameter.Add("@zipcode", poiFilter.Zipcode.ToLower());
                    query = query + " and LOWER(l.zipcode) = @zipcode ";
                }
                if (!string.IsNullOrEmpty(poiFilter.Type) && poiFilter.Type.ToUpper() != "NONE")
                {
                    parameter.Add("@type", MapLandmarkTypeToChar(poiFilter.Type));
                    query = query + " and l.type = @type";
                }
                if (!string.IsNullOrEmpty(poiFilter.State) && poiFilter.State.ToUpper() != "NONE")
                {
                    parameter.Add("@state", MapLandmarkStateToChar(poiFilter.State));
                    query = query + " and l.state = @state";
                }
                if (poiFilter.Latitude > 0)
                {
                    parameter.Add("@latitude", poiFilter.Latitude);
                    query = query + " and l.latitude = @latitude ";
                }
                if (poiFilter.Longitude > 0)
                {
                    parameter.Add("@longitude", poiFilter.Longitude);
                    query = query + " and l.longitude= @longitude ";
                }
                if (poiFilter.TripId > 0)
                {
                    parameter.Add("@trip_id", poiFilter.TripId);
                    query = query + " and l.trip_id= @trip_id ";
                }
                if (poiFilter.CreatedAt > 0)
                {
                    parameter.Add("@created_at", poiFilter.CreatedAt);
                    query = query + " and l.created_at= @created_at ";
                }
                dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);

                foreach (dynamic record in result)
                {

                    pois.Add(Map(record));
                }
                return pois;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<POI> CreatePOI(POI poi)
        {
            try
            {
                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@name", poi.Name);
                var queryduplicate = @"SELECT id FROM master.landmark where name=@name";
                int poiexist = await dataAccess.ExecuteScalarAsync<int>(queryduplicate, parameterduplicate);

                if (poiexist > 0)
                {
                    poi.Id = 0;
                    return poi;
                }

                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", poi.OrganizationId != null ? poi.OrganizationId : 0);
                parameter.Add("@category_id", poi.CategoryId);
                parameter.Add("@sub_category_id", poi.SubCategoryId);
                parameter.Add("@name", poi.Name);
                parameter.Add("@address", poi.Address);
                parameter.Add("@city", poi.City);
                parameter.Add("@country", poi.Country);
                parameter.Add("@zipcode", poi.Zipcode);
                parameter.Add("@type", MapLandmarkTypeToChar(poi.Type));
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
        public async Task<POI> UpdatePOI(POI poi)
        {
            try
            {
                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@name", poi.Name);
                var queryduplicate = @"SELECT id FROM master.landmark where name=@name";
                int poiexist = await dataAccess.ExecuteScalarAsync<int>(queryduplicate, parameterduplicate);

                if (poiexist > 0)
                {
                    poi.Id = -1;// POI is already exist with same name.
                    return poi;
                }
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", poi.OrganizationId != null ? poi.OrganizationId : 0);
                parameter.Add("@category_id", poi.CategoryId);
                parameter.Add("@sub_category_id", poi.SubCategoryId);
                parameter.Add("@name", poi.Name);
                parameter.Add("@address", poi.Address);
                parameter.Add("@city", poi.City);
                parameter.Add("@country", poi.Country);
                parameter.Add("@zipcode", poi.Zipcode);
                parameter.Add("@type", MapLandmarkTypeToChar(poi.Type));
                parameter.Add("@state", Convert.ToChar(poi.State));
                //parameter.Add("@latitude", poi.Latitude);
                //parameter.Add("@longitude", poi.Longitude);
                parameter.Add("@distance", poi.Distance);
                parameter.Add("@trip_id", poi.TripId);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@modified_by", poi.ModifiedBy);

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
		                                //latitude=@latitude, 
		                                //longitude=@longitude, 
		                                distance@distance, 
		                                trip_id=@trip_id, 
		                                state=@state,
                                        modified_at=@modified_at,
                                        modified_by=@modified_by
		                                ) RETURNING id;";

                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (id > 0)
                    poi.Id = id;
                else
                    poi.Id = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return await Task.FromResult(poi);
        }
        public async Task<bool> DeletePOI(int poiId)
        {
            bool result = false;
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", poiId);
                var query = @"update master.landmark set state='D' where id=@id";
                int isdelete = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (isdelete > 0)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public POI Map(dynamic record)
        {
            POI poi = new POI();
            poi.Id = record.id;
            poi.OrganizationId = !string.IsNullOrEmpty(record.Organization_Id) ? record.Organization_Id : 0;
            poi.CategoryId = !string.IsNullOrEmpty(record.category_id) ? record.category_id : 0;
            poi.CategoryName = !string.IsNullOrEmpty(record.categoryname) ? record.categoryname : string.Empty;
            poi.SubCategoryId = !string.IsNullOrEmpty(record.sub_category_id) ? record.sub_category_id : 0;
            poi.SubCategoryName = !string.IsNullOrEmpty(record.subcategoryname) ? record.subcategoryname : string.Empty;
            poi.Name = !string.IsNullOrEmpty(record.name) ? record.name : string.Empty;
            poi.Address = !string.IsNullOrEmpty(record.address) ? record.address : string.Empty;
            poi.City = !string.IsNullOrEmpty(record.city) ? record.city : string.Empty;
            poi.Country = !string.IsNullOrEmpty(record.country) ? record.country : string.Empty;
            poi.Zipcode = !string.IsNullOrEmpty(record.zipcode) ? record.zipcode : string.Empty;
            poi.Type = MapCharToLandmarkState(record.type);
            poi.Latitude = !string.IsNullOrEmpty(record.latitude) ? record.latitude : 0;
            poi.Longitude = !string.IsNullOrEmpty(record.longitude) ? record.longitude : 0;
            poi.Distance = !string.IsNullOrEmpty(record.distance) ? record.distance : 0;
            poi.TripId = !string.IsNullOrEmpty(record.trip_id) ? record.trip_id : 0;
            poi.CreatedAt = !string.IsNullOrEmpty(record.created_at) ? record.created_at : string.Empty;
            poi.State = MapCharToLandmarkState(record.state);
            poi.CreatedBy = !string.IsNullOrEmpty(record.created_by) ? record.created_by : 0;
            poi.ModifiedAt = !string.IsNullOrEmpty(record.modified_at) ? record.modified_at : string.Empty;
            poi.ModifiedBy = !string.IsNullOrEmpty(record.modified_by) ? record.modified_by : 0;
            return poi;
        }
        public string MapCharToLandmarkState(string state)
        {
            string landmarktype = string.Empty;
            switch (state)
            {
                case "A":
                    landmarktype = "Active";
                    break;
                case "I":
                    landmarktype = "Inactive";
                    break;
                case "D":
                    landmarktype = "Delete";
                    break;
            }
            return landmarktype;
        }
        public string MapCharToLandmarkType(string type)
        {
            string ptype = string.Empty;
            switch (type)
            {
                case "N":
                    ptype = "None";
                    break;
                case "P":
                    ptype = "POI";
                    break;
                case "C":
                    ptype = "CircularGeofence";
                    break;
                case "O":
                    ptype = "PolygonGeofence";
                    break;
                case "R":
                    ptype = "Corridor";
                    break;
                case "U":
                    ptype = "Route";
                    break;
            }
            return ptype;
        }
        public char MapLandmarkStateToChar(string state)
        {
            char landmarkState = 'N';
            switch (state)
            {
                case "Active":
                    landmarkState = 'A';
                    break;
                case "Inactive":
                    landmarkState = 'I';
                    break;
                case "Delete":
                    landmarkState = 'D';
                    break;
            }
            return landmarkState;
        }
        public char MapLandmarkTypeToChar(string type)
        {
            char ptype = 'N';
            switch (type)
            {
                case "None":
                    ptype = 'N';
                    break;
                case "POI":
                    ptype = 'P';
                    break;
                case "CircularGeofence":
                    ptype = 'C';
                    break;
                case "PolygonGeofence":
                    ptype = 'O';
                    break;
                case "Corridor":
                    ptype = 'R';
                    break;
                case "Route":
                    ptype = 'U';
                    break;
            }
            return ptype;
        }
    }
}
