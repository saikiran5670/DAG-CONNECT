namespace net.atos.daf.ct2.vehicle.entity
{
    public class DtoVehicleMileage
    {
        public string EvtDateTime { get; set; }
        public string VIN { get; set; }
        public decimal? TachoMileage { get; set; }
        public decimal? GPSMileage { get; set; }
        public string RealMileageAlgorithmVersion { get; set; }
    }
}
