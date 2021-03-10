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
using FeatureComponent = net.atos.daf.ct2.features;


namespace net.atos.daf.ct2.featureservicerest.Controllers
{
     [ApiController]
    [Route("feature")]

    public class FeatureController :ControllerBase
    {
        private readonly ILogger<FeatureController> _logger;
        private readonly IFeatureManager _FeatureManager;
       // private readonly IGroupManager _groupManager;
       // private readonly AccountComponent.IAccountManager accountmanager;
        IAuditTraillib _auditlog;

        private string FK_Constraint = "violates foreign key constraint";
        public FeatureController(ILogger<FeatureController> logger, IFeatureManager featureManager, IAuditTraillib auditlog)
        {
            _logger = logger;
            _FeatureManager = featureManager;
            _auditlog = auditlog;

        } 
        

        [HttpGet]      
        [Route("getfeatures")]
        public async Task<IActionResult> GetFeatures(int RoleId, int OrganizationId,char featuretype)
        {    
               try
               { 
                    var feature = await _FeatureManager.GetFeatures(RoleId,OrganizationId,0,0,featuretype);

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
                _logger.LogError(ex.Message +" " +ex.StackTrace);
                    return StatusCode(500,"Internal Server Error.");
               }
        }
         [HttpPost]      
        [Route("createfeatureset")]
        public async Task<IActionResult>  CreateFeatureSet(FeatureSet featureSetRequest)
        {    
               try
               {
                _logger.LogInformation("Create method in FeatureSet API called.");


                if (string.IsNullOrEmpty(featureSetRequest.Name))
                {
                    return StatusCode(401, "invalid featureSet Name: The featureSet Name is Empty.");
                }
                if (string.IsNullOrEmpty(featureSetRequest.description))
                {
                    return StatusCode(401, "invalid FeatureSet Description : Feature Description is Empty.");
                }
                FeatureSet ObjResponse = new FeatureSet();
                FeatureSet featureset = new FeatureSet();
                featureset.Name = featureSetRequest.Name; // "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds()
                featureset.description = featureSetRequest.description;
                featureset.status = featureSetRequest.status;
                featureset.created_by = featureSetRequest.created_by;
                featureset.modified_by = featureSetRequest.modified_by;
                featureset.Features = new List<Feature>(featureSetRequest.Features);

                ObjResponse = await _FeatureManager.CreateFeatureSet(featureset);
                featureset.FeatureSetID = ObjResponse.FeatureSetID;
                _logger.LogInformation("Feature Set created with id." + ObjResponse.FeatureSetID);

                await _auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Feature Component","Feature Service",AuditTrailEnum.Event_type.CREATE,AuditTrailEnum.Event_status.SUCCESS, "CreateFeatureSet method in Feature manager", ObjResponse.FeatureSetID, ObjResponse.FeatureSetID,JsonConvert.SerializeObject(ObjResponse.FeatureSetID));
                return Ok(featureSetRequest);
               } 
               catch(Exception ex)
               {
                _logger.LogError("FeatureSet Service:Create : " + ex.Message + " " + ex.StackTrace);

                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpDelete]
        [Route("FeatureSet/delete")]
        public async Task<IActionResult> DeleteFeatureSet(int FeatureSetId)
        {    
               try
               {
                _logger.LogInformation("DeleteFeatureSet method in Feature API called.");

                if (FeatureSetId == null) 
                {
                    return StatusCode(401, "invalid FeatureSet Id: The Feature Set id is Empty.");
                }
                bool IsFeatureSetIDDeleted = await _FeatureManager.DeleteFeatureSet(FeatureSetId);
                
                await _auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Feature Component","Feature Service",AuditTrailEnum.Event_type.DELETE,AuditTrailEnum.Event_status.SUCCESS, "DeleteFeatureSet method in Feature manager", FeatureSetId,FeatureSetId,JsonConvert.SerializeObject(FeatureSetId));
                
                return Ok(IsFeatureSetIDDeleted);
               } 
               catch(Exception ex)
               {
                 _logger.LogError("Feature Service:DeleteFeatureSet : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("FeatureSet/Update")]
        public async Task<IActionResult> UpdateFeatureSet(FeatureSet featureSetRequest)
        {
            try
            {
                _logger.LogInformation("UpdateFeatureSet method in Feature API called.");

                if (featureSetRequest.FeatureSetID == null)
                {
                    return StatusCode(401, "invalid FeatureSet Id: The Feature Set id is Empty.");
                }
                FeatureSet ObjResponse = new FeatureSet();
                FeatureSet featureset = new FeatureSet();
                featureset.Name = featureSetRequest.Name; // "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds()
                featureset.description = featureSetRequest.description;
                featureset.status = featureSetRequest.status;
                featureset.created_by = featureSetRequest.created_by;
                featureset.modified_by = featureSetRequest.modified_by;
                featureset.Features = new List<Feature>(featureSetRequest.Features);

                ObjResponse = await _FeatureManager.UpdateFeatureSet(featureset);
                featureset.FeatureSetID = ObjResponse.FeatureSetID;
                _logger.LogInformation("Feature Set created with id." + ObjResponse.FeatureSetID);

                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Feature Component", "Feature Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Update method in Feature manager", ObjResponse.FeatureSetID, ObjResponse.FeatureSetID, JsonConvert.SerializeObject(ObjResponse.FeatureSetID));

                return Ok(featureSetRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError("FeatureSet Service:Update : " + ex.Message + " " + ex.StackTrace);

                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }


        //public async Task<IActionResult> isValidcheck(int code, string msg) {

        //        return  StatusCode(code, msg);
        //}


        [HttpPost]      
        [Route("createdataattributset")]
        public async Task<IActionResult>  CreateDataattributeSet(DataAttributeSet dataattributesetRequest)
        {    
               try
               {
                _logger.LogInformation("Create method in FeatureSet(DataAttribute) API called.");


                if (string.IsNullOrEmpty(dataattributesetRequest.Name))
                {
                    return StatusCode(401, "invalid DataAttributeSet Name: The DataAttribute Name is Empty.");
                }
                if (string.IsNullOrEmpty(dataattributesetRequest.Description))
                {
                    return StatusCode(401, "invalid DataAttributeSet Description : DataAttributeSet Description is Empty.");
                }

                DataAttributeSet ObjResponse = new DataAttributeSet();
                DataAttributeSet dataattributeset = new DataAttributeSet();
                dataattributeset.Name = dataattributesetRequest.Name; // "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds()
                dataattributeset.Description = dataattributesetRequest.Description;
                dataattributeset.Is_exlusive = dataattributesetRequest.Is_exlusive;
                dataattributeset.status = dataattributesetRequest.status;
                dataattributeset.created_by = dataattributesetRequest.created_by;
                dataattributeset.modified_by = dataattributesetRequest.modified_by;
                dataattributeset.DataAttributes = new List<DataAttribute>(dataattributesetRequest.DataAttributes);

                ObjResponse = await _FeatureManager.CreateDataattributeSet(dataattributeset);
                dataattributeset.ID = ObjResponse.ID;
                _logger.LogInformation("Data Attribute Set created with id." + ObjResponse.ID);

                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Feature Component", "Feature Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "CreateDataattributeSet method in Feature manager", dataattributesetRequest.ID, dataattributesetRequest.ID, JsonConvert.SerializeObject(dataattributesetRequest.ID));
                return Ok(dataattributeset);
              
               } 
               catch(Exception ex)
               {
                _logger.LogError("DataAttributeSet Service:Create : " + ex.Message + " " + ex.StackTrace);

                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("DataAttibuteSet/Update")]
        public async Task<IActionResult> UpdatedataattributeSet(DataAttributeSet dataattributesetRequest)
        {
            try
            {
                _logger.LogInformation("Update method in FeatureSet(DataAttribute) API called.");


                if (string.IsNullOrEmpty(dataattributesetRequest.Name))
                {
                    return StatusCode(401, "invalid DataAttributeSet Name: The DataAttribute Name is Empty.");
                }
                if (string.IsNullOrEmpty(dataattributesetRequest.Description))
                {
                    return StatusCode(401, "invalid DataAttributeSet Description : DataAttributeSet Description is Empty.");
                }

                DataAttributeSet ObjResponse = new DataAttributeSet();
                DataAttributeSet dataattributeset = new DataAttributeSet();
                dataattributeset.Name = dataattributesetRequest.Name; // "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds()
                dataattributeset.Description = dataattributesetRequest.Description;
                dataattributeset.Is_exlusive = dataattributesetRequest.Is_exlusive;
                dataattributeset.status = dataattributesetRequest.status;
                dataattributeset.created_by = dataattributesetRequest.created_by;
                dataattributeset.modified_by = dataattributesetRequest.modified_by;
                dataattributeset.DataAttributes = new List<DataAttribute>(dataattributesetRequest.DataAttributes);

                ObjResponse = await _FeatureManager.UpdatedataattributeSet(dataattributeset);
                dataattributeset.ID = ObjResponse.ID;
                _logger.LogInformation("Data Attribute Set updated with id." + ObjResponse.ID);

                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Feature Component", "Feature Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "UpdatedataattributeSet method in Feature manager", dataattributesetRequest.ID, dataattributesetRequest.ID, JsonConvert.SerializeObject(dataattributesetRequest.ID));
                return Ok(dataattributeset);

            }
            catch (Exception ex)
            {
                _logger.LogError("DataAttributeSet Service:Create : " + ex.Message + " " + ex.StackTrace);

                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }
        [HttpDelete]
        [Route("FeatureSet/delete")]
        public async Task<IActionResult> DeleteDataAttribute(int DataAttributeSetId)
        {
            try
            {
                _logger.LogInformation("DeleteDataAttribute method in Feature API called.");

                if (DataAttributeSetId == null)
                {
                    return StatusCode(401, "invalid DataAttributeSetId : The DataAttributeSet id is Empty.");
                }
                bool IsDataAttributeSetIDDeleted = await _FeatureManager.DeleteDataAttribute(DataAttributeSetId);

                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Feature Component", "Feature Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "DeleteFeatureSet method in Feature manager", DataAttributeSetId, DataAttributeSetId, JsonConvert.SerializeObject(DataAttributeSetId));

                return Ok(IsDataAttributeSetIDDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError("Feature Service:DeleteDataAttributeSet : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("getfeatures")]
        public async Task<IActionResult> GetDataAttributeSetDetails(int DataAttributeSetId)
        {
            try
            {
                var dataattribute = await _FeatureManager.GetDataAttributeSetDetails(DataAttributeSetId);

                List<DataAttributeSet> dataattributeList = new List<DataAttributeSet>();
                foreach (var dataattributeitem in dataattribute)
                {
                    DataAttributeSet obj = new DataAttributeSet();
                    obj.ID = dataattributeitem.ID;
                    obj.Name = dataattributeitem.Name;
                    obj.Description = dataattributeitem.Description;
                    obj.Is_exlusive = dataattributeitem.Is_exlusive;
                    obj.created_by = dataattributeitem.created_by;
                    obj.modified_by = dataattributeitem.modified_by;
                    obj.status = dataattributeitem.status;
                    dataattributeList.Add(obj);
                }

                return Ok(dataattributeList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

    }
}
