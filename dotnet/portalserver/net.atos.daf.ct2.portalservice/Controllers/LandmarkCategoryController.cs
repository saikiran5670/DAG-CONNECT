using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.Entity.POI;
using System;
using net.atos.daf.ct2.poigeofences;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("poigeofence")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LandmarkCategoryController : ControllerBase
    {
        private readonly CategoryService.CategoryServiceClient _categoryServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly CategoryMapper _categoryMapper;
        public LandmarkCategoryController(CategoryService.CategoryServiceClient categoryServiceClient, AuditHelper auditHelper)
        {
            _categoryServiceClient = categoryServiceClient;
            _auditHelper = auditHelper;
            _categoryMapper = new CategoryMapper();
        }


        [HttpPost]
        [Route("addcategory")]
        [AllowAnonymous]
        public async Task<IActionResult> AddCategory(AddCategoryRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.IconName))
                {
                    return StatusCode(401, "invalid Category Name: The Category or Icon Name is Empty.");
                }
                if (request.Organization_Id <=0)
                {
                    return StatusCode(401, "invalid Organization_Id Please Enter Valid Organization ID.");
                }
                if (string.IsNullOrEmpty(request.IconName) || request.icon.Length <=0)
                {
                    return StatusCode(401, "Icon Details is required ");
                }
                var Request = _categoryMapper.MapCategory(request);
                var data = await _categoryServiceClient.AddCategoryAsync(Request);
                if (data != null && data.Code == Responcecode.Success)
                {
                    return Ok(data);
                }
                else if (data != null && data.Code == Responcecode.Conflict)
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
                //_logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpPut]
        [Route("editcategory")]
        [AllowAnonymous]
        public async Task<IActionResult> EditCategory(EditCategoryRequest request)
        {
            try
            {
                if (request.Id <= 0)
                {
                    return StatusCode(400, "Category id is required.");
                }
                var Request = _categoryMapper.MapCategoryforEdit(request);
                var data = await _categoryServiceClient.EditCategoryAsync(Request);
                if (data != null && data.Code == Responcecode.Success)
                {
                    return Ok(data);
                }
                else if (data != null && data.Code == Responcecode.NotFound)
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
                //_logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

        [HttpDelete]
        [Route("deletecategory")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCategory(DeleteCategoryRequest request)
        {
            try
            {
                if (request.Id <= 0)
                {
                    return StatusCode(400, "Category id is required.");
                }
                var Request = _categoryMapper.MapCategoryforDelete(request);
                var data = await _categoryServiceClient.DeleteCategoryAsync(Request);
                if (data != null && data.Code == Responcecode.Success)
                {
                    return Ok(data);
                }
                else if (data != null && data.Code == Responcecode.NotFound)
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
                //_logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpGet]
        [Route("getcategoryType")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryType([FromQuery]GetCategoryTypes request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Type) && request.Type.Length > 1)
                {
                    return StatusCode(400, "The category type is not valid. It should be of single character");
                }
                var Request = _categoryMapper.MapCategoryType(request);
                var data = await _categoryServiceClient.GetCategoryTypeAsync(Request);

                if (data != null && data.Code == Responcecode.Success)
                {
                    if (data.Categories != null && data.Categories.Count > 0)
                    {
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
                //_logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
        [HttpGet]
        [Route("getcategoryDetails")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryDetails([FromQuery] GetRequest Request)
        {
            try
            {
                    var response = await _categoryServiceClient.GetCategoryDetailsAsync(Request);


                    if (response != null)
                    {
                        if (response.Categories != null && response.Categories.Count > 0)
                        {
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
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }

    }
}
