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

        public async override Task<SubscribeListResponce> Get(SubscriptionDetails objgrpcSubscriptionDetail, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.subscription.entity.SubscriptionDetails objSubscriptionDetails = new subscription.entity.SubscriptionDetails();
                objSubscriptionDetails.subscription_id = objgrpcSubscriptionDetail.SubscriptionId;
                var listsubscription = await _SubscriptionManager.Get(objSubscriptionDetails);
                SubscribeListResponce objSubscribeListResponce = new SubscribeListResponce();
                foreach (var item in listsubscription)
                {
                    objgrpcSubscriptionDetail = new SubscriptionDetails();
                    objSubscriptionDetails.subscription_id = item.subscription_id;
                    objSubscriptionDetails.type = item.type;
                    objSubscriptionDetails.name = item.name;
                    objSubscriptionDetails.package_code = item.package_code;
                    objSubscriptionDetails.subscription_start_date = item.subscription_start_date;
                    objSubscriptionDetails.subscription_end_date = item.subscription_end_date;
                    objSubscriptionDetails.is_active = item.is_active;
                    objSubscribeListResponce.Responce.Add(objgrpcSubscriptionDetail);
                }
                //Dataresponce.Code = Responcecode.Success;
                return objSubscribeListResponce;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
