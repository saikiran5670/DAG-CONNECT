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
        // private readonly ILogger<SubscriptionManagementService> _logger;

        private ILog _logger;
        private readonly ISubscriptionManager _subscriptionManager;

        public SubscriptionManagementService(ISubscriptionManager SubscriptionManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _subscriptionManager = SubscriptionManager;
        }

        public async override Task<SubscribeListResponce> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.subscription.entity.SubscriptionDetailsRequest objentityRequest = new net.atos.daf.ct2.subscription.entity.SubscriptionDetailsRequest();
                objentityRequest.OrganizationId = objSubscriptionDetailsRequest.OrganizationId;
                objentityRequest.Type = objSubscriptionDetailsRequest.Type;
                objentityRequest.State = (net.atos.daf.ct2.subscription.entity.StatusType)objSubscriptionDetailsRequest.State;

                SubscribeListResponce objSubscribeListResponce = new SubscribeListResponce();

                var listsubscription = await _subscriptionManager.Get(objentityRequest);
                foreach (var item in listsubscription)
                {
                    SubscriptionDetails objSubscriptionDetails = new SubscriptionDetails();
                    objSubscriptionDetails.SubscriptionId = item.SubscriptionId;
                    objSubscriptionDetails.Type = item.Type ?? null;
                    objSubscriptionDetails.Name = item.Name ?? null;
                    objSubscriptionDetails.PackageCode = item.PackageCode ?? null;
                    objSubscriptionDetails.SubscriptionStartDate = item.SubscriptionStartDate;
                    objSubscriptionDetails.SubscriptionEndDate = item.SubscriptionEndDate;
                    objSubscriptionDetails.State = item.State ?? null;
                    objSubscriptionDetails.Count = item.Count;
                    objSubscriptionDetails.OrgName = item.OrgName ?? null;
                    objSubscribeListResponce.SubscriptionList.Add(objSubscriptionDetails);
                }
                return objSubscribeListResponce;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw;
            }
        }
    }
}
