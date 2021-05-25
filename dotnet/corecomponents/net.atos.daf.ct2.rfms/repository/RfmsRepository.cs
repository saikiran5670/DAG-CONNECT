using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.data;
using Dapper;
using net.atos.daf.ct2.rfms.responce;
using net.atos.daf.ct2.rfms.entity;
using System;
//using net.atos.daf.ct2.rfms.response;

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
        public async Task<RfmsVehicles> GetVehicles(RfmsVehicleRequest rfmsVehicleRequest)
        {
            try
            {
                var queryStatement = @"select id,vin
                                   ,customer_vehicle_name
                                   ,brand 
                                   ,type 
                                   ,model 
                                   ,production_date 
                                   ,possible_fuel_type 
                                   ,emission_level
                                   ,tell_tale_code 
                                   ,authorized_paths
                                   from master.vehicle 
                                   where 1=1";
                var parameter = new DynamicParameters();


                if (rfmsVehicleRequest.Id != null)
                {
                    parameter.Add("@id", "%" + rfmsVehicleRequest.Id + "%");
                    queryStatement = queryStatement + " and id LIKE @id";

                }
                if (rfmsVehicleRequest.MoreDataAvailable && rfmsVehicleRequest.LastVin != null) // LastVin is mendatory when rfmsVehicleRequest is true and it is required for pagination
                {
                    parameter.Add("@vin", "%" + rfmsVehicleRequest.LastVin + "%");
                    queryStatement = queryStatement + " and vin LIKE @id";

                }
                var rfmsVehicles = new RfmsVehicles();
                dynamic result = await _dataMartDataAccess.QueryAsync<dynamic>(queryStatement, parameter);
                return rfmsVehicles;

            }

            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<RfmsVehiclePositionRequest> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest)
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
                var rfmsVehiclePosition = new RfmsVehiclePositionRequest();

                dynamic result = await _dataAccess.QueryAsync<dynamic>(queryStatement, parameter);

                return rfmsVehiclePosition;

            }

            catch (Exception ex)
            {
                throw ex;
            }


        }

    }
}