using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.featureactivationservice.Common;
using net.atos.daf.ct2.featureactivationservice.CustomAttributes;
using net.atos.daf.ct2.featureactivationservice.Entity;
using net.atos.daf.ct2.kafkacdc;
using net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.entity;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.featureactivationservice.Controllers
{
    [ApiController]
    [Route("subscription")]
    [Authorize(Policy = AccessPolicies.MAIN_ACCESS_POLICY)]
    public class FeatureActivationController : ControllerBase
    {
        private readonly ILogger<FeatureActivationController> _logger;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly FeatureActivationCdcHelper _featureActivationCdcHelper;
        private readonly IFeatureActivationCdcManager _featureActivationCdcManager;
        private readonly IVehicleManager _vehicleManager;

        public FeatureActivationController(ILogger<FeatureActivationController> logger, ISubscriptionManager subscriptionManager,
            IFeatureActivationCdcManager featureActivationCdcManager, IVehicleManager vehicleManager)
        {
            this._logger = logger;
            this._subscriptionManager = subscriptionManager;
            _featureActivationCdcManager = featureActivationCdcManager;
            _featureActivationCdcHelper = new FeatureActivationCdcHelper(_featureActivationCdcManager);
            _vehicleManager = vehicleManager;
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Subscription([FromBody] SubsCriptionEntity objsubscriptionActivation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var key = ModelState.Keys.First();
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_PARAMETER", value: key.Substring(key.LastIndexOf(".") + 1));
                }

                if (objsubscriptionActivation.SubscribeEvent != null)
                {
                    if (string.IsNullOrEmpty(objsubscriptionActivation.SubscribeEvent.OrganizationId))
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, value: nameof(objsubscriptionActivation.SubscribeEvent.OrganizationId));
                    else if (string.IsNullOrEmpty(objsubscriptionActivation.SubscribeEvent.PackageId))
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, value: nameof(objsubscriptionActivation.SubscribeEvent.PackageId));

                    SubscriptionActivation objSubs = new SubscriptionActivation();
                    objSubs.OrganizationId = objsubscriptionActivation.SubscribeEvent.OrganizationId;
                    objSubs.PackageId = objsubscriptionActivation.SubscribeEvent.PackageId;
                    objSubs.VINs = new List<string>();
                    if (objsubscriptionActivation.SubscribeEvent.VINs != null && objsubscriptionActivation.SubscribeEvent.VINs.Count > 0)
                    {
                        if (objsubscriptionActivation.SubscribeEvent.VINs
                            .GroupBy(x => x)
                            .Where(g => g.Count() > 1).Count() > 0)
                            return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_PARAMETER", value: objsubscriptionActivation.SubscribeEvent.VINs);

                        objSubs.VINs.AddRange(objsubscriptionActivation.SubscribeEvent.VINs);
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(objsubscriptionActivation.SubscribeEvent.StartDateTime))
                            objSubs.StartDateTime = UTCHandling.GetUTCFromDateTime(Convert.ToDateTime(objsubscriptionActivation.SubscribeEvent.StartDateTime), "UTC");
                        else
                            objSubs.StartDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now, "UTC");
                    }
                    catch (Exception)
                    {
                        _logger.LogInformation($"Not valid date in subscription event - {Newtonsoft.Json.JsonConvert.SerializeObject(objsubscriptionActivation.SubscribeEvent)}");
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_PARAMETER", value: objsubscriptionActivation.SubscribeEvent.StartDateTime);
                    }

                    var visibleVINs = await GetVisibleVINsToOrg(objSubs);

                    var order = await _subscriptionManager.Subscribe(objSubs, visibleVINs);
                    if (order.Item1 == HttpStatusCode.BadRequest)
                    {
                        if (order.Item2.Value is string[])
                            return GenerateErrorResponse(order.Item1, errorCode: order.Item2.ErrorCode, value: order.Item2.Value);
                        else
                            return GenerateErrorResponse(order.Item1, errorCode: order.Item2.ErrorCode, value: order.Item2.Value);
                    }
                    else if (order.Item1 == HttpStatusCode.NotFound)
                        return GenerateErrorResponse(order.Item1, errorCode: order.Item2.ErrorCode, value: order.Item2.Value);
                    else if (!string.IsNullOrEmpty(order.Item2.Response.OrderId) && order.Item2.Response.OrderId != "0")
                    {
                        //Triggering subscription cdc 
                        int subscriptionId = Convert.ToInt32(order.Item2.Response.OrderId);
                        //await _featureActivationCdcHelper.TriggerSubscriptionCdc(subscriptionId, "N", Convert.ToInt32(objSubs.OrganizationId), objSubs.VINs);
                    }

                    _logger.LogInformation($"Subscription data has been Inserted, order ID - {order.Item2.Response.OrderId}");
                    return Ok(order.Item2.Response);
                }
                else
                if (objsubscriptionActivation.UnsubscribeEvent != null)
                {
                    if (string.IsNullOrEmpty(objsubscriptionActivation.UnsubscribeEvent.OrganizationID))
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, value: nameof(objsubscriptionActivation.UnsubscribeEvent.OrganizationID));

                    if (string.IsNullOrEmpty(objsubscriptionActivation.UnsubscribeEvent.OrderID))
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, value: nameof(objsubscriptionActivation.UnsubscribeEvent.OrderID));

                    if (!long.TryParse(objsubscriptionActivation.UnsubscribeEvent.OrderID, out _))
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_PARAMETER", value: nameof(objsubscriptionActivation.UnsubscribeEvent.OrderID));

                    UnSubscription objUnsubs = new UnSubscription();
                    objUnsubs.OrganizationID = objsubscriptionActivation.UnsubscribeEvent.OrganizationID;
                    objUnsubs.OrderID = objsubscriptionActivation.UnsubscribeEvent.OrderID;
                    objUnsubs.VINs = new List<string>();

                    if (objsubscriptionActivation.UnsubscribeEvent.VINs != null && objsubscriptionActivation.UnsubscribeEvent.VINs.Count > 0)
                    {
                        if (objsubscriptionActivation.UnsubscribeEvent.VINs
                            .GroupBy(x => x)
                            .Where(g => g.Count() > 1).Count() > 0)
                            return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_PARAMETER", value: objsubscriptionActivation.UnsubscribeEvent.VINs);

                        objUnsubs.VINs.AddRange(objsubscriptionActivation.UnsubscribeEvent.VINs);
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(objsubscriptionActivation.UnsubscribeEvent.EndDateTime))
                            objUnsubs.EndDateTime = UTCHandling.GetUTCFromDateTime(Convert.ToDateTime(objsubscriptionActivation.UnsubscribeEvent.EndDateTime), "UTC");
                        else
                            objUnsubs.EndDateTime = UTCHandling.GetUTCFromDateTime(DateTime.Now, "UTC");
                    }
                    catch (Exception)
                    {
                        _logger.LogInformation($"Not valid date in unsubscription event - {Newtonsoft.Json.JsonConvert.SerializeObject(objsubscriptionActivation.SubscribeEvent)}");
                        return GenerateErrorResponse(HttpStatusCode.BadRequest, errorCode: "INVALID_PARAMETER", value: objsubscriptionActivation.UnsubscribeEvent.EndDateTime);
                    }

                    var order = await _subscriptionManager.Unsubscribe(objUnsubs);

                    if (order.Item1 == HttpStatusCode.BadRequest)
                    {
                        if (order.Item2.Value is string[])
                            return GenerateErrorResponse(order.Item1, errorCode: order.Item2.ErrorCode, value: order.Item2.Value);
                        else
                            return GenerateErrorResponse(order.Item1, errorCode: order.Item2.ErrorCode, value: order.Item2.Value);
                    }
                    else if (order.Item1 == HttpStatusCode.NotFound)
                        return GenerateErrorResponse(order.Item1, errorCode: order.Item2.ErrorCode, value: order.Item2.Value);
                    else if (!string.IsNullOrEmpty(objUnsubs.OrderID) && objUnsubs.OrderID != "0")
                    {
                        //Triggering subscription cdc 
                        int subscriptionId = Convert.ToInt32(objUnsubs.OrderID);
                        //await _featureActivationCdcHelper.TriggerSubscriptionCdc(subscriptionId, "N", Convert.ToInt32(objUnsubs.OrganizationID), objUnsubs.VINs);
                    }

                    _logger.LogInformation($"UnSubscription data has been Inserted, order ID - {objUnsubs.OrderID}");
                    return Ok(order.Item2.Response);
                }
                else
                {
                    return GenerateErrorResponse(HttpStatusCode.BadRequest, value: new string[] { nameof(objsubscriptionActivation.SubscribeEvent), nameof(objsubscriptionActivation.UnsubscribeEvent) });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, string.Empty);
            }
        }

        private async Task<IEnumerable<string>> GetVisibleVINsToOrg(SubscriptionActivation objSubs)
        {
            var package = await _subscriptionManager.GetPackageTypeByCode(objSubs.PackageId);

            if (new string[] { "v", "n" }.Contains(package?.Type?.ToLower()))
            {
                int orgId = await _subscriptionManager.GetOrganizationIdByCode(objSubs.OrganizationId);
                if (orgId > 0 && objSubs.VINs.Count() > 0)
                {
                    var resultDict = await _vehicleManager.GetVisibilityVehiclesByOrganization(orgId);
                    var visibleVehicles = resultDict.Values.SelectMany(x => x).Distinct(new ObjectComparer()).Where(x => x.HasOwned == false).Select(x => x.VIN);
                    return visibleVehicles.Intersect(objSubs.VINs);
                }
            }

            return new List<string>() { };
        }

        private IActionResult GenerateErrorResponse(HttpStatusCode statusCode, string errorCode = "", object value = null)
        {
            switch (statusCode)
            {
                case HttpStatusCode.BadRequest:
                    if (!string.IsNullOrEmpty(errorCode))
                        return StatusCode((int)statusCode, new
                        {
                            ResponseCode = ((int)statusCode).ToString(),
                            Message = errorCode,
                            Value = value
                        });
                    else
                        return StatusCode((int)statusCode, new
                        {
                            ResponseCode = ((int)statusCode).ToString(),
                            Message = "MISSING_PARAMETER",
                            Value = value
                        });
                case HttpStatusCode.NotFound:
                    return StatusCode((int)statusCode, new
                    {
                        ResponseCode = ((int)statusCode).ToString(),
                        Message = errorCode,
                        Value = value
                    });
                default:
                    return null;
            }
        }

        internal class ObjectComparer : IEqualityComparer<VisibilityVehicle>
        {
            public bool Equals(VisibilityVehicle x, VisibilityVehicle y)
            {
                if (object.ReferenceEquals(x, y))
                {
                    return true;
                }
                if (x is null || y is null)
                {
                    return false;
                }
                return x.Id == y.Id && x.VIN == y.VIN;
            }

            public int GetHashCode([DisallowNull] VisibilityVehicle obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                int idHashCode = obj.Id.GetHashCode();
                int vinHashCode = obj.VIN == null ? 0 : obj.VIN.GetHashCode();
                return idHashCode ^ vinHashCode;
            }
        }
    }
}
