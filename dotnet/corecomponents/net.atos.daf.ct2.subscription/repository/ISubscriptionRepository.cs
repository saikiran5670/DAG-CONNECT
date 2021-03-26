using net.atos.daf.ct2.subscription.entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.subscription.repository
{
    public interface ISubscriptionRepository
    {
         Task<SubscriptionResponse> Subscribe(SubscriptionActivation objSubscription);
         Task<SubscriptionResponse> Unsubscribe(UnSubscription objUnSubscription);
        Task<SubscriptionResponse> Create(int orgId,int packageId);
        Task<List<SubscriptionDetails>> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest);
    }
}
