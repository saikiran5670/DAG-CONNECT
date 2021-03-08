using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
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

        #endregion

        #region Constructor
        public FeatureController(FeatuseBusinessService.FeatureService.FeatureServiceClient Featureclient, ILogger<AccountController> logger)
        {
            _featureclient = Featureclient;
            _logger = logger;
            _mapper = new Mapper();
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
        public async Task<IActionResult> CreateFeatureSet(Features featureRequest)
        {
            try
            {
                _logger.LogInformation("Create method in FeatureSet API called.");


                if (string.IsNullOrEmpty(featureRequest.Name))
                {
                    return StatusCode(401, "invalid featureSet Name: The featureSet Name is Empty.");
                }
                if (string.IsNullOrEmpty(featureRequest.Key))
                {
                    return StatusCode(401, "invalid FeatureSet Description : Feature Key is Empty.");
                }
                FeatureRequest FeatureObj = new FeatureRequest();
                FeatureObj.Name = featureRequest.Name;
                FeatureObj.Level = featureRequest.Level;
                FeatureObj.Status = featureRequest.Is_Active;
                FeatureObj.Description = featureRequest.Description;
                FeatureObj.DataAttribute = new DataAttributeSetRequest();
                FeatureObj.DataAttribute.Name = featureRequest.DataattributeSet.Name;
                FeatureObj.DataAttribute.Description = featureRequest.DataattributeSet.Description;
                //FeatureObj.DataAttributeSets.Is_exlusive = featureRequest.DataAttribute.AttributeType.ToString();
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
            catch (Exception)
            {
                //throw;
                return StatusCode(500, "Internal Server Error.");
            }

        }

        [HttpGet]
        [Route("GetDataAttribute")]
        public async Task<IActionResult> GetDataAttributes()
        {
            try
            {
                DataAtributeRequest request = new DataAtributeRequest();
                var responce = await _featureclient.GetDataAttributesAsync(request);
                return Ok(responce);
            }
            catch (Exception)
            {

                //throw;
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("getfeatures")]
        public async Task<IActionResult> GetFeatures(FeaturesFilterRequest request)
        {
            try
            {
                
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
