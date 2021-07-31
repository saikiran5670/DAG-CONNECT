using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.response;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.rfms.repository
{
    public class RfmsRepository : IRfmsRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public RfmsRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartAccess)
        {
            _dataMartDataAccess = dataMartAccess;
            _dataAccess = dataAccess;
        }
        public async Task<RfmsVehicles> GetVehicles(string visibleVins, int lastVinId)
        {
            try
            {
                var queryStatement = @" SELECT DISTINCT 
                                        V.ID
                                        ,V.VIN
                                        ,V.NAME AS CUSTOMER_VEHICLE_NAME
                                        ,'DAF' AS BRAND 
                                        ,VP.MANUFACTURE_DATE AS PRODUCTION_DATE 
                                        ,'TRUCK' AS TYPE
                                        ,VP.SERIES_VEHICLE_RANGE AS MODEL
                                        ,V.FUEL_TYPE AS POSSIBLE_FUEL_TYPE
                                        ,VP.ENGINE_EMISSION_LEVEL AS EMISSIONLEVEL
										,VP.TYPE_ID AS CHASSISTYPE
                                        ,(SELECT COUNT(*) FROM MASTER.VEHICLEAXLEPROPERTIES WHERE VEHICLE_ID = V.ID) AS NOOFAXLES
                                        ,(SELECT COALESCE(SUM(CAST(CHASIS_FUEL_TANK_VOLUME AS double precision)),0) FROM MASTER.VEHICLEFUELTANKPROPERTIES VFTP WHERE VEHICLE_ID = V.ID) AS TOTALFUELTANKVOLUME
                                        ,VP.TRANSMISSION_GEARBOX_TYPE AS GEARBOXTYPE
                                        FROM MASTER.VEHICLE V 
                                        LEFT OUTER JOIN MASTER.VEHICLEPROPERTIES VP ON VP.ID = V.VEHICLE_PROPERTY_ID";
                var parameter = new DynamicParameters();
                if ((!string.IsNullOrEmpty(visibleVins)))
                {
                    List<string> lstVisibleVins = visibleVins.Split(',').ToList();
                    parameter.Add("@visibleVins", lstVisibleVins);
                    queryStatement = queryStatement + " WHERE V.Vin = ANY(@visibleVins)";
                }
                if (lastVinId > 0)
                {
                    parameter.Add("@lastVinId", lastVinId);
                    queryStatement = queryStatement + " AND V.ID > (SELECT VV.ID FROM MASTER.VEHICLE VV WHERE VV.ID = @lastVinId)";
                }

                queryStatement += " ORDER BY V.ID";

                RfmsVehicles rfmsVehicles = new RfmsVehicles();
                List<Vehicle> vehicles = new List<Vehicle>();
                dynamic result = await _dataAccess.QueryAsync<dynamic>(queryStatement, parameter);
                foreach (dynamic record in result)
                {
                    vehicles.Add(Map(record));
                }
                rfmsVehicles.Vehicles = vehicles;
                return rfmsVehicles;

            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest, string visibleVins, int lastVinId)
        {
            try
            {
                string queryStatement = string.Empty;
                if (rfmsVehiclePositionRequest.LatestOnly)
                {
                    queryStatement = @"SELECT Id, 
                                        T1.vin,
                                        vehicle_msg_trigger_type_id as triggertype,
                                        'RFMS' as context,
                                        vehicle_msg_trigger_additional_info as triggerinfo,
                                        driver1_id as tachodriveridentification,
                                        driver_auth_equipment_type_id as driverauthenticationequipment,
                                        card_replacement_index as cardreplacementindex,
                                        --'0' as cardrenewalindex,
                                        --'S' as cardissuingmemberstate,
                                        oem_driver_id as oemdriveridentification,
                                        oem_driver_id_type as oemidtype,
                                        pto_id as ptoid,
                                        oem_telltale as oemtelltale,
                                        telltale_state_id as state,
                                        telltale_id as telltale,
                                        created_datetime as createddatetime,
                                        received_datetime as receiveddatetime,
                                        gps_altitude as altitude,
                                        gps_heading as heading,
                                        gps_latitude as latitude,
                                        gps_longitude as longitude,
                                        gps_datetime as positiondatetime,
                                        gps_speed as speed,
                                        tachgraph_speed as tachographspeed,
                                        wheelbased_speed as wheelbasespeed
									    from livefleet.livefleet_position_statistics T1
										INNER JOIN 
										(SELECT MAX(Created_DateTime) as lastDate, vin
										from livefleet.livefleet_position_statistics 
										Group By Vin) T2
										on T1.created_datetime = T2.lastDate
										and T1.Vin = T2.Vin";
                }
                else
                {
                    queryStatement = @"SELECT Id, 
                                        vin,
                                        vehicle_msg_trigger_type_id as triggertype,
                                        'RFMS' as context,
                                        vehicle_msg_trigger_additional_info as triggerinfo,
                                        driver1_id as tachodriveridentification,
                                        driver_auth_equipment_type_id as driverauthenticationequipment,
                                        card_replacement_index as cardreplacementindex,
                                        --'0' as cardrenewalindex,
                                        --'S' as cardissuingmemberstate,
                                        oem_driver_id as oemdriveridentification,
                                        oem_driver_id_type as oemidtype,
                                        pto_id as ptoid,
                                        oem_telltale as oemtelltale,
                                        telltale_state_id as state,
                                        telltale_id as telltale,
                                        created_datetime as createddatetime,
                                        received_datetime as receiveddatetime,
                                        gps_altitude as altitude,
                                        gps_heading as heading,
                                        gps_latitude as latitude,
                                        gps_longitude as longitude,
                                        gps_datetime as positiondatetime,
                                        gps_speed as speed,
                                        tachgraph_speed as tachographspeed,
                                        wheelbased_speed as wheelbasespeed
									    from livefleet.livefleet_position_statistics";
                }
                var parameter = new DynamicParameters();

                if (!string.IsNullOrEmpty(visibleVins))
                {
                    List<string> lstVisibleVins = visibleVins.Split(',').ToList();
                    parameter.Add("@visibleVins", lstVisibleVins);
                    if (rfmsVehiclePositionRequest.LatestOnly)
                        queryStatement = queryStatement + " WHERE T1.vin = ANY(@visibleVins)";
                    else
                        queryStatement = queryStatement + " WHERE vin = ANY(@visibleVins)";
                }

                //If Not Latest Only Check
                if (!rfmsVehiclePositionRequest.LatestOnly)
                {
                    //Parameter add for starttime
                    if (rfmsVehiclePositionRequest.StartTime != null)
                    {
                        parameter.Add("@start_time", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehiclePositionRequest.StartTime));

                        if (rfmsVehiclePositionRequest.Type == DateType.Created)
                        {
                            queryStatement = queryStatement + " and created_datetime > @start_time";
                        }
                        else
                        {
                            queryStatement = queryStatement + " and received_datetime > @start_time";
                        }
                    }

                    //Parameter add for starttime
                    if (rfmsVehiclePositionRequest.StopTime != null)
                    {
                        parameter.Add("@stop_time", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehiclePositionRequest.StopTime));

                        if (rfmsVehiclePositionRequest.Type == DateType.Created)
                        {
                            queryStatement = queryStatement + " and created_datetime < @stop_time";
                        }
                        else
                        {
                            queryStatement = queryStatement + " and received_datetime < @stop_time";
                        }
                    }
                }
                //Parameter add for TriggerFilter
                if (Int32.TryParse(rfmsVehiclePositionRequest.TriggerFilter, out int triggerFilter))
                {
                    parameter.Add("@triggerFilter", triggerFilter);
                    queryStatement += " AND vehicle_msg_trigger_type_id = @triggerFilter";
                }

                if (lastVinId > 0)
                {
                    parameter.Add("@lastVinReceivedDateTime", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehiclePositionRequest.StartTime));
                    if (!rfmsVehiclePositionRequest.LatestOnly)
                        queryStatement = queryStatement + " AND received_datetime > (SELECT received_datetime FROM LIVEFLEET.LIVEFLEET_POSITION_STATISTICS VV WHERE VV.received_datetime = @lastVinReceivedDateTime)";
                }
                if (rfmsVehiclePositionRequest.LatestOnly)
                {
                    queryStatement += " ORDER BY T1.received_datetime";
                }
                else
                {
                    queryStatement += " ORDER BY received_datetime";
                }

                var rfmsVehiclePosition = new RfmsVehiclePosition();

                dynamic result = await _dataMartDataAccess.QueryAsync<dynamic>(queryStatement, parameter);

                VehiclePositionResponse vehiclePositionResponse = new VehiclePositionResponse();

                List<VehiclePosition> lstVehiclePosition = new List<VehiclePosition>();

                foreach (dynamic record in result)
                {
                    lstVehiclePosition.Add(MapVehiclePositions(record));
                }

                vehiclePositionResponse.VehiclePositions = lstVehiclePosition;
                rfmsVehiclePosition.VehiclePositionResponse = vehiclePositionResponse;
                return rfmsVehiclePosition;

            }

            catch (Exception ex)
            {
                throw;
            }


        }

        private VehiclePosition MapVehiclePositions(dynamic record)
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
                DateTime.TryParse(utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.createddatetime, "UTC", "yyyy-MM-ddTHH:mm:ss"), out DateTime createdDateTime);
                vehiclePosition.CreatedDateTime = createdDateTime;
            }

            if (record.receiveddatetime != null)
            {
                DateTime.TryParse(utilities.UTCHandling.GetConvertedDateTimeFromUTC(record.receiveddatetime, "UTC", "yyyy-MM-ddTHH:mm:ss"), out DateTime receivedDateTime);
                vehiclePosition.ReceivedDateTime = receivedDateTime;
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
                gnssPosition.PositionDateTime = positionDateTime;
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

        private Vehicle Map(dynamic record)
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
            else
                vehicle.TotalFuelTankVolume = 0;

            vehicle.GearboxType = record.gearboxtype;
            return vehicle;
        }

        public async Task<string> GetRFMSFeatureRate(string emailId, string featureName)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@email", emailId.ToLower());
            parameter.Add("@feature", featureName);

            try
            {
                var queryStatement = @"SELECT f.name FROM master.account acc
                                        INNER JOIN master.AccountRole ar ON acc.id = ar.account_id AND lower(acc.email) = @email AND acc.state = 'A'
                                        INNER JOIN master.Role r ON r.id = ar.role_id AND r.state = 'A'
                                        INNER JOIN master.FeatureSet fset ON r.feature_set_id = fset.id AND fset.state = 'A'
                                        INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
                                        INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.name like '" + featureName + "%' FETCH FIRST ROW ONLY";

                var featureRateName = await _dataAccess.ExecuteScalarAsync<string>(queryStatement, parameter);
                return featureRateName;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<MasterTableCacheObject>> GetMasterTableCacheData()
        {
            try
            {
                var queryStatement = @" WITH CTE_CACHE AS (SELECT ID, NAME, 'telltalestate' AS CTABLE FROM MASTER.TELLTALESTATE
                                         UNION
                                         SELECT ID, NAME, 'telltale' AS CTABLE FROM MASTER.TELLTALE
                                         UNION 
                                         SELECT ID, NAME, 'vehiclemsgtriggertype' AS CTABLE FROM MASTER.VEHICLEMSGTRIGGERTYPE
                                         UNION 
                                         SELECT ID, NAME, 'driverauthequipment' AS CTABLE FROM MASTER.DRIVERAUTHEQUIPMENT)
                                         SELECT * FROM CTE_CACHE 
                                         ORDER BY CTABLE";
                dynamic result = await _dataAccess.QueryAsync<dynamic>(queryStatement);

                List<MasterTableCacheObject> lstCacheObject = new List<MasterTableCacheObject>();

                foreach (dynamic record in result)
                {
                    lstCacheObject.Add(new MasterTableCacheObject()
                    {
                        Id = record.id,
                        Name = record.name,
                        TableName = record.ctable
                    });
                }

                return lstCacheObject;
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}