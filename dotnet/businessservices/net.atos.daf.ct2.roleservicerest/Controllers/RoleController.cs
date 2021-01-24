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
using net.atos.daf.ct2.roleservicerest.Entity;
using System.Linq;

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
        public async Task<IActionResult> Create(Rolerequest roleMaster)
        {    
               try
               {

                 if ((string.IsNullOrEmpty(roleMaster.RoleName)) || (roleMaster.OrganizationId == 0) )
                {
                    return StatusCode(400, "Role name and organization Id required.");
                }
                if(roleMaster.FeatureIds.Length ==  0 )
                {
                      return StatusCode(400, "Feature Ids required.");
                }
                    RoleEntity.RoleMaster ObjRole = new RoleEntity.RoleMaster();
                    ObjRole.Organization_Id =roleMaster.OrganizationId;
                    ObjRole.Name = roleMaster.RoleName;
                    ObjRole.Createdby = roleMaster.Createdby;
                    ObjRole.Description = roleMaster.Description;
                    ObjRole.Feature_set_id=0;                  
                    
                    ObjRole.FeatureSet = new FeatureSet();
                    ObjRole.FeatureSet.Features = new List<Feature>();
                    foreach(var item in roleMaster.FeatureIds)
                    {
                        ObjRole.FeatureSet.Features.Add(new Feature() { Id = item });
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
        public async Task<IActionResult> Update(Roleupdaterequest roleMaster)
        {    
               if ((string.IsNullOrEmpty(roleMaster.RoleName)) || (roleMaster.OrganizationId == 0) ||(roleMaster.RoleId == 0)  )
                {
                    return StatusCode(400, "Role name and organization Id required Roleid required");
                }
                if(roleMaster.FeatureIds.Length ==  0 )
                {
                      return StatusCode(400, "Feature Ids required.");
                }
               try
               {
                    RoleEntity.RoleMaster ObjRole = new RoleEntity.RoleMaster();                   
                    ObjRole.Name = roleMaster.RoleName;
                    ObjRole.Id = roleMaster.RoleId;
                    ObjRole.Description=roleMaster.Description;
                    ObjRole.Updatedby = roleMaster.Updatedby;        
                    ObjRole.FeatureSet = new FeatureSet();
                    ObjRole.FeatureSet.Features = new List<Feature>();
                    foreach(var item in roleMaster.FeatureIds)
                    {
                        ObjRole.FeatureSet.Features.Add(new Feature() { Id = item });
                    }            
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
                if (roleId == 0) 
                {
                    return StatusCode(400, "Role id required ");
                }
                                    
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
                 if (roleFilter.RoleId == 0 && roleFilter.Organization_Id == 0) 
                {
                    return StatusCode(400, "Role or organization Id required ");
                }
                    var role = await roleManager.GetRoles(roleFilter); 
                    List<Rolerequest> roleList =new List<Rolerequest>();
                    foreach(var roleitem in role)
                    {
                         Rolerequest obj = new Rolerequest();
                         obj.RoleId = roleitem.Id;
                         obj.RoleName = roleitem.Name;
                         obj.Description = roleitem.Description;
                         obj.OrganizationId = roleitem.Organization_Id ?? 0;
                         obj.FeatureIds = roleitem.FeatureSet.Features.Select(i=> i.Id).ToArray();
                         roleList.Add(obj);
                    }
                                 
                  
                    return Ok(roleList);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500, ex.ToString());
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
