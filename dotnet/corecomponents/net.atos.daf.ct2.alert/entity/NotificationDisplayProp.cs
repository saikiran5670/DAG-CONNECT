using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.alert.entity
{
    public class NotificationDisplayProp
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
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
    }

    public class NotificationAccount
    {
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
        public int NotificationCount { get; set; }

    }

    public class OfflinePushNotification
    {
        public List<NotificationDisplayProp> NotificationDisplayProp { get; set; }
        public NotificationAccount NotificationAccount { get; set; }
    }

    public class OfflinePushNotificationFilter
    {
        public int AccountId { get; set; }
        public int OrganizationId { get; set; }
        public List<string> Vins { get; set; }
        public List<int> FeatureIds { get; set; }
    }

    public class AlertVehicleGroup
    {
        public int AlertId { get; set; }
        public int VehicleGroupId { get; set; }
        public string VehicleGroupName { get; set; }
        public int OrganizationId { get; set; }
        public int AlertCreatedAccountId { get; set; }
    }
}
