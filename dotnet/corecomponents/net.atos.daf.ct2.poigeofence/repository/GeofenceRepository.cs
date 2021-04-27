using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.ENUM;
using net.atos.daf.ct2.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence.repository
{

    public class GeofenceRepository : IGeofenceRepository
    {
        private readonly IDataAccess dataAccess;
        private readonly ICategoryRepository _categoryRepository;
        private static readonly log4net.ILog log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GeofenceRepository(IDataAccess _dataAccess, ICategoryRepository categoryRepository)
        {
            dataAccess = _dataAccess;
            _categoryRepository = categoryRepository;
        }

        #region Geofence

        public async Task<Geofence> CreatePolygonGeofence(Geofence geofence, bool IsBulkImport = false)
        {
            int categoryId = 0;
            try
            {
                geofence = await Exists(geofence, ((char)LandmarkType.PolygonGeofence).ToString());

                // duplicate Geofence
                if (geofence.Exists)
                {
                    if (IsBulkImport && geofence.CategoryId > 0)
                    {
                        return await UpdateGeofenceForBulkImport(geofence, ((char)LandmarkType.PolygonGeofence).ToString());
                    }
                    else
                    {
                        geofence.IsFailed = true;
                        geofence.Message = "Polygon Geofence already exist in database.";
                    }
                    return geofence;
                }

                if (IsBulkImport)
                {
                    var OrganizationId = geofence.OrganizationId;
                    if (OrganizationId > 0)
                    {
                        categoryId = await GetCategoryId(OrganizationId, categoryId);
                        if (categoryId == 0)
                        {
                            geofence.IsFailed = true;
                            geofence.Message = "Category Id is not available.";
                            return geofence;
                        }
                    }
                    else
                    {
                        geofence.IsFailed = true;
                        geofence.Message = "Organization Id is not available.";
                        return geofence;
                    }
                }
                var parameter = new DynamicParameters();
                if (geofence.OrganizationId > 0)
                    parameter.Add("@organization_id", geofence.OrganizationId);
                else
                    parameter.Add("@organization_id", null);

                if (geofence.SubCategoryId > 0)
                    parameter.Add("@sub_category_id", geofence.SubCategoryId);
                else
                    parameter.Add("@sub_category_id", null);

                parameter.Add("@category_id", geofence.CategoryId > 0 ? geofence.CategoryId : categoryId);
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
                geofence.IsFailed = false;
                geofence.IsAdded = true;
                if (geofence.Id > 0)
                {
                    foreach (var item in geofence.Nodes)
                    {
                        try
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
                            item.Id = nodeId;
                            item.IsFailed = false;
                            item.IsAdded = true;
                        }
                        catch (Exception ex)
                        {
                            item.IsFailed = false;
                            item.Message = $"There was an error inserting data :- {ex.Message}";
                        }
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                geofence.IsFailed = false;
                geofence.Message = $"There was an error inserting data :- {ex.Message}";
            }
            return geofence;
        }

        public async Task<bool> DeleteGeofence(GeofenceDeleteEntity objGeofenceDeleteEntity)
        {
            log.Info("Delete geofenceIds method called in repository");
            try
            {
                if (objGeofenceDeleteEntity.GeofenceId.Count > 0)
                {
                    foreach (var item in objGeofenceDeleteEntity.GeofenceId)
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("@id", item);
                        var queryLandmark = @"update master.landmark set state='D' where id=@id";
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
                // throw ex;
                return false;
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
                }
                else
                {
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
                }  
                
                return await dataAccess.QueryAsync<GeofenceEntityResponce>(query, parameter);
            }
            catch (System.Exception ex)
            {
                log.Info("GetAllGeofence  method in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }
        }

        public async Task<IEnumerable<Geofence>> GetGeofenceByGeofenceID(int organizationId, int geofenceId)
        {
            log.Info("Get GetAllGeofence method called in repository");
            GeofenceEntityRequest geofenceEntityRequestList = new GeofenceEntityRequest();
            try
            {
                string query = string.Empty;
                var parameter = new DynamicParameters();
                parameter.Add("@Id", geofenceId);
                query = @"select L.id Id,
                                 L.organization_id OrganizationId,
                                 L.name, 
                                 case when C.type='P' then C.name end categoryName,
                                 case when C.type='S' then C.name end subcategoryName,
                                 L.state State,
                                 L.address,
                                 L.city,
                                 L.country,
                                 L.zipcode,
                                 L.latitude,
                                 L.longitude,
                                 L.distance Distance,
                                 L.created_at CreatedAt,
                                 L.created_by CreatedBy,
                                 L.modified_at ModifiedAt,
                                 L.modified_by ModifiedBy
                                 from master.landmark L
	                             left join master.category C on L.category_id=C.id
	                             where L.id=@Id and L.state='A'";

                if (organizationId>0)
                {
                    parameter.Add("@organization_id", organizationId);
                    query = $"{query} and L.organization_id=@organization_id ";
                }                
                return await dataAccess.QueryAsync<Geofence>(query, parameter);
            }
            catch (System.Exception ex)
            {
                log.Info("GetGeofenceByGeofenceID  method in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }
        }
        public async Task<List<Geofence>> CreateCircularGeofence(List<Geofence> geofences, bool IsBulkImport = false)
        {
            int categoryId = 0;
            try
            {
                Geofence geofence1 = new Geofence();
                geofence1 = await Exists(geofences.FirstOrDefault(), ((char)LandmarkType.CircularGeofence).ToString());
                // duplicate Geofence
                if (geofence1.Exists)
                {
                    if (IsBulkImport)
                    {
                        foreach (var geofence in geofences)
                        {
                            if (geofence.CategoryId > 0)
                                await UpdateGeofenceForBulkImport(geofence, ((char)LandmarkType.CircularGeofence).ToString());
                            else
                            {
                                geofence.IsFailed = true;
                                geofence.Message = "Circular Geofence already exist in database.";
                            }
                        }
                    }
                    return geofences;
                }
                if (IsBulkImport)
                {
                    var OrganizationId = geofences.Where(w => w.OrganizationId > 0).FirstOrDefault().OrganizationId;
                    if (OrganizationId > 0)
                    {
                        categoryId = await GetCategoryId(OrganizationId, categoryId);
                        if (categoryId == 0)
                        {
                            foreach (var geofence in geofences)
                            {
                                geofence.IsFailed = true;
                                geofence.Message = "Category Id is not available.";
                            }
                            return geofences;
                        }
                    }
                    else
                    {
                        foreach (var geofence in geofences)
                        {
                            geofence.IsFailed = true;
                            geofence.Message = "Organization Id is not available.";
                        }
                        return geofences;
                    }
                }
                foreach (var item in geofences)
                {
                    try
                    {
                        var parameter = new DynamicParameters();

                        parameter.Add("@category_id", item.CategoryId > 0 ? item.CategoryId : categoryId);
                        if (item.OrganizationId > 0)
                            parameter.Add("@organization_id", item.OrganizationId);
                        else
                            parameter.Add("@organization_id", null);

                        if (item.SubCategoryId > 0)
                            parameter.Add("@sub_category_id", item.SubCategoryId);
                        else
                            parameter.Add("@sub_category_id", null);

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
                        item.IsFailed = false;
                        item.IsAdded = true;
                    }
                    catch (Exception ex)
                    {
                        item.IsFailed = true;
                        item.Message = $"There was an error inserting data :- {ex.Message}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return geofences;
        }

        public async Task<Geofence> UpdatePolygonGeofence(Geofence geofence)
        {
            try
            {
                geofence = await Exists(geofence, ((char)LandmarkType.PolygonGeofence).ToString());

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
                                    and state='A'
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

        public async Task<List<Geofence>> BulkImportGeofence(List<Geofence> geofences)
        {
            var geofenceList = new List<Geofence>();
            try
            {
                foreach (var name in geofences.Where(w => w.Type == "C" || w.Type == "c").Select(s => s.Name).Distinct())
                    geofenceList.AddRange(await CreateCircularGeofence(geofences.Where(w => (w.Name == name) && (w.Type == "C" || w.Type == "c")).ToList(), true));

                foreach (var item in geofences.Where(w => w.Type == "O" || w.Type == "o"))
                    geofenceList.Add(await CreatePolygonGeofence(item, true));


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return geofenceList;
        }

        private async Task<int> GetCategoryId(int OrganizationId, int categoryId)
        {
            var categoryList = await _categoryRepository.GetCategory(new CategoryFilter { OrganizationId = OrganizationId, Type = ((char)CategoryType.Category).ToString() });
            categoryId = categoryList.FirstOrDefault()?.Id ?? 0;
            if (!(categoryId > 0))
            {
                var taskId = await _categoryRepository.AddCategory(new Category { Name = "BulkImport", Organization_Id = OrganizationId, Type = ((char)CategoryType.Category).ToString(), State = ((char)CategoryState.Active).ToString() });
                return taskId.Id;
            }

            return categoryId;
        }

        private async Task<Geofence> Exists(Geofence geofenceRequest, string type)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Geofence> groupList = new List<Geofence>();
                var query = @"select id from master.landmark where type=@type and 1=1";
                parameter.Add("@type", type);
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

        public async Task<Geofence> UpdateCircularGeofence(Geofence geofence)
        {
            try
            {
                geofence = await Exists(geofence, ((char)LandmarkType.CircularGeofence).ToString());

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
                                    and state='A'
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

        public async Task<Geofence> UpdateGeofenceForBulkImport(Geofence geofence, string type)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@category_id", geofence.CategoryId);
                if (geofence.SubCategoryId > 0)
                    parameter.Add("@sub_category_id", geofence.SubCategoryId);
                else
                    parameter.Add("@sub_category_id", null);
                parameter.Add("@name", geofence.Name);
                if (geofence.OrganizationId > 0)
                    parameter.Add("@organization_id", geofence.OrganizationId);
                else
                    parameter.Add("@organization_id", null);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@modified_by", geofence.ModifiedBy);
                parameter.Add("@type", type);
                string query = @"UPDATE master.landmark
	                                SET category_id=@category_id
	                                   ,sub_category_id=@sub_category_id
	                                   ,modified_at=@modified_at
	                                   ,modified_by=@modified_by
	                                WHERE State = 'A' and type=@type and name=@name and organization_id = @organization_id
	                                returning id;";
                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                geofence.Id = id;
                geofence.IsFailed = false;
                geofence.IsAdded = false;
            }
            catch (Exception ex)
            {
                geofence.IsFailed = true;
                geofence.Message = $"There was an error updateing landmark :- {ex.Message}";
            }
            return geofence;
        }

        #endregion
    }


}
