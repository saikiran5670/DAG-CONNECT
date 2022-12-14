using System;
using System.Collections.Generic;
using System.Linq;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.rfms.response
{
    public class RfmsVehicleMapper
    {
        private readonly RfmsVehicleStatusAccumulator _rfmsVehicleStatusAccumulator;
        public RfmsVehicleMapper()
        {
            _rfmsVehicleStatusAccumulator = new RfmsVehicleStatusAccumulator();
        }
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

            TachoDriverIdentification tachoDriverIdentification = GetDriverCardDetails(record.tachodriveridentification, record.driverauthenticationequipment);

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
            vehicleStatus.RecordId = null;// record.id;
            vehicleStatus.Vin = record.vin;

            TriggerType triggerType = new TriggerType();
            triggerType.Context = record.context;
            triggerType.Type = Convert.ToString(record.triggertype);

            List<string> listTriggerInfo = new List<string>();
            listTriggerInfo.Add(record.triggerinfo);

            triggerType.TriggerInfo = listTriggerInfo;

            DriverId driverId = new DriverId();

            TachoDriverIdentification tachoDriverIdentification = GetDriverCardDetails(record.tachodriveridentification, record.driverauthenticationequipment);
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


            if (record.totalvehicledistance != null)
            {
                vehicleStatus.HrTotalVehicleDistance = record.totalvehicledistance;
            }

            if (record.totalenginehours != null)
            {
                vehicleStatus.TotalEngineHours = record.totalenginehours;
            }
            if (record.enginetotalfuelused != null)
            {
                vehicleStatus.EngineTotalFuelUsed = record.enginetotalfuelused;
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
            if (record.production_date != null && record.production_date != 0)
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

            if (record.accelerationpedalposclassmaxrange != null && record.accelerationpedalposclassminrange != null && record.accelerationpedalposclassdistrstep != null)
            {
                accumulatedData.AccelerationPedalPositionClass = _rfmsVehicleStatusAccumulator.AccumulateAccelerationPedalPositionClass(record);
            }

            if (record.retardertorqueclassmaxrange != null && record.retardertorqueclassminrange != null && record.retardertorqueclassdistrstep != null)
            {
                accumulatedData.RetarderTorqueClass = _rfmsVehicleStatusAccumulator.AccumulateRetarderTorqueClass(record);
            }


            if (record.enginetorqueengineloadclassmaxrange != null && record.enginetorqueengineloadclassminrange != null && record.enginetorqueengineloadclassdistrstep != null)
            {
                accumulatedData.EngineTorqueAtCurrentSpeedClass = _rfmsVehicleStatusAccumulator.AccumulateEngineTorqueAtCurrentSpeedClass(record);
            }


            return accumulatedData;
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
            var driver2Id = new Driver2Id();
            driver2Id.TachoDriverIdentification = GetDriverCardDetails(record.tachodriver2identification, record.driver2authenticationequipment);
            driver2Id.OemDriverIdentification = new OemDriverIdentification()
            {
                IdType = record.driver2oemidtype,
                DriverIdentification = record.oemdriver2identification
            };

            snapshotData.Driver2WorkingState = record.driver2workingstate;
            snapshotData.AmbientAirTemperature = record.ambientairtemperature;
            snapshotData.ParkingBrakeSwitch = null;
            snapshotData.HybridBatteryPackRemainingCharge = null;

            return snapshotData;

        }

        public UptimeData MapUptimeData(dynamic record)
        {
            var tellTaleStates = new List<TellTaleInfo>();
            if (record.uptimetelltale_state != null && record.uptimetelltale_state.Length > 0)
            {
                for (var i = 1; i < record.uptimetelltale_state.Length; i++)

                {
                    tellTaleStates.Add(new TellTaleInfo() { TellTale = i.ToString(), State = record.uptimetelltale_state[i].ToString() });
                }
            }
            var uptimeData = new UptimeData()
            {

                TellTaleInfo = tellTaleStates,
                //new List<TellTaleInfo>(){ new TellTaleInfo()
                //{
                //    TellTale =  Convert.ToString(record.telltale),
                //    OemTellTale =record.oemtelltale,
                //    State = Convert.ToString(record.state)
                //}
                //},
                ServiceDistance = record.serviceDitance,//100000,
                EngineCoolantTemperature = record.enginecoolanttemperature,
                ServiceBrakeAirPressureCircuit1 = record.servicebrakeairpressurecircuit1,
                ServiceBrakeAirPressureCircuit2 = record.servicebrakeairpressurecircuit2,
                AlternatorInfo = new AlternatorInfo()
                {
                    AlternatorNumber = 1
                },

            };
            return uptimeData;


        }


        public TachoDriverIdentification GetDriverCardDetails(string driverIdentification, int? authenticationEquipment)
        {
            if (driverIdentification != null && (driverIdentification.Length == 19 || driverIdentification == "*"))
            {
                return new TachoDriverIdentification()
                {
                    DriverIdentification = driverIdentification,
                    CardIssuingMemberState = driverIdentification == "*" ? "*" : driverIdentification.Substring(0, 3).Trim(),//first three with trim
                    DriverAuthenticationEquipment = authenticationEquipment?.ToString(),
                    CardReplacementIndex = driverIdentification == "*" ? "*" : driverIdentification.Substring((driverIdentification.Length - 4), 2),//16,17th index
                    CardRenewalIndex = driverIdentification == "*" ? "*" : driverIdentification.Substring(driverIdentification.Length - 2) // last two index
                };
            }

            else
                return new TachoDriverIdentification();
        }
    }
}
