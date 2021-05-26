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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using log4net;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("feature")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class FeatureController : BaseController
    {

        #region Private Variable
        //private readonly ILogger<AccountController> _logger;
        private readonly AuditHelper _auditHelper;
        private ILog _logger;
        private readonly FeatuseBusinessService.FeatureService.FeatureServiceClient _featureclient;
        private readonly Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private IMemoryCacheProvider _cache;       
        private readonly PortalCacheConfiguration _cachesettings;
        private readonly Common.AccountPrivilegeChecker _privilegeChecker;
        #endregion

        #region Constructor
        public FeatureController(FeatureService.FeatureServiceClient Featureclient, IMemoryCacheProvider cache, IOptions<PortalCacheConfiguration> cachesettings,
             AuditHelper auditHelper, Common.AccountPrivilegeChecker privilegeChecker, IHttpContextAccessor _httpContextAccessor, SessionHelper sessionHelper) : base(_httpContextAccessor, sessionHelper)
        {
            _featureclient = Featureclient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); ;
            _mapper = new Mapper();
            _cache = cache;
            _cachesettings = cachesettings.Value;
            _privilegeChecker = privilegeChecker;
            _auditHelper = auditHelper;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
            //headers = GetHeaders(Request.Headers);
        }
        #endregion

        private Dictionary<string, string> GetHeaders(IHeaderDictionary headers)
        {
            return new Dictionary<string, string>()
            {
                { "RoleId", headers["roleId"] },
                { "RoleId", headers["roleId"] },
                { "RoleId", headers["roleId"] }
            };
        }


        [HttpPost]
        [Route("createfeatureset")]
        public async Task<IActionResult> CreateFeatureSet(FeatureSet featureSetRequest)
        {
           
            try
            {
                _logger.Info("Create method in FeatureSet API called.");


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
                _logger.Info("Feature Set created with id." + ObjResponse.FeatureSetID);

                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Feature Component",
                    "Feature service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "CreateFeatureSet method in Feature manager", 0, ObjResponse.FeatureSetID, JsonConvert.SerializeObject(featureSetRequest),
                     Request);
                return Ok(featureSetRequest);
            }
            catch (Exception ex)
            {

                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Feature Component",
                   "Feature service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                   "CreateFeatureSet method in Feature manager",0,0, JsonConvert.SerializeObject(featureSetRequest),
                    Request);
                _logger.Error(null, ex);

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
                _logger.Info("Create method in FeatureSet API called.");


                if (string.IsNullOrEmpty(featureRequest.Name))
                {
                    return StatusCode(400, "invalid Feature Name: The feature Name is Empty.");
                }
                if (string.IsNullOrEmpty(featureRequest.Name))
                {
                    return StatusCode(400, "invalid Feature Name: The feature Name is Empty.");
                }
                if (featureRequest.DataAttributeIds.Count() <= 0)
                {
                    return StatusCode(400, "Data attribute Id's required.");
                }
                //if (string.IsNullOrEmpty(featureRequest.Key))
                //{
                //    return StatusCode(401, "invalid FeatureSet Description : Feature Key is Empty.");
                //}
                int level = await _privilegeChecker.GetLevelByRoleId(_userDetails.orgId, _userDetails.roleId);
                
                FeatureRequest FeatureObj = new FeatureRequest();
                FeatureObj.Name = featureRequest.Name;
                FeatureObj.Level = level;
                //FeatureObj.Level = featureRequest.Level;
                FeatureObj.State = featureRequest.FeatureState;//(FeatureState)Enum.Parse(typeof(FeatureState), featureRequest.FeatureState.ToString());
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
                  
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now,  "Feature Component",
                                               "Feature service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                               "CreateFeature method in Feature controller", 0, responce.FeatureID, JsonConvert.SerializeObject(featureRequest),
                                                Request);


                    if (responce.Message == "Feature name allready exists")
                    {
                        return StatusCode(409, responce.Message)
;
                    }
                    return Ok(responce);
                }
                else
                {
                    _logger.Error(responce.Message);
                    return StatusCode(500, "Internal Server Error.");
                }
            }
            catch (Exception ex)
            {
                //throw;

                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Feature Component",
                                             "Feature service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                             "CreateFeature method in Feature controller", 0, 0, JsonConvert.SerializeObject(featureRequest),
                                              Request);

                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }

        }


        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> update(Features featureRequest)
        {
            

            try
            {
                _logger.Info("Update method in FeatureSet API called.");


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
                FeatureObj.State = featureRequest.FeatureState;//(FeatureState)Enum.Parse(typeof(FeatureState), featureRequest.FeatureState.ToString());
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
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Feature Component",
                                            "Feature service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                            "Update Feature method in Feature controller", FeatureObj.Id, FeatureObj.Id, JsonConvert.SerializeObject(featureRequest),
                                             Request);

                    if (responce.Message == "Feature name allready exists")
                    {
                        return StatusCode(409, responce.Message)
;
                    }
                    return Ok(responce);
                }
                else
                {
                    _logger.Error(responce);
                    return StatusCode(400, "Error in feature update.");
                }
            }
            catch (Exception ex)
            {
                //throw;
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Feature Component",
                                            "Feature service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                            "Update Feature method in Feature controller", featureRequest.Id, featureRequest.Id, JsonConvert.SerializeObject(featureRequest),
                                             Request);
                _logger.Error(featureRequest, ex);
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
                request.LangaugeCode = (LangaugeCode == null || LangaugeCode == "") ? "EN-GB" : LangaugeCode;
                Google.Protobuf.Collections.RepeatedField<DataAttributeResponce> cachedataAttributes = _cache.GetFromCache<Google.Protobuf.Collections.RepeatedField<DataAttributeResponce>>(request.LangaugeCode);
                if (cachedataAttributes != null && cachedataAttributes.Count > 0) return Ok(cachedataAttributes);


                var responce = await _featureclient.GetDataAttributesAsync(request);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cachesettings.ExpiryInMinutes));

                _cache.SetCache(request.LangaugeCode, responce.Responce, cacheEntryOptions);

                if (responce.Code== Responcecode.Failed)
                {
                    return StatusCode(500, "Internal Server Error");
                }
                return Ok(responce.Responce);
            }
            catch (Exception ex)
            {

                //throw;
                _logger.Error(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error. Exception - " + ex.ToString());
            }
        }

        [HttpGet]
        [Route("getfeatures")]

        public async Task<IActionResult> GetFeatures([FromQuery] FeaturesFilterRequest request)
        {
            try
            {

                //Google.Protobuf.Collections.RepeatedField<FeatureRequest> cachedfeature = _cache.GetFromCache<Google.Protobuf.Collections.RepeatedField<FeatureRequest>>(request.LangaugeCode);
                //if (cachedfeature != null) return Ok(cachedfeature);

                request.LangaugeCode = (request.LangaugeCode == null || request.LangaugeCode == "") ? "EN-GB" : request.LangaugeCode;
                int level = await _privilegeChecker.GetLevelByRoleId(_userDetails.orgId, _userDetails.roleId);
                request.Level = level;
                if (request.OrganizationID != 0)
                {
                    request.OrganizationID = GetContextOrgId();
                }
                
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
                //var cacheEntryOptions = new MemoryCacheEntryOptions()
                //    // Keep in cache for this time, reset time if accessed.
                //    .SetSlidingExpiration(TimeSpan.FromMinutes(_cachesettings.ExpiryInMinutes));

                //_cache.SetCache(request.LangaugeCode, feature.Features, cacheEntryOptions);
                return Ok(feature.Features);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error. Exception - " + ex.Message);
            }
        }

        [HttpPost]
        [Route("Delete")]

        public async Task<IActionResult> DeleteFeatures([FromQuery] int FeatureId)
        {
            FeatureRequest FeatureObj = new FeatureRequest();
            try
            {


               

                FeatureObj.Id = FeatureId;
                var feature = await _featureclient.DeleteAsync(FeatureObj);
                if (feature.Code == Responcecode.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Feature Component",
                                               "Feature service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                               "DeleteFeatures method in Feature controller", FeatureObj.Id, FeatureObj.Id, JsonConvert.SerializeObject(FeatureObj),
                                                Request);
                }

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
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Feature Component",
                                          "Feature service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                          "DeleteFeatures method in Feature controller", FeatureObj.Id, FeatureObj.Id, JsonConvert.SerializeObject(FeatureObj),
                                           Request);
                _logger.Error(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("featurestate/update")]

        public async Task<IActionResult> ChangeFeatureState([FromQuery] int FeatureId, FeatureState featurestate)
        {
            FeatureStateRequest FeatureObj = new FeatureStateRequest();
            try
            {


               

                FeatureObj.Featureid = FeatureId;
                FeatureObj.FeatureState = featurestate == FeatureState.Active ? "A" : "I";
                var feature = await _featureclient.ChangeFeatureStateAsync(FeatureObj);


                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Feature Component",
                                          "Feature service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                                          "ChangeFeatureState  method in Feature controller", FeatureId, FeatureId, JsonConvert.SerializeObject(FeatureObj),
                                           Request);

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
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Feature Component",
                                "Feature service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                                "ChangeFeatureState  method in Feature controller", FeatureId, FeatureId, JsonConvert.SerializeObject(FeatureObj),
                                 Request);
                _logger.Error(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}
