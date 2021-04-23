using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.featureactivationservice.CustomAttributes;
using net.atos.daf.ct2.featureactivationservice.Entity;
using net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.entity;
using net.atos.daf.ct2.utilities;
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
        //private readonly IPreferenceManager preferencemanager;
        private readonly IAuditTraillib AuditTrail;
        public FeatureActivationController(ILogger<FeatureActivationController> _logger, IAuditTraillib _AuditTrail, ISubscriptionManager _subscriptionManager, AccountComponent.IAccountIdentityManager _accountIdentityManager)// IPreferenceManager _preferencemanager,
        {
            logger = _logger;
            AuditTrail = _AuditTrail;
            subscriptionManager = _subscriptionManager;
            //preferencemanager = _preferencemanager;
            accountIdentityManager = _accountIdentityManager;
        }
        
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Subscription(SubsCriptionEntity objsubscriptionActivation)
        {
            try
            {
                if (objsubscriptionActivation.SubscribeEvent != null)
                {
                    if (string.IsNullOrEmpty(objsubscriptionActivation.SubscribeEvent.OrganizationId))
                    {
                        return StatusCode(400, string.Empty);
                    }
                    else if (string.IsNullOrEmpty(objsubscriptionActivation.SubscribeEvent.packageId))
                    {
                        return StatusCode(400, string.Empty);
                    }
                    

                    SubscriptionActivation Objsubs = new SubscriptionActivation();
                    Objsubs.OrganizationId = objsubscriptionActivation.SubscribeEvent.OrganizationId;
                    Objsubs.packageId = objsubscriptionActivation.SubscribeEvent.packageId;
                    Objsubs.VINs = new List<string>();
                    Objsubs.VINs.AddRange(objsubscriptionActivation.SubscribeEvent.VINs);
                    try
                    {
                        if (objsubscriptionActivation.SubscribeEvent.StartDateTime != string.Empty)
                        {
                            Objsubs.StartDateTime = UTCHandling.GetUTCFromDateTime(Convert.ToDateTime(objsubscriptionActivation.SubscribeEvent.StartDateTime));
                        }
                        else
                        {
                            Objsubs.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                        }
                       
                    }
                    catch (Exception)
                    {

                        logger.LogInformation($"Not valid date in subcription event - {Newtonsoft.Json.JsonConvert.SerializeObject(objsubscriptionActivation.SubscribeEvent)}");
                        return StatusCode(400, string.Empty);
                    }

                    var order = await subscriptionManager.Subscribe(Objsubs);
                    if (order == null)
                    {
                        logger.LogInformation($"No Data found for Subscription, payload - {Newtonsoft.Json.JsonConvert.SerializeObject(objsubscriptionActivation)}");
                        return StatusCode(404, string.Empty);
                    }
                    logger.LogInformation($"Subscription data has been Inserted, order ID - {order.orderId}");
                    return Ok(order);
                }
                else 
                if (objsubscriptionActivation.UnsubscribeEvent != null)
                {
                    if (string.IsNullOrEmpty(objsubscriptionActivation.UnsubscribeEvent.OrganizationID))
                    {
                        return StatusCode(400, string.Empty);
                    }
                    else if (objsubscriptionActivation.UnsubscribeEvent.OrderID <= 0)
                    {
                        return StatusCode(400, string.Empty);
                    }
                    UnSubscription Objunsubs = new UnSubscription();
                    Objunsubs.OrganizationID = objsubscriptionActivation.UnsubscribeEvent.OrganizationID;
                    Objunsubs.OrderID = objsubscriptionActivation.UnsubscribeEvent.OrderID;
                    Objunsubs.VINs = new List<string>();
                    Objunsubs.VINs.AddRange(objsubscriptionActivation.UnsubscribeEvent.VINs);
                    
                    try
                    {
                        if (objsubscriptionActivation.UnsubscribeEvent.EndDateTime != string.Empty)
                        {
                            Objunsubs.EndDateTime = UTCHandling.GetUTCFromDateTime(Convert.ToDateTime(objsubscriptionActivation.UnsubscribeEvent.EndDateTime));
                        }
                        else
                        {
                            Objunsubs.EndDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);
                        }
                    }
                    catch (Exception)
                    {
                        logger.LogInformation($"Not valid date in unsubcription event - {Newtonsoft.Json.JsonConvert.SerializeObject(objsubscriptionActivation.SubscribeEvent)}");
                        return StatusCode(400, string.Empty); ;
                    }

                    var orderId = await subscriptionManager.Unsubscribe(Objunsubs);
                    if (orderId == null)
                    {
                        logger.LogInformation($"No Data found for UnSubscription, payload - {Newtonsoft.Json.JsonConvert.SerializeObject(objsubscriptionActivation.UnsubscribeEvent)}");
                        return StatusCode(404, string.Empty);
                    }
                    logger.LogInformation($"Subscription data has been UnSubscribed, order ID - {orderId}");
                    return Ok(orderId);
                }
                else
                {
                    return StatusCode(400, string.Empty);
                }
               
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }
        }
      
    }
}
