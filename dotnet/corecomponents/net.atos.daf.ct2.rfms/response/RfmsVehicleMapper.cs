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

            TachoDriverIdentification tachoDriverIdentification = GetDriverCardDetails(record.tachodriveridentification, Convert.ToString(record.driverauthenticationequipment));
            //new TachoDriverIdentification();
            //tachoDriverIdentification.DriverIdentification = record.tachodriveridentification;
            //tachoDriverIdentification.DriverAuthenticationEquipment = Convert.ToString(record.driverauthenticationequipment);
            //tachoDriverIdentification.CardReplacementIndex = record.cardreplacementindex;
            //tachoDriverIdentification.CardRenewalIndex = record.cardrenewalindex;
            //tachoDriverIdentification.CardIssuingMemberState = record.cardissuingmemberstate;

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

        internal VehicleStatus MapVehicleStatus(dynamic record, string contentFilter)
        {
            VehicleStatus vehicleStatus = new VehicleStatus();
            vehicleStatus.RecordId = record.id;
            vehicleStatus.Vin = record.vin;

            TriggerType triggerType = new TriggerType();
            triggerType.Context = record.context;
            triggerType.Type = Convert.ToString(record.triggertype);

            List<string> listTriggerInfo = new List<string>();
            listTriggerInfo.Add(record.triggerinfo);

            triggerType.TriggerInfo = listTriggerInfo;

            DriverId driverId = new DriverId();

            TachoDriverIdentification tachoDriverIdentification = GetDriverCardDetails(record.tachodriveridentification, Convert.ToString(record.driverauthenticationequipment));
            // new TachoDriverIdentification();
            //tachoDriverIdentification.DriverIdentification = record.tachodriveridentification;
            //tachoDriverIdentification.DriverAuthenticationEquipment = Convert.ToString(record.driverauthenticationequipment);
            //tachoDriverIdentification.CardReplacementIndex = record.cardreplacementindex;
            //tachoDriverIdentification.CardRenewalIndex = record.cardrenewalindex;
            //tachoDriverIdentification.CardIssuingMemberState = record.cardissuingmemberstate;

            OemDriverIdentification oemDriverIdentification = new OemDriverIdentification();
            oemDriverIdentification.DriverIdentification = record.oemdriveridentification;
            oemDriverIdentification.IdType = record.oemidtype;

            driverId.TachoDriverIdentification = tachoDriverIdentification;
            driverId.OemDriverIdentification = oemDriverIdentification;

            triggerType.DriverId = driverId;
            triggerType.PtoId = "1";// record.ptoid = 1; //This is which PTO is activacted, DAF only gives the info on CAN of all PTO together. So always 1.

            vehicleStatus.Driver1Id = new Driver1Id() { OemDriverIdentification = driverId.OemDriverIdentification, TachoDriverIdentification = driverId.TachoDriverIdentification };
            TellTaleInfo tellTaleInfo = new TellTaleInfo();
            tellTaleInfo.OemTellTale = record.oemtelltale;
            tellTaleInfo.State = Convert.ToString(record.state);
            tellTaleInfo.TellTale = Convert.ToString(record.telltale);

            triggerType.TellTaleInfo = tellTaleInfo;

            vehicleStatus.TriggerType = triggerType;

            if (record.createddatetime != null)
            {
                vehicleStatus.CreatedDateTime = UTCHandling.GetConvertedDateTimeFromUTC(record.createddatetime, "UTC", "yyyy-MM-ddThh:mm:ss.fffZ"); //createdDateTime;
            }

            if (record.receiveddatetime != null)
            {
                vehicleStatus.ReceivedDateTime = UTCHandling.GetConvertedDateTimeFromUTC(record.receiveddatetime, "UTC", "yyyy-MM-ddThh:mm:ss.fffZ"); //receivedDateTime;
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
            if (!string.IsNullOrEmpty(contentFilter))
            {
                vehicleStatus.AccumulatedData = contentFilter.Contains(ContentType.ACCUMULATED.ToString()[0].ToString()) ? MapAccumuatedData(record) : null;
                vehicleStatus.SnapshotData = contentFilter.Contains(ContentType.SNAPSHOT.ToString()[0].ToString()) ? MapSnapShotData(record) : null;
                vehicleStatus.UptimeData = contentFilter.Contains(ContentType.UPTIME.ToString()[0].ToString()) ? MapUptimeData(record) : null;
            }
            else
            {
                vehicleStatus.AccumulatedData = MapAccumuatedData(record);
                vehicleStatus.SnapshotData = MapSnapShotData(record);
                vehicleStatus.UptimeData = MapUptimeData(record);
            }
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
        public AccumulatedData MapAccumuatedData(dynamic record)
        {

            var accumulatedData = new AccumulatedData()
            {
                DurationWheelbaseSpeedOverZero = record.durationWheelbaseSpeedOverZero,
                DistanceCruiseControlActive = record.distancecruisecontrolactive,
                DurationCruiseControlActive = record.durationcruisecontrolactive,
                FuelConsumptionDuringCruiseActive = record.fuelconsumptionduringcruiseactive,
                DurationWheelbaseSpeedZero = record.durationwheelbasespeedzero,
                FuelWheelbaseSpeedZero = record.fuelduringwheelbasespeedzero,
                BrakePedalCounterSpeedOverZero = record.brakepedalcounterspeedoverzero,
                DistanceBrakePedalActiveSpeedOverZero = record.distancebrakepedalactivespeedoverzero,
                FuelWheelbaseSpeedOverZero = record.fuelwheelbasespeedoverzero
            };

            //PtoActiveClass = new List<PtoActiveClass>
            //{ new PtoActiveClass()
            //{
            //Label= MasterMemoryObjectCacheConstants.WHEELBASED_SPEED_OVER_ZERO,
            //Seconds= 12345,
            //Meters= 2345,
            //MilliLitres= 3456
            //}
            //},

            if (record.accelerationpedalposclassmaxrange != null && record.accelerationpedalposclassmaxrange != null && record.accelerationpedalposclassmaxrangek != null)
            {
                accumulatedData.AccelerationPedalPositionClass = AccumulateAccelerationPedalPositionClass(record);
            }
            //new List<AccelerationPedalPositionClass>() { new AccelerationPedalPositionClass() {  From = 0,
            //    To = 20,
            //    Seconds = 23456,
            //    Meters = 345678,
            //    MilliLitres = 678345} },
            //       BrakePedalPositionClass = new List<BrakePedalPositionClass>() { new BrakePedalPositionClass() {  From = 0,
            //           To = 20,
            //           Seconds = 2456,
            //           Meters = 34578,
            //           MilliLitres = 67345 } },
            //       AccelerationClass = new List<AccelerationClass>{ new AccelerationClass() { From = -1.1,
            //           To = -0.9,
            //           Seconds = 23456,
            //           Meters = 345678,
            //           MilliLitres = 678345
            //       } },

            //       HighAccelerationClass = new List<HighAccelerationClass>(){new HighAccelerationClass()
            //   {
            //           From = -3,
            //           To = -2.5,
            //           Seconds = 23456,
            //           Meters = 345678,
            //           MilliLitres = 678345
            //       }
            //       },
            //       RetarderTorqueClass = new List<RetarderTorqueClass>(){new RetarderTorqueClass()
            //   {
            //           From = 0,
            //           To = 20,
            //           Seconds = 23456,
            //           Meters = 345678,
            //           MilliLitres = 678345
            //       }
            //       },
            //       DrivingWithoutTorqueClass = new List<DrivingWithoutTorqueClass>(){new DrivingWithoutTorqueClass()
            //   {
            //           Label =MasterMemoryObjectCacheConstants.DRIVING_WITHOUT_TORQUE,
            //           Seconds = 12345,
            //           Meters = 2345,
            //           MilliLitres = 3456
            //       }
            //       },
            //       EngineTorqueClass = new List<EngineTorqueClass>() {new EngineTorqueClass()            {
            //           From = 0,
            //           To = 10,
            //           Seconds = 23456,
            //           Meters = 345678,
            //           MilliLitres = 678345
            //       }
            //},
            //       EngineTorqueAtCurrentSpeedClass = new List<EngineTorqueAtCurrentSpeedClass>() {new EngineTorqueAtCurrentSpeedClass()
            //   {
            //           From = 0,
            //           To = 10,
            //           Seconds = 23456,
            //           Meters = 345678,
            //           MilliLitres = 678345
            //       }
            //},
            //       VehicleSpeedClass = new List<VehicleSpeedClass>(){new VehicleSpeedClass()
            //   {
            //           From = 0,
            //           To = 4,
            //           Seconds = 23456,
            //           Meters = 345678,
            //           MilliLitres = 678345
            //       }
            //},
            //       EngineSpeedClass = new List<EngineSpeedClass>() {new EngineSpeedClass()
            //   {
            //           From = 0,
            //           To = 400,
            //           Seconds = 23456,
            //           Meters = 345678,
            //           MilliLitres = 678345
            //       }
            //},
            //       AccelerationDuringBrakeClass = new List<AccelerationDuringBrakeClass>() { new  AccelerationDuringBrakeClass()
            //   {
            //           From = -1.1,
            //           To = -0.9,
            //           Seconds = 23456,
            //           Meters = 345678,
            //           MilliLitres = 678345
            //       }
            //},
            //       SelectedGearClass = new List<SelectedGearClass>() {new SelectedGearClass()
            //   {
            //           Label = "0",
            //           Seconds = 12345,
            //           Meters = 2345,
            //           MilliLitres = 3456
            //       }
            //},
            //       CurrentGearClass = new List<CurrentGearClass>() { new CurrentGearClass()
            //   {
            //           Label = "0",
            //           Seconds = 12345,
            //           Meters = 2345,
            //           MilliLitres = 3456
            //       }
            //       },

            // };

            return accumulatedData;
        }
        private List<string> GetPedalInterval(int maxSize, int minSize, int step)
        {
            Dictionary<int, string> intervalRanges = new Dictionary<int, string>();
            List<string> intervals = new List<string>();
            int index = 0;
            int[] res = new int[10];
            for (int i = minSize; i <= maxSize;
            i += step)
            {
                index += 1;
                var a = i;//== 0 ? i : i += 1;
                var b = i + step > maxSize ? maxSize : i + step;
                if (a < maxSize)
                {
                    var range = a.ToString() + "-" + b.ToString();
                    intervalRanges.Add(index, range);
                    intervals.Add(range);
                };

            }
            return intervals;// intervalRanges;
        }
        private List<AccelerationPedalPositionClass> AccumulateAccelerationPedalPositionClass(dynamic record)
        {

            var accumulatedClassRequest = new AccumulatedClassRequest()
            {
                ClassDistanceData = record.accelerationpedalposclassdistr = new int[40, 10, 20, 2000, 30, 40, 10, 20, 2000, 30],
                MaxRange = record.accelerationpedalposclassmaxrange,
                MinRange = record.accelerationpedalposclassminrange,
                NoOfStep = record.accelerationpedalposclassdistrstep,

            };
            var accClass = new List<AccelerationPedalPositionClass>();
            var intervals = GetPedalInterval(accumulatedClassRequest.MaxRange, accumulatedClassRequest.MinRange, accumulatedClassRequest.NoOfStep);
            foreach (var item in intervals.Select((value, i) => new { i, value }))
            {
                accClass.Add(new AccelerationPedalPositionClass()
                {
                    From = Convert.ToInt32(item.value.Split('-')[0]),
                    To = Convert.ToInt32(item.value.Split('-')[1]),
                    Seconds = accumulatedClassRequest.ClassDistanceData[item.i]
                });
            }
            return accClass;
        }

        public SnapshotData MapSnapShotData(dynamic record)
        {

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
                gnssPosition.PositionDateTime = UTCHandling.GetConvertedDateTimeFromUTC(record.positiondatetime, "UTC", "yyyy-MM-ddThh:mm:ss.fffZ");
            }

            if (record.speed != null)
            {
                gnssPosition.Speed = Convert.ToDouble(record.speed);
            }

            var snapshotData = new SnapshotData();



            snapshotData.GnssPosition = gnssPosition;
            if (record.wheelBasedSpeed != null)
            {
                snapshotData.WheelBasedSpeed = Convert.ToDouble(record.wheelBasedSpeed);
            }
            if (record.tachographspeed != null)
            {
                snapshotData.TachographSpeed = Convert.ToDouble(record.tachographspeed);
            }
            snapshotData.EngineSpeed = record.enginespeed;
            snapshotData.FuelType = record.fuelType = "Diesel"; //Default value as Diesel as per DAf discussion
            snapshotData.FuelLevel1 = record.fuelLevel1;
            // FuelLevel2 = 45,
            snapshotData.CatalystFuelLevel = record.catalystfuellevel;
            snapshotData.Driver1WorkingState = record.driver1workingstate;
            snapshotData.Driver2Id = new Driver2Id()
            {

                TachoDriverIdentification = GetDriverCardDetails(record.tachodriver2identification, record.driver2authenticationequipment.toString()),
                OemDriverIdentification = new OemDriverIdentification()
                {
                    IdType = record.driver2oemidtype,
                    DriverIdentification = record.oemdriver2identification
                }
            };
            snapshotData.Driver2WorkingState = record.driver2workingstate;
            snapshotData.AmbientAirTemperature = record.ambientairtemperature;
            snapshotData.ParkingBrakeSwitch = null;
            snapshotData.HybridBatteryPackRemainingCharge = null;

            return snapshotData;

        }

        public UptimeData MapUptimeData(dynamic record)
        {

            var uptimeData = new UptimeData()
            {

                TellTaleInfo = new List<TellTaleInfo>(){ new TellTaleInfo()
                {
                    TellTale =  Convert.ToString(record.telltale),
                    OemTellTale =record.oemtelltale,
                    State = Convert.ToString(record.state)
                }
                },
                ServiceDistance = record.serviceDitance,//100000,
                EngineCoolantTemperature = record.enginecoolanttemperature,
                ServiceBrakeAirPressureCircuit1 = record.servicebrakeairpressurecircuit1,
                ServiceBrakeAirPressureCircuit2 = record.servicebrakeairpressurecircuit2,
                // DurationAtLeastOneDoorOpen = 0,
                AlternatorInfo = new AlternatorInfo()
                {
                    //AlternatorStatus = MasterMemoryObjectCacheConstants.CHARGING,
                    AlternatorNumber = 1
                },
                //BellowPressureFrontAxleLeft = 234000,
                //BellowPressureFrontAxleRight = 234000,
                //BellowPressureRearAxleLeft = 234000,
                //BellowPressureRearAxleRight = 234000
            };
            return uptimeData;


        }


        public TachoDriverIdentification GetDriverCardDetails(string driverIdentification, string authenticationEquipment)
        {
            //TachoDriverIdentification tacho = new TachoDriverIdentification();
            // record.tachodriver2identification;
            //string driverid = record.tachodriveridentification;
            if (driverIdentification != null && (driverIdentification.Length == 19 || driverIdentification == "*"))
            {
                return new TachoDriverIdentification()
                {
                    DriverIdentification = driverIdentification,
                    CardIssuingMemberState = driverIdentification == "*" ? "*" : driverIdentification.Substring(0, 3).Trim(),//first three with trim
                    DriverAuthenticationEquipment = authenticationEquipment,
                    CardReplacementIndex = driverIdentification == "*" ? "*" : driverIdentification.Substring((driverIdentification.Length - 4), 2),//16,17th index
                    CardRenewalIndex = driverIdentification == "*" ? "*" : driverIdentification.Substring(driverIdentification.Length - 2) // last two index
                };
            }

            else
                return new TachoDriverIdentification();
        }
    }
}
