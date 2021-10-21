using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class OfflinePushNotificationFilter
    {
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class NotificationAlertMessages
    {
        public string Vin { get; set; }
        public string AlertCategory { get; set; }
        public string AlertType { get; set; }
        public int AlertId { get; set; }
        public string VehicleName { get; set; }
        public string VehicleLicencePlate { get; set; }
        public string UrgencyLevel { get; set; }
        public int TripAlertId { get; set; }
        public string TripId { get; set; }
        public string AlertName { get; set; }
        public long AlertGeneratedTime { get; set; }
        public string AlertCategoryKey { get; set; }
        public string AlertTypeKey { get; set; }
        public string UrgencyTypeKey { get; set; }
        public int VehicleGroupId { get; set; }
        public string VehicleGroupName { get; set; }
        public int CreatedBy { get; set; }
        public int OrganizationId { get; set; }
    }
}
