using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc;

namespace net.atos.daf.ct2.customerdataservice.Common
{
    public class CustomerDataCdcHelper
    {
        private readonly ICustomerDataCdcManager _customerDataCdcManager;

        public CustomerDataCdcHelper(ICustomerDataCdcManager customerDataCdcManager)
        {
            _customerDataCdcManager = customerDataCdcManager;
        }
        public async Task TriggerSubscriptionCdc(int subscriptionid, string subscriptionState)
        {
            _ = await Task.Run(() => _customerDataCdcManager.GetVehiclesAndAlertFromCustomerDataConfiguration(subscriptionid, subscriptionState));
        }
    }
}
