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
            //This whole query needs to be corrected once db design is ready
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

        public async Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest)
        {
            try
            {
                var queryStatement = @"select id, trigger_type
                                   ,request_server_date_time
                                   , received_date_time
                                   ,latitude
                                   ,longitude 
                                   ,heading 
                                   ,altitude 
                                   ,speed
                                   ,position_date_time 
                                   ,created_date_time
                                   ,vin
                                   ,wheel_based_speed
                                   ,tachograph_speed
                                   ,more_data_availabe
                                   from master.vehicle 
                                   where 1=1";
                var parameter = new DynamicParameters();

                //filter by date type****



                //filter start time
                if (rfmsVehiclePositionRequest.StartTime != null)
                {
                    parameter.Add("@start_time", "%" + rfmsVehiclePositionRequest.StartTime + "%");
                    queryStatement = queryStatement + " and start_time < @start_time";

                }

                //filter stop time  
                if (rfmsVehiclePositionRequest.StopTime != null)
                {
                    parameter.Add("@stop_time", "%" + rfmsVehiclePositionRequest.StopTime + "%");
                    queryStatement = queryStatement + " and stop_time > @stop_time";

                }
                //filter vin
                if (rfmsVehiclePositionRequest.Vin != null)
                {
                    parameter.Add("@vin", "%" + rfmsVehiclePositionRequest.Vin + "%");
                    queryStatement = queryStatement + " and vin LIKE @vin";

                }

                //filter latest only*****



                // filter trigger 
                if (rfmsVehiclePositionRequest.TriggerFilter != null)
                {
                    parameter.Add("@trigger_filter", "%" + rfmsVehiclePositionRequest.TriggerFilter + "%");
                    queryStatement = queryStatement + " and trigger_filter LIKE @trigger_filter";

                }
                var rfmsVehiclePosition = new RfmsVehiclePosition();

                dynamic result = await _dataAccess.QueryAsync<dynamic>(queryStatement, parameter);

                return rfmsVehiclePosition;

            }

            catch (Exception)
            {
                throw;
            }


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
                                        INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.name like 'api.rfms3#rate%' FETCH FIRST ROW ONLY";

                var featureRateName = await _dataAccess.ExecuteScalarAsync<string>(queryStatement, parameter);
                return featureRateName;
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}