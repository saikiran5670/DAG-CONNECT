
using System;
using System.Collections.Generic;

namespace net.atos.daf.ct2.rfms.response
{

    public class Driver1Id
    {
        public TachoDriverIdentification TachoDriverIdentification { get; set; }
        public OemDriverIdentification OemDriverIdentification { get; set; }
    }

    public class DoorStatu
    {
        public string DoorEnabledStatus { get; set; }
        public string DoorOpenStatus { get; set; }
        public string DoorLockStatus { get; set; }
        public int DoorNumber { get; set; }
    }

    public class PtoActiveClass
    {
        public string Label { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class AccelerationPedalPositionClass
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class BrakePedalPositionClass
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class AccelerationClass
    {
        public double From { get; set; }
        public double To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class HighAccelerationClass
    {
        public int From { get; set; }
        public double To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class RetarderTorqueClass
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class DrivingWithoutTorqueClass
    {
        public string Label { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class EngineTorqueClass
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class EngineTorqueAtCurrentSpeedClass
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class VehicleSpeedClass
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class EngineSpeedClass
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class AccelerationDuringBrakeClass
    {
        public double From { get; set; }
        public double To { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class SelectedGearClass
    {
        public string Label { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class CurrentGearClass
    {
        public string Label { get; set; }
        public int Seconds { get; set; }
        public int Meters { get; set; }
        public int MilliLitres { get; set; }
    }

    public class AccumulatedData
    {
        public int? DurationWheelbaseSpeedOverZero { get; set; }
        public int? DistanceCruiseControlActive { get; set; }
        public int? DurationCruiseControlActive { get; set; }
        public int? FuelConsumptionDuringCruiseActive { get; set; }
        public int? DurationWheelbaseSpeedZero { get; set; }
        public int? FuelWheelbaseSpeedZero { get; set; }
        public int? FuelWheelbaseSpeedOverZero { get; set; }
        public List<PtoActiveClass> PtoActiveClass { get; set; }
        public int? BrakePedalCounterSpeedOverZero { get; set; }
        public int? DistanceBrakePedalActiveSpeedOverZero { get; set; }
        public List<AccelerationPedalPositionClass> AccelerationPedalPositionClass { get; set; }
        public List<BrakePedalPositionClass> BrakePedalPositionClass { get; set; }
        public List<AccelerationClass> AccelerationClass { get; set; }
        public List<HighAccelerationClass> HighAccelerationClass { get; set; }
        public List<RetarderTorqueClass> RetarderTorqueClass { get; set; }
        public List<DrivingWithoutTorqueClass> DrivingWithoutTorqueClass { get; set; }
        public List<EngineTorqueClass> EngineTorqueClass { get; set; }
        public List<EngineTorqueAtCurrentSpeedClass> EngineTorqueAtCurrentSpeedClass { get; set; }
        public List<VehicleSpeedClass> VehicleSpeedClass { get; set; }
        public List<EngineSpeedClass> EngineSpeedClass { get; set; }
        public List<AccelerationDuringBrakeClass> AccelerationDuringBrakeClass { get; set; }
        public List<SelectedGearClass> SelectedGearClass { get; set; }
        public List<CurrentGearClass> CurrentGearClass { get; set; }
        //public int ChairliftCounter { get; set; }
        //public int StopRequestCounter { get; set; }
        //public int KneelingCounter { get; set; }
        //public int PramRequestCounter { get; set; }
    }

    public class Driver2Id
    {
        public TachoDriverIdentification TachoDriverIdentification { get; set; }
        public OemDriverIdentification OemDriverIdentification { get; set; }
    }

    public class SnapshotData
    {
        public GnssPosition GnssPosition { get; set; }
        public double? WheelBasedSpeed { get; set; }
        public double? TachographSpeed { get; set; }
        public int? EngineSpeed { get; set; }
        public string FuelType { get; set; }
        public int? FuelLevel1 { get; set; }
        //public int? FuelLevel2 { get; set; }
        public int? CatalystFuelLevel { get; set; }
        public string Driver1WorkingState { get; set; }
        public Driver2Id Driver2Id { get; set; }
        public string Driver2WorkingState { get; set; }
        public double? AmbientAirTemperature { get; set; }
        public bool? ParkingBrakeSwitch { get; set; } //optional for this delivery
        public int? HybridBatteryPackRemainingCharge { get; set; } //optional for this delivery
    }

    public class AlternatorInfo
    {
        //public string AlternatorStatus { get; set; }
        public int? AlternatorNumber { get; set; }
    }

    public class UptimeData
    {
        public List<TellTaleInfo> TellTaleInfo { get; set; }
        public int? ServiceDistance { get; set; }
        public int? EngineCoolantTemperature { get; set; }
        public int? ServiceBrakeAirPressureCircuit1 { get; set; }
        public int? ServiceBrakeAirPressureCircuit2 { get; set; }
       // public int DurationAtLeastOneDoorOpen { get; set; }
        public AlternatorInfo AlternatorInfo { get; set; }
        //public int BellowPressureFrontAxleLeft { get; set; }
        //public int BellowPressureFrontAxleRight { get; set; }
        //public int BellowPressureRearAxleLeft { get; set; }
        //public int BellowPressureRearAxleRight { get; set; }
    }

    public class VehicleStatus
    {
        public int RecordId { get; set; }
        public string Vin { get; set; }
        public TriggerType TriggerType { get; set; }
        public string CreatedDateTime { get; set; }
        public string ReceivedDateTime { get; set; }
        public int? HrTotalVehicleDistance { get; set; }
        public double TotalEngineHours { get; set; }
        public Driver1Id Driver1Id { get; set; }
        public int? GrossCombinationVehicleWeight { get; set; }
        public int? EngineTotalFuelUsed { get; set; }
        public int? TotalFuelUsedGaseous { get; set; } //optional
       // public string Status2OfDoors { get; set; }
        public AccumulatedData AccumulatedData { get; set; }
        public SnapshotData SnapshotData { get; set; }
        public UptimeData UptimeData { get; set; }
    }

    public class VehicleStatusResponse
    {
        public List<VehicleStatus> VehicleStatuses { get; set; }
    }

    public class RfmsVehicleStatus
    {
        public VehicleStatusResponse VehicleStatusResponse { get; set; }
        public bool MoreDataAvailable { get; set; }
        public string MoreDataAvailableLink { get; set; }
        public string RequestServerDateTime { get; set; }

    }
}