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
                parameter.Add("@width", geofence.Width);
                parameter.Add("@state", 'A');
                parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@created_by", geofence.CreatedBy);

                string query = @"INSERT INTO master.landmark(organization_id, category_id, sub_category_id, name, address, city, country, zipcode, type, latitude, longitude, distance, width, state, created_at, created_by)
	                              VALUES (@organization_id, @category_id, @sub_category_id, @name, @address, @city, @country, @zipcode, @type, @latitude, @longitude, @distance, @width, @state, @created_at, @created_by) RETURNING id";

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
                            nodeparameter.Add("@address", string.IsNullOrEmpty(item.Address) ? null : item.Address);
                            nodeparameter.Add("@trip_id", string.IsNullOrEmpty(item.TripId) ? null : item.TripId);
                            string nodeQuery = @"INSERT INTO master.nodes(landmark_id, seq_no, latitude, longitude, state, created_at, created_by,address,trip_id)
	                                VALUES (@landmark_id, @seq_no, @latitude, @longitude, @state, @created_at, @created_by,@address,@trip_id) RETURNING id";
                            var nodeId = await dataAccess.ExecuteScalarAsync<int>(nodeQuery, nodeparameter);
                            item.Id = nodeId;
                            item.IsFailed = false;
                            item.IsAdded = true;
                        }
                        catch (Exception ex)
                        {
                            log.Info("Creating Polygon geofence method for Node in repository failed for Name {geofence.Name}:");
                            log.Error(ex.ToString());
                            item.IsFailed = true;
                            item.Message = $"There was an error inserting node data";
                            if (!IsBulkImport) throw ex;
                        }
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                log.Info("Creating Polygon geofence method in repository failed for Name {geofence.Name}:");
                log.Error(ex.ToString());
                geofence.IsFailed = true;
                geofence.Message = $"There was an error inserting polygon data";
                if (!IsBulkImport) throw ex;
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
                                 L.name geofenceName, L.type ,L.category_id categoryID,L.sub_category_id subcategoryId,
                                 case when C.type='P' then C.name end category,
                                 case when C.type='S' then C.name end subCategory                                
                                 from master.landmark L
	                             left join master.category C on L.category_id=C.id
	                             where L.state='A'";
                var parameter = new DynamicParameters();
                if (geofenceEntityRequest.organization_id > 0)
                {
                    //It will return organization specific geofence along with global geofence 
                    parameter.Add("@organization_id", geofenceEntityRequest.organization_id);
                    query = $"{query} and (L.organization_id=@organization_id or L.organization_id is null)";
                }
                else
                {
                    //only return global geofence 
                    query = $"{query} and L.organization_id is null ";
                }
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
                                 case when L.type='O' then 'PolygonGeofence'
                                  when L.type='C' then 'CircularGeofence'
                                  when L.type='P' then 'POI'
                                  when L.type='R' then 'Corridor'
                                  when L.type='U' then 'Route'
                                  end as type,
                                 L.category_id CategoryId,                                
                                 L.sub_category_id SubCategoryId,
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

                if (organizationId > 0)
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
                            geofence.Exists = true;
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
                    var OrganizationId = geofences.Where(w => w.OrganizationId > 0).FirstOrDefault()?.OrganizationId ?? 0;
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
                        if (IsBulkImport && !(item.Distance > 0))
                        {
                            item.IsFailed = true;
                            item.Message = $"Please enter a radius bigger than zero.";
                            continue;
                        }

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
                        parameter.Add("@width", item.Width);
                        parameter.Add("@state", 'A');
                        parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                        parameter.Add("@created_by", item.CreatedBy);

                        string query = @"INSERT INTO master.landmark(organization_id, category_id, sub_category_id, name, address, city, country, zipcode, type, latitude, longitude, distance, width, state, created_at, created_by)
	                              VALUES (@organization_id, @category_id, @sub_category_id, @name, @address, @city, @country, @zipcode, @type, @latitude, @longitude, @distance, @width, @state, @created_at, @created_by) RETURNING id";

                        var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                        item.Id = id;
                        item.IsFailed = false;
                        item.IsAdded = true;
                    }
                    catch (Exception ex)
                    {
                        log.Info($"Creating Circuler geofence method in repository failed Name {item.Name}:");
                        log.Error(ex.ToString());
                        item.IsFailed = true;
                        item.Message = $"There was an error inserting Circuler geofence data.";
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
            var categoryList = await _categoryRepository.GetCategory(new CategoryFilter { OrganizationId = OrganizationId, Type = ((char)CategoryType.Category).ToString(), CategoryName = "BulkImportGeofence" });
            categoryId = categoryList.FirstOrDefault()?.Id ?? 0;
            if (!(categoryId > 0))
            {
                var taskId = await _categoryRepository.AddCategory(new Category { Name = "BulkImportGeofence", Organization_Id = OrganizationId, Type = ((char)CategoryType.Category).ToString(), State = ((char)CategoryState.Active).ToString() });
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
                parameter.Add("@distance", geofence.Distance);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@modified_by", geofence.ModifiedBy);
                parameter.Add("@id", geofence.Id);
                string query = @"UPDATE master.landmark
	                                SET category_id=@category_id
	                                   ,sub_category_id=@sub_category_id
	                                   ,name=@name
                                       ,distance=@distance
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
                if (geofence.OrganizationId <= 0)
                {
                    geofence.IsFailed = true;
                    geofence.Message = "Organization Id is not available.";
                    return geofence;
                }
                var categoryList = await _categoryRepository.GetCategory(new CategoryFilter { State = "A", CategoryID = geofence.CategoryId });
                var categoryId = categoryList.FirstOrDefault()?.Id ?? 0;
                if (!(categoryId > 0))
                {
                    geofence.IsFailed = true;
                    geofence.Message = $"Cannot Update, As Category is not active.";
                    return geofence;
                }

                var parameter = new DynamicParameters();
                parameter.Add("@category_id", geofence.CategoryId);
                if (geofence.SubCategoryId > 0)
                    parameter.Add("@sub_category_id", geofence.SubCategoryId);
                else
                    parameter.Add("@sub_category_id", null);
                parameter.Add("@name", geofence.Name);
                parameter.Add("@organization_id", geofence.OrganizationId);
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
                if (id > 0)
                {
                    geofence.Id = id;
                    geofence.IsFailed = false;
                    geofence.IsAdded = false;
                }
                else
                {
                    geofence.IsFailed = true;
                    geofence.Message = $"Geofence is not active.";
                }
            }
            catch (Exception ex)
            {
                log.Info($"Updating geofence for Bulk import method in repository failed Name {geofence.Name}:");
                log.Error(ex.ToString());
                geofence.IsFailed = true;
                geofence.Message = $"There was an error updating geofence data.";
            }
            return geofence;
        }
        public async Task<IEnumerable<Geofence>> GetAllGeofence(Geofence geofenceFilter)
        {
            GeofenceEntityRequest geofenceEntityRequestList = new GeofenceEntityRequest();
            try
            {
                string query = string.Empty;
                var parameter = new DynamicParameters();
                query = @" select l.id Id,
                        l.organization_id OrganizationId,
                        l.Name, 
                        c.name CategoryName,
                        s.name SubcategoryName,
                        l.type as Type,
                        l.category_id CategoryId,                                
                        l.sub_category_id SubCategoryId,
                        l.state State,
                        l.address Address ,
                        l.city City,
                        l.country Country,
                        l.zipcode Zipcode,
                        l.latitude Latitude,
                        l.longitude Logitude,
                        l.distance Distance,
                        l.created_at CreatedAt,
                        l.created_by CreatedBy,
                        l.modified_at ModifiedAt,
                        l.modified_by ModifiedBy,
                        n.id as NodeId, 
                        n.landmark_id as LandmarkId, 
                        n.seq_no as SeqNo , 
                        n.latitude NodeLatitude, 
                        n.longitude NodeLongitude , 
                        n.state as NodeState, 
                        n.created_at as NodeCreatedAt, 
                        n.created_by as NodeCreatedBy , 
                        n.modified_at as NodeModifiedAt, 
                        n.modified_by as NodeModifiedBy
                        from master.landmark l
                        LEFT JOIN MASTER.CATEGORY c on l.category_id = c.id  and l.state <>'D' and c.state <>'D'
                        LEFT JOIN MASTER.CATEGORY s on l.sub_category_id = s.id and l.state <>'D' and s.state <>'D'
                        Left JOIN MASTER.NODES n on l.id=n.landmark_id and l.state <>'D' and n.state <>'D'
                        where 1=1  and l.type in ('C','O') ";

                if (geofenceFilter.Id > 0)
                {
                    parameter.Add("@id", geofenceFilter.Id);
                    query = query + " and l.id=@id ";
                }
                if (geofenceFilter.OrganizationId > 0)
                {
                    //It will return organization specific geofence along with global geofence 
                    parameter.Add("@organization_id", geofenceFilter.OrganizationId);
                    query = query + " and (l.organization_id = @organization_id or l.organization_id is null) ";
                }
                else
                {
                    //only return global geofence 
                    query = query + "  and l.organization_id is null ";
                }
                if (geofenceFilter.CategoryId > 0)
                {
                    parameter.Add("@category_id", geofenceFilter.CategoryId);
                    query = query + " and l.category_id= @category_id ";
                }
                if (geofenceFilter.SubCategoryId > 0)
                {
                    parameter.Add("@sub_category_id", geofenceFilter.SubCategoryId);
                    query = query + " and l.sub_category_id= @sub_category_id ";
                }
                if (!string.IsNullOrEmpty(geofenceFilter.Name))
                {
                    parameter.Add("@name", geofenceFilter.Name.ToLower());
                    query = query + " and LOWER(l.name) = @name ";
                }
                if (!string.IsNullOrEmpty(geofenceFilter.Address))
                {
                    parameter.Add("@address", geofenceFilter.Address.ToLower());
                    query = query + " and LOWER(l.address) = @address ";
                }
                if (!string.IsNullOrEmpty(geofenceFilter.City))
                {
                    parameter.Add("@city", geofenceFilter.City.ToLower());
                    query = query + " and LOWER(l.city) = @city ";
                }
                if (!string.IsNullOrEmpty(geofenceFilter.Country))
                {
                    parameter.Add("@country", geofenceFilter.Country.ToLower());
                    query = query + " and LOWER(l.country) = @country ";
                }
                if (!string.IsNullOrEmpty(geofenceFilter.Zipcode))
                {
                    parameter.Add("@zipcode", geofenceFilter.Zipcode.ToLower());
                    query = query + " and LOWER(l.zipcode) = @zipcode ";
                }
                if (!string.IsNullOrEmpty(geofenceFilter.Type) && geofenceFilter.Type.ToUpper() != "NONE" && geofenceFilter.Type.Length == 1)
                {
                    parameter.Add("@type", Convert.ToChar(geofenceFilter.Type));
                    query = query + " and l.type = @type ";
                }
                if (!string.IsNullOrEmpty(geofenceFilter.State) && geofenceFilter.State.ToUpper() != "NONE" && geofenceFilter.State.Length == 1)
                {
                    parameter.Add("@state", geofenceFilter.State);
                    query = query + " and l.state = @state ";
                }
                if (string.IsNullOrEmpty(geofenceFilter.State) || geofenceFilter.State.ToUpper() == "NONE")
                {
                    //parameter.Add("@state", MapLandmarkStateToChar(geofenceFilter.State));
                    query = query + " and l.state in ('A','I') ";
                }
                if (geofenceFilter.Latitude > 0)
                {
                    parameter.Add("@latitude", geofenceFilter.Latitude);
                    query = query + " and l.latitude = @latitude ";
                }
                if (geofenceFilter.Longitude > 0)
                {
                    parameter.Add("@longitude", geofenceFilter.Longitude);
                    query = query + " and l.longitude= @longitude ";
                }
                if (geofenceFilter.Width > 0)
                {
                    parameter.Add("@width", geofenceFilter.Width);
                    query = query + " and l.width= @width ";
                }
                if (geofenceFilter.CreatedBy > 0)
                {
                    parameter.Add("@created_at", geofenceFilter.CreatedAt);
                    query = query + " and l.created_at= @created_at ";
                }
                if (geofenceFilter.ModifiedBy > 0)
                {
                    parameter.Add("@modified_at", geofenceFilter.CreatedAt);
                    query = query + " and l.modified_at= @modified_at ";
                }
                Dictionary<int, Geofence> geofencelookup = new Dictionary<int, Geofence>();
                var nodelookup = new Dictionary<int, Nodes>();
                dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                List<Geofence> geofencelst = new List<Geofence>();
                Geofence geofence = new Geofence();
                Nodes node = new Nodes();
                foreach (var item in result)
                {
                    //if present, return existing geofence (avoid duplicate entry of geofence)
                    if (!geofencelookup.TryGetValue(Convert.ToInt32(item.id), out geofence))
                        //add only distinct records into geofence based on id
                        geofencelookup.Add(Convert.ToInt32(item.id), geofence = Map(item));

                    if (geofence.Nodes == null)
                        geofence.Nodes = new List<Nodes>();

                    // check if geofence has node(s) 
                    if (item.nodeid != null && item.id == item.landmarkid)
                    {
                        //if present, return existing nodes(avoid duplicate entry of node)
                        if (!nodelookup.TryGetValue(Convert.ToInt32(item.nodeid), out node))
                        {
                            node = new Nodes();
                            node.Id = item.nodeid;
                            node.LandmarkId = item.landmarkid != null ? item.landmarkid : 0;
                            node.SeqNo = item.seqno != null ? item.seqno : 0;
                            node.Latitude = item.nodelatitude != null ? Convert.ToDouble(item.nodelatitude) : 0;
                            node.Longitude = item.nodelongitude != null ? Convert.ToDouble(item.nodelongitude) : 0;
                            node.State = string.IsNullOrEmpty(item.nodestate) ? item.nodestate : string.Empty;
                            node.CreatedAt = item.nodecreatedat != null ? item.nodecreatedat : 0;
                            node.CreatedBy = item.nodecreatedby != null ? item.nodecreatedby : 0;
                            node.ModifiedAt = item.nodemodifiedat != null ? item.nodemodifiedat : 0;
                            node.ModifiedBy = item.nodemodifiedby != null ? item.nodemodifiedby : 0;
                            //add node to exisitng geofence
                            geofence.Nodes.Add(node);
                            //add distinct node to node dictionary.
                            nodelookup.Add(Convert.ToInt32(item.nodeid), node);
                        }
                    }
                }
                foreach (var keyValuePair in geofencelookup)
                {
                    //add geofence object along with nodes to geofence list 
                    geofencelst.Add(keyValuePair.Value);
                }
                return geofencelst;
            }
            catch (System.Exception ex)
            {
                log.Info("GetGeofenceByGeofenceID  method in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }
        }
        public Geofence Map(dynamic record)
        {
            Geofence geofence = new Geofence();
            geofence.Id = record.id;
            geofence.OrganizationId = record.organizationid != null ? record.organizationid : 0;
            geofence.CategoryId = record.categoryid != null ? record.categoryid : 0;
            geofence.CategoryName = !string.IsNullOrEmpty(record.categoryname) ? record.categoryname : string.Empty;
            geofence.SubCategoryId = record.subcategoryid != null ? record.subcategoryid : 0;
            geofence.SubCategoryName = !string.IsNullOrEmpty(record.subcategoryname) ? record.subcategoryname : string.Empty;
            geofence.Name = !string.IsNullOrEmpty(record.name) ? record.name : string.Empty;
            geofence.Address = !string.IsNullOrEmpty(record.address) ? record.address : string.Empty;
            geofence.City = !string.IsNullOrEmpty(record.city) ? record.City : string.Empty;
            geofence.Country = !string.IsNullOrEmpty(record.country) ? record.country : string.Empty;
            geofence.Zipcode = !string.IsNullOrEmpty(record.zipcode) ? record.zipcode : string.Empty;
            geofence.Type = record.type;
            geofence.Latitude = Convert.ToDouble(record.latitude);
            geofence.Longitude = Convert.ToDouble(record.logitude);
            geofence.Distance = Convert.ToDouble(record.distance);
            geofence.Width = record.Width != null ? record.Width : 0;
            geofence.CreatedAt = record.createdat != null ? record.createdat : 0;
            geofence.State = record.state;
            geofence.CreatedBy = record.createdby != null ? record.createdby : 0;
            geofence.ModifiedAt = record.modifiedat != null ? record.modifiedat : 0;
            geofence.ModifiedBy = record.modifiedby != null ? record.modifiedby : 0;
            return geofence;
        }
        #endregion
    }


}
