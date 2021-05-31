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
        private readonly ISubscriptionManager _SubscriptionManager;

        public SubscriptionManagementService(ISubscriptionManager SubscriptionManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _SubscriptionManager = SubscriptionManager;
        }

        public async override Task<SubscribeListResponce> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.subscription.entity.SubscriptionDetailsRequest objentityRequest = new net.atos.daf.ct2.subscription.entity.SubscriptionDetailsRequest();
                objentityRequest.organization_id = objSubscriptionDetailsRequest.OrganizationId;
                objentityRequest.type = objSubscriptionDetailsRequest.Type;
                objentityRequest.state = (net.atos.daf.ct2.subscription.entity.StatusType)objSubscriptionDetailsRequest.State;

                SubscribeListResponce objSubscribeListResponce = new SubscribeListResponce();

                var listsubscription = await _SubscriptionManager.Get(objentityRequest);
                foreach (var item in listsubscription)
                {
                    SubscriptionDetails objSubscriptionDetails = new SubscriptionDetails();
                    objSubscriptionDetails.SubscriptionId = item.subscription_id;
                    objSubscriptionDetails.Type = item.type == null ? string.Empty : item.type;
                    objSubscriptionDetails.Name = item.name == null ? string.Empty : item.name;
                    objSubscriptionDetails.PackageCode = item.package_code == null ? string.Empty : item.package_code;
                    objSubscriptionDetails.SubscriptionStartDate = item.subscription_start_date;
                    objSubscriptionDetails.SubscriptionEndDate = item.subscription_end_date;
                    objSubscriptionDetails.State = item.state == null ? string.Empty : item.state;
                    objSubscriptionDetails.Count = item.count;
                    objSubscriptionDetails.OrgName = item.orgname == null ? string.Empty : item.orgname;
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
