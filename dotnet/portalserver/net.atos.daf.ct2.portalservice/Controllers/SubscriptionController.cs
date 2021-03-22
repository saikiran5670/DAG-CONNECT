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
using net.atos.daf.ct2.subscription.entity;
using SubscriptionBusinessService = net.atos.daf.ct2.subscriptionservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("subscription")]
    public class SubscriptionController : Controller
    {

        #region Private Variable
        private readonly ILogger<AccountController> _logger;
        private readonly SubscriptionBusinessService.SubscribeGRPCService.SubscribeGRPCServiceClient  _subscribeClient;
        private readonly Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";
        //private IMemoryCacheProvider _cache;
        //private readonly PortalCacheConfiguration _cachesettings;

        #endregion

        #region Constructor
        public SubscriptionController(SubscriptionBusinessService.SubscribeGRPCService.SubscribeGRPCServiceClient subscribeClient, ILogger<AccountController> logger/*, IMemoryCacheProvider cache, IOptions<PortalCacheConfiguration> cachesettings*/)
        {
            _subscribeClient = subscribeClient;
            _logger = logger;
            _mapper = new Mapper();
            //_cache = cache;
            //_cachesettings = cachesettings.Value;
        }
        #endregion


        [HttpGet]
        [Route("getsubscriptiondetails")]
        public async Task<IActionResult> GetSubscriptionDetails(SubscriptionDetailsRequest objSubscriptionDetailsRequest)
        {
            try
            {
                _logger.LogInformation("GetSubscriptionDetails method in Subscription API called.");
                SubscriptionBusinessService.SubscriptionDetailsRequest objBusinessEntity = new SubscriptionBusinessService.SubscriptionDetailsRequest();
                objBusinessEntity.OrganizationId = objSubscriptionDetailsRequest.organization_id;
                objBusinessEntity.Type = objSubscriptionDetailsRequest.type == null ? string.Empty : objSubscriptionDetailsRequest.type;
                objBusinessEntity.IsActive = (SubscriptionBusinessService.StatusType) objSubscriptionDetailsRequest.is_active;
                var responce = await _subscribeClient.GetAsync(objBusinessEntity);

                return Ok(responce);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ Service:Create : " + ex.Message + " " + ex.StackTrace);

                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }

    }
}
