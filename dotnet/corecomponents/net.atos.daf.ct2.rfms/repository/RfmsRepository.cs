using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.data;
using Dapper;
using net.atos.daf.ct2.rfms.responce;
using net.atos.daf.ct2.rfms.entity;
using System;

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
        public async Task<RfmsVehicles> Get(RfmsVehicleRequest rfmsVehicleRequest)
        {
            try
            {
                var queryStatement = @"select vin
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
                dynamic result = await _dataAccess.QueryAsync<dynamic>(queryStatement, parameter);
                return rfmsVehicles;

            }

            catch (Exception ex)
            {
                throw ex;
            }


        }

    }
}