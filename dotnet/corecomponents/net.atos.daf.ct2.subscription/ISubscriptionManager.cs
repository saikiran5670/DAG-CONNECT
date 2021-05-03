using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using net.atos.daf.ct2.subscription.entity;

namespace net.atos.daf.ct2.subscription
{
    public interface ISubscriptionManager
    {
        Task<Tuple<HttpStatusCode, SubscriptionResponse>> Subscribe(SubscriptionActivation objSubscription);
        Task<Tuple<HttpStatusCode, SubscriptionResponse>> Unsubscribe(UnSubscription objUnSubscription);
        Task<SubscriptionResponse> Create(int orgId, int packageId);
        Task<List<SubscriptionDetails>> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest);
    }
}
