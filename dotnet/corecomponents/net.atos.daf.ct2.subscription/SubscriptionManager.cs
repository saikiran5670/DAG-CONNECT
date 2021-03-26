﻿using net.atos.daf.ct2.subscription.entity;
using System;
using net.atos.daf.ct2.subscription.repository;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace net.atos.daf.ct2.subscription
{
    public class SubscriptionManager : ISubscriptionManager
    {

        ISubscriptionRepository subscriptionRepository;

        public SubscriptionManager(ISubscriptionRepository _subscriptionRepository)
        {
            subscriptionRepository = _subscriptionRepository;
        }

        public async Task<SubscriptionResponse> Subscribe(SubscriptionActivation objSubscription)
        {
            return await subscriptionRepository.Subscribe(objSubscription);
        }

        public async Task<SubscriptionResponse> Unsubscribe(UnSubscription objUnSubscription)
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
