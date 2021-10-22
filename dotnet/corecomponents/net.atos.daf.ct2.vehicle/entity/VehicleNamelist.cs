﻿namespace net.atos.daf.ct2.vehicle.entity
{
    public class VehicleNamelist
    {
        public string Name { get; set; }
        public string RegistrationNo { get; set; }
    }

    public enum VehicleNamelistSSOContext
    {
        None = 0,
        Token,
        Org
    }
}
