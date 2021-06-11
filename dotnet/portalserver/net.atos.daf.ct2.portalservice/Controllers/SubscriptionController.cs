using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.subscription.entity;
using SubscriptionBusinessService = net.atos.daf.ct2.subscriptionservice;
namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("subscribe")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SubscriptionController : BaseController
    {

        #region Private Variable
        //private readonly ILogger<SubscriptionController> _logger;

        private ILog _logger;
        private readonly SubscriptionBusinessService.SubscribeGRPCService.SubscribeGRPCServiceClient _subscribeClient;
        private readonly AuditHelper _auditHelper;

        #endregion

        #region Constructor
        public SubscriptionController(SubscriptionBusinessService.SubscribeGRPCService.SubscribeGRPCServiceClient subscribeClient, AuditHelper auditHelper, IHttpContextAccessor _httpContextAccessor, SessionHelper sessionHelper) : base(_httpContextAccessor, sessionHelper)
        {
            _subscribeClient = subscribeClient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _auditHelper = auditHelper;
        }
        #endregion


        [HttpGet]
        [Route("getsubscriptiondetails")]
        public async Task<IActionResult> GetSubscriptionDetails([FromQuery] SubscriptionDetailsRequest objSubscriptionDetailsRequest)
        {
            try
            {
                _logger.Info("GetSubscriptionDetails method in Subscription API called.");

                if (objSubscriptionDetailsRequest.Organization_id == 0)
                {
                    return StatusCode(400, string.Empty);
                }
                //Assign context orgId
                objSubscriptionDetailsRequest.Organization_id = GetContextOrgId();

                SubscriptionBusinessService.SubscriptionDetailsRequest objBusinessEntity = new SubscriptionBusinessService.SubscriptionDetailsRequest();
                objBusinessEntity.OrganizationId = objSubscriptionDetailsRequest.Organization_id;
                objBusinessEntity.Type = objSubscriptionDetailsRequest.Type ?? string.Empty;
                objBusinessEntity.State = (SubscriptionBusinessService.StatusType)objSubscriptionDetailsRequest.State;
                var data = await _subscribeClient.GetAsync(objBusinessEntity);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
