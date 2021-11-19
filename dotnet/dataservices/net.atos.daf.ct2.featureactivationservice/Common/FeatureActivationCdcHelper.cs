using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc;

namespace net.atos.daf.ct2.featureactivationservice.Common
{
    public class FeatureActivationCdcHelper
    {
        private readonly IFeatureActivationCdcManager _featureActivationCdcManager;

        public FeatureActivationCdcHelper(IFeatureActivationCdcManager featureActivationCdcManager)
        {
            _featureActivationCdcManager = featureActivationCdcManager;
        }
        public async Task TriggerSubscriptionCdc(int subscriptionid, string subscriptionState, int organisationId, List<string> vins)
        {
            _ = await Task.Run(() => _featureActivationCdcManager.GetVehiclesAndAlertFromSubscriptionConfiguration(subscriptionid, subscriptionState, organisationId, vins));
        }
    }
}
