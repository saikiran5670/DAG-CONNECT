using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.rfms.response
{
    public class RfmsVehicleMapper
    {
        internal VehiclePosition MapVehiclePositions(dynamic record)
        {
            VehiclePosition vehiclePosition = new VehiclePosition();
            vehiclePosition.RecordId = record.id;
            vehiclePosition.Vin = record.vin;

            TriggerType triggerType = new TriggerType();
            triggerType.Context = record.context;
            triggerType.Type = Convert.ToString(record.triggertype);

            List<string> listTriggerInfo = new List<string>();
            listTriggerInfo.Add(record.triggerinfo);

            triggerType.TriggerInfo = listTriggerInfo;

            DriverId driverId = new DriverId();

            TachoDriverIdentification tachoDriverIdentification = new TachoDriverIdentification();
            tachoDriverIdentification.DriverIdentification = record.tachodriveridentification;
            tachoDriverIdentification.DriverAuthenticationEquipment = Convert.ToString(record.driverauthenticationequipment);
            tachoDriverIdentification.CardReplacementIndex = record.cardreplacementindex;
            tachoDriverIdentification.CardRenewalIndex = record.cardrenewalindex;
            tachoDriverIdentification.CardIssuingMemberState = record.cardissuingmemberstate;

            OemDriverIdentification oemDriverIdentification = new OemDriverIdentification();
            oemDriverIdentification.DriverIdentification = record.oemdriveridentification;
            oemDriverIdentification.IdType = record.oemidtype;

            driverId.TachoDriverIdentification = tachoDriverIdentification;
            driverId.OemDriverIdentification = oemDriverIdentification;

            triggerType.DriverId = driverId;
            triggerType.PtoId = record.ptoid;

            TellTaleInfo tellTaleInfo = new TellTaleInfo();
            tellTaleInfo.OemTellTale = record.oemtelltale;
            tellTaleInfo.State = Convert.ToString(record.state);
            tellTaleInfo.TellTale = Convert.ToString(record.telltale);

            triggerType.TellTaleInfo = tellTaleInfo;

            vehiclePosition.TriggerType = triggerType;

            if (record.createddatetime != null)
            {
                DateTime.TryParse(utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.createddatetime, "UTC", "yyyy-MM-ddThh:mm:ss.fffZ"), out DateTime createdDateTime);
                vehiclePosition.CreatedDateTime = utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.createddatetime, "UTC", "yyyy-MM-ddThh:mm:ss.fffZ");// createdDateTime;

            }

            if (record.receiveddatetime != null)
            {
                // DateTime.TryParse(utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.receiveddatetime, "UTC", "yyyy-MM-ddThh:mm:ss.fffZ"), out DateTime receivedDateTime);
                vehiclePosition.ReceivedDateTime = utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.receiveddatetime, "UTC", "yyyy-MM-ddThh:mm:ss.fffZ");// receivedDateTime;
            }

            GnssPosition gnssPosition = new GnssPosition();
            if (record.altitude != null)
            {
                gnssPosition.Altitude = Convert.ToInt32(record.altitude);
            }

            if (record.heading != null)
            {
                gnssPosition.Heading = Convert.ToInt32(record.heading);
            }

            if (record.latitude != null)
            {
                gnssPosition.Latitude = Convert.ToDouble(record.latitude);
            }

            if (record.longitude != null)
            {
                gnssPosition.Longitude = Convert.ToDouble(record.longitude);
            }

            if (record.positiondatetime != null)
            {
                DateTime.TryParse(utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.positiondatetime, "UTC", "yyyy-MM-ddTHH:mm:ss"), out DateTime positionDateTime);
                gnssPosition.PositionDateTime = utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.positiondatetime, "UTC", "yyyy-MM-ddThh:mm:ss.fffZ"); //positionDateTime;
            }

            if (record.speed != null)
            {
                gnssPosition.Speed = Convert.ToDouble(record.speed);
            }
            vehiclePosition.GnssPosition = gnssPosition;

            if (record.tachographspeed != null)
            {
                vehiclePosition.TachographSpeed = Convert.ToDouble(record.tachographspeed);
            }

            if (record.wheelbasespeed != null)
            {
                vehiclePosition.WheelBasedSpeed = Convert.ToDouble(record.wheelbasespeed);
            }

            return vehiclePosition;
        }

        internal VehicleStatus MapVehicleStatus(dynamic record)
        {
            VehicleStatus vehicleStatus = new VehicleStatus();
            //vehicleStatus.RecordId = record.id;
            vehicleStatus.Vin = record.vin;

            TriggerType triggerType = new TriggerType();
            triggerType.Context = record.context;
            triggerType.Type = Convert.ToString(record.triggertype);

            List<string> listTriggerInfo = new List<string>();
            listTriggerInfo.Add(record.triggerinfo);

            triggerType.TriggerInfo = listTriggerInfo;

            DriverId driverId = new DriverId();

            TachoDriverIdentification tachoDriverIdentification = new TachoDriverIdentification();
            tachoDriverIdentification.DriverIdentification = record.tachodriveridentification;
            tachoDriverIdentification.DriverAuthenticationEquipment = Convert.ToString(record.driverauthenticationequipment);
            tachoDriverIdentification.CardReplacementIndex = record.cardreplacementindex;
            tachoDriverIdentification.CardRenewalIndex = record.cardrenewalindex;
            tachoDriverIdentification.CardIssuingMemberState = record.cardissuingmemberstate;

            OemDriverIdentification oemDriverIdentification = new OemDriverIdentification();
            oemDriverIdentification.DriverIdentification = record.oemdriveridentification;
            oemDriverIdentification.IdType = record.oemidtype;

            driverId.TachoDriverIdentification = tachoDriverIdentification;
            driverId.OemDriverIdentification = oemDriverIdentification;

            triggerType.DriverId = driverId;
            triggerType.PtoId = record.ptoid;

            TellTaleInfo tellTaleInfo = new TellTaleInfo();
            tellTaleInfo.OemTellTale = record.oemtelltale;
            tellTaleInfo.State = Convert.ToString(record.state);
            tellTaleInfo.TellTale = Convert.ToString(record.telltale);

            triggerType.TellTaleInfo = tellTaleInfo;

            vehicleStatus.TriggerType = triggerType;

            if (record.createddatetime != null)
            {
                DateTime.TryParse(utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.createddatetime, "UTC", "yyyy-MM-ddTHH:mm:ss"), out DateTime createdDateTime);
                vehicleStatus.CreatedDateTime = createdDateTime;
            }

            if (record.receiveddatetime != null)
            {
                DateTime.TryParse(utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.receiveddatetime, "UTC", "yyyy-MM-ddTHH:mm:ss"), out DateTime receivedDateTime);
                vehicleStatus.ReceivedDateTime = receivedDateTime;
            }


            if (record.HrTotalVehicleDistance != null)
            {
                vehicleStatus.HrTotalVehicleDistance = Convert.ToString(record.HrTotalVehicleDistance);
            }

            if (record.totalEngineHours != null)
            {
                vehicleStatus.TotalEngineHours = Convert.ToString(record.totalEngineHours);
            }
            if (record.totalFuelUsedGaseous != null)
            {
                vehicleStatus.TotalFuelUsedGaseous = Convert.ToString(record.totalFuelUsedGaseous);
            }
            if (record.grossCombinationVehicleWeight != null)
            {
                vehicleStatus.GrossCombinationVehicleWeight = Convert.ToString(record.GrossCombinationVehicleWeight);
            }
            if (record.status2OfDoors != null)
            {
                vehicleStatus.Status2OfDoors = Convert.ToString(record.status2OfDoors);
            }
            if (record.doorStatus != null)
            {
                vehicleStatus.DoorStatus = Convert.ToString(record.doorStatus);
            }
            vehicleStatus.AccumulatedData = MapAccumuatedData();
            vehicleStatus.SnapshotData = MapSnapShotData();
            vehicleStatus.UptimeData = MapUptimeData();
            return vehicleStatus;
        }

        internal Vehicle Map(dynamic record)
        {
            string targetdateformat = "MM/DD/YYYY";
            Vehicle vehicle = new Vehicle();
            string possibleFuelTypes = record.possible_fuel_type;
            vehicle.Vin = record.vin;
            vehicle.CustomerVehicleName = record.customer_vehicle_name;
            vehicle.Brand = record.brand;
            if (record.production_date != null)
            {
                DateTime dtProdDate = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.production_date, "UTC", targetdateformat));
                ProductionDate pd = new ProductionDate();
                pd.Day = dtProdDate.Day;
                pd.Month = dtProdDate.Month;
                pd.Year = dtProdDate.Year;
                vehicle.ProductionDate = pd;
            }
            vehicle.Type = record.type;
            vehicle.Model = record.model;
            vehicle.ChassisType = record.chassistype;
            if (!string.IsNullOrEmpty(possibleFuelTypes))
                vehicle.PossibleFuelType = possibleFuelTypes.Split(",").ToList();
            vehicle.EmissionLevel = record.emissionlevel;
            if (!string.IsNullOrEmpty(Convert.ToString(record.noofaxles)))
                vehicle.NoOfAxles = Convert.ToInt32(record.noofaxles);
            else
                vehicle.NoOfAxles = 0;

            if (!string.IsNullOrEmpty(Convert.ToString(record.totalfueltankvolume)))
                vehicle.TotalFuelTankVolume = Convert.ToInt32(record.totalfueltankvolume);
           // else
              //  vehicle.TotalFuelTankVolume = 0;

            vehicle.GearboxType = record.gearboxtype;
            return vehicle;
        }
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
                    PositionDateTime = DateTime.UtcNow.ToString()//2021-08-23T08=07=40.446Z
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
