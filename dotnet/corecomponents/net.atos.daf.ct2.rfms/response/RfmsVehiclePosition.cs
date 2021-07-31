using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.rfms.response
{
    public class TachoDriverIdentification
    {
        public string DriverIdentification { get; set; }
        public string CardIssuingMemberState { get; set; }
        public string DriverAuthenticationEquipment { get; set; }
        public string CardReplacementIndex { get; set; }
        public string CardRenewalIndex { get; set; }
    }

    public class OemDriverIdentification
    {
        public string IdType { get; set; }
        public string DriverIdentification { get; set; }
    }

    public class DriverId
    {
        public TachoDriverIdentification TachoDriverIdentification { get; set; }
        public OemDriverIdentification OemDriverIdentification { get; set; }
    }

    public class TellTaleInfo
    {
        public string TellTale { get; set; }
        public string OemTellTale { get; set; }
        public string State { get; set; }
    }

    public class TriggerType
    {
        public string Type { get; set; } //TriggerType
        public string Context { get; set; }
        public List<string> TriggerInfo { get; set; }
        public DriverId DriverId { get; set; }
        public string PtoId { get; set; }
        public TellTaleInfo TellTaleInfo { get; set; }
    }

    public class GnssPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Heading { get; set; }
        public int Altitude { get; set; }
        public double Speed { get; set; }
        public DateTime PositionDateTime { get; set; }
    }

    public class VehiclePosition
    {
        public int RecordId { get; set; }
        public string Vin { get; set; }
        public TriggerType TriggerType { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public GnssPosition GnssPosition { get; set; }
        public double WheelBasedSpeed { get; set; }
        public double TachographSpeed { get; set; }
    }

    public class VehiclePositionResponse
    {
        public List<VehiclePosition> VehiclePositions { get; set; }
    }

    public class RfmsVehiclePosition
    {
        public VehiclePositionResponse VehiclePositionResponse { get; set; }
        public bool MoreDataAvailable { get; set; }
        public string MoreDataAvailableLink { get; set; }
        public DateTime RequestServerDateTime { get; set; }
    }

}
