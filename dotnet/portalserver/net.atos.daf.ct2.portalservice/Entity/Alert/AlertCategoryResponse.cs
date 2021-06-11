using System.Collections.Generic;

namespace net.atos.daf.ct2.portalservice.Entity.Alert
{
    public class AlertCategoryResponse
    {
        public List<EnumTranslation> EnumTranslation { get; set; } = new List<EnumTranslation>();
        public List<VehicleGroup> VehicleGroup { get; set; } = new List<VehicleGroup>();
        public List<NotificationTemplate> NotificationTemplate { get; set; } = new List<NotificationTemplate>();
    }
    public class EnumTranslation
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Enum { get; set; }
        public string ParentEnum { get; set; }
        public string Key { get; set; }
    }
    public class VehicleGroup
    {
        public int VehicleGroupId { get; set; }
        public string VehicleGroupName { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; }
        public string Vin { get; set; }
        public string RegNo { get; set; }
        public bool SubcriptionStatus { get; set; }
    }
    public class NotificationTemplate
    {
        public int Id { get; set; }
        public string AlertCategoryType { get; set; }
        public string AlertType { get; set; }
        public string Text { get; set; }
        public string Subject { get; set; }
    }
}
