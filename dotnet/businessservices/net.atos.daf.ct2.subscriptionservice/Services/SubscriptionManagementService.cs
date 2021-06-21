using System;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.subscription;

namespace net.atos.daf.ct2.subscriptionservice
{
    public class SubscriptionManagementService : SubscribeGRPCService.SubscribeGRPCServiceBase
    {
        private readonly ILog _logger;
        private readonly ISubscriptionManager _subscriptionManager;

        public SubscriptionManagementService(ISubscriptionManager subscriptionManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _subscriptionManager = subscriptionManager;
        }

        public async override Task<SubscribeListResponce> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest, ServerCallContext context)
        {
            try
            {
                subscription.entity.SubscriptionDetailsRequest objentityRequest = new subscription.entity.SubscriptionDetailsRequest();
                objentityRequest.OrganizationId = objSubscriptionDetailsRequest.OrganizationId;
                objentityRequest.Type = objSubscriptionDetailsRequest.Type;
                objentityRequest.State = (subscription.entity.StatusType)objSubscriptionDetailsRequest.State;

                SubscribeListResponce objSubscribeListResponse = new SubscribeListResponce();

                var listsubscription = await _subscriptionManager.Get(objentityRequest);
                foreach (var item in listsubscription)
                {
                    SubscriptionDetails objSubscriptionDetails = new SubscriptionDetails();
                    objSubscriptionDetails.SubscriptionId = item.SubscriptionId;
                    objSubscriptionDetails.Type = item.Type ?? string.Empty;
                    objSubscriptionDetails.Name = item.Name ?? string.Empty;
                    objSubscriptionDetails.PackageCode = item.PackageCode ?? string.Empty;
                    objSubscriptionDetails.SubscriptionStartDate = item.SubscriptionStartDate;
                    objSubscriptionDetails.SubscriptionEndDate = item.SubscriptionEndDate;
                    objSubscriptionDetails.State = item.State ?? string.Empty;
                    objSubscriptionDetails.Count = item.Count;
                    objSubscriptionDetails.OrgName = item.OrgName ?? string.Empty;
                    objSubscribeListResponse.SubscriptionList.Add(objSubscriptionDetails);
                }
                return objSubscribeListResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return new SubscribeListResponce();
            }
        }
    }
}
