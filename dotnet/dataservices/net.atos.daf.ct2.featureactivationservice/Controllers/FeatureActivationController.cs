using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly AccountComponent.IAccountIdentityManager accountIdentityManager;
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
        public async Task<IActionResult> Subscription([FromBody] SubsCriptionEntity objsubscriptionActivation)
        {
            try
            {
                if (objsubscriptionActivation.SubscribeEvent != null)
                {
                    if (string.IsNullOrEmpty(objsubscriptionActivation.SubscribeEvent.OrganizationId))
                    {
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(objsubscriptionActivation.SubscribeEvent.OrganizationId));
                    }
                    else if (string.IsNullOrEmpty(objsubscriptionActivation.SubscribeEvent.packageId))
                    {
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(objsubscriptionActivation.SubscribeEvent.packageId));
                    }                    

                    SubscriptionActivation Objsubs = new SubscriptionActivation();
                    Objsubs.OrganizationId = objsubscriptionActivation.SubscribeEvent.OrganizationId;
                    Objsubs.packageId = objsubscriptionActivation.SubscribeEvent.packageId;
                    Objsubs.VINs = new List<string>();

                    if (objsubscriptionActivation.SubscribeEvent.VINs != null && objsubscriptionActivation.SubscribeEvent.VINs.Count > 0)
                        Objsubs.VINs.AddRange(objsubscriptionActivation.SubscribeEvent.VINs);

                    try
                    {
                        if (objsubscriptionActivation.SubscribeEvent.StartDateTime != string.Empty)
                            Objsubs.StartDateTime = UTCHandling.GetUTCFromDateTime(Convert.ToDateTime(objsubscriptionActivation.SubscribeEvent.StartDateTime));
                        else
                            Objsubs.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now);                     
                    }
                    catch (Exception)
                    {
                        logger.LogInformation($"Not valid date in subscription event - {Newtonsoft.Json.JsonConvert.SerializeObject(objsubscriptionActivation.SubscribeEvent)}");
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(objsubscriptionActivation.SubscribeEvent.StartDateTime), "INVALID_PARAMETER");
                    }

                    var order = await subscriptionManager.Subscribe(Objsubs);
                    if (order.Item1 == HttpStatusCode.BadRequest)
                    {
                        if(order.Item2 == null)
                            return GenerateErrorResponse(order.Item1, "VINs");
                        else
                            return GenerateErrorResponse(order.Item1, "VINs", errorCode: order.Item2.ErrorCode);
                    }                        
                    else if (order.Item1 == HttpStatusCode.NotFound)
                        return GenerateErrorResponse(order.Item1, errorCode: order.Item2.ErrorCode, description: order.Item2.ErrorMessage);
                    else if (order.Item1 == HttpStatusCode.Conflict)
                        return GenerateErrorResponse(order.Item1, errorCode: order.Item2.ErrorCode, description: order.Item2.ErrorMessage);

                    logger.LogInformation($"Subscription data has been Inserted, order ID - {order.Item2.Response.orderId}");
                    return Ok(order.Item2.Response);
                }
                else 
                if (objsubscriptionActivation.UnsubscribeEvent != null)
                {
                    if (string.IsNullOrEmpty(objsubscriptionActivation.UnsubscribeEvent.OrganizationID))
                    {
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(objsubscriptionActivation.UnsubscribeEvent.OrganizationID));
                    }
                    else if (objsubscriptionActivation.UnsubscribeEvent.OrderID <= 0)
                    {
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(objsubscriptionActivation.UnsubscribeEvent.OrderID));
                    }
                    UnSubscription Objunsubs = new UnSubscription();
                    Objunsubs.OrganizationID = objsubscriptionActivation.UnsubscribeEvent.OrganizationID;
                    Objunsubs.OrderID = objsubscriptionActivation.UnsubscribeEvent.OrderID;
                    Objunsubs.VINs = new List<string>();

                    if(objsubscriptionActivation.UnsubscribeEvent.VINs != null && objsubscriptionActivation.UnsubscribeEvent.VINs.Count > 0)
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
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(objsubscriptionActivation.UnsubscribeEvent.EndDateTime), "INVALID_PARAMETER");
                    }

                    var order = await subscriptionManager.Unsubscribe(Objunsubs);

                    if (order.Item1 == HttpStatusCode.BadRequest)
                    {
                        if (order.Item2 == null)
                            return GenerateErrorResponse(order.Item1, "VINs");
                        else
                            return GenerateErrorResponse(order.Item1, "VINs", errorCode: order.Item2.ErrorCode);
                    }
                    else if (order.Item1 == HttpStatusCode.NotFound)
                        return GenerateErrorResponse(order.Item1, errorCode: order.Item2.ErrorCode, description: order.Item2.ErrorMessage);
                   
                    logger.LogInformation($"UnSubscription data has been Inserted, order ID - {Objunsubs.OrderID}");
                    return Ok(order.Item2.Response);
                }
                else
                {
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, nameof(objsubscriptionActivation.SubscribeEvent) + " or " + nameof(objsubscriptionActivation.UnsubscribeEvent));
                }               
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string parameter= null, string errorCode = null, string description = null)
        {
            switch (statusCode)
            {
                case HttpStatusCode.BadRequest:
                    if (string.IsNullOrEmpty(errorCode))
                        return StatusCode((int)statusCode, new ErrorResponse()
                        {
                            ResponseCode = ((int)statusCode).ToString(),
                            Message = "MISSING_PARAMETER",
                            Value = "A mandatory field " + parameter + " was not provided."
                        });
                    else
                        return StatusCode((int)statusCode, new ErrorResponse()
                        {
                            ResponseCode = ((int)statusCode).ToString(),
                            Message = errorCode,
                            Value = parameter + " parameter has an invalid value."
                        });
                case HttpStatusCode.NotFound:
                case HttpStatusCode.Conflict:
                    return StatusCode((int)statusCode, new ErrorResponse()
                    {
                        ResponseCode = ((int)statusCode).ToString(),
                        Message = errorCode,
                        Value = description
                    });
                default:
                    return null;
            }            
        }
    }
}
