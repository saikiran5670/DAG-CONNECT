using System;
using System.Threading.Tasks;
using net.atos.daf.ct2.subscription.entity;

namespace net.atos.daf.ct2.subscription
{
    public interface ISubscriptionManager
    {
        Task<SubscriptionResponse> Subscribe(Subscription objSubscription);
        Task<SubscriptionResponse> Unsubscribe(UnSubscription objUnSubscription);
    }
}
