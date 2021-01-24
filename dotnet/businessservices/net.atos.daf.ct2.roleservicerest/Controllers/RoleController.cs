using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.role.repository;
using RoleEntity=net.atos.daf.ct2.role.entity;
using RoleComponent=net.atos.daf.ct2.role;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;

namespace net.atos.daf.ct2.roleservicerest.Controllers
{
    [ApiController]
    [Route("role")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger logger;
        RoleComponent.IRoleManagement roleManager;
        private readonly IFeatureManager featuresManager;
        public RoleController(RoleComponent.IRoleManagement _roleManager,IFeatureManager _featureManager,ILogger<RoleController> _logger)
        {
            roleManager =_roleManager;
            logger=_logger;
            featuresManager=_featureManager;
        } 
       
        [HttpPost]      
        [Route("create")]
        public async Task<IActionResult> Create(RoleEntity.RoleMaster roleMaster)
        {    
               try
               {
                    RoleEntity.RoleMaster ObjRole = new RoleEntity.RoleMaster();
                    ObjRole.Organization_Id =roleMaster.Organization_Id;
                    ObjRole.Name = roleMaster.Name;
                    ObjRole.Createdby = roleMaster.Createdby;
                    ObjRole.Description = roleMaster.Description;
                    ObjRole.Feature_set_id=0;                  
                    
                    ObjRole.FeatureSet = new FeatureSet();
                    ObjRole.FeatureSet.Features = new List<Feature>();
                    foreach(var item in roleMaster.FeatureSet.Features)
                    {
                        ObjRole.FeatureSet.Features.Add(new Feature() { Id = item.Id });
                    }
                    int roleId = await roleManager.CreateRole(ObjRole);
                   return Ok(roleId);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
               }
        }

        [HttpPost]      
        [Route("update")]
        public async Task<IActionResult> Update(RoleEntity.RoleMaster roleMaster)
        {    
               try
               {
                    RoleEntity.RoleMaster ObjRole = new RoleEntity.RoleMaster();                   
                    ObjRole.Name = roleMaster.Name;
                    ObjRole.Id = roleMaster.Id;
                    ObjRole.Description=roleMaster.Description;
                    ObjRole.Updatedby = roleMaster.Updatedby;                    
                    int roleId = await roleManager.UpdateRole(ObjRole);
                    logger.LogInformation(roleId+"Role Master Updated");
                    return Ok(roleId);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
               }
        }
        
        [HttpPost]      
        [Route("delete")]
        public async Task<IActionResult> Delete(int roleId, int updatedby)
        {    
               try
               {                           
                    int role_Id = await roleManager.DeleteRole(roleId,updatedby);
                    return Ok(role_Id);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
               }
        }

        [HttpPost]      
        [Route("get")]
        public async Task<IActionResult> Get(RoleEntity.RoleFilter roleFilter)
        {    
               try
               { 
                    var role = await roleManager.GetRoles(roleFilter); 
                    List<RoleEntity.RoleMaster> roleList =new List<RoleEntity.RoleMaster>();
                    foreach(var roleitem in role)
                    {
                         roleList.Add(roleitem);
                    }
                                 
                  
                    return Ok(roleList);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
               }
        }

        [HttpGet]      
        [Route("getfeatures")]
        public async Task<IActionResult> GetFeatures(char featuretype,bool active)
        {    
               try
               { 
                    var feature = await featuresManager.GetFeatures(featuretype,active);  

                    List<Feature> featureList =new List<Feature>();
                    foreach(var featureitem in feature)
                    {
                         featureList.Add(featureitem);
                    }               
                  
                    return Ok(featureList);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
               }
        }
    }
}
