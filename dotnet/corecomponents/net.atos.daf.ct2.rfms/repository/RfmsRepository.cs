using System;
using System.Collections.Generic;
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


        public RfmsRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartAccess)
        {
            _dataMartDataAccess = dataMartAccess;
            _dataAccess = dataAccess;
        }
        public async Task<RfmsVehicles> GetVehicles(string lastVin, bool moreData)
        {
            //This whole query needs to be corrected once db design is ready
            try
            {
                var queryStatement = @"select v.id,v.vin
                                   ,v.name as customer_vehicle_name
                                   ,v.tcu_brand as brand 
                                   ,v.type 
                                   ,vp.manufacture_date as production_date 
                                   ,v.fuel_type as possible_fuel_type 
                                   from master.vehicle v 
                                    inner join master.vehicleproperties vp	 on
                                    vp.id = v.vehicle_property_id
                                    where 1=1";
                var parameter = new DynamicParameters();

                if (moreData && lastVin != null) // LastVin is mendatory when rfmsVehicleRequest is true and it is required for pagination
                {
                    parameter.Add("@vin", "%" + lastVin + "%");
                    queryStatement = queryStatement + " and v.vin LIKE @vin";

                }
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
            Vehicle vehicle = new Vehicle();
            vehicle.Vin = record.vin;
            vehicle.CustomerVehicleName = record.customer_vehicle_name;
            return vehicle;
        }

    }
}