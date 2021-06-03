using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.poigeofences;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Category;
using net.atos.daf.ct2.portalservice.Entity.POI;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("poigeofence")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LandmarkCategoryController : BaseController
    {
        private readonly CategoryService.CategoryServiceClient _categoryServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly CategoryMapper _categoryMapper;
        private ILog _logger;

        public LandmarkCategoryController(CategoryService.CategoryServiceClient categoryServiceClient,
            AuditHelper auditHelper, AccountPrivilegeChecker privilegeChecker
            , IHttpContextAccessor _httpContextAccessor, SessionHelper sessionHelper) : base(_httpContextAccessor, sessionHelper, privilegeChecker)
        {
            _categoryServiceClient = categoryServiceClient;
            _auditHelper = auditHelper;
            _categoryMapper = new CategoryMapper();
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
        }


        [HttpPost]
        [Route("addcategory")]

        public async Task<IActionResult> AddCategory(AddCategoryRequest request)
        {
            try
            {
                if (request.Organization_Id == 0)
                {
                    bool hasRights = await HasAdminPrivilege();
                    if (!hasRights)
                        return StatusCode(400, "You cannot create global category.");
                }
                else
                {
                    request.Organization_Id = GetContextOrgId();
                }
                if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.IconName))
                {
                    return StatusCode(401, "invalid Category Name: The Category or Icon Name is Empty.");
                }

                if (string.IsNullOrEmpty(request.IconName) || request.Icon.Length <= 0)
                {
                    return StatusCode(401, "Icon Details is required ");
                }
                if (string.IsNullOrEmpty(request.Type))
                {
                    return StatusCode(401, "invalid Category Type.");
                }
                if (!string.IsNullOrEmpty(request.Type) && request.Type == "S" && request.Parent_Id == 0)
                {
                    return StatusCode(401, "Category Required.");
                }
                var MapRequest = _categoryMapper.MapCategory(request);
                var data = await _categoryServiceClient.AddCategoryAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                           "Category service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                           "AddCategory method in Category controller", data.CategoryID, data.CategoryID, JsonConvert.SerializeObject(request),
                                            Request);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                         "Category service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "AddCategory method in Landmark Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);

                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("editcategory")]

        public async Task<IActionResult> EditCategory(EditCategoryRequest request)
        {
            try
            {
                if (request.Id <= 0)
                {
                    return StatusCode(400, "Category id is required.");
                }
                if (request.Organization_Id == 0)
                {
                    bool hasRights = await HasAdminPrivilege();
                    if (!hasRights)
                        return StatusCode(400, "You cannot edit global category.");
                }
                else
                {
                    request.Organization_Id = GetContextOrgId();
                }
                var MapRequest = _categoryMapper.MapCategoryforEdit(request);
                var data = await _categoryServiceClient.EditCategoryAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                          "Category service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                          "EditCategory method in Category controller", data.CategoryID, data.CategoryID, JsonConvert.SerializeObject(request),
                                           Request);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.NotFound)
                {
                    return StatusCode(404, data.Message);
                }
                else if (data != null && data.Code == Responsecode.Conflict)
                {
                    return StatusCode(409, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                         "Category service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "EditCategory method in Landmark Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("deletecategory")]

        public async Task<IActionResult> DeleteCategory([FromQuery] DeleteCategoryRequest request)
        {
            try
            {
                bool hasRights = await HasAdminPrivilege();

                if (request.Id <= 0)
                {
                    return StatusCode(400, "Category id is required.");
                }
                var MapRequest = _categoryMapper.MapCategoryforDelete(request);
                var data = await _categoryServiceClient.DeleteCategoryAsync(MapRequest);
                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                         "Category service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                         "DeleteCategory method in Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                    return Ok(data);
                }
                else if (data != null && data.Code == Responsecode.NotFound)
                {
                    return StatusCode(404, data.Message);
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                         "Category service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "DeleteCategory method in Landmark Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpGet]
        [Route("getcategoryType")]

        public async Task<IActionResult> GetCategoryType([FromQuery] GetCategoryTypes request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Type) && request.Type.Length > 1)
                {
                    return StatusCode(400, "The category type is not valid. It should be of single character");
                }
                if (request.Organization_Id != 0)
                {
                    request.Organization_Id = GetContextOrgId();
                }
                var MapRequest = _categoryMapper.MapCategoryType(request);
                var data = await _categoryServiceClient.GetCategoryTypeAsync(MapRequest);

                if (data != null && data.Code == Responsecode.Success)
                {
                    if (data.Categories != null && data.Categories.Count > 0)
                    {
                        await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                        "Category service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                        "GetCategoryType method in Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                         Request);
                        return Ok(data);
                    }
                    else
                    {
                        return StatusCode(404, "Category type details are not found.");
                    }
                }
                else
                {
                    return StatusCode(500, data.Message);
                }

            }

            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                         "Category service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "GetCategoryType method in Landmark Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpGet]
        [Route("getcategoryDetails")]

        public async Task<IActionResult> GetCategoryDetails([FromQuery] GetRequest request)
        {
            try
            {
                var response = await _categoryServiceClient.GetCategoryDetailsAsync(request);


                if (response != null)
                {
                    if (response.Categories != null && response.Categories.Count > 0)
                    {
                        await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                        "Category service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                        "GetCategoryDetails method in Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                         Request);
                        return Ok(response);
                    }
                    else
                    {
                        return StatusCode(404, "Category details are not found.");
                    }
                }
                else
                {
                    return StatusCode(500, response.Message);
                }

            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                         "Category service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "GetCategoryDetails method in Landmark Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("deletebulkcategory")]
        public async Task<IActionResult> BulkDeleteCategory(DeleteCategory request)
        {
            DeleteResponse response = new DeleteResponse();
            try
            {
                _logger.Info("Delete Category .");

                DeleteRequest objlist = new DeleteRequest();

                //objlist.MultiCategoryID.Add(request.Ids);

                objlist = _categoryMapper.MapCategoryforBulkDelete(request);

                var data = await _categoryServiceClient.BulkDeleteCategoryAsync(objlist);

                if (data != null && data.Code == Responsecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                         "Category service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                         "BulkDeleteCategory method in Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                    return Ok(data);
                }
                else
                {
                    return data != null && data.Code == Responsecode.NotFound ? StatusCode(404, data.Message) : StatusCode(500, data.Message);
                }
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Landmark Category Component",
                                         "Category service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                         "DeleteCategory method in Landmark Category controller", 0, 0, JsonConvert.SerializeObject(request),
                                          Request);
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }

        }
    }
}
