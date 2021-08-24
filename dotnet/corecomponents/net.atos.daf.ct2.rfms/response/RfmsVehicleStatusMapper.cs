using System;
using System.Collections.Generic;
using net.atos.daf.ct2.rfms.entity;

namespace net.atos.daf.ct2.rfms.response
{
    public class RfmsVehicleStatusMapper
    {
        public AccumulatedData MapAccumuatedData()
        {

            var accumulatedData = new AccumulatedData()
            {
                DurationWheelbaseSpeedOverZero = 123456,
                DistanceCruiseControlActive = 23456,
                DurationCruiseControlActive = 45678,
                FuelConsumptionDuringCruiseActive = 987654,
                DurationWheelbaseSpeedZero = 12345,
                FuelWheelbaseSpeedZero = 87654,
                FuelWheelbaseSpeedOverZero = 0,
                PtoActiveClass = new List<PtoActiveClass> { new PtoActiveClass() {
               Label= MasterMemoryObjectCacheConstants.WHEELBASED_SPEED_OVER_ZERO,
              Seconds= 12345,
              Meters= 2345,
              MilliLitres= 3456} },
                BrakePedalCounterSpeedOverZero = 12765,
                DistanceBrakePedalActiveSpeedOverZero = 1456,
                AccelerationPedalPositionClass = new List<AccelerationPedalPositionClass>() { new AccelerationPedalPositionClass() {  From = 0,
                    To = 20,
                    Seconds = 23456,
                    Meters = 345678,
                    MilliLitres = 678345} },
                BrakePedalPositionClass = new List<BrakePedalPositionClass>() { new BrakePedalPositionClass() {  From = 0,
                    To = 20,
                    Seconds = 2456,
                    Meters = 34578,
                    MilliLitres = 67345 } },
                AccelerationClass = new List<AccelerationClass>{ new AccelerationClass() { From = -1.1,
                    To = -0.9,
                    Seconds = 23456,
                    Meters = 345678,
                    MilliLitres = 678345
                } },

                HighAccelerationClass = new List<HighAccelerationClass>(){new HighAccelerationClass()
            {
                    From = -3,
                    To = -2.5,
                    Seconds = 23456,
                    Meters = 345678,
                    MilliLitres = 678345
                }
                },
                RetarderTorqueClass = new List<RetarderTorqueClass>(){new RetarderTorqueClass()
            {
                    From = 0,
                    To = 20,
                    Seconds = 23456,
                    Meters = 345678,
                    MilliLitres = 678345
                }
                },
                DrivingWithoutTorqueClass = new List<DrivingWithoutTorqueClass>(){new DrivingWithoutTorqueClass()
            {
                    Label =MasterMemoryObjectCacheConstants.DRIVING_WITHOUT_TORQUE,
                    Seconds = 12345,
                    Meters = 2345,
                    MilliLitres = 3456
                }
                },
                EngineTorqueClass = new List<EngineTorqueClass>() {new EngineTorqueClass()            {
                    From = 0,
                    To = 10,
                    Seconds = 23456,
                    Meters = 345678,
                    MilliLitres = 678345
                }
         },
                EngineTorqueAtCurrentSpeedClass = new List<EngineTorqueAtCurrentSpeedClass>() {new EngineTorqueAtCurrentSpeedClass()
            {
                    From = 0,
                    To = 10,
                    Seconds = 23456,
                    Meters = 345678,
                    MilliLitres = 678345
                }
         },
                VehicleSpeedClass = new List<VehicleSpeedClass>(){new VehicleSpeedClass()
            {
                    From = 0,
                    To = 4,
                    Seconds = 23456,
                    Meters = 345678,
                    MilliLitres = 678345
                }
         },
                EngineSpeedClass = new List<EngineSpeedClass>() {new EngineSpeedClass()
            {
                    From = 0,
                    To = 400,
                    Seconds = 23456,
                    Meters = 345678,
                    MilliLitres = 678345
                }
         },
                AccelerationDuringBrakeClass = new List<AccelerationDuringBrakeClass>() { new  AccelerationDuringBrakeClass()
            {
                    From = -1.1,
                    To = -0.9,
                    Seconds = 23456,
                    Meters = 345678,
                    MilliLitres = 678345
                }
         },
                SelectedGearClass = new List<SelectedGearClass>() {new SelectedGearClass()
            {
                    Label = "0",
                    Seconds = 12345,
                    Meters = 2345,
                    MilliLitres = 3456
                }
         },
                CurrentGearClass = new List<CurrentGearClass>() { new CurrentGearClass()
            {
                    Label = "0",
                    Seconds = 12345,
                    Meters = 2345,
                    MilliLitres = 3456
                }
                },
                ChairliftCounter = 568,
                StopRequestCounter = 4567,
                KneelingCounter = 976,
                PramRequestCounter = 123


            };

            return accumulatedData;
        }


        public SnapshotData MapSnapShotData()
        {
            var snapshotData = new SnapshotData()
            {
                GnssPosition = new GnssPosition()
                {
                    Latitude = 57.71727,
                    Longitude = 11.921161,
                    Heading = 30,
                    Altitude = 32,
                    Speed = 54.5,
                    PositionDateTime = DateTime.UtcNow//2021-08-23T08=07=40.446Z
                },
                WheelBasedSpeed = 54.3,
                TachographSpeed = 54.4,
                EngineSpeed = 1234,
                FuelType = "1A",
                FuelLevel1 = 86,
                FuelLevel2 = 45,
                CatalystFuelLevel = 43,
                Driver1WorkingState = MasterMemoryObjectCacheConstants.DRIVE,
                Driver2Id = new Driver2Id()
                {
                    TachoDriverIdentification = new TachoDriverIdentification()
                    {
                        DriverIdentification = "12345678901234",
                        CardIssuingMemberState = "S",
                        DriverAuthenticationEquipment = MasterMemoryObjectCacheConstants.DRIVER_CARD,
                        CardReplacementIndex = "0",
                        CardRenewalIndex = "0"
                    },
                    OemDriverIdentification = new OemDriverIdentification()
                    {
                        IdType = "USB",
                        DriverIdentification = "ABC-123-DEF"
                    }
                },
                Driver2WorkingState = MasterMemoryObjectCacheConstants.DRIVE,
                AmbientAirTemperature = 23.7,
                ParkingBrakeSwitch = false,
                HybridBatteryPackRemainingCharge = 76
            };
            return snapshotData;

        }

        public UptimeData MapUptimeData()
        {

            var uptimeData = new UptimeData()
            {

                TellTaleInfo = new List<TellTaleInfo>(){ new TellTaleInfo()
                {
                    TellTale = MasterMemoryObjectCacheConstants.FUEL_LEVEL,
                    OemTellTale = MasterMemoryObjectCacheConstants.NO_GPS_SIGNAL,
                    State = MasterMemoryObjectCacheConstants.YELLOW
                }
                },
                ServiceDistance = 100000,
                EngineCoolantTemperature = 90,
                ServiceBrakeAirPressureCircuit1 = 512000,
                ServiceBrakeAirPressureCircuit2 = 534000,
                DurationAtLeastOneDoorOpen = 0,
                AlternatorInfo = new AlternatorInfo()
                {
                    AlternatorStatus = MasterMemoryObjectCacheConstants.CHARGING,
                    AlternatorNumber = 1
                },
                BellowPressureFrontAxleLeft = 234000,
                BellowPressureFrontAxleRight = 234000,
                BellowPressureRearAxleLeft = 234000,
                BellowPressureRearAxleRight = 234000
            };
            return uptimeData;


        }


    }
}
