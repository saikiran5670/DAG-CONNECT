namespace net.atos.daf.ct2.poigeofence.entity
{
    public class TripDetails
    {
        public int Id { get; set; }
        public string TripId { get; set; }
        public string VIN { get; set; }
        public long StartTimeStamp { get; set; }
        public long EndTimeStamp { get; set; }
        public int VehMessageDistance { get; set; }
        public long EtlGPSDistance { get; set; }
        public int IdleDuration { get; set; }
        public double AverageSpeed { get; set; }
        public double AverageWeight { get; set; }
        public long StartOdometer { get; set; }
        public long FastOdometer { get; set; }
        public double StartPositionLattitude { get; set; }
        public double StartPositionLognitude { get; set; }
        public double EndPositionLattitude { get; set; }
        public double EndPositionLognitude { get; set; }
        public string StartPosition { get; set; }
        public string EndPosition { get; set; }
        public int VehMessageFuelConsumed { get; set; }
        public long EtlGpsFuelConsumed { get; set; }
        public int VehMessageDrivingTime { get; set; }
        public int EtlGpsDrivingTime { get; set; }
        public int NoOfAlerts { get; set; }
        public int NoOfEvents { get; set; }
        public long MessageReceivedTimestamp { get; set; }
        public long MessageInsertedIntoKafkaTimestamp { get; set; }
        public long MessageInsertedIntohbaseTimestamp { get; set; }
        public long MessageProcessedByEtlProcessTimestamp { get; set; }
        public decimal Co2Emission { get; set; }
        public decimal FuelConsumed { get; set; }
        public decimal MaxSpeed { get; set; }
        public decimal AverageGrossWeightComb { get; set; }
        public decimal PtoDuration { get; set; }
        public decimal HarshBrakeDuration { get; set; }
        public decimal HeavyThrottleDuration { get; set; }
        public decimal CruiseControlDistance3050 { get; set; }
        public decimal CruiseControlDistance5075 { get; set; }
        public decimal CruiseControlDistanceMoreThan75 { get; set; }
        public string AverageTrafficClassification { get; set; }
        public decimal CcFuelConsumption { get; set; }
        public int VCruiseControlFuelConsumedForCcfuelConsumption { get; set; }
        public int VCruiseControlDistForCcfuelConsumption { get; set; }
        public decimal FuelConsumptionCcNonActive { get; set; }
        public int IdlingConsumption { get; set; }
        public decimal DpaScore { get; set; }
        public decimal EnduranceBrake { get; set; }
        public decimal Coasting { get; set; }
        public decimal EcoRolling { get; set; }
        public string Driver1Id { get; set; }
        public string Driver2Id { get; set; }
        public int EtlGpstripTime { get; set; }
        public bool IsOngoingTrip { get; set; }
    }
}