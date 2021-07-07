using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDataAccess _dataAccess;

        private readonly CatogoryCoreMapper _catogoryCoreMapper;
        private static readonly log4net.ILog _log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public CategoryRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;

            _catogoryCoreMapper = new CatogoryCoreMapper();

        }


        public async Task<Category> AddCategory(Category category)
        {
            try
            {
                if (category.Type.Length > 1)
                {
                    category.Type = _catogoryCoreMapper.MapType(category.Type.ToUpper());
                }
                var icon_ID = await InsertIcons(category);

                var isexist = CheckCategoryIsexist(category.Name, category.Organization_Id, category.Id);
                if (!isexist)
                {

                    var parameter = new DynamicParameters();
                    var insertCategory = @"INSERT INTO master.category(
                                   organization_id, name, icon_id, type, parent_id, state,description, created_at, created_by)
                                  values(@organization_id,@name,@icon_id,@type,@parent_id,@state,@description,@created_at,@created_by) RETURNING id";

                    parameter.Add("@organization_id", category.Organization_Id != 0 ? category.Organization_Id : null);
                    parameter.Add("@name", category.Name);
                    parameter.Add("@icon_id", icon_ID);
                    parameter.Add("@type", category.Type);
                    parameter.Add("@parent_id", category.Parent_Id);
                    parameter.Add("@state", category.State);
                    parameter.Add("@description", category.Description);
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    parameter.Add("@created_by", category.Created_By);
                    var id = await _dataAccess.ExecuteScalarAsync<int>(insertCategory, parameter);
                    category.Id = id;
                }
                else
                {
                    category.Id = -1;//to check either code exists or not
                }
            }
            catch (Exception ex)
            {
                _log.Info("Add Category method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(category.Id));
                _log.Error(ex.ToString());
                // throw;
            }
            return category;
        }

        public async Task<CategoryID> DeleteCategory(int categoryId)
        {
            _log.Info("Delete Category method called in repository");
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var id = 0;
                    int isbulk = 0;
                    CategoryID categoryID = new CategoryID();
                    var parameter = new DynamicParameters();

                    var isexistSubcategory = await GetSubCategory(categoryId, isbulk);
                    var isexistPOIGeofence = await GetPOICategory(categoryId);

                    if (isexistSubcategory.Count() <= 0 && isexistPOIGeofence.Count() <= 0)
                    {


                        var deleteCategory = @"update master.category set state='D' 
                                   WHERE id = @ID RETURNING id ";

                        parameter.Add("@ID", categoryId);

                        id = await _dataAccess.ExecuteScalarAsync<int>(deleteCategory, parameter);

                        transactionScope.Complete();
                    }
                    if (id > 0)
                    {
                        categoryID.ID = id;
                    }
                    else if (isexistSubcategory.Count() > 0)
                    {
                        categoryID.ID = -1;
                    }
                    else if (isexistPOIGeofence.Count() > 0)
                    {
                        categoryID.ID = -2;
                    }
                    else
                    {
                        categoryID.ID = -3; // Not Found
                    }
                    return categoryID;
                }
            }
            catch (Exception ex)
            {
                _log.Info("Delete Category method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(categoryId));
                _log.Error(ex.ToString());
                throw;
            }
        }
        public async Task<IEnumerable<int>> GetPOICategory(int categoryId)
        {
            try
            {

                List<int> obj = new List<int>();
                var parameter = new DynamicParameters();
                var query = @"select id 
                              from master.Landmark 
                              where category_id = @categoryId and type in ('C','O','P')";
                parameter.Add("@categoryId", categoryId);
                IEnumerable<int> subCategoryDetails = await _dataAccess.QueryAsync<int>(query, parameter);
                if (subCategoryDetails != null)
                {
                    foreach (dynamic record in subCategoryDetails)
                    {
                        obj.Add(record);

                    }
                }

                return obj;

            }
            catch (Exception)
            {
                throw;
            }


        }

        public async Task<IEnumerable<int>> GetSubCategory(int categoryId, int isbulk)
        {
            try
            {

                List<int> obj = new List<int>();
                var parameter = new DynamicParameters();
                var query = @"select id, name  from master.category where parent_id = @categoryId ";
                parameter.Add("@categoryId", categoryId);
                IEnumerable<int> subCategoryDetails = await _dataAccess.QueryAsync<int>(query, parameter);
                if (subCategoryDetails != null)
                {
                    foreach (dynamic record in subCategoryDetails)
                    {
                        obj.Add(record);
                        if (isbulk == 1)
                        {
                            await DeleteSubCategoryBulk(record);
                        }

                    }
                }

                return obj;

            }
            catch (Exception)
            {
                throw;
            }


        }

        public async Task<Category> EditCategory(Category category)
        {
            try
            {
                var icon_ID = await InsertIcons(category);
                var isCategoryUpdate = CheckCategoryForUpdate(category.Id, category.Organization_Id);

                if (isCategoryUpdate)
                {
                    var isCategoryNameExist = CheckCategoryIsexist(category.Name, category.Organization_Id, category.Id);

                    if (!isCategoryNameExist)
                    {
                        var parameter = new DynamicParameters();
                        var insertCategory = @"UPDATE master.category
                                   SET  name=@name, icon_id=@icon_id, description=@description,  modified_at=@modified_at, modified_by=@modified_by
                                  WHERE id = @ID RETURNING id";

                        parameter.Add("@name", category.Name);
                        parameter.Add("@icon_id", icon_ID);
                        parameter.Add("@description", category.Description);
                        parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                        parameter.Add("@modified_by", category.Modified_By);
                        parameter.Add("@ID", category.Id);

                        var id = await _dataAccess.ExecuteScalarAsync<int>(insertCategory, parameter);
                        category.Id = id;
                    }
                    else
                    {
                        category.Id = -2;
                    }
                }
                else
                {
                    category.Id = -1;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return category;
        }

        private bool CheckCategoryNameForUpdate(string categoryName, int? organization_Id)
        {
            CategoryFilter categoryFilter = new CategoryFilter();
            categoryFilter.CategoryName = categoryName;
            categoryFilter.OrganizationId = organization_Id;

            var categories = GetCategory(categoryFilter);

            var codeExistsForUpdate = categories.Result.Where(t => t.Name == categoryName).Count();
            if (codeExistsForUpdate == 0)
                return false;
            else if (codeExistsForUpdate > 0)
                return true;
            else
                return codeExistsForUpdate == 0 ? false : true;
        }

        private bool CheckCategoryForUpdate(int id, int? organization_Id)
        {
            CategoryFilter categoryFilter = new CategoryFilter();
            categoryFilter.CategoryID = id;
            categoryFilter.OrganizationId = organization_Id;

            var categories = GetCategory(categoryFilter);

            var codeExistsForUpdate = categories.Result.Where(t => t.Id == id).Count();
            if (codeExistsForUpdate == 0)
                return false;
            else if (codeExistsForUpdate > 0)
                return true;
            else
                return codeExistsForUpdate == 0 ? false : true;
        }

        public Task<IEnumerable<Category>> GetCategoryType(string type, int organizationId)

        {
            CategoryFilter categoryFilter = new CategoryFilter();
            categoryFilter.Type = type.ToUpper();
            categoryFilter.OrganizationId = organizationId;
            if (categoryFilter.Type.Length > 1)
            {
                categoryFilter.Type = _catogoryCoreMapper.MapType(categoryFilter.Type);
            }
            var categories = GetCategory(categoryFilter);
            return categories;
        }

        public async Task<IEnumerable<Category>> GetCategory(CategoryFilter categoryFilter)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Category> categories = new List<Category>();
                string getQuery = string.Empty;

                getQuery = @"SELECT id, organization_id, name, icon_id, type, parent_id, state, created_at, created_by, modified_at, modified_by FROM master.category where 1=1 ";

                if (categoryFilter != null)
                {
                    // id filter
                    if (categoryFilter.CategoryID > 0)
                    {
                        parameter.Add("@id", categoryFilter.CategoryID);
                        getQuery = getQuery + " and id=@id ";
                    }
                    // Category Type Filter
                    if (categoryFilter.Type != null)
                    {
                        parameter.Add("@type", categoryFilter.Type);
                        getQuery = getQuery + " and type= @type ";
                    }
                    // Category Name Filter
                    if (!string.IsNullOrEmpty(categoryFilter.CategoryName))
                    {
                        parameter.Add("@Name", categoryFilter.CategoryName);
                        getQuery = getQuery + " and name= @Name ";
                    }
                    if (categoryFilter.OrganizationId > 0)
                    {
                        //It will return organization specific category/subcategory
                        parameter.Add("@organization_id", categoryFilter.OrganizationId);
                        getQuery = getQuery + " and organization_id=@organization_id  ";
                    }
                    else
                    {
                        //only return global poi
                        getQuery = getQuery + " and organization_id is null ";
                    }
                    parameter.Add("@State", "A");
                    getQuery = getQuery + " and state= @State ";

                    getQuery = getQuery + " ORDER BY id ASC; ";
                    dynamic result = await _dataAccess.QueryAsync<dynamic>(getQuery, parameter);

                    foreach (dynamic record in result)
                    {
                        categories.Add(_catogoryCoreMapper.Map(record));
                    }
                }
                return categories;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CheckCategoryIsexist(string categoryName, int? organizationId, int categoryid)
        {
            CategoryFilter categoryFilter = new CategoryFilter();
            categoryFilter.CategoryName = categoryName;
            categoryFilter.OrganizationId = organizationId;

            var categories = GetCategory(categoryFilter);

            var nameExistsForInsert = categories.Result.Where(t => t.Name == categoryName && t.Id != categoryid).Count();
            if (nameExistsForInsert == 0)
                return false;
            else if (nameExistsForInsert > 0)
                return true;
            else
                return nameExistsForInsert == 0 ? false : true;
        }


        public async Task<int> InsertIcons(Category category)
        {
            try
            {
                var queryStatement = @"INSERT INTO master.icon(
                                     icon, type, name, state, created_at, created_by)  
                                    VALUES (@icon, @type, @name, @state, @created_at, @created_by)
                                    RETURNING id;";

                var updateQueryStatement = @" UPDATE master.icon
                                    SET                                
                                    icon=@icon, 
                                    modified_at=@modified_at,
                                    modified_by=@modified_by                                    
                                    WHERE name=@name 
                                    and type = @type
                                    RETURNING id;";


                int iconId = 0;


                //If name is exist then update
                int name_cnt = await _dataAccess.QuerySingleAsync<int>("select coalesce((SELECT count(*) FROM master.icon where name=@name and type = @type))", new { name = category.IconName, type = category.Type });

                if (name_cnt > 0)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@icon", category.Icon);
                    parameter.Add("@type", "L");
                    parameter.Add("@name", category.IconName);
                    parameter.Add("@state", "A");
                    parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameter.Add("@modified_by", category.Modified_By);

                    iconId = await _dataAccess.ExecuteScalarAsync<int>(updateQueryStatement, parameter);
                }
                else
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@icon", category.Icon);
                    parameter.Add("@type", "L");
                    parameter.Add("@name", category.IconName);
                    parameter.Add("@state", "A");
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameter.Add("@created_by", category.Created_By);
                    iconId = await _dataAccess.ExecuteScalarAsync<int>(queryStatement, parameter);
                }

                return iconId;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<CategoryList>> GetCategoryDetails(int orgId)
        {
            try
            {

                var parameter = new DynamicParameters();
                string getQuery = string.Empty;

                getQuery = @"with result As
                            (
                            select pcat.id as Parent_id, pcat.name as Pcategory, COALESCE(scat.id,0) as Subcategory_id, scat.name as Scategory, pcat.icon_id as Parent_category_Icon,
							pcat.description,pcat.created_at,i.name As Icon_Name, COALESCE(pcat.organization_id,0) as organization_id
                            from master.category pcat
                            left join master.category scat on pcat.id = scat.parent_id and scat.state='A'
							left join master.icon i on i.id = pcat.icon_id
                            where pcat.type ='C' and pcat.state ='A' and (pcat.organization_id= @organizationID or pcat.organization_id is null)
								union
								select pcat.id as Parent_id, pcat.name as Pcategory, COALESCE(scat.id,0) as Subcategory_id, scat.name as Scategory, pcat.icon_id as Parent_category_Icon,
							pcat.description,pcat.created_at,i.name As Icon_Name, COALESCE(pcat.organization_id,0) as organization_id
                            from master.category pcat
                            left join master.category scat on pcat.id = 0 --and scat.state='A'
							left join master.icon i on i.id = pcat.icon_id
                            where pcat.type ='C' and pcat.state ='A' and (pcat.organization_id= @organizationID or pcat.organization_id is null)
                            ) 
                            select r.Parent_id ,r.Pcategory As ParentCategory,r.Subcategory_id,r.Scategory As SubCategory ,
                            
							(select (case r.Subcategory_id when 0
									then  
									(select Count(id) from master.landmark where category_id in(r.parent_id) and type in ('C','O') and state ='A' and (organization_id= @organizationID or organization_id is null)) 
									else 
									(select Count(id) from master.landmark where category_id in(r.parent_id) and (sub_category_id = r.Subcategory_id ) and type in ('C','O') and state ='A' and (organization_id=@organizationID or organization_id is null)) 
									end) ) as No_of_Geofence,
							
							
							(select (case r.Subcategory_id when 0
									then  
									(select Count(id) from master.landmark where category_id in(r.parent_id) and type in ('P') and state ='A' and (organization_id= @organizationID or organization_id is null)) 
									else 
									(select Count(id) from master.landmark where category_id in(r.parent_id) and (sub_category_id = r.Subcategory_id ) and type in ('P') and state ='A' and (organization_id=@organizationID or organization_id is null)) 
									end) )  as No_of_POI,
									
                            r.Parent_category_Icon As IconId,
                            (select icon from master.icon where id in (r.Parent_category_Icon)) as Icon,
							r.description,r.created_at,r.Icon_Name,r.organization_id
                            from result r 
							
							 ";
                parameter.Add("@organizationID", orgId);

                dynamic result = await _dataAccess.QueryAsync<dynamic>(getQuery, parameter);

                IEnumerable<CategoryList> categories = await _dataAccess.QueryAsync<CategoryList>(getQuery, parameter);
                return categories;
            }
            catch (Exception)
            {
                throw;
            }


        }

        public async Task<Category_SubCategory_ID_Class> BulkDeleteCategory(DeleteCategoryclass deleteCategoryclass)
        {
            //1 | -               | check parent cat having any sub cat in query, if yes..then delete parent and all related sub-cat
            //1 | -               | check parent cat having any sub cat in query, if No...then simply delete parent category
            //1 | 1 | check sub cat from req.payload, if yes...delete particular sub cat from par cat.
            _log.Info("Delete Category method called in repository");
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Category_SubCategory_ID_Class obj = new Category_SubCategory_ID_Class();
                    foreach (var item in deleteCategoryclass.Category_SubCategory_s)
                    {
                        var temp = new Category_SubCategory_ID_Class();
                        temp.CategoryId = item.CategoryId;
                        temp.SubCategoryId = item.SubCategoryId;
                        var del = await DeleteSubCategory(temp);

                        if (del.CategoryId > 0)
                        {
                            obj.CategoryId = del.CategoryId;
                        }

                    }
                    transactionScope.Complete();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                _log.Info("Delete Category method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(deleteCategoryclass));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<Category_SubCategory_ID_Class> DeleteSubCategory(Category_SubCategory_ID_Class categoryobj)
        {
            _log.Info("Delete Category method called in repository");
            try
            {
                //1 | -               | check parent cat having any sub cat in query, if yes..then delete parent and all related sub-cat
                //1 | -               | check parent cat having any sub cat in query, if No...then simply delete parent category
                //1 | 1 | check sub cat from req.payload, if yes...delete particular sub cat from par cat.

                int isbulk = 1;
                int id = 0;
                Category_SubCategory_ID_Class obj = new Category_SubCategory_ID_Class();

                if (categoryobj.CategoryId > 0 && categoryobj.SubCategoryId == 0)
                {
                    var isExistSubcategory = await GetSubCategory(categoryobj.CategoryId, isbulk);

                    var parameter = new DynamicParameters();
                    var updatecategory = @"update master.category 
                                          set state='D' 
                                          where id =@ID RETURNING id  ";

                    parameter.Add("@ID", categoryobj.CategoryId);

                    id = await _dataAccess.ExecuteScalarAsync<int>(updatecategory, parameter);

                }
                if (categoryobj.CategoryId > 0 && categoryobj.SubCategoryId > 0)
                {

                    var parameter = new DynamicParameters();
                    var updatecategory = @"update master.category 
                                          set state='D' 
                                          where id =@ID RETURNING id  ";

                    parameter.Add("@ID", categoryobj.SubCategoryId);

                    id = await _dataAccess.ExecuteScalarAsync<int>(updatecategory, parameter);
                }
                obj.CategoryId = id;

                return obj;

            }
            catch (Exception ex)
            {
                _log.Info("Delete Category method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(categoryobj.CategoryId));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<CategoryID> DeleteSubCategoryBulk(int subcategoryId)
        {
            _log.Info("Delete Category method called in repository");
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var id = 0;

                    CategoryID categoryID = new CategoryID();
                    var parameter = new DynamicParameters();

                    var deleteCategory = @"update master.category set state='D' 
                                   WHERE id = @ID RETURNING id ";

                    parameter.Add("@ID", subcategoryId);

                    id = await _dataAccess.ExecuteScalarAsync<int>(deleteCategory, parameter);
                    categoryID.ID = id;
                    transactionScope.Complete();


                    return categoryID;
                }
            }
            catch (Exception ex)
            {
                _log.Info("Delete Category method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(subcategoryId));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<List<CategoryWisePOI>> GetCategoryWisePOI(int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = @"SELECT c.id as CategoryId,
	                               c.name as CategoryName,
		                           l.id as POIId,
								   l.name as POIName,
                                   l.address as POIAddress,
                                   l.latitude as Latitude,
								   l.longitude as Longitude,
								   l.width as Width,
								   l.distance as Distance,
	                               l.sub_category_id as SubCategoryId,
								   s.name as SubCategoryName
	                            FROM master.landmark l
	                            LEFT JOIN master.category c  on c.id = l.category_id 
								LEFT JOIN master.category s  on s.id = l.sub_category_id
	                            WHERE l.organization_id = @organization_id
	                            AND l.type = 'P' 
	                            AND l.state= 'A'";
                parameter.Add("@organization_id", organizationId);
                var data = await _dataAccess.QueryAsync<CategoryWisePOI>(query, parameter);
                return data.ToList();
            }
            catch (Exception ex)
            {
                _log.Info($"Get CategoryWisePOI method in repository failed : {Newtonsoft.Json.JsonConvert.SerializeObject(organizationId)}");
                _log.Error(ex.ToString());
                throw;
            }
        }
    }
}
