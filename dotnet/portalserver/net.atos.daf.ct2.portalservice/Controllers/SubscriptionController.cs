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
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Reflection;
using Microsoft.AspNetCore.Http;
namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("subscribe")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SubscriptionController : Controller
    {

        #region Private Variable
        //private readonly ILogger<SubscriptionController> _logger;

        private ILog _logger;
        private readonly SubscriptionBusinessService.SubscribeGRPCService.SubscribeGRPCServiceClient  _subscribeClient;
        private readonly SessionHelper _sessionHelper;
        private readonly HeaderObj _userDetails;
        private readonly AuditHelper _auditHelper;

        #endregion

        #region Constructor
        public SubscriptionController(SubscriptionBusinessService.SubscribeGRPCService.SubscribeGRPCServiceClient subscribeClient, AuditHelper auditHelper, IHttpContextAccessor _httpContextAccessor, SessionHelper sessionHelper)
        {
            _subscribeClient = subscribeClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _sessionHelper = sessionHelper;
            _userDetails = _sessionHelper.GetSessionInfo(_httpContextAccessor.HttpContext.Session);
            _auditHelper = auditHelper;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
        }
        #endregion


        [HttpGet]
        [Route("getsubscriptiondetails")]
        public async Task<IActionResult> GetSubscriptionDetails([FromQuery] SubscriptionDetailsRequest objSubscriptionDetailsRequest)
        {
            try
            {
                _logger.Info("GetSubscriptionDetails method in Subscription API called.");
                //Assign context orgId
                objSubscriptionDetailsRequest.organization_id = _userDetails.contextOrgId;
                if (objSubscriptionDetailsRequest.organization_id == 0)
                {
                    return StatusCode(400, string.Empty);
                }
                SubscriptionBusinessService.SubscriptionDetailsRequest objBusinessEntity = new SubscriptionBusinessService.SubscriptionDetailsRequest();
                objBusinessEntity.OrganizationId = objSubscriptionDetailsRequest.organization_id; 
                objBusinessEntity.Type = objSubscriptionDetailsRequest.type == null ? string.Empty : objSubscriptionDetailsRequest.type;
                objBusinessEntity.State = (SubscriptionBusinessService.StatusType)objSubscriptionDetailsRequest.state;
                var data = await _subscribeClient.GetAsync(objBusinessEntity);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.Error(null,ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
