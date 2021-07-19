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
                string queryAlertLevelPull = @"select vin,alert_id as AlertId, state  from tripdetail.vehiclealertref where alert_id = any(@alertids);";

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
                string query = @"SELECT key as Name,
                                                         enum as Value
	                                              FROM translation.enumtranslation 
                                                  Where type=@type";

                IEnumerable<VehicleAlertRef> vehicleAlertRefs = await _dataAccess.QueryAsync<VehicleAlertRef>(query, parameter);
                return vehicleAlertRefs.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> InsertVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            bool isSucceed = false;
            try
            {
                foreach (VehicleAlertRef item in vehicleAlertRefs)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@vin", item.VIN);
                    parameter.Add("@alertid", item.AlertId);
                    parameter.Add("@state", "I");
                    parameter.Add("@createdat", System.DateTime.UtcNow.Millisecond);
                    string query = @"INSERT INTO tripdetail.vehiclealertref(vin, alert_id, state, created_at)
                                                    VALUES (@vin, @alertid, @state, @createdat) select id;";
                    int result = await _dataMartdataAccess.ExecuteAsync(query, parameter);
                    if (result <= 0)
                    {
                        isSucceed = false;
                        break;
                    }
                    isSucceed = true;
                }
                transactionScope.Commit();
            }
            catch (Exception)
            {
                transactionScope.Rollback();
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
            return isSucceed;
        }
        public async Task<bool> UpdateVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs)
        {
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            bool isSucceed = false;
            try
            {
                foreach (VehicleAlertRef item in vehicleAlertRefs)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@vin", vehicleAlertRefs);
                    parameter.Add("@alertid", vehicleAlertRefs);
                    parameter.Add("@state", "U");
                    parameter.Add("@createdat", System.DateTime.UtcNow.Millisecond);
                    string queryAlertLevelPull = @"UPDATE tripdetail.vehiclealertref
                                               SET state=@state, created_at=@createdat
                                                WHERE vin=any(@vins) and alert_id=any(@alertids) ";
                    int result = await _dataMartdataAccess.ExecuteAsync(queryAlertLevelPull, parameter);
                    if (result <= 0)
                    {
                        isSucceed = false;
                        break;
                    }
                    isSucceed = true;
                }
                transactionScope.Rollback();
            }
            catch (Exception)
            {
                transactionScope.Rollback();
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
            return isSucceed;
        }
        public async Task<bool> DeleteVehicleAlertRef(List<VehicleAlertRef> vehicleAlertRefs)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alertids", vehicleAlertRefs);
                string queryAlertLevelPull = @"DELETE FROM tripdetail.vehiclealertref
                                               WHERE vin=any(@vins) and alert_id=any(@alertids) ";
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
