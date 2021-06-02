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
        ISubscriptionRepository _subscriptionRepository;

        public SubscriptionManager(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Tuple<HttpStatusCode, SubscriptionResponse>> Subscribe(SubscriptionActivation objSubscription)
        {
            return await _subscriptionRepository.Subscribe(objSubscription);
        }

        public async Task<Tuple<HttpStatusCode, SubscriptionResponse>> Unsubscribe(UnSubscription objUnSubscription)
        {
            return await _subscriptionRepository.Unsubscribe(objUnSubscription);
        }

        public async Task<SubscriptionResponse> Create(int orgId, int packageId)
        {
            return await _subscriptionRepository.Create(orgId, packageId);
        }
        public async Task<List<SubscriptionDetails>> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest)
        {
            return await _subscriptionRepository.Get(objSubscriptionDetailsRequest);
        }

    }
}
