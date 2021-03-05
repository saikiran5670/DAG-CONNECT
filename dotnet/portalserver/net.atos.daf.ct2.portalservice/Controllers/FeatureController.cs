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
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Feature;
using FeatuseBusinessService = net.atos.daf.ct2.featureservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("account")]
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
                FeatureSet featureset = new FeatureSet();
                featureset.Name = featureSetRequest.Name; // "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds()
                featureset.description = featureSetRequest.description;
                featureset.status = featureSetRequest.status;
                featureset.created_by = featureSetRequest.created_by;
                featureset.modified_by = featureSetRequest.modified_by;
                featureset.Features = new List<Features>(featureSetRequest.Features);

                //ObjResponse = await _featureclient.CreateFeatureSet(featureset);
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
    }
}
