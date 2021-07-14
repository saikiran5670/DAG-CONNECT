using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.vehiclealertaggregator.entity;
using Dapper;
using System;

namespace net.atos.daf.ct2.vehiclealertaggregator.repository
{
    public class VehicleAlertRepository : IVehicleAlertRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;

        public VehicleAlertRepository(IDataAccess dataAccess
                                , IDataMartDataAccess dataMartdataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartdataAccess = dataMartdataAccess;
        }
        public async Task<List<VehicleAlertRef>> GetVehicleAlertRefByAlertIds(List<int> alertIds)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alertids", alertIds);
                string queryAlertLevelPull = @"SELECT key as Name,
                                                         enum as Value
	                                              FROM translation.enumtranslation 
                                                  Where type=@type";

                IEnumerable<VehicleAlertRef> vehicleAlertRefs = await _dataMartdataAccess.QueryAsync<VehicleAlertRef>(queryAlertLevelPull, parameter);
                return vehicleAlertRefs.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<VehicleAlertRef>> GetVehiclesFromAlertConfiguration(List<int> alertIds)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alertids", alertIds);
                string queryAlertLevelPull = @"SELECT key as Name,
                                                         enum as Value
	                                              FROM translation.enumtranslation 
                                                  Where type=@type";

                IEnumerable<VehicleAlertRef> vehicleAlertRefs = await _dataAccess.QueryAsync<VehicleAlertRef>(queryAlertLevelPull, parameter);
                return vehicleAlertRefs.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> InsertVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alertids", vehicleAlertRefs);
                string queryAlertLevelPull = @"SELECT key as Name,
                                                         enum as Value
	                                              FROM translation.enumtranslation 
                                                  Where type=@type";
                int result = await _dataMartdataAccess.ExecuteAsync(queryAlertLevelPull, parameter);
                return result > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> UpdateVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alertids", vehicleAlertRefs);
                string queryAlertLevelPull = @"SELECT key as Name,
                                                         enum as Value
	                                              FROM translation.enumtranslation 
                                                  Where type=@type";
                int result = await _dataMartdataAccess.ExecuteAsync(queryAlertLevelPull, parameter);
                return result > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeleteVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alertids", vehicleAlertRefs);
                string queryAlertLevelPull = @"SELECT key as Name,
                                                         enum as Value
	                                              FROM translation.enumtranslation 
                                                  Where type=@type";
                int result = await _dataMartdataAccess.ExecuteAsync(queryAlertLevelPull, parameter);
                return result > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
