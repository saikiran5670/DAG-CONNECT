using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.response;

namespace net.atos.daf.ct2.rfms.repository
{
    public class RfmsRepository : IRfmsRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartDataAccess;
        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly RfmsVehicleMapper _rfmsVehicleStatusMapper;
        public RfmsRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartAccess)
        {
            _dataMartDataAccess = dataMartAccess;
            _dataAccess = dataAccess;
            _rfmsVehicleStatusMapper = new RfmsVehicleMapper();
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
                                        ,(SELECT case when (((SELECT COUNT(CHASIS_FUEL_TANK_VOLUME) FROM MASTER.VEHICLEFUELTANKPROPERTIES
                                         where  VEHICLE_ID =  V.ID  and (CHASIS_FUEL_TANK_VOLUME !~ '^[0-9.]+$')) = 0)
                                         and (select exists(select CHASIS_FUEL_TANK_VOLUME from master.vehiclefueltankproperties where VEHICLE_ID=V.ID)))
                                         then (SELECT  COALESCE(SUM(CAST(CHASIS_FUEL_TANK_VOLUME AS double precision)),0) 
                                         FROM MASTER.VEHICLEFUELTANKPROPERTIES VFTP WHERE VEHICLE_ID =  V.ID ) end as TOTALFUELTANKVOLUME)
                                        -- ,(SELECT COALESCE(SUM(CAST(CHASIS_FUEL_TANK_VOLUME AS double precision)),0) FROM MASTER.VEHICLEFUELTANKPROPERTIES VFTP WHERE VEHICLE_ID = V.ID and CHASIS_FUEL_TANK_VOLUME ~ '^[0-9.]+$') AS TOTALFUELTANKVOLUME
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
                    vehicles.Add(_rfmsVehicleStatusMapper.Map(record));
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
                if (rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.LatestOnly)
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
                    if (rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.LatestOnly)
                        queryStatement = queryStatement + " WHERE T1.vin = ANY(@visibleVins)";
                    else
                        queryStatement = queryStatement + " WHERE vin = ANY(@visibleVins)";
                }

                //If Not Latest Only Check
                if (!rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.LatestOnly)
                {
                    //Parameter add for starttime
                    if (rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StartTime != null)
                    {
                        if (lastVinId > 0)
                        {
                            var second = 1;
                            parameter.Add("@start_time", utilities.UTCHandling.RfmsGetUTCFromDateTime(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StartTime, second));
                        }
                        else
                        {
                            parameter.Add("@start_time", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StartTime));

                        }
                        if (rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.Type == DateType.Created.ToString())
                        {
                            queryStatement = queryStatement + " and created_datetime >= @start_time";
                        }
                        else
                        {
                            queryStatement = queryStatement + " and received_datetime >= @start_time";
                        }
                    }

                    //Parameter add for starttime
                    if (rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StopTime != null)
                    {
                        parameter.Add("@stop_time", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StopTime));

                        if (rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.Type == DateType.Created.ToString())
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
                if (Int32.TryParse(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.TriggerFilter, out int triggerFilter))
                {
                    parameter.Add("@triggerFilter", triggerFilter);
                    queryStatement += " AND vehicle_msg_trigger_type_id = @triggerFilter";
                }

                if (lastVinId > 0)
                {
                    parameter.Add("@lastVinReceivedDateTime", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StartTime));
                    if (!rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.LatestOnly)
                        queryStatement = queryStatement + " AND received_datetime > (SELECT distinct received_datetime FROM LIVEFLEET.LIVEFLEET_POSITION_STATISTICS VV WHERE VV.received_datetime = @lastVinReceivedDateTime)";
                    //require to confirm from Anirudha
                }
                if (rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.LatestOnly)
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
                    lstVehiclePosition.Add(_rfmsVehicleStatusMapper.MapVehiclePositions(record));
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

        public async Task<RfmsVehicleStatus> GetRfmsVehicleStatus(RfmsVehicleStatusRequest rfmsVehicleStatusRequest, string visibleVins, int lastVinId)
        {

            try
            {
                //var parameter = new DynamicParameters();
                //parameter.Add("@requestId", rfmsVehicleStatusRequest.RequestId);
                // To do rfms vehicle status....
                string queryStatement = string.Empty;
                string contentFilterQuery = GetContentFilterQuery(rfmsVehicleStatusRequest.ContentFilter);

                if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LatestOnly)
                {
                    queryStatement = @"SELECT t1.Id, 
                                        t1.vin,
                                        t1.vehicle_msg_trigger_type_id as triggertype,
                                        'RFMS' as context,
                                        t1.vehicle_msg_trigger_additional_info as triggerinfo,
                                        T1.driver1_id as tachodriveridentification,
                                        t1.driver_auth_equipment_type_id as driverauthenticationequipment,
                                        t1.card_replacement_index as cardreplacementindex,
                                        t1.oem_driver_id as oemdriveridentification,
                                        t1.oem_driver_id_type as oemidtype,
                                        t1.pto_id as ptoid,
                                        t1.oem_telltale as oemtelltale,
                                        t1.telltale_state_id as state,
                                        t1.telltale_id as telltale,
                                        t1.created_datetime as createddatetime,
                                        t1.received_datetime as receiveddatetime,
                                        t1.total_vehicle_distance as totalvehicledistance,
                                        t1.total_engine_hours as totalenginehours,

                                        t1.driver1_id as tachodriver1identification,
                                        t1.driver_auth_equipment_type_id as driverauthenticationequipment,
                                        t1.card_replacement_index as cardreplacementindex,                                     
                                        t1.oem_driver_id as oemdriveridentification,
                                        t1.oem_driver_id_type as oemidtype,

                                        t1.gross_combination_vehicle_weight as grogrossCombinationVehicleWeight,
                                        t1.total_engine_fuel_used as engineTotalFuelUsed ";
                    var selectQuery = @" from livefleet.livefleet_position_statistics T1
                                        inner  join tripdetail.trip_statistics t2
                                        on t1.trip_id = t2.trip_id
                                        INNER JOIN (SELECT MAX(Created_DateTime) as lastDate, vin
										from livefleet.livefleet_position_statistics Group By Vin) T3
										on T1.created_datetime = T3.lastDate
										and T1.Vin = T3.Vin";

                    queryStatement += contentFilterQuery + selectQuery;


                }
                else
                {
                    queryStatement = @"SELECT t1.Id, 
                                        t1.vin,
                                        t1.vehicle_msg_trigger_type_id as triggertype,
                                        'RFMS' as context,
                                        t1.vehicle_msg_trigger_additional_info as triggerinfo,
                                        T1.driver1_id as tachodriveridentification,
                                        t1.driver_auth_equipment_type_id as driverauthenticationequipment,
                                        t1.card_replacement_index as cardreplacementindex,
                                        t1.oem_driver_id as oemdriveridentification,
                                        t1.oem_driver_id_type as oemidtype,
                                        t1.pto_id as ptoid,
                                        t1.oem_telltale as oemtelltale,
                                        t1.telltale_state_id as state,
                                        t1.telltale_id as telltale,
                                        t1.created_datetime as createddatetime,
                                        t1.received_datetime as receiveddatetime,
                                        t1.total_vehicle_distance as totalvehicledistance,
                                        t1.total_engine_hours as totalenginehours,

                                        t1.driver1_id as tachodriver1identification,
                                        t1.driver_auth_equipment_type_id as driverauthenticationequipment,
                                        t1.card_replacement_index as cardreplacementindex,                                     
                                        t1.oem_driver_id as oemdriveridentification,
                                        t1.oem_driver_id_type as oemidtype,

                                        t1.gross_combination_vehicle_weight as grogrossCombinationVehicleWeight,
                                        t1.total_engine_fuel_used as engineTotalFuelUsed ";
                    var selectQuery = @"from livefleet.livefleet_position_statistics t1 inner
                                        join tripdetail.trip_statistics t2
                                        on t1.trip_id = t2.trip_id";
                    queryStatement += contentFilterQuery + selectQuery;
                }



                var parameter = new DynamicParameters();

                if (!string.IsNullOrEmpty(visibleVins))
                {
                    List<string> lstVisibleVins = visibleVins.Split(',').ToList();
                    parameter.Add("@visibleVins", lstVisibleVins);
                    if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LatestOnly)
                        queryStatement = queryStatement + " WHERE t1.vin = ANY(@visibleVins)";
                    else
                        queryStatement = queryStatement + " WHERE t1.vin = ANY(@visibleVins)";
                }

                //If Not Latest Only Check
                if (!rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LatestOnly)
                {
                    //Parameter add for starttime
                    if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StartTime != null)
                    {
                        if (lastVinId > 0)
                        {
                            var second = 1;
                            parameter.Add("@start_time", utilities.UTCHandling.RfmsGetUTCFromDateTime(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StartTime, second));
                        }
                        else
                        {
                            parameter.Add("@start_time", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StartTime));

                        }
                        if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.Type == DateType.Created.ToString())
                        {
                            queryStatement = queryStatement + " and created_datetime >= @start_time";
                        }
                        else
                        {
                            queryStatement = queryStatement + " and received_datetime >= @start_time";
                        }
                    }

                    //Parameter add for starttime
                    if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StopTime != null)
                    {
                        parameter.Add("@stop_time", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StopTime));

                        if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.Type == DateType.Created.ToString())
                        {
                            queryStatement = queryStatement + " and created_datetime < @stop_time";
                        }
                        else
                        {
                            queryStatement = queryStatement + " and received_datetime < @stop_time";
                        }
                    }
                }
                // Parameter add for content filter


                //Parameter add for TriggerFilter
                if (Int32.TryParse(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.TriggerFilter, out int triggerFilter))
                {
                    parameter.Add("@triggerFilter", triggerFilter);
                    queryStatement += " AND vehicle_msg_trigger_type_id = @triggerFilter";
                }

                if (lastVinId > 0)
                {
                    parameter.Add("@lastVinReceivedDateTime", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StartTime));
                    if (!rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LatestOnly)
                        queryStatement = queryStatement + " AND received_datetime > (SELECT distinct received_datetime FROM LIVEFLEET.LIVEFLEET_POSITION_STATISTICS VV WHERE VV.received_datetime = @lastVinReceivedDateTime)";
                }
                if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LatestOnly)
                {
                    queryStatement += " ORDER BY T1.received_datetime";
                }
                else
                {
                    queryStatement += " ORDER BY received_datetime";
                }

                var rfmsVehicleStatus = new RfmsVehicleStatus();

                dynamic result = await _dataMartDataAccess.QueryAsync<dynamic>(queryStatement, parameter);


                VehicleStatusResponse vehicleStatusResponse = new VehicleStatusResponse();

                List<VehicleStatus> lstVehicleStatus = new List<VehicleStatus>();
                foreach (dynamic record in result)
                {
                    lstVehicleStatus.Add(_rfmsVehicleStatusMapper.MapVehicleStatus(record, rfmsVehicleStatusRequest.ContentFilter));
                }


                vehicleStatusResponse.VehicleStatuses = lstVehicleStatus;
                rfmsVehicleStatus.VehicleStatusResponse = vehicleStatusResponse;
                return rfmsVehicleStatus;
                //return new RfmsVehicleStatus();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private string GetContentFilterQuery(string contentFilter)
        {
            var query = string.Empty;
            if (string.IsNullOrEmpty(contentFilter) || contentFilter.Contains(ContentType.ACCUMULATED.ToString()[0].ToString()))
            {
                query += @" ,t2.duration_wheelbase_speed_over_zero as durationwheelspeedoverzero,
                            t2.distance_cruise_control_active as distancecruisecontrolactive,
                            t2.duration_cruise_control_active as durationcruisecontrolactive,
                            t2.fuel_consumption_during_cruise_active as fuelconsumptionduringcruiseactive,
                            t2.duration_wheelbase_speed_zero as durationwheelbasespeedzero,
                            t2.fuel_during_wheelbase_speed_zero as fuelduringwheelbasespeedzero,
                            t2.fuel_wheelbase_speed_over_zero as fuelwheelbasespeedoverzero,
                            t2.brake_pedal_counter_speed_over_zero as brakepedalcounterspeedoverzero,
                            t2.distance_brake_pedal_active_speed_over_zero as distancebrakepedalactivespeedoverzero,
--classes
                            t2.pto_active_class_pto_duration as ptoactiveclassptoduration,
                            t2.pto_active_class_pto_fuel_consumed as ptoactiveclassptofuelconsumed,
                            t2.acceleration_pedal_pos_class_distr as accelerationpedalposclassdistr,
                            t2.acceleration_pedal_pos_class_min_range as accelerationpedalposclassminrange,
                            t2.acceleration_pedal_pos_class_max_range as accelerationpedalposclassmaxrange,
                            t2.acceleration_pedal_pos_class_distr_step as accelerationpedalposclassdistrstep, 
                            t2.acceleration_pedal_pos_class_distr_array_time as accelerationpedalposclassdistrarraytime,
                            t2.retarder_torque_class_distr as retardertorqueclassdistr, 
                            t2.retarder_torque_class_min_range as retardertorqueclassminrange,
                            t2.retarder_torque_class_max_range as retardertorqueclassmaxrange,
                            t2.retarder_torque_class_distr_step as retardertorqueclassdistrstep,
                            t2.retarder_torque_class_distr_array_time as retardertorqueclassdistrarray_time,
                            t2.engine_torque_engine_load_class_distr as enginetorqueengineloadclassdistr, 
                            t2.engine_torque_engine_load_class_min_range as enginetorqueengineloadclassminrange, 
                            t2.engine_torque_engine_load_class_max_range as enginetorqueengineloadclassmaxrange,
                            t2.engine_torque_engine_load_class_distr_step as enginetorqueengineloadclassdistrstep, 
                            t2.engine_torque_engine_load_class_distr_array_time  as enginetorqueengineloadclassdistrarraytime
                            ";


            }
            if (string.IsNullOrEmpty(contentFilter) || contentFilter.Contains(ContentType.SNAPSHOT.ToString()[0].ToString()))
            {
                query += @" ,t1.gps_altitude as altitude,
                            t1.gps_heading as heading,
                            t1.gps_latitude as latitude,
                            t1.gps_longitude as longitude,
                            t1.gps_datetime as positiondatetime,
                            t1.gps_speed as speed,
                            t1.tachgraph_speed as tachographspeed,
                            t1.wheelbased_speed as wheelbasespeed,
                            t1.fuel_level1 as fuellevel1,
                            t1.catalyst_fuel_level as catalystfuellevel,										
                            t1.driver1_working_state as driver1workingstate,
                            t1.driver2_id as tachodriver2identification,
                            t1.driver2_auth_equipment_type_id as driver2authenticationequipment,
                            t1.driver2_card_replacement_index as cardreplacementindex,                                     
                            t1.oem_driver2_id as oemdriver2identification,
                            t1.oem_driver2_id_type as driver2oemidtype,
                            t1.driver2_working_state as driver2workingstate,
                            t1.ambient_air_temperature as ambientairtemperature ";

            }
            if (string.IsNullOrEmpty(contentFilter) || contentFilter.Contains(ContentType.UPTIME.ToString()[0].ToString()))
            {
                query += @" ,t1.oem_telltale as oemtelltale,
                            t1.telltale_state_id as state,
                            t1.telltale_id as telltale,
                            t1.distance_until_next_service as serviceDistance,
                            t1.engine_coolant_temperature as enginecoolanttemperature,
                            t1.service_brake_air_pressure_circuit1 as servicebrakeairpressurecircuit1,
                            t1.service_brake_air_pressure_circuit2 as servicebrakeairpressurecircuit2 ";

            }
            return query;

        }
    }
}