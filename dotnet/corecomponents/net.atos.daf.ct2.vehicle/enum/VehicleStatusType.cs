namespace net.atos.daf.ct2.vehicle
{
    public enum VehicleStatusType
    {
        None = 'N',
        OptIn = 'I',
        OptOut = 'U',
        Terminate = 'T',
        Ota = 'O',
        Inherit = 'H'
    }
    public enum VehicleCalculatedStatus
    {
        Off = 'O',
        Connected = 'C',
        Connected_OTA = 'N',
        OTA = 'A',
        Terminate = 'T'
    }
}
