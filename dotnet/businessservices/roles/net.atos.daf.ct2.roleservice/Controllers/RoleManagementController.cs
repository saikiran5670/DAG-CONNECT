using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using net.atos.daf.ct2.role.entity;
//using net.atos.daf.ct2.rolerepository;
using net.atos.daf.ct2.role.repository;
namespace net.atos.daf.ct2.roleservice.Controllers
{
     [ApiController]
    [Route("[controller]")]
    public class RoleManagementController : ControllerBase
    {
        private readonly ILogger<RoleManagementController> logger;
        private readonly IRoleManagement roleManagement;
        private readonly IFeature feature;
        public RoleManagementController(IRoleManagement _roleManagement, IFeature _feature,ILogger<RoleManagementController> _logger)
        {
            roleManagement = _roleManagement;
            feature=_feature;
            logger = _logger;
        }


        [HttpPost]
        [Route("AddRole")]
        public async Task<IActionResult> AddRole(RoleMaster roleMaster)
        {
            try
            {
                if (string.IsNullOrEmpty(roleMaster.Name.ToString()))
                {
                    return StatusCode(501, "The role name is Empty.");
                }
                logger.LogInformation("AddRole function called -" + roleMaster.createdby);
                int roleId = await roleManagement.AddRole(roleMaster);
                logger.LogInformation("Role added with Role Id - " + roleId);
                return Ok(roleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");

            }
        }

        [HttpPut]
        [Route("UpdateRole")]
        public async Task<IActionResult>  UpdateRole(RoleMaster roleMaster)
        {
            try
            {
               
                if (string.IsNullOrEmpty(roleMaster.RoleMasterId.ToString()) || roleMaster.RoleMasterId == 0)
                {
                    return StatusCode(501, "The role id is Empty.");
                }
                if (string.IsNullOrEmpty(roleMaster.Name.ToString()))
                {
                    return StatusCode(501, "The role name is Empty.");
                }
                logger.LogInformation("UpdateRole function called -" + roleMaster.modifiedby);
                int roleId = await roleManagement.UpdateRole(roleMaster);
                logger.LogInformation("Role updated with role Id - " + roleId);

                return Ok(roleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("DeleteRole")]
        public async Task<IActionResult> DeleteRole(int roleId, int userId)
        {
            try
            {
                
                if (string.IsNullOrEmpty(roleId.ToString()))
                {
                    return StatusCode(501, "The roleId is Empty.");
                }

                if (string.IsNullOrEmpty(userId.ToString()))
                {
                    return StatusCode(501, "The userId is Empty.");
                }
                logger.LogInformation("DeleteRole function called " + roleId + " userId -" + userId);
                int DeletedroleId = await roleManagement.DeleteRole(roleId, userId);
                logger.LogInformation("Role deleted with Role Id - " + roleId);

                return Ok(DeletedroleId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("GetRoles")]
        public async Task<IActionResult> GetRoles(int roleId)
        {
            try
            {
                if (string.IsNullOrEmpty(roleId.ToString()))
                {
                    return StatusCode(501, "The roleId is Empty.");
                }
                logger.LogInformation("GetRoles function called " + roleId);
                return Ok(await roleManagement.GetRoles(roleId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("CheckRoleNameExist")]
        public async Task<IActionResult> CheckRoleNameExist(string roleName)
        {
            try
            {
              
                if (string.IsNullOrEmpty(roleName))
                {
                    return StatusCode(501, "The roleName is Empty.");
                }
                logger.LogInformation("CheckRoleNameExist function called " + roleName);
                return Ok(await roleManagement.CheckRoleNameExist(roleName));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    
    
        [HttpPost]
        [Route("AddFeatureType")]
        public async Task<IActionResult> AddFeatureType(FeatureType featureType)
        {
            try
            {
                if (string.IsNullOrEmpty(featureType.FeatureTypeDescription.ToString()))
                {
                    return StatusCode(501, "The feature type is Empty.");
                }
                logger.LogInformation("AddFeatureType function called -" + featureType.createdby);
                int FeatureTypeId = await feature.AddFeatureType(featureType);
                logger.LogInformation("Feature Type added with Id - " + FeatureTypeId);
                return Ok(FeatureTypeId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");

            }
        }
        
        [HttpPut]
        [Route("UpdateFeatureType")]
        public async Task<IActionResult> UpdateFeatureType(FeatureType featureType)
        {
            try
            {
               
                if (string.IsNullOrEmpty(featureType.RoleFeatureTypeId.ToString()) || featureType.RoleFeatureTypeId == 0)
                {
                    return StatusCode(501, "The feature type id is Empty.");
                }

                if (string.IsNullOrEmpty(featureType.FeatureTypeDescription.ToString()))
                {
                    return StatusCode(501, "The feature type is Empty.");
                }
                logger.LogInformation("UpdateFeatureType function called -" + featureType.modifiedby);
                int FeatureTypeId = await feature.UpdateFeatureType(featureType);
                logger.LogInformation("Feature type updated with Id - " + FeatureTypeId);

                return Ok(FeatureTypeId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    
        [HttpPut]
        [Route("DeleteFeatureType")]
        public async Task<IActionResult> DeleteFeatureType(int FeatureTypeId, int Userid)
        {
            try
            {
                
                if (string.IsNullOrEmpty(FeatureTypeId.ToString()))
                {
                    return StatusCode(501, "The FeatureTypeId is Empty.");
                }

                if (string.IsNullOrEmpty(Userid.ToString()))
                {
                    return StatusCode(501, "The userId is Empty.");
                }
                logger.LogInformation("DeleteFeatureType function called " + FeatureTypeId + " UserId -" + Userid);
                int DeletedFeatureTypeId = await feature.DeleteFeatureType(FeatureTypeId, Userid);
                logger.LogInformation("Feature type deleted with Id - " + DeletedFeatureTypeId);

                return Ok(DeletedFeatureTypeId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    
        [HttpGet]
        [Route("GetFeatureType")]
        public async Task<IActionResult> GetFeatureType(int FeatureTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(FeatureTypeId.ToString()))
                {
                    return StatusCode(501, "The FeatureTypeId is Empty.");
                }
                logger.LogInformation("GetFeatureType function called " + FeatureTypeId);
                return Ok(await feature.GetFeatureType(FeatureTypeId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("CheckFeatureTypeExist")]
        public async Task<IActionResult> CheckFeatureTypeExist(string FeatureType)
        {
            try
            {
              
                if (string.IsNullOrEmpty(FeatureType))
                {
                    return StatusCode(501, "The Feature Type is Empty.");
                }
                logger.LogInformation("CheckFeatureTypeExist function called " + FeatureType);
                return Ok(await feature.CheckFeatureTypeExist(FeatureType));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    
    
        // [HttpPost]
        // [Route("AddFeature")]
        // public async Task<IActionResult> AddFeature(Feature feature)
        // {
        //     try
        //     {
        //         if (string.IsNullOrEmpty(feature.FeatureDescription.ToString()))
        //         {
        //             return StatusCode(501, "Feature name is Empty.");
        //         }

        //         if (string.IsNullOrEmpty(feature.RoleFeatureTypeId.ToString()))
        //         {
        //             return StatusCode(501, "Feature Type is not selected.");
        //         }

        //         if (string.IsNullOrEmpty(feature.SeqNum.ToString()))
        //         {
        //             return StatusCode(501, "Feature sequence is not entered.");
        //         }

        //         logger.LogInformation("AddFeature function called -" + feature.createdby);
        //         int FeatureId = await feature.AddFeature(feature);
        //         logger.LogInformation("Feature added with Id - " + FeatureId);
        //         return Ok(FeatureId);
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex.Message);
        //         return StatusCode(500, "Internal Server Error.");
        //     }
        // }

        // [HttpPut]
        // [Route("UpdateFeature")]
        // public async Task<IActionResult> UpdateFeature(Feature feature)
        // {
        //     try
        //     {
               
        //         if (string.IsNullOrEmpty(feature.RoleFeatureId.ToString()) || feature.RoleFeatureId == 0)
        //         {
        //             return StatusCode(501, "The feature id is Empty.");
        //         }

        //         if (string.IsNullOrEmpty(feature.FeatureDescription.ToString()))
        //         {
        //             return StatusCode(501, "Feature name is Empty.");
        //         }

        //         if (string.IsNullOrEmpty(feature.RoleFeatureTypeId.ToString()))
        //         {
        //             return StatusCode(501, "Feature Type is not selected.");
        //         }

        //         if (string.IsNullOrEmpty(feature.SeqNum.ToString()))
        //         {
        //             return StatusCode(501, "Feature sequence is not entered.");
        //         }
                
        //         logger.LogInformation("UpdateFeature function called -" + feature.modifiedby);
        //         int FeatureId = await feature.UpdateFeature(feature);
        //         logger.LogInformation("Feature updated with Id - " + FeatureId);

        //         return Ok(FeatureId);
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex.Message);
        //         return StatusCode(500, "Internal Server Error.");
        //     }
        // }

        [HttpPut]
        [Route("DeleteFeature")]
        public async Task<IActionResult> DeleteFeature(int RoleFeatureId, int UserId)
        {
            try
            {
                
                if (string.IsNullOrEmpty(RoleFeatureId.ToString()))
                {
                    return StatusCode(501, "The Feature Id is Empty.");
                }

                if (string.IsNullOrEmpty(UserId.ToString()))
                {
                    return StatusCode(501, "The userId is Empty.");
                }
                logger.LogInformation("DeleteFeature function called " + RoleFeatureId + " UserId -" + UserId);
                int DeletedFeatureId = await feature.DeleteFeature(RoleFeatureId, UserId);
                logger.LogInformation("Feature deleted with Id - " + DeletedFeatureId);

                return Ok(DeletedFeatureId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    
   
        [HttpGet]
        [Route("CheckFeatureExist")]
        public async Task<IActionResult> CheckFeatureExist(string FeatureName)
        {
            try
            {
              
                if (string.IsNullOrEmpty(FeatureName))
                {
                    return StatusCode(501, "The Feature is Empty.");
                }
                logger.LogInformation("CheckFeatureExist function called " + FeatureName);
                return Ok(await feature.CheckFeatureExist(FeatureName));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        // [HttpGet]
        // [Route("GetFeature")]
        // public async Task<IActionResult> GetFeature(int RoleFeatureId)
        // {
        //     try
        //     {
        //         if (string.IsNullOrEmpty(RoleFeatureId.ToString()))
        //         {
        //             return StatusCode(501, "The RoleFeatureId is Empty.");
        //         }
        //         logger.LogInformation("GetFeature function called " + RoleFeatureId);
        //         return Ok(await feature.GetFeature(RoleFeatureId));
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex.Message);
        //         return StatusCode(500, "Internal Server Error.");
        //     }
        // }
  

        [HttpPost]
        [Route("AddFeatureSet")]
        public async Task<IActionResult> AddFeatureSet(FeatureSet featureSet)
        {
            try
            {
                if (string.IsNullOrEmpty(featureSet.FeatureSetName.ToString()))
                {
                    return StatusCode(501, "The feature is Empty.");
                }
                logger.LogInformation("AddFeatureSet function called -" + featureSet.createdby);
                int FeatureSetId = await feature.AddFeatureSet(featureSet);
                logger.LogInformation("FeatureSet added with Id - " + FeatureSetId);
                return Ok(FeatureSetId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("UpdateFeatureSet")]
        public async Task<IActionResult> UpdateFeatureSet(FeatureSet featureSet)
        {
            try
            {
               
                if (string.IsNullOrEmpty(featureSet.FeatureSetID.ToString()) || featureSet.FeatureSetID == 0)
                {
                    return StatusCode(501, "The featureSet id is Empty.");
                }

                if (string.IsNullOrEmpty(featureSet.FeatureSetName.ToString()))
                {
                    return StatusCode(501, "The featureSet Name is Empty.");
                }
                logger.LogInformation("UpdateFeatureSet function called -" + featureSet.modifiedby);
                int FeatureSetd = await feature.UpdateFeatureSet(featureSet);
                logger.LogInformation("FeatureSet updated with Id - " + FeatureSetd);

                return Ok(FeatureSetd);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }
  
    
        [HttpPut]
        [Route("DeleteFeatureSet")]
        public async Task<IActionResult> DeleteFeatureSet(int FeatureSetId, int UserId)
        {
            try
            {
                
                if (string.IsNullOrEmpty(FeatureSetId.ToString()))
                {
                    return StatusCode(501, "The FeatureSet Id is Empty.");
                }

                if (string.IsNullOrEmpty(UserId.ToString()))
                {
                    return StatusCode(501, "The userId is Empty.");
                }
                logger.LogInformation("DeleteFeatureSet function called " + FeatureSetId + " UserId -" + UserId);
                int DeletedFeatureSetId = await feature.DeleteFeatureSet(FeatureSetId, UserId);
                logger.LogInformation("FeatureSet deleted with Id - " + DeletedFeatureSetId);

                return Ok(DeletedFeatureSetId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

    
        [HttpGet]
        [Route("CheckFeatureSetExist")]
        public async Task<IActionResult> CheckFeatureSetExist(string FeatureSetName)
        {
            try
            {
              
                if (string.IsNullOrEmpty(FeatureSetName))
                {
                    return StatusCode(501, "The FeatureSet is Empty.");
                }
                logger.LogInformation("CheckFeatureSetExist function called " + FeatureSetName);
                return Ok(await feature.CheckFeatureSetExist(FeatureSetName));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("GetFeatureSet")]
        public async Task<IActionResult> GetFeatureSet(int FeatureSetId)
        {
            try
            {
                if (string.IsNullOrEmpty(FeatureSetId.ToString()))
                {
                    return StatusCode(501, "The FeatureSetId is Empty.");
                }
                logger.LogInformation("GetFeatureSet function called " + FeatureSetId);
                return Ok(await feature.GetFeatureSet(FeatureSetId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }
   
        [HttpGet]
        [Route("GetFeatureSetFeature")]
        public async Task<IActionResult> GetFeatureSetFeature(int FeatureSetId)
        {
            try
            {
                if (string.IsNullOrEmpty(FeatureSetId.ToString()))
                {
                    return StatusCode(501, "The FeatureSetId is Empty.");
                }
                logger.LogInformation("GetFeatureSet function called " + FeatureSetId);
                return Ok(await feature.GetFeatureSetFeature(FeatureSetId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

    }
}
