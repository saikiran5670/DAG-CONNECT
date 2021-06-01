namespace net.atos.daf.ct2.vehicle.entity
{
    public class DtoVehicleMileage
    {
        public int Id { get; set; }
        public long Evt_timestamp { get; set; }
        public decimal Odo_mileage { get; set; }
        public decimal Odo_distance { get; set; }
        public decimal Real_distance { get; set; }
        public string Vin { get; set; }
        public long Modified_at { get; set; }
    }
}
