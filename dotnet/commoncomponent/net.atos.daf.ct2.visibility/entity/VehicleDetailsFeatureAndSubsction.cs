using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.visibility.entity
{
    public class VehicleDetailsFeatureAndSubsction
    {
        public int FeatureId { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public int VehicleId { get; set; }

        public int OrganizationId { get; set; }

        public string SubscriptionType { get; set; }

        public string FeatureEnum { get; set; }
    }
}
