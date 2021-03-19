using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.subscription.entity;

namespace net.atos.daf.ct2.subscription
{
    public interface ISubscriptionManager
    {
        Task<SubscriptionResponse> Subscribe(SubscriptionActivation objSubscription);
        Task<SubscriptionResponse> Unsubscribe(UnSubscription objUnSubscription);

        Task<Subscription> Create(Subscription subscription);
        Task<Subscription> Update(Subscription subscription);
        //Task<Subscription> Get(int subscriptionId);
        Task<Subscription> Get(int organizationId, int vehicleId, char status, DateTime StartDate, DateTime EndDate);
        Task<Subscription> Get(char status, int vehicleGroupID, int vehicleId,  DateTime StartDate, DateTime EndDate);
        Task<SubscriptionResponse> Create(int orgId, int packageId);
        Task<IEnumerable<SubscriptionDetails>> Get(SubscriptionDetails objSubscriptionDetails);
    }
}
