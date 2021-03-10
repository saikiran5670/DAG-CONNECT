using net.atos.daf.ct2.subscription.entity;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.subscription.repository
{
    public interface ISubscriptionRepository
    {
         Task<SubscriptionResponse> Subscribe(Subscription objSubscription);
         Task<SubscriptionResponse> Unsubscribe(UnSubscription objUnSubscription);
    }
}
