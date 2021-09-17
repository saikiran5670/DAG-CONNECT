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
                        parameter.Add("@start_time", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StartTime));
                        if (lastVinId > 0)
                        {
                            var second = 1;
                            parameter.Add("@start_time", utilities.UTCHandling.RfmsGetUTCFromDateTime(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StartTime, second));
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
                        queryStatement = queryStatement + " AND received_datetime >= (SELECT distinct received_datetime FROM LIVEFLEET.LIVEFLEET_POSITION_STATISTICS VV WHERE VV.received_datetime = @lastVinReceivedDateTime)";
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
                if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LatestOnly)
                {
                    queryStatement = @"SELECT Id, 
                                    vin as vin,
                                    vehicle_msg_trigger_type_id as triggertype,
                                    'RFMS' as context,
                                    vehicle_msg_trigger_additional_info as triggerinfo,
                                    driver1_id as tachodriveridentification,
                                    created_datetime as createddatetime,
                                    received_datetime as receiveddatetime,                                 
                                    'hr_total_vehicle_distance' as hrtotalvehicledistance,
                                    'total_engine_hours' as totalenginehours,
                                    'engine_total_fuel_used' as engineTotalFuelUsed,
                                    'gross_combination_vehicle_weight' as grosscombinationvehicleweight,
                                    driver1_id as driver1Id,
                                    'accumulateddata' as accumulateddata,
                                    'snapshotdata' as snapshotdata,
                                    'uptimedata' as uptimedata,
                                    'status2ofdoors' as status2ofdoors, 
                                    'doorstatus' as doorstatus
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
                                    vin as vin,
                                    vehicle_msg_trigger_type_id as triggertype,
                                    'RFMS' as context,
                                    vehicle_msg_trigger_additional_info as triggerinfo,
                                    driver1_id as tachodriveridentification,
                                    created_datetime as createddatetime,
                                    received_datetime as receiveddatetime,                                 
                                    'hr_total_vehicle_distance' as hrtotalvehicledistance,
                                    'total_engine_hours' as totalenginehours,
                                    'engine_total_fuel_used' as engineTotalFuelUsed,
                                    'gross_combination_vehicle_weight' as grosscombinationvehicleweight,
                                    driver1_id as driver1Id,
                                    'accumulateddata' as accumulateddata,
                                    'snapshotdata' as snapshotdata,
                                    'uptimedata' as uptimedata,
                                    'status2ofdoors' as status2ofdoors, 
                                    'doorstatus' as doorstatus
                                    
									    from livefleet.livefleet_position_statistics";
                }



                var parameter = new DynamicParameters();

                if (!string.IsNullOrEmpty(visibleVins))
                {
                    List<string> lstVisibleVins = visibleVins.Split(',').ToList();
                    parameter.Add("@visibleVins", lstVisibleVins);
                    if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LatestOnly)
                        queryStatement = queryStatement + " WHERE T1.vin = ANY(@visibleVins)";
                    else
                        queryStatement = queryStatement + " WHERE vin = ANY(@visibleVins)";
                }

                //If Not Latest Only Check
                if (!rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.LatestOnly)
                {
                    //Parameter add for starttime
                    if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StartTime != null)
                    {
                        parameter.Add("@start_time", utilities.UTCHandling.GetUTCFromDateTime(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StartTime));

                        if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.Type == DateType.Created.ToString())
                        {
                            queryStatement = queryStatement + " and created_datetime > @start_time";
                        }
                        else
                        {
                            queryStatement = queryStatement + " and received_datetime > @start_time";
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

                //if (rfmsVehicleStatusRequest.ContentFilter == ContentType.ACCUMULATED)
                //{

                //    queryStatement += "@select";
                //}

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
                        queryStatement = queryStatement + " AND received_datetime > (SELECT received_datetime FROM LIVEFLEET.LIVEFLEET_POSITION_STATISTICS VV WHERE VV.received_datetime = @lastVinReceivedDateTime)";
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
                    lstVehicleStatus.Add(_rfmsVehicleStatusMapper.MapVehicleStatus(record));
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
    }
}