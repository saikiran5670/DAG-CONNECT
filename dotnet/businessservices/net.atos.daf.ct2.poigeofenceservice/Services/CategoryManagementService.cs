using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofences;
using net.atos.daf.ct2.poigeofenceservice;
using net.atos.daf.ct2.poigeofenceservice.Entity;

namespace net.atos.daf.ct2.poigeofenservice
{
    public class CategoryManagementService : CategoryService.CategoryServiceBase
    {
        private ILog _logger;
       // private readonly Mapper _mapper;
        private readonly ICategoryManager _categoryManager;
        public CategoryManagementService(IPoiManager poiManager, ICategoryManager categoryManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
          //  _mapper = new Mapper();
            _categoryManager = categoryManager;

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
                obj.Created_By = request.CreatedBy;
                obj.icon = request.Icon.ToByteArray();

                var result = await _categoryManager.AddCategory(obj);
                if (result != null)
                {
                    response.Message = "Added successfully";
                    response.Code = Responcecode.Success;
                }
                else 
                {
                    response.Message = "Add category Fail";
                    response.Code = Responcecode.Failed;
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
                obj.icon = request.Icon.ToByteArray();
                obj.Modified_By = request.ModifiedBy;

                var result = await _categoryManager.EditCategory(obj);
                if (result != null)
                {
                    response.Message = "Edit successfully";
                    response.Code = Responcecode.Success;
                }
                else
                {
                    response.Message = "Edit category Fail";
                    response.Code = Responcecode.Failed;
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
                if (result )
                {
                    response.Message = "Delete successfully";
                    response.Code = Responcecode.Success;
                }
                if (!result)
                {
                    response.Message = "Delete category Fail";
                    response.Code = Responcecode.Failed;
                }

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

                var result = await _categoryManager.GetCategory(request.Type);
                foreach (var item in result)
                {
                    var Data = new GetCategoryType();
                    Data.Id = item.Id;
                    //Data.OrganizationId = item.Organization_Id;
                    Data.Name = item.Name;
                    //Data.IconId = item.Icon_Id;
                   // Data.Type = item.Type;
                    //Data.ParentId = item.Parent_Id;
                    //Data.State = item.State;
                    //Data.CreatedAt = item.Created_At;
                    //Data.CreatedBy = item.Created_By;
                    //Data.ModifiedAt = item.Modified_At;
                    //Data.ModifiedBy = item.Modified_By;
                    response.Categories.Add(Data);
                }
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
            }
            return await Task.FromResult(response);
        }
        // END - Category
    }
}
