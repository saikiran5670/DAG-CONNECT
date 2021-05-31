namespace net.atos.daf.ct2.vehicle.entity
{
    public class dtoVehicleMileage
    {
        public int id { get; set; }
        public long evt_timestamp { get; set; }
        public decimal odo_mileage { get; set; }
        public decimal odo_distance { get; set; }
        public decimal real_distance { get; set; }
        public string vin { get; set; }
        public long modified_at { get; set; }
    }
}
