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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("subscribe")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SubscriptionController : Controller
    {

        #region Private Variable
        private readonly ILogger<SubscriptionController> _logger;
        private readonly SubscriptionBusinessService.SubscribeGRPCService.SubscribeGRPCServiceClient  _subscribeClient;
       
       

        #endregion

        #region Constructor
        public SubscriptionController(SubscriptionBusinessService.SubscribeGRPCService.SubscribeGRPCServiceClient subscribeClient, ILogger<SubscriptionController> logger)
        {
            _subscribeClient = subscribeClient;
            _logger = logger;
        }
        #endregion


        [HttpGet]
        [Route("getsubscriptiondetails")]
        public async Task<IActionResult> GetSubscriptionDetails([FromQuery] SubscriptionDetailsRequest objSubscriptionDetailsRequest)
        {
            try
            {
                _logger.LogInformation("GetSubscriptionDetails method in Subscription API called.");
                if (objSubscriptionDetailsRequest.organization_id == 0)
                {
                    return StatusCode(400, string.Empty);
                }
                SubscriptionBusinessService.SubscriptionDetailsRequest objBusinessEntity = new SubscriptionBusinessService.SubscriptionDetailsRequest();
                objBusinessEntity.OrganizationId = objSubscriptionDetailsRequest.organization_id; 
                objBusinessEntity.Type = objSubscriptionDetailsRequest.type == null ? string.Empty : objSubscriptionDetailsRequest.type;
                objBusinessEntity.IsActive = (SubscriptionBusinessService.StatusType)objSubscriptionDetailsRequest.is_active;
                var data = await _subscribeClient.GetAsync(objBusinessEntity);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ Exception in Subscription GetSubscriptionDetails {ex.Message} {ex.StackTrace}}");
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
