using net.atos.daf.ct2.subscription.entity;
using System;
using net.atos.daf.ct2.subscription.repository;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.subscription
{
    public class SubscriptionManager : ISubscriptionManager
    {

        ISubscriptionRepository subscriptionRepository;

        public SubscriptionManager(ISubscriptionRepository _subscriptionRepository)
        {
            subscriptionRepository = _subscriptionRepository;
        }

        public async Task<SubscriptionResponse> Subscribe(Subscription objSubscription)
        {
            return await subscriptionRepository.Subscribe(objSubscription);
        }

        public async Task<SubscriptionResponse> Unsubscribe(UnSubscription objUnSubscription)
        {
            return await subscriptionRepository.Unsubscribe(objUnSubscription);
        }
    }
}
