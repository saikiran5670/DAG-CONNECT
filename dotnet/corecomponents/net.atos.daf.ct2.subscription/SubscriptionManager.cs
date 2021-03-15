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

        public async Task<SubscriptionResponse> Subscribe(SubscriptionActivation objSubscription)
        {
            return await subscriptionRepository.Subscribe(objSubscription);
        }

        public async Task<SubscriptionResponse> Unsubscribe(UnSubscription objUnSubscription)
        {
            return await subscriptionRepository.Unsubscribe(objUnSubscription);
        }

        public async Task<Subscription> Create(Subscription subscription)
        {
            return await subscriptionRepository.Create(subscription);
        }

        public async Task<Subscription> Update(Subscription subscription)
        {
            return await subscriptionRepository.Update(subscription);
        }

        public async Task<Subscription> Get(int subscriptionId)
        {
            return await subscriptionRepository.Get(subscriptionId);
        }

        public async Task<Subscription> Get(int organizationId, int vehicleId, char status, DateTime StartDate, DateTime EndDate)
        {
            return await subscriptionRepository.Get(organizationId, vehicleId, status, StartDate, EndDate);
        }

        public async Task<Subscription> Get(char status, int vehicleGroupID, int vehicleId, DateTime StartDate, DateTime EndDate)
        {
            return await subscriptionRepository.Get(status, vehicleGroupID, vehicleId, StartDate, EndDate);
        }
    }
}
