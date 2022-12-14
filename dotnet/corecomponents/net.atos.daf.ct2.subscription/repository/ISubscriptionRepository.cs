using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using net.atos.daf.ct2.subscription.entity;

namespace net.atos.daf.ct2.subscription.repository
{
    public interface ISubscriptionRepository
    {
        Task<Tuple<HttpStatusCode, SubscriptionResponse>> Subscribe(SubscriptionActivation objSubscription, IEnumerable<string> visibleVINs);
        Task<Tuple<HttpStatusCode, SubscriptionResponse>> Unsubscribe(UnSubscription objUnSubscription);
        Task<SubscriptionResponse> Create(int orgId, int packageId);
        Task<List<SubscriptionDetails>> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest);
        Task<int> GetOrganizationIdByCode(string organizationCode);
        Task<Package> GetPackageTypeByCode(string packageCode);
    }
}
