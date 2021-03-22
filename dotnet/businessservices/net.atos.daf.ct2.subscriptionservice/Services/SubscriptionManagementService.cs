using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.subscription.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.subscriptionservice
{
    public class SubscriptionManagementService : SubscribeGRPCService.SubscribeGRPCServiceBase
    {
        private readonly ILogger<SubscriptionManagementService> _logger;
        private readonly ISubscriptionManager _SubscriptionManager;

        public SubscriptionManagementService(ILogger<SubscriptionManagementService> logger, ISubscriptionManager SubscriptionManager)
        {
            _logger = logger;
            _SubscriptionManager = SubscriptionManager;
        }

        public async override Task<SubscribeListResponce> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.subscription.entity.SubscriptionDetailsRequest objentityRequest = new net.atos.daf.ct2.subscription.entity.SubscriptionDetailsRequest();
                objentityRequest.organization_id = objSubscriptionDetailsRequest.OrganizationId;
                objentityRequest.type = objSubscriptionDetailsRequest.Type;
                objentityRequest.is_active = (net.atos.daf.ct2.subscription.entity.StatusType)objSubscriptionDetailsRequest.IsActive;

                SubscribeListResponce objSubscribeListResponce = new SubscribeListResponce();
                
                var listsubscription = await _SubscriptionManager.Get(objentityRequest);
                foreach (var item in listsubscription)
                {
                    SubscriptionDetails objSubscriptionDetails = new SubscriptionDetails();
                    objSubscriptionDetails.SubscriptionId = item.subscription_id;
                    objSubscriptionDetails.Type = item.type == null? string.Empty: item.type;
                    objSubscriptionDetails.Name = item.name == null ? string.Empty : item.name;
                    objSubscriptionDetails.PackageCode = item.package_code == null ? string.Empty : item.package_code;
                    objSubscriptionDetails.SubscriptionStartDate = item.subscription_start_date;
                    objSubscriptionDetails.SubscriptionEndDate = item.subscription_end_date;
                    objSubscriptionDetails.IsActive = item.is_active;
                    objSubscriptionDetails.Count = item.count;

                    objSubscribeListResponce.Responce.Add(objSubscriptionDetails);
                }
                return objSubscribeListResponce;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
