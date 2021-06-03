namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleDataMart
    {
        public int ID { get; set; }
        public int VehicleID { get; set; }
        public string Name { get; set; }
        public string VIN { get; set; }
        public string Registration_No { get; set; }
        public string Vid { get; set; }
        public string Type { get; set; }
        public string Engine_Type { get; set; }
        public string Model_Type { get; set; }
        public bool IsIPPS { get; set; } = false;

    }
}
