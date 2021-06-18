using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.vehicle.repository
{
    public partial class VehicleRepository : IVehicleRepository
    {
        public async Task<VehicleConnectedResult> VehicleConnectAll(List<VehicleConnect> vehicleConnects)
        {
            var connectedVehicles = new VehicleConnectedResult();
            try
            {
                foreach (var vehicle in vehicleConnects)
                {
                    bool result = await SetOptInStatus(vehicle.Opt_In, vehicle.ModifiedBy, vehicle.VehicleId);
                    if (result)
                    {
                        connectedVehicles.VehicleConnectedList.Add(vehicle);
                    }
                    else
                    {
                        connectedVehicles.VehicleConnectionfailedList.Add(vehicle);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            return connectedVehicles;
        }


        public async Task<IEnumerable<Vehicle>> GetVehicleSetting(VehicleSettings vehicleSettings)
        {

            var QueryStatement = @"select id
                                   ,name 
                                   ,vin  
                                   ,termination_date 
                                   ,vid 
                                   ,type 
                                   ,tcu_id 
                                   ,tcu_serial_number 
                                   ,tcu_brand 
                                   ,tcu_version 
                                   ,is_tcu_register                                   
                                   ,created_at 
                                   ,model_id
                                   ,opt_in
                                   ,is_ota
                                   ,oem_id
                                   ,oem_organisation_id
                                   from master.vehicle 
                                   where 1=1";
            var parameter = new DynamicParameters();

            // Vehicle Id
            if (vehicleSettings.Id > 0)
            {
                parameter.Add("@id", vehicleSettings.Id);
                QueryStatement = QueryStatement + " and id=@id";

            }

            // VIN Id
            if (vehicleSettings.VIN != null && Convert.ToInt32(vehicleSettings.VIN.Length) > 0)
            {
                parameter.Add("@vin", "%" + vehicleSettings.VIN + "%");
                QueryStatement = QueryStatement + " and vin LIKE @vin";

            }


            List<Vehicle> vehicles = new List<Vehicle>();
            dynamic result = await _dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            foreach (dynamic record in result)
            {
                vehicles.Add(Map(record));
            }
            return vehicles.AsEnumerable();
        }
    }
}
