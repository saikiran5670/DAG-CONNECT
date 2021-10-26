using System.Collections.Generic;

namespace net.atos.daf.ct2.vehicle.entity
{
    public class VisibilityVehicle
    {
        public int Id { get; set; }
        public string VIN { get; set; }
        public string Name { get; set; }
        public string RegistrationNo { get; set; }
        public bool HasOwned { get; set; }
        public long[] Btype_Features { get; set; }
        public List<long> FeatureIds { get; set; }
        public string SubscriptionType { get; set; }
    }
}
