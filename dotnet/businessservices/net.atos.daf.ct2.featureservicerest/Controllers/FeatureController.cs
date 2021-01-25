using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using System.Linq;
using net.atos.daf.ct2.featureservicerest.Entity;

namespace net.atos.daf.ct2.featureservicerest.Controllers
{
     [ApiController]
    [Route("feature")]

    public class FeatureController :ControllerBase
    {
         private readonly ILogger logger;
        private readonly IFeatureManager featuresManager;
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
    }
}
