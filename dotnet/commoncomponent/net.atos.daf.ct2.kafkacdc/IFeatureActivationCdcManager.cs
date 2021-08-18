using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.kafkacdc
{
    public interface IFeatureActivationCdcManager
    {
        Task<bool> GetVehiclesAndAlertFromSubscriptionConfiguration(int subscriptionId, string operation);
    }
}
