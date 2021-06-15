namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleConnectSettings
    {
        public int Id { get; set; }
        public char Status { get; set; }
        public char Opt_In { get; set; }
    }

    public class VehicleConnect : VehicleConnectSettings
    {
        public bool IsConnected { get; set; }


    }
    public class VehicleTerminate : VehicleConnectSettings
    {
        public bool IsTerminated { get; set; }


    }
}
