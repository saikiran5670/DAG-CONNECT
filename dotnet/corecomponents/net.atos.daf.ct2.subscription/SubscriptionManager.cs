using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using net.atos.daf.ct2.subscription.entity;
using net.atos.daf.ct2.subscription.repository;

namespace net.atos.daf.ct2.subscription
{
    public class SubscriptionManager : ISubscriptionManager
    {
        ISubscriptionRepository subscriptionRepository;

        public SubscriptionManager(ISubscriptionRepository _subscriptionRepository)
        {
            subscriptionRepository = _subscriptionRepository;
        }

        public async Task<Tuple<HttpStatusCode, SubscriptionResponse>> Subscribe(SubscriptionActivation objSubscription)
        {
            return await subscriptionRepository.Subscribe(objSubscription);
        }

        public async Task<Tuple<HttpStatusCode, SubscriptionResponse>> Unsubscribe(UnSubscription objUnSubscription)
        {
            return await subscriptionRepository.Unsubscribe(objUnSubscription);
        }

        public async Task<SubscriptionResponse> Create(int orgId, int packageId)
        {
            return await subscriptionRepository.Create(orgId, packageId);
        }
        public async Task<List<SubscriptionDetails>> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest)
        {
            return await subscriptionRepository.Get(objSubscriptionDetailsRequest);
        }

    }
}
