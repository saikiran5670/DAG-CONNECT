using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.entity;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.featureactivationservice.Controllers
{
    [ApiController]
    [Route("subscription")]
    public class FeatureActivationController : ControllerBase
    {
        private readonly ILogger<FeatureActivationController> logger;
        private readonly ISubscriptionManager subscriptionManager;
        AccountComponent.IAccountIdentityManager accountIdentityManager;
        private readonly IPreferenceManager preferencemanager;
        private readonly IAuditTraillib AuditTrail;
        public FeatureActivationController(ILogger<FeatureActivationController> _logger, IAuditTraillib _AuditTrail, ISubscriptionManager _subscriptionManager, IPreferenceManager _preferencemanager, AccountComponent.IAccountIdentityManager _accountIdentityManager)
        {
            logger = _logger;
            AuditTrail = _AuditTrail;
            subscriptionManager = _subscriptionManager;
            preferencemanager = _preferencemanager;
            accountIdentityManager = _accountIdentityManager;
        }

        [HttpPost]
        [Route("subscribe")]
        public async Task<IActionResult> Subscription(SubscriptionActivation objsubscriptionActivation)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool valid = false;
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    logger.LogInformation($"Subscription function called with empty token, with Package Id - {objsubscriptionActivation.packageId}");
                    return StatusCode(400, "Bad Request:");
                }
                else
                {
                    logger.LogInformation($"Subscription function called , with Package Id - {objsubscriptionActivation.packageId}");
                    valid = await accountIdentityManager.ValidateToken(token);
                    if (valid)
                    {
                        if (string.IsNullOrEmpty(objsubscriptionActivation.OrganizationId))
                        {
                            return StatusCode(400, "Please provide organization ID:");
                        }
                        else if (string.IsNullOrEmpty(objsubscriptionActivation.packageId))
                        {
                            return StatusCode(400, "Please provide packageId ");
                        }


                        var orderId = await subscriptionManager.Subscribe(objsubscriptionActivation);
                        logger.LogInformation($"Subscription data has been Inserted, order ID - {orderId}");
                        return Ok(orderId);
                    }
                    else
                    {
                        logger.LogInformation($"Subscription function called with invalid Token, with Package Id - {objsubscriptionActivation.packageId}");
                        return StatusCode(401, "Forbidden:");
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("unsubscribe")]
        public async Task<IActionResult> UnSubscribe(UnSubscription objUnSubscription)
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            bool valid = false;
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    logger.LogInformation($"UnSubscription function called with empty token, with Package Id - {objUnSubscription.OrderID}");
                    return StatusCode(400, "Bad Request:");
                }
                else
                {
                    logger.LogInformation($"UnSubscription function called , with Package Id - {objUnSubscription.OrderID}");
                    valid = await accountIdentityManager.ValidateToken(token);
                    if (valid)
                    {
                        if (string.IsNullOrEmpty(objUnSubscription.OrganizationID))
                        {
                            return StatusCode(400, "Please provide organization ID:");
                        }
                        else if (string.IsNullOrEmpty(objUnSubscription.PackageId))
                        {
                            return StatusCode(400, "Please provide packageId ");
                        }


                        var orderId = await subscriptionManager.Unsubscribe(objUnSubscription);
                        logger.LogInformation($"Subscription data has been Inserted, order ID - {orderId}");
                        return Ok(orderId);
                    }
                    else
                    {
                        logger.LogInformation($"Subscription function called with invalid Token, with Package Id - {objUnSubscription.OrderID}");
                        return StatusCode(401, "Forbidden:");
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    }
}
