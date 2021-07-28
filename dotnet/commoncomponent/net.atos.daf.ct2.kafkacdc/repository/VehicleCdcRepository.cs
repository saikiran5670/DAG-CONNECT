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

        public async Task<List<VehicleCdc>> GetVehicleCdc(List<int> vids)
        {
            try
            {
                List<VehicleCdc> vehicleCdcs = new List<VehicleCdc>();
                var parameter = new DynamicParameters();
                parameter.Add("@vids", vids);


                var query = @"SELECT id,
                            vin, status, vid, fuel_type
                            FROM master.vehicle where vid = @any(vids)";

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
