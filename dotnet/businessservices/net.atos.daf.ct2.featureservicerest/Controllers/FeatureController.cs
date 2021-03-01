using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using System.Linq;
using net.atos.daf.ct2.featureservicerest.Entity;
using Newtonsoft.Json;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;

namespace net.atos.daf.ct2.featureservicerest.Controllers
{
     [ApiController]
    [Route("feature")]

    public class FeatureController :ControllerBase
    {
         private readonly ILogger logger;
        private readonly IFeatureManager featuresManager;
         IAuditTraillib auditlog;
        public FeatureController(IFeatureManager _featureManager,ILogger<IFeatureManager> _logger)
        {
            
            logger=_logger;
            featuresManager=_featureManager;
        } 
        

        [HttpGet]      
        [Route("getfeatures")]
        public async Task<IActionResult> GetFeatures(int RoleId, int OrganizationId,char featuretype)
        {    
               try
               { 
                    var feature = await featuresManager.GetFeatures(RoleId,OrganizationId,featuretype);

                    List<FeatureResponce> featureList =new List<FeatureResponce>();
                    foreach(var featureitem in feature)
                    {   
                        FeatureResponce obj  = new FeatureResponce();
                        obj.Id = featureitem.Id;
                        obj.CreatedBy = featureitem.Createdby;
                        obj.FeatureName = featureitem.Name;
                        obj.Description = featureitem.Description;
                        obj.RoleId = featureitem.RoleId;
                        obj.OrganizationId = featureitem.Organization_Id;
                        obj.FeatureType = featureitem.Type;
                        featureList.Add(obj);
                    }               
                  
                    return Ok(featureList);
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
               }
        }
         [HttpPost]      
        [Route("createfeatureset")]
        public async Task<int>  CreateFeatureSet(FeatureSet featureSet)
        {    
               try
               { 
                      featureSet.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
                      int featuresetid = await featuresManager.AddFeatureSet(featureSet);
                      auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Feature Component","Feature Service",AuditTrailEnum.Event_type.DELETE,AuditTrailEnum.Event_status.SUCCESS,"AddFeatureSet method in Feature manager",featureSet.FeatureSetID,featureSet.FeatureSetID,JsonConvert.SerializeObject(featureSet.FeatureSetID));
                    return featuresetid;
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                     isValidcheck(500,"Internal Server Error.");
                      throw ex;
               }
        }
        [HttpDelete]      
        [Route("deletefeatureset")]
        public async Task<bool> DeleteFeatureSet(int FeatureSetId)
        {    
               try
               { 
                    if (FeatureSetId == 0) 
                {
                    isValidcheck (400,"FeatureSet id is required ");
                }
                    var feature = await featuresManager.DeleteFeatureSet( FeatureSetId);
                    auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Feature Component","Feature Service",AuditTrailEnum.Event_type.DELETE,AuditTrailEnum.Event_status.SUCCESS,"Delete method in Feature manager",FeatureSetId,FeatureSetId,JsonConvert.SerializeObject(FeatureSetId));
                    return true;
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                    isValidcheck(500,"Internal Server Error.");
                     throw ex;
               }
        }

        public async Task<IActionResult> isValidcheck(int code, string msg) {

                return  StatusCode(code, msg);
        }

        
         [HttpPost]      
        [Route("createdataattributset")]
        public async Task<int>  CreateDataattributeSet(DataAttributeSet dataAttributeSet)
        {    
               try
               { 
                     // dataAttributeSet.Name = "DataAttributeSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
                      int dataattributesetid = await featuresManager.CreateDataattributeSet(dataAttributeSet);
                      auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Feature Component","Feature Service",AuditTrailEnum.Event_type.DELETE,AuditTrailEnum.Event_status.SUCCESS,"AddFeatureSet method in Feature manager",dataAttributeSet.ID,dataAttributeSet.ID,JsonConvert.SerializeObject(dataAttributeSet.ID));
                    return dataattributesetid;
               } 
               catch(Exception ex)
               {
                    logger.LogError(ex.Message +" " +ex.StackTrace);
                     isValidcheck(500,"Internal Server Error.");
                      throw ex;
               }
        }
     //     public async Task<int>  UpdatedataattributeSet(DataAttributeSet dataAttributeSet)
     //    { 
     //         try{

     //         } 
     //         catch (Exception ex)
     //         {
     //              throw ex;
     //         }
     //    }
     //     public async Task<int>  DeleteDataAttribute(DataAttributeSet dataAttributeSet)
     //    { 
     //         try{

     //         } 
     //         catch (Exception ex)
     //         {
     //              throw ex;
     //         }
     //    }
     //     public async Task<int>  CreateDataattributeSetFeature(DataAttributeSet dataAttributeSet)
     //    { 
     //         try{

     //         } 
     //         catch (Exception ex)
     //         {
     //              throw ex;
     //         }
     //    }
     //    public async Task<int>  UpdatedataattributeSetFeature(DataAttributeSet dataAttributeSet)
     //    { 
     //         try{

     //         } 
     //         catch (Exception ex)
     //         {
     //              throw ex;
     //         }
     //    }
     //    public async Task<int>  DeleteDataAttributeSetFeature(DataAttributeSet dataAttributeSet)
     //    { 
     //         try{

     //         } 
     //         catch (Exception ex)
     //         {
     //              throw ex;
     //         }
     //    }
     //    public async Task<int>  DeleteDataAttributeSetFeature(DataAttributeSet dataAttributeSet)
     //    { 
     //         try{

     //         } 
     //         catch (Exception ex)
     //         {
     //              throw ex;
     //         }
     //    }
    }
}
