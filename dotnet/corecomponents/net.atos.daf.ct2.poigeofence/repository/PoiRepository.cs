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
                            l.organization_id as organizationid,
                            l.category_id as categoryid,
                            c.name as categoryname,                            
                            l.sub_category_id as subcategoryid, 
                            s.name as subcategoryname,
                            l.name as name,
                            l.address as address,
                            l.city as city,
                            l.country as country,
                            l.zipcode as zipcode,
                            l.type as type,
                            l.latitude as latitude,
                            l.longitude as longitude,
                            l.distance as distance,
                            l.trip_id as tripid,
                            l.state as state,
                            l.created_at as createdat,
                            l.created_by as createdby,
                            l.modified_at as modifiedat,
                            l.modified_by as modifiedby
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
                if (string.IsNullOrEmpty(poiFilter.State) || poiFilter.State.ToUpper() == "NONE")
                {
                    //parameter.Add("@state", MapLandmarkStateToChar(poiFilter.State));
                    query = query + " and l.state in ('A','I')";
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
                string queryduplicate = string.Empty;
                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@name", poi.Name);

                if (poi.OrganizationId > 0)
                {
                    parameterduplicate.Add("@organization_id", poi.OrganizationId);
                    queryduplicate = @"SELECT id FROM master.landmark where state in ('A','I')  and type = 'P' and name=@name and organization_id=@organization_id;";
                }
                else
                    queryduplicate = @"SELECT id FROM master.landmark where state in ('A','I')  and type = 'P' and name=@name;";
                
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
                string queryduplicate = string.Empty;
                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@name", poi.Name);
                if (poi.OrganizationId > 0)
                {
                    parameterduplicate.Add("@organization_id", poi.OrganizationId);
                    queryduplicate = @"SELECT id FROM master.landmark where state in ('A','I') and type = 'P' and name=@name and id <> @id and organization_id=@organization_id;";
                }
                else
                    queryduplicate = @"SELECT id FROM master.landmark where state in ('A','I') and type = 'P' and name=@name and id <> @id;";

                int poiexist = await dataAccess.ExecuteScalarAsync<int>(queryduplicate, parameterduplicate);

                if (poiexist > 0)
                {
                    poi.Id = -1;// POI is already exist with same name.
                    return poi;
                }

                var parameter = new DynamicParameters();
                string query = @"Update master.landmark
                                SET organization_id=@organization_id ";

                if (poi.CategoryId > 0)
                {
                    parameter.Add("@category_id", poi.CategoryId);
                    query = query + ", category_id=@category_id ";
                }
                if (poi.SubCategoryId > 0)
                {
                    parameter.Add("@sub_category_id", poi.SubCategoryId);
                    query = query + ", sub_category_id=@sub_category_id ";
                }
                if (!string.IsNullOrEmpty(poi.Name))
                {
                    parameter.Add("@name", poi.Name);
                    query = query + ",name=@name ";
                }
                if (!string.IsNullOrEmpty(poi.Address))
                {
                    parameter.Add("@address", poi.Address);
                    query = query + ", address=@address ";
                }
                if (!string.IsNullOrEmpty(poi.City))
                {
                    parameter.Add("@city", poi.City);
                    query = query + ", city=@city ";
                }
                if (!string.IsNullOrEmpty(poi.Country))
                {
                    parameter.Add("@country", poi.Country);
                    query = query + ", country=@country ";
                }
                if (!string.IsNullOrEmpty(poi.Zipcode))
                {
                    parameter.Add("@zipcode", poi.Zipcode);
                    query = query + ", zipcode=@zipcode ";
                }
                if (!string.IsNullOrEmpty(poi.Type) && poi.Type.ToUpper() != "NONE")
                {
                    parameter.Add("@type", MapLandmarkTypeToChar(poi.Type));
                    query = query + ", type=@type ";
                }
                if (!string.IsNullOrEmpty(poi.State) && poi.State.ToUpper() != "NONE")
                {
                    parameter.Add("@state", MapLandmarkStateToChar(poi.State));
                    query = query + ", state=@state ";
                }
                //if (poi.Latitude > 0)
                //{
                //    parameter.Add("@latitude", poi.Latitude);
                //    query = query + ", l.latitude = @latitude ";
                //}
                //if (poi.Longitude > 0)
                //{
                //    parameter.Add("@longitude", poi.Longitude);
                //    query = query + ", l.longitude= @longitude ";
                //}
                //if (poi.TripId > 0)
                //{
                //    parameter.Add("@trip_id", poi.TripId);
                //    query = query + ", l.trip_id= @trip_id ";
                //}
                if (poi.ModifiedBy > 0)
                {
                    parameter.Add("@modified_by", poi.ModifiedBy);
                    query = query + ", modified_by=@modified_by ";
                }
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                query = query + ", modified_at=@modified_at ";

                parameter.Add("@id", poi.Id);
                query = query + " where id=@id and type = 'P' RETURNING id";

                parameter.Add("@organization_id", poi.OrganizationId);

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
                var query = @"update master.landmark set state='D' where id=@id and type = 'P' RETURNING id";
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
            poi.OrganizationId = record.organizationid != null ? record.organizationid : 0;
            poi.CategoryId = record.categoryid != null ? record.categoryid : 0;
            poi.CategoryName = !string.IsNullOrEmpty(record.categoryname) ? record.categoryname : string.Empty;
            poi.SubCategoryId = record.subcategoryid != null ? record.subcategoryid : 0;
            poi.SubCategoryName = !string.IsNullOrEmpty(record.subcategoryname) ? record.subcategoryname : string.Empty;
            poi.Name = !string.IsNullOrEmpty(record.name) ? record.name : string.Empty;
            poi.Address = !string.IsNullOrEmpty(record.address) ? record.address : string.Empty;
            poi.City = !string.IsNullOrEmpty(record.city) ? record.city : string.Empty;
            poi.Country = !string.IsNullOrEmpty(record.country) ? record.country : string.Empty;
            poi.Zipcode = !string.IsNullOrEmpty(record.zipcode) ? record.zipcode : string.Empty;
            poi.Type = MapCharToLandmarkState(record.type);
            poi.Latitude = Convert.ToDouble(record.latitude);
            poi.Longitude = Convert.ToDouble(record.longitude);
            poi.Distance = Convert.ToDouble(record.distance);
            poi.TripId = record.tripid != null ? record.tripid : 0;
            poi.CreatedAt = record.createdat != null ? record.createdat : 0;
            poi.State = MapCharToLandmarkState(record.state);
            poi.CreatedBy = record.createdby != null ? record.createdby : 0;
            poi.ModifiedAt = record.modifiedat != null ? record.modifiedat : 0;
            poi.ModifiedBy = record.modifiedby != null ? record.modifiedby : 0;
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
