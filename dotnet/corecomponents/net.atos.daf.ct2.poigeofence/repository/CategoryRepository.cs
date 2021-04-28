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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDataAccess _dataAccess;
        
        private readonly catogoryCoreMapper _catogoryCoreMapper;
        private static readonly log4net.ILog log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public CategoryRepository (IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
           
            _catogoryCoreMapper = new catogoryCoreMapper();

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

                var isexist = CheckCategoryIsexist(category.Name, category.Organization_Id);
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
                log.Info("Add Category method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(category.Id));
                log.Error(ex.ToString());
               // throw ex;
            }
            return category;
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            log.Info("Delete Category method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                var Deletecategory = @"update master.category set state='D' 
                                   WHERE id= @ID  RETURNING id ";

                parameter.Add("@ID", categoryId);

                var id = await _dataAccess.ExecuteScalarAsync<int>(Deletecategory, parameter);
                if (id > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception ex)
            {
                log.Info("Delete Category method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(categoryId));
                log.Error(ex.ToString());
                throw ex;
            }
        }

        public async Task<Category> EditCategory(Category category)
        {
            try
            {
                var icon_ID = await InsertIcons(category);
                var isCategoryUpdate = CheckCategoryForUpdate(category.Id);
                
                if (isCategoryUpdate)
                {
                    var isCategoryNameExist = CheckCategoryNameForUpdate(category.Name,category.Organization_Id);

                    if (!isCategoryNameExist)
                    {
                        var parameter = new DynamicParameters();
                        var Insertcategory = @"UPDATE master.category
                                   SET  name=@name, icon_id=@icon_id, description=@description,  modified_at=@modified_at, modified_by=@modified_by
                                  WHERE id = @ID RETURNING id";

                        parameter.Add("@name", category.Name);
                        parameter.Add("@icon_id", icon_ID);
                        parameter.Add("@description", category.Description);
                        parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                        parameter.Add("@modified_by", category.Modified_By);
                        parameter.Add("@ID", category.Id);

                        var id = await _dataAccess.ExecuteScalarAsync<int>(Insertcategory, parameter);
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
            catch (Exception ex)
            {
                throw ex;
            }
            return category;
        }

        private bool CheckCategoryNameForUpdate(string categoryName, int? Organization_Id)
        {
            CategoryFilter categoryFilter = new CategoryFilter();
            categoryFilter.CategoryName = categoryName;
            categoryFilter.OrganizationId = Organization_Id;

            var categories = GetCategory(categoryFilter);

            var codeExistsForUpdate = categories.Result.Where(t => t.Name == categoryName).Count();
            if (codeExistsForUpdate == 0)
                return false;
            else if (codeExistsForUpdate > 0)
                return true;
            else
                return codeExistsForUpdate == 0 ? false : true;
        }

        private bool CheckCategoryForUpdate(int id)
        {
            CategoryFilter categoryFilter = new CategoryFilter();
            categoryFilter.CategoryID = id;

            var categories = GetCategory(categoryFilter);

            var codeExistsForUpdate = categories.Result.Where(t => t.Id == id).Count();
            if (codeExistsForUpdate == 0 )
                return false;
            else if (codeExistsForUpdate > 0)
                return true;
            else
                return codeExistsForUpdate == 0 ? false : true;
        }

        public Task<IEnumerable<Category>> GetCategoryType(string Type)
        
        {
            CategoryFilter categoryFilter = new CategoryFilter();
            categoryFilter.Type = Type.ToUpper();
            if (categoryFilter.Type.Length > 1)
            {
                categoryFilter.Type= _catogoryCoreMapper.MapType(categoryFilter.Type);
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
                        getQuery = getQuery + " and organization_id=@organization_id or organization_id is null ";
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckCategoryIsexist(string categoryName, int? OrganizationId)
        {
            CategoryFilter categoryFilter = new CategoryFilter();
            categoryFilter.CategoryName = categoryName;
            categoryFilter.OrganizationId = OrganizationId;

            var categories = GetCategory(categoryFilter);

            var nameExistsForInsert = categories.Result.Where(t => t.Name == categoryName ).Count();
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
                var QueryStatement = @"INSERT INTO master.icon(
                                     icon, type, name, state, created_at, created_by)  
                                    VALUES (@icon, @type, @name, @state, @created_at, @created_by)
                                    RETURNING id;";

                var UpdateQueryStatement = @" UPDATE master.icon
                                    SET                                
                                    icon=@icon, 
                                    modified_at=@modified_at,
                                    modified_by=@modified_by                                    
                                    WHERE name=@name 
                                    and type = @type
                                    RETURNING id;"; 


                int iconId = 0;
                
                  
                    //If name is exist then update
                    int name_cnt = await _dataAccess.QuerySingleAsync<int>("select coalesce((SELECT count(*) FROM master.icon where name=@name and type = @type))", new { name = category.IconName , type = category.Type});

                    if (name_cnt > 0)
                    {
                    var parameter = new DynamicParameters();
                    parameter.Add("@icon", category.icon);
                    parameter.Add("@type", "L");
                    parameter.Add("@name", category.IconName);
                    parameter.Add("@state", "A");
                    parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameter.Add("@modified_by", category.Modified_By);
                    
                    iconId = await _dataAccess.ExecuteScalarAsync<int>(UpdateQueryStatement, parameter);
                    }
                    else
                    {
                    var parameter = new DynamicParameters();
                    parameter.Add("@icon", category.icon);
                    parameter.Add("@type", "L");
                    parameter.Add("@name", category.IconName);
                    parameter.Add("@state", "A");
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameter.Add("@created_by", category.Created_By);
                    iconId = await _dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                    }

                return iconId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CategoryList>> GetCategoryDetails()
        {
            try
            {

                var parameter = new DynamicParameters();
                string getQuery = string.Empty;

                getQuery = @"with result As
                            (
                            select pcat.id as Parent_id, pcat.name as Pcategory,scat.id as Subcategory_id, scat.name as Scategory, pcat.icon_id as Parent_category_Icon
                            from master.category pcat
                            left join master.category scat on pcat.id = scat.parent_id
                            where pcat.type ='C' and pcat.state ='A'
                            ) 
                            select r.Parent_id ,r.Pcategory As ParentCategory,r.Subcategory_id,r.Scategory As SubCategory ,
                            (select Count(id) from master.landmark where category_id in(r.parent_id) and type in ('C','O') ) as No_of_Geofence,
                            (select Count(id) from master.landmark where sub_category_id in (r.subcategory_id) and type in ('P')) as No_of_POI,
                            r.Parent_category_Icon As IconId,
                            (select icon from master.icon where id in (r.Parent_category_Icon)) as Icon
                            from result r ";
                dynamic result = await _dataAccess.QueryAsync<dynamic>(getQuery, parameter);

                IEnumerable<CategoryList> categories = await _dataAccess.QueryAsync<CategoryList>(getQuery, parameter);
                return categories;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        


    }
}
