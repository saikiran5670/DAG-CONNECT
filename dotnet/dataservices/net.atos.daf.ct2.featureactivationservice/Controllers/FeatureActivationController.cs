﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.featureactivationservice.CustomAttributes;
using net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.entity;
using AccountComponent = net.atos.daf.ct2.account;

namespace net.atos.daf.ct2.featureactivationservice.Controllers
{
    [ApiController]
    [Route("subscription")]
    [Authorize(Policy = AccessPolicies.MainAccessPolicy)]
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
            try
            {                
                if (string.IsNullOrEmpty(objsubscriptionActivation.OrganizationId))
                {
                    return StatusCode(400, string.Empty);
                }
                else if (string.IsNullOrEmpty(objsubscriptionActivation.packageId))
                {
                    return StatusCode(400, string.Empty);
                }

                var order = await subscriptionManager.Subscribe(objsubscriptionActivation);
                if (order == null)
                {
                    logger.LogInformation($"No Data found for Subscription, payload - {Newtonsoft.Json.JsonConvert.SerializeObject(objsubscriptionActivation)}");
                    return StatusCode(400, string.Empty);
                }
                logger.LogInformation($"Subscription data has been Inserted, order ID - {order.orderId}");
                return Ok(order);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }
        }

        [HttpPost]
        [Route("unsubscribe")]
        public async Task<IActionResult> UnSubscribe(UnSubscription objUnSubscription)
        {
            try
            {
                if (string.IsNullOrEmpty(objUnSubscription.OrganizationID))
                {
                    return StatusCode(400, string.Empty);
                }
                else if (string.IsNullOrEmpty(objUnSubscription.PackageId))
                {
                    return StatusCode(400, string.Empty);
                }
                var orderId = await subscriptionManager.Unsubscribe(objUnSubscription);
                if (orderId == null)
                {
                    logger.LogInformation($"No Data found for UnSubscription, payload - {Newtonsoft.Json.JsonConvert.SerializeObject(objUnSubscription)}");
                    return StatusCode(400, string.Empty);
                }
                logger.LogInformation($"Subscription data has been UnSubscribed, order ID - {orderId}");
                return Ok(orderId);                    
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500,string.Empty);
            }
        }
    }
}
