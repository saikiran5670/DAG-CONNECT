using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Feature;
using FeatuseBusinessService = net.atos.daf.ct2.featureservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController] 
    [Route("feature")]
    public class FeatureController : Controller
    {

        #region Private Variable
        private readonly ILogger<AccountController> _logger;
        private readonly FeatuseBusinessService.FeatureService.FeatureServiceClient _featureclient;
        private readonly Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        private IMemoryCacheProvider _cache;
        #endregion

        #region Constructor
        public FeatureController(FeatuseBusinessService.FeatureService.FeatureServiceClient Featureclient, ILogger<AccountController> logger, IMemoryCacheProvider cache)
        {
            _featureclient = Featureclient;
            _logger = logger;
            _mapper = new Mapper();
            _cache = cache;
        }
        #endregion


        [HttpPost]
        [Route("createfeatureset")]
        public async Task<IActionResult> CreateFeatureSet(FeatureSet featureSetRequest)
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
                FetureSetRequest featureset = new FetureSetRequest();
                featureset.Name = featureSetRequest.Name; // "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds()
                //featureset. = featureSetRequest.description;
                featureset.CreatedBy = featureSetRequest.created_by;
                foreach (var item in featureSetRequest.Features)
                {
                    featureset.Features.Add(item.Id);
                }

                var responce = await _featureclient.CreateFeatureSetAsync(featureset);
                featureset.FeatureSetID = ObjResponse.FeatureSetID;
                _logger.LogInformation("Feature Set created with id." + ObjResponse.FeatureSetID);

                //await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Feature Component", "Feature Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "CreateFeatureSet method in Feature manager", ObjResponse.FeatureSetID, ObjResponse.FeatureSetID, JsonConvert.SerializeObject(ObjResponse.FeatureSetID));
                return Ok(featureSetRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError("FeatureSet Service:Create : " + ex.Message + " " + ex.StackTrace);

                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("createfeature")]
        public async Task<IActionResult> CreateFeature(Features featureRequest)
        {
            try
            {
                _logger.LogInformation("Create method in FeatureSet API called.");


                if (string.IsNullOrEmpty(featureRequest.Name))
                {
                    return StatusCode(401, "invalid featureSet Name: The featureSet Name is Empty.");
                }
                //if (string.IsNullOrEmpty(featureRequest.Key))
                //{
                //    return StatusCode(401, "invalid FeatureSet Description : Feature Key is Empty.");
                //}
                FeatureRequest FeatureObj = new FeatureRequest();
                FeatureObj.Name = featureRequest.Name;
                FeatureObj.Level = featureRequest.Level;
                FeatureObj.State = (FeatureState)Enum.Parse(typeof(FeatureState), featureRequest.FeatureState.ToString());
                FeatureObj.Description = featureRequest.Description;
                FeatureObj.DataAttribute = new DataAttributeSetRequest();
                FeatureObj.DataAttribute.Name = featureRequest.Name;
                FeatureObj.DataAttribute.Description = featureRequest.Description;
                FeatureObj.DataAttribute.IsExclusive = featureRequest.DataattributeSet.is_Exclusive;
                //FeatureObj.DataAttribute. = (DataAttributeSetType)Enum.Parse(typeof(DataAttributeSetType), featureRequest.DataAttribute.AttributeType.ToString().ToUpper());

                foreach (var item in featureRequest.DataAttributeIds)
                {
                    FeatureObj.DataAttribute.DataAttributeIDs.Add(item);
                }

                var responce = await _featureclient.CreateAsync(FeatureObj);
                if (responce.Code == Responcecode.Success)
                {
                    return Ok(responce);
                }
                else
                {
                    return StatusCode(500, "Internal Server Error.");
                }
            }
            catch (Exception ex)
            {
                //throw;
                _logger.LogInformation("Create method in FeatureSet API called failed." + ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }

        }


        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> update(Features featureRequest)
        {
            try
            {
                _logger.LogInformation("Create method in FeatureSet API called.");


                if (string.IsNullOrEmpty(featureRequest.Name))
                {
                    return StatusCode(401, "invalid featureSet Name: The featureSet Name is Empty.");
                }
                //if (string.IsNullOrEmpty(featureRequest.Key))
                //{
                //    return StatusCode(401, "invalid FeatureSet Description : Feature Key is Empty.");
                //}
                FeatureRequest FeatureObj = new FeatureRequest();
                FeatureObj.Name = featureRequest.Name;
                FeatureObj.Id = featureRequest.Id;
                FeatureObj.Level = featureRequest.Level;
                FeatureObj.State = (FeatureState)Enum.Parse(typeof(FeatureState), featureRequest.FeatureState.ToString());
                FeatureObj.Description = featureRequest.Description;
                FeatureObj.DataAttribute = new DataAttributeSetRequest();
                FeatureObj.DataAttribute.Name = featureRequest.Name;
                FeatureObj.DataAttribute.Description = featureRequest.Description;
                FeatureObj.DataAttribute.IsExclusive = featureRequest.DataattributeSet.is_Exclusive;
                FeatureObj.DataAttribute.DataAttributeSetId = featureRequest.DataattributeSet.ID;
                //FeatureObj.DataAttribute. = (DataAttributeSetType)Enum.Parse(typeof(DataAttributeSetType), featureRequest.DataAttribute.AttributeType.ToString().ToUpper());

                foreach (var item in featureRequest.DataAttributeIds)
                {
                    FeatureObj.DataAttribute.DataAttributeIDs.Add(item);
                }

                var responce = await _featureclient.UpdateAsync(FeatureObj);
                if (responce.Code == Responcecode.Success)
                {
                    return Ok(responce);
                }
                else
                {
                    return StatusCode(500, "Internal Server Error.");
                }
            }
            catch (Exception ex)
            {
                //throw;
                _logger.LogInformation("Create method in FeatureSet API called failed." + ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }

        }
        [HttpGet]
        [Route("GetDataAttribute")]
        public async Task<IActionResult> GetDataAttributes(string LangaugeCode)
        {
            try
            {
                DataAtributeRequest request = new DataAtributeRequest();
                request.LangaugeCode = (LangaugeCode == null ||LangaugeCode == "") ? "EN-GB" : LangaugeCode ;
                var responce = await _featureclient.GetDataAttributesAsync(request);
                return Ok(responce.Responce);
            }
            catch (Exception)
            {

                //throw;
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]        
        [Route("getfeatures")]

        public async Task<IActionResult> GetFeatures([FromQuery] FeaturesFilterRequest request)
        {
            try
            {
                //var users = _cacheProvider.GetFromCache<IEnumerable<User>>(cacheKey);
                //if (users != null) return users;

                request.LangaugeCode = (request.LangaugeCode == null || request.LangaugeCode == "") ? "EN-GB" : request.LangaugeCode;

                var feature = await _featureclient.GetFeaturesAsync(request);

                //List<FeatureResponce> featureList = new List<FeatureResponce>();
                //foreach (var featureitem in feature.Features)
                //{
                //    FeatureResponce obj = new FeatureResponce();
                //    obj.I = featureitem.Id;
                //    obj.CreatedBy = featureitem.Createdby;
                //    obj.FeatureName = featureitem.Name;
                //    obj.Description = featureitem.Description;
                //    obj.RoleId = featureitem.RoleId;
                //    obj.OrganizationId = featureitem.Organization_Id;
                //    obj.FeatureType = featureitem.Type;
                //    featureList.Add(obj);
                //}
                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(10));

                _cache.SetCache(request.LangaugeCode, feature.Features, cacheEntryOptions);
                return Ok(feature.Features);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("Delete")]

        public async Task<IActionResult> DeleteFeatures([FromQuery] int FeatureId)
        {
            try
            {

               
                FeatureRequest FeatureObj = new FeatureRequest();
               
                FeatureObj.Id = FeatureId;
                var feature = await _featureclient.DeleteAsync(FeatureObj);
                //List<FeatureResponce> featureList = new List<FeatureResponce>();
                //foreach (var featureitem in feature.Features)
                //{
                //    FeatureResponce obj = new FeatureResponce();
                //    obj.I = featureitem.Id;
                //    obj.CreatedBy = featureitem.Createdby;
                //    obj.FeatureName = featureitem.Name;
                //    obj.Description = featureitem.Description;
                //    obj.RoleId = featureitem.RoleId;
                //    obj.OrganizationId = featureitem.Organization_Id;
                //    obj.FeatureType = featureitem.Type;
                //    featureList.Add(obj);
                //}

                return Ok(feature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}
