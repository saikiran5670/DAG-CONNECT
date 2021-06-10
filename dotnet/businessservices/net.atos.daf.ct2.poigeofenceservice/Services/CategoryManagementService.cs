using System;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofences;
using net.atos.daf.ct2.poigeofenceservice.entity;

namespace net.atos.daf.ct2.poigeofenservice
{
    public class CategoryManagementService : CategoryService.CategoryServiceBase
    {
        private ILog _logger;
        // private readonly Mapper _mapper;
        private readonly ICategoryManager _categoryManager;
        private readonly DeleteCategoryMapper _deleteCategoryMapper;
        public CategoryManagementService(ICategoryManager categoryManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            //  _mapper = new Mapper();
            _categoryManager = categoryManager;
            _deleteCategoryMapper = new DeleteCategoryMapper();

        }


        // Start - category section
        public override async Task<CategoryAddResponse> AddCategory(CategoryAddRequest request, ServerCallContext context)
        {
            CategoryAddResponse response = new CategoryAddResponse();
            try
            {
                _logger.Info("Add Category .");
                Category obj = new Category();
                obj.Organization_Id = request.OrganizationId;
                obj.Name = request.Name;
                obj.IconName = request.IconName;
                obj.Type = request.Type;
                obj.Parent_Id = request.ParentId;
                obj.State = request.State;
                obj.Description = request.Description;
                obj.Created_By = request.CreatedBy;
                obj.Icon = request.Icon.ToByteArray();

                var result = await _categoryManager.AddCategory(obj);
                if (result.Id == -1)
                {
                    response.Message = "Category Name is " + obj.Name + " already exists ";
                    response.Code = Responsecode.Conflict;
                    response.CategoryID = result.Id;

                }
                else if (result != null && result.Id > 0)
                {
                    response.Message = "Added successfully";
                    response.Code = Responsecode.Success;
                    response.CategoryID = result.Id;
                }
                else
                {
                    response.Message = "Add category Fail";
                    response.Code = Responsecode.Failed;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                //response.Message = "Not Deleted";
            }
            return await Task.FromResult(response);
        }

        public override async Task<CategoryEditResponse> EditCategory(CategoryEditRequest request, ServerCallContext context)
        {
            CategoryEditResponse response = new CategoryEditResponse();
            try
            {
                _logger.Info("Edit Category .");
                Category obj = new Category();
                obj.Id = request.Id;
                obj.Name = request.Name;
                obj.IconName = request.IconName;
                obj.Icon = request.Icon.ToByteArray();
                obj.Description = request.Description;
                obj.Modified_By = request.ModifiedBy;
                obj.Organization_Id = request.OrganizationId;

                var result = await _categoryManager.EditCategory(obj);
                if (result != null && result.Id >= 0)
                {
                    response.Message = "Edit successfully";
                    response.Code = Responsecode.Success;
                    response.CategoryID = result.Id;
                }
                else if (result != null && result.Id == -1)
                {
                    response.Message = "Category Not Found";
                    response.Code = Responsecode.NotFound;
                }
                else if (result != null && result.Id == -2)
                {
                    response.Message = "Category Name already exist with the same Name ";
                    response.Code = Responsecode.Conflict;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                //response.Message = "Not Deleted";
            }
            return await Task.FromResult(response);
        }

        public override async Task<CategoryDeleteResponse> DeleteCategory(CategoryDeleteRequest request, ServerCallContext context)
        {
            CategoryDeleteResponse response = new CategoryDeleteResponse();
            try
            {
                _logger.Info("Delete Category .");


                var result = await _categoryManager.DeleteCategory(request.Id);
                if (result.ID >= 0)
                {
                    response.Message = "Delete successfully";
                    response.Code = Responsecode.Success;
                    response.CategoryID = request.Id;

                }
                else if (result.ID == -1)
                {
                    response.Message = "You can not delete the category it contain subcategory  ";
                    response.Code = Responsecode.Failed;
                }
                else if (result.ID == -2)
                {
                    response.Message = "You can not delete the category it contain POI or Geofence  ";
                    response.Code = Responsecode.Failed;
                }
                else
                {
                    response.Message = "Category Not found";
                    response.Code = Responsecode.NotFound;
                }
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
            return await Task.FromResult(response);
        }

        public override async Task<CategoryGetResponse> GetCategoryType(CategoryGetRequest request, ServerCallContext context)
        {
            CategoryGetResponse response = new CategoryGetResponse();
            try
            {
                _logger.Info("Get Category .");

                var result = await _categoryManager.GetCategory(request.Type, request.OrganizationId);
                foreach (var item in result)
                {
                    var Data = new GetCategoryType();
                    Data.Id = item.Id;
                    Data.Name = !string.IsNullOrEmpty(item.Name) ? item.Name : string.Empty;
                    response.Categories.Add(Data);
                }

                _logger.Info("GetCategoryType service called.");

                if (result != null)
                {
                    response.Code = Responsecode.Success;
                    response.Message = "Get Category Type Details";
                }
                else
                {
                    response.Code = Responsecode.Failed;
                    response.Message = "Resource Not Found ";
                }
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
            return await Task.FromResult(response);
        }

        public async override Task<GetResponse> GetCategoryDetails(GetRequest request, ServerCallContext context)
        {
            GetResponse response = new GetResponse();
            try
            {
                var categoryListDetails = _categoryManager.GetCategoryDetails().Result;

                foreach (var item in categoryListDetails)
                {
                    var catdetails = new categoryDetails();
                    catdetails.ParentCategoryId = item.Parent_id;
                    catdetails.SubCategoryId = item.Subcategory_id;
                    catdetails.IconId = item.IconId > 0 ? 0 : item.IconId;
                    if (item.Icon != null)
                    {
                        catdetails.Icon = ByteString.CopyFrom(item.Icon);
                    }
                    catdetails.ParentCategoryName = item.ParentCategory ?? "";
                    catdetails.SubCategoryName = item.SubCategory ?? "";
                    catdetails.NoOfPOI = item.No_of_POI;
                    catdetails.NoOfGeofence = item.No_of_Geofence;
                    catdetails.Description = !string.IsNullOrEmpty(item.Description) ? item.Description : string.Empty;
                    catdetails.CreatedAt = item.Created_at;
                    catdetails.IconName = !string.IsNullOrEmpty(item.Icon_Name) ? item.Icon_Name : string.Empty;
                    catdetails.OrganizationId = item.Organization_Id;
                    response.Categories.Add(catdetails);

                }
                _logger.Info("GetCategoryDetails service called.");

                if (categoryListDetails != null)
                {
                    response.Code = Responsecode.Success;
                    response.Message = "Get Category Type Details";
                }
                else
                {
                    response.Code = Responsecode.Failed;
                    response.Message = "Resource Not Found ";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
            return await Task.FromResult(response);
        }

        public override async Task<DeleteResponse> BulkDeleteCategory(DeleteRequest request, ServerCallContext context)
        {
            DeleteResponse response = new DeleteResponse();
            CategoryID obj = new CategoryID();


            DeleteCategoryclass deleteCategoryclass = _deleteCategoryMapper.ToTranslationDeleteEntity(request);
            var result = await _categoryManager.BulkDeleteCategory(deleteCategoryclass);

            if (result.CategoryId > 0)
            {
                response.Code = Responsecode.Success;
                response.Message = " Category deleted Sucessfully";
            }
            else
            {
                response.Code = Responsecode.Failed;
                response.Message = "Resource Not Found ";
            }
            return await Task.FromResult(response);
        }


        //public override async Task<DeleteResponse> GetOrganisationId(CategoryDeleteRequest request, ServerCallContext context)
        //{
        //    DeleteResponse response = new DeleteResponse();
        //    CategoryID obj = new CategoryID();
        //    var result = await _categoryManager.GetOrganisationId(request.Id);
        //    return await Task.FromResult(response);

        //}

        public override async Task<CategoryWisePOIResponse> GetCategoryWisePOI(CategoryWisePOIRequest request, ServerCallContext context)
        {
            CategoryWisePOIResponse objCategoryWisePOIResponse = new CategoryWisePOIResponse();
            try
            {
                var data = await _categoryManager.GetCategoryWisePOI(request.OrganizationId);
                _logger.Info("Get GetCategoryWisePOI Service Called .");

                if (data.Count > 0)
                {
                    objCategoryWisePOIResponse.Code = Responsecode.Success;
                    objCategoryWisePOIResponse.Message = POIGeoFenceServiceConstants.GET_POI_DETAILS_SUCCESS_MSG;
                    for (int i = 0; i < data.Count; i++)
                    {
                        net.atos.daf.ct2.poigeofences.CategoryWisePOI objCategoryWisePOI = new net.atos.daf.ct2.poigeofences.CategoryWisePOI();
                        objCategoryWisePOI.CategoryId = data[i].CategoryId;
                        objCategoryWisePOI.CategoryName = data[i].CategoryName ?? null;
                        objCategoryWisePOI.POIName = data[i].POIName ?? null;
                        objCategoryWisePOI.POIAddress = data[i].POIAddress ?? null;
                        objCategoryWisePOI.Latitude = data[i].Latitude;
                        objCategoryWisePOI.Longitude = data[i].Longitude;
                        objCategoryWisePOIResponse.CategoryWisePOI.Add(objCategoryWisePOI);
                    }
                }
                else
                {
                    objCategoryWisePOIResponse.Code = Responsecode.Success;
                    objCategoryWisePOIResponse.Message = POIGeoFenceServiceConstants.GET_POI_DETAILS__NORESULTFOUND_MSG;
                }
                return await Task.FromResult(objCategoryWisePOIResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                objCategoryWisePOIResponse.Code = Responsecode.Failed;
                objCategoryWisePOIResponse.Message = string.Format(POIGeoFenceServiceConstants.GET_POI_DETAILS_FAILURE_MSG, ex.Message);
            }
            return await Task.FromResult(objCategoryWisePOIResponse);
        }
        // END - Category

    }
}
