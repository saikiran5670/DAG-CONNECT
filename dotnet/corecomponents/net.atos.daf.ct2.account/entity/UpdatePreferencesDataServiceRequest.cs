namespace net.atos.daf.ct2.account.entity
{
    public class UpdatePreferencesDataServiceRequest
    {
        public string AccountEmail { get; set; }
        public string DriverId { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string UnitDisplay { get; set; }
        public string VehicleDisplay { get; set; }
    }
}