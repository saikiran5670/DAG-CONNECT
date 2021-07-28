using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public class VehicleCdcRepository : IVehicleCdcRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;

        public VehicleCdcRepository(IDataAccess dataAccess
                                , IDataMartDataAccess dataMartdataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartdataAccess = dataMartdataAccess;
        }

        public async Task<List<VehicleCdc>> GetVehicleCdc(List<int> vehicleId)
        {
            try
            {
                List<VehicleCdc> vehicleCdcs = new List<VehicleCdc>();
                var parameter = new DynamicParameters();
                parameter.Add("@vehicleId", vehicleId);


                var query = @"SELECT id,
                            vin, status, vid, fuel_type as FuelType
                            FROM master.vehicle where id =ANY(@vehicleId)";

                var data = await _dataAccess.QueryAsync<VehicleCdc>(query, parameter);
                return vehicleCdcs = data.Cast<VehicleCdc>().ToList();

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
