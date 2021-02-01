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
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.audit;
using Newtonsoft.Json;
using net.atos.daf.ct2.audit.Enum;

namespace net.atos.daf.ct2.roleservicerest.Controllers
{
    [ApiController]
    [Route("role")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger logger;
        RoleComponent.IRoleManagement roleManager;
        private readonly IFeatureManager featuresManager;
        IAuditTraillib auditlog;
        public RoleController(RoleComponent.IRoleManagement _roleManager,IFeatureManager _featureManager,ILogger<RoleController> _logger,IAuditTraillib _auditlog)
        {
            roleManager =_roleManager;
            logger=_logger;
            featuresManager=_featureManager;
            auditlog = _auditlog;
        } 
       
        [HttpPost]      
        [Route("create")]
        public async Task<IActionResult> Create(Rolerequest roleMaster)
        {    
               try
               {

                 if ((string.IsNullOrEmpty(roleMaster.RoleName.Trim())) )
                {
                    return StatusCode(400, "Role name  is required");
                }
                if(roleMaster.FeatureIds.Length ==  0 )
                {
                      return StatusCode(400, "Feature Ids are required.");
                }
                int Rid=  roleManager.CheckRoleNameExist(roleMaster.RoleName.Trim(),roleMaster.OrganizationId,0);
                    if(Rid > 0)
                    {
                            return StatusCode(400, "Role name already exist.");
                    }
                    
                    RoleEntity.RoleMaster ObjRole = new RoleEntity.RoleMaster();
                    ObjRole.Organization_Id =roleMaster.OrganizationId;
                    ObjRole.Name = roleMaster.RoleName.Trim();
                    ObjRole.Createdby = roleMaster.Createdby;                    
                    if(ObjRole.Description!= null || string.IsNullOrEmpty(ObjRole.Description))
                    ObjRole.Description=roleMaster.Description.Trim();
                    else ObjRole.Description=string.Empty;

                    ObjRole.Feature_set_id=0;                  
                    
                    ObjRole.FeatureSet = new FeatureSet();
                    ObjRole.FeatureSet.Features = new List<Feature>();
                    foreach(var item in roleMaster.FeatureIds)
                    {
                        ObjRole.FeatureSet.Features.Add(new Feature() { Id = item });
                    }
                    int roleId = await roleManager.CreateRole(ObjRole);
                    auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Role Component","Role Service",AuditTrailEnum.Event_type.CREATE,AuditTrailEnum.Event_status.SUCCESS,"Create method in Role manager",0,roleId,JsonConvert.SerializeObject(roleMaster));
                   return Ok(roleId);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Role Component","Role Service",AuditTrailEnum.Event_type.CREATE,AuditTrailEnum.Event_status.FAILED,"Create method in Role manager",0,0,JsonConvert.SerializeObject(roleMaster));
                     if (ex.Message.Contains("foreign key"))
                    {
                        return StatusCode(400, "The foreign key violation in one of dependant data.");
                    } 
                    return StatusCode(500,"Internal Server Error.");
               }
        }

        [HttpPost]      
        [Route("update")]
        public async Task<IActionResult> Update(Roleupdaterequest roleMaster)
        {    
               if ((string.IsNullOrEmpty(roleMaster.RoleName.Trim()))  ||(roleMaster.RoleId == 0)  )
                {
                    return StatusCode(400, "Role name and Role Id required Roleid required");
                }
                if(roleMaster.FeatureIds.Length ==  0 )
                {
                      return StatusCode(400, "Feature Ids required.");
                }
                int Rid=  roleManager.CheckRoleNameExist(roleMaster.RoleName.Trim(),roleMaster.OrganizationId,roleMaster.RoleId);
                    if(Rid > 0)
                    {
                            return StatusCode(400, "Role name already exist.");
                    }
               try
               {
                    RoleEntity.RoleMaster ObjRole = new RoleEntity.RoleMaster();                   
                    ObjRole.Name = roleMaster.RoleName.Trim();
                    ObjRole.Id = roleMaster.RoleId;
                    if(ObjRole.Description!= null || string.IsNullOrEmpty(ObjRole.Description))
                    ObjRole.Description=roleMaster.Description.Trim();
                    else ObjRole.Description=string.Empty;

                    ObjRole.Updatedby = roleMaster.Updatedby;        
                    ObjRole.FeatureSet = new FeatureSet();
                    ObjRole.FeatureSet.Features = new List<Feature>();
                    foreach(var item in roleMaster.FeatureIds)
                    {
                        ObjRole.FeatureSet.Features.Add(new Feature() { Id = item });
                    }            
                    int roleId = await roleManager.UpdateRole(ObjRole);
                    auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Role Component","Role Service",AuditTrailEnum.Event_type.UPDATE,AuditTrailEnum.Event_status.SUCCESS,"Create method in Role manager",roleMaster.RoleId,0,JsonConvert.SerializeObject(roleMaster));
                    logger.LogInformation(roleId+"Role Master Updated");
                    return Ok(roleId);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Role Component","Role Service",AuditTrailEnum.Event_type.UPDATE,AuditTrailEnum.Event_status.FAILED,"Create method in Role manager",roleMaster.RoleId,0,JsonConvert.SerializeObject(roleMaster));
                     if (ex.Message.Contains("foreign key"))
                    {
                        return StatusCode(400, "The foreign key violation in one of dependant data.");
                    } 
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
                     auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Role Component","Role Service",AuditTrailEnum.Event_type.DELETE,AuditTrailEnum.Event_status.SUCCESS,"Delete method in Role manager",roleId,roleId,JsonConvert.SerializeObject(roleId));
                    return Ok(role_Id);
               } 
               catch(Exception ex)
               {
                    await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Role Component","Role Service",AuditTrailEnum.Event_type.DELETE,AuditTrailEnum.Event_status.FAILED,"Create method in Role manager",0,0,JsonConvert.SerializeObject(roleId));
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
               }
        }

        [HttpGet]      
        [Route("get")]
        public async Task<IActionResult> Get(int Roleid, int? Organizationid,bool IsGlobal)
        {    
               try
               { 
                   if(string.IsNullOrEmpty(IsGlobal.ToString()))
                    {
                         return StatusCode(400, "Is global role filter is required");
                    }
                   
                    RoleFilter obj = new RoleFilter();
                    obj.RoleId = Roleid;
                    obj.Organization_Id = Organizationid == null ? 0 : Convert.ToInt32(Organizationid);
                    obj.IsGlobal = IsGlobal;
                    if(obj.Organization_Id == 0 && IsGlobal == false && obj.RoleId <= 0)
                    {
                         return StatusCode(400, "Organization id required ");
                    }

                    var role = await roleManager.GetRoles(obj); 
                    List<Rolerequest> roleList =new List<Rolerequest>();
                    foreach(var roleitem in role)
                    {
                         Rolerequest Robj = new Rolerequest();
                         Robj.RoleId = roleitem.Id;
                         Robj.RoleName = roleitem.Name;
                         Robj.Description = roleitem.Description;
                         Robj.OrganizationId = roleitem.Organization_Id ?? 0;
                         Robj.FeatureIds = roleitem.FeatureSet.Features.Select(i=> i.Id).ToArray();
                         roleList.Add(Robj);
                    }
                                 
                  
                    return Ok(roleList);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500, ex.ToString());
               }
        }

        // [HttpGet]      
        // [Route("getfeatures")]
        // public async Task<IActionResult> GetFeatures(char featuretype,bool active)
        // {    
        //        try
        //        { 
        //             var feature = await featuresManager.GetFeatures(featuretype,active);  

        //             List<Feature> featureList =new List<Feature>();
        //             foreach(var featureitem in feature)
        //             {
        //                  featureList.Add(featureitem);
        //             }               
                  
        //             return Ok(featureList);
        //        } 
        //        catch(Exception ex)
        //        {
        //             logger.LogError(ex.Message +" " +ex.StackTrace);
        //             return StatusCode(500,"Internal Server Error.");
        //        }
        // }
    }
}
