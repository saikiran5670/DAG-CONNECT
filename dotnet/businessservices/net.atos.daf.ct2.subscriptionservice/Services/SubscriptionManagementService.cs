using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.subscription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.subscriptionservice
{
    public class SubscriptionManagementService
    {
        private readonly ILogger<SubscriptionManagementService> _logger;
        private readonly ISubscriptionManager _SubscriptionManager;

        public SubscriptionManagementService(ILogger<SubscriptionManagementService> logger, ISubscriptionManager SubscriptionManager)
        {
            _logger = logger;
            _SubscriptionManager = SubscriptionManager;
        }

    }
}
