using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.kafkacdc.entity;
using Dapper;
using System;

namespace net.atos.daf.ct2.kafkacdc.repository
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
                string query = @"with cte_alert_vehicle_groupanddynamic
                    AS (
                    select distinct 
                    cte.id as AlertId,
                    grp.id as VehicleGroupId
                    ,grp.object_type
                    ,grp.group_type
                    ,grp.function_enum
                    ,grp.organization_id
                    ,grp.name as VehicleGroupName
                    ,veh.id as VehicleId
                    ,veh.name as VehicleName
                    ,veh.vin as Vin
                    ,veh.license_plate_number as RegistrationNo	
                    from master.alert cte
                    inner join master.group grp 
                    on cte.vehicle_group_id = grp.id and grp.object_type='V' and cte.id=12 --and grp.group_type='G'
                    left join master.groupref vgrpref
                    on  grp.id=vgrpref.group_id
                    left join master.vehicle veh
                    on vgrpref.ref_id=veh.id
                    )
                    --select * from cte_alert_vehicle_groupanddynamic;

                    ,cte_account_visibility_for_vehicle_group
                    AS (
                    select distinct 
                    AlertId
                    ,VehicleGroupId
                    ,object_type
                    ,group_type
                    ,function_enum
                    ,organization_id
                    ,VehicleGroupName
                    ,VehicleId
                    ,VehicleName
                    ,Vin
                    ,RegistrationNo	
                    from cte_alert_vehicle_groupanddynamic
                    where group_type='G'
                    )
                    --select * from cte_account_visibility_for_vehicle_group
                    ,cte_account_visibility_for_vehicle_single
                    AS (
                    select distinct 
                    cte.AlertId,
                    grp.id as VehicleGroupId
                    ,grp.object_type
                    ,grp.group_type
                    ,grp.function_enum
                    ,grp.organization_id
                    ,grp.name as VehicleGroupName
                    ,veh.id as VehicleId
                    ,veh.name as VehicleName
                    ,veh.vin as Vin
                    ,veh.license_plate_number as RegistrationNo
                    from cte_alert_vehicle_groupanddynamic cte
                    inner join master.group grp 
                    on cte.vehiclegroupid=grp.id --and grp.object_type='V' --and grp.group_type='S'
                    inner join master.vehicle veh
                    on grp.ref_id=veh.id and grp.group_type='S'
                    where grp.organization_id=cte.organization_id
                    )
                    --select * from cte_account_visibility_for_vehicle_single
                    ,cte_account_visibility_for_vehicle_dynamic_unique
                    AS (
	                    select distinct 
	                    AlertId
	                    ,VehicleGroupId
	                    ,object_type
	                    ,group_type
	                    ,function_enum
	                    ,Organization_Id
	                    ,VehicleGroupName
	                    From cte_alert_vehicle_groupanddynamic 
	                    group by AlertId
	                    ,VehicleGroupId
	                    ,object_type
	                    ,group_type
	                    ,function_enum
	                    ,Organization_Id
	                    ,VehicleGroupName
	                    having group_type='D'
                    )
                    --select * from cte_account_visibility_for_vehicle_dynamic_unique
                    ,
                    cte_account_vehicle_DynamicAll
                    AS (
	                    select distinct 
	                     du1.AlertId
	                    ,du1.VehicleGroupId
	                    ,du1.object_type
	                    ,du1.group_type
	                    ,du1.function_enum
	                    ,du1.Organization_Id
	                    ,du1.VehicleGroupName
	                    ,veh.id as VehicleId
	                    ,veh.name as VehicleName
	                    ,veh.vin as Vin
	                    ,veh.license_plate_number as RegistrationNo
	                    from master.vehicle veh
	                    Inner join master.orgrelationshipmapping  orm
	                    on orm.vehicle_id=veh.id
	                    Inner join master.orgrelationship ors
	                    on ors.id=orm.relationship_id
	                    Inner join cte_account_visibility_for_vehicle_dynamic_unique du1
	                    on ((orm.owner_org_id = du1.Organization_Id and ors.code='Owner') 
	                    or (orm.target_org_id= du1.Organization_Id and ors.code<>'Owner'))
	                    and du1.function_enum='A'
	                    --Left join cte_account_visibility_for_vehicle_dynamic_unique du2
	                    --on orm.target_org_id=du2.Organization_Id and ors.code<>'Owner' and du2.function_enum='A'
	                    where ors.state='A'
	                    and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
	                    else COALESCE(end_date,0) =0 end  
                    )
                    --select * from cte_account_vehicle_DynamicAll
                    , 
                    cte_account_vehicle_DynamicOwned
                    AS (
	                    select distinct 
	                    du1.AlertId
	                    ,du1.VehicleGroupId
	                    ,du1.object_type
	                    ,du1.group_type
	                    ,du1.function_enum
	                    ,du1.Organization_Id
	                    ,du1.VehicleGroupName
	                    ,veh.id as VehicleId
	                    ,veh.name as VehicleName
	                    ,veh.vin as Vin
	                    ,veh.license_plate_number as RegistrationNo
	                    from master.vehicle veh
	                    Inner join master.orgrelationshipmapping  orm
	                    on orm.vehicle_id=veh.id
	                    Inner join master.orgrelationship ors
	                    on ors.id=orm.relationship_id
	                    Inner join cte_account_visibility_for_vehicle_dynamic_unique du1
	                    on ((orm.owner_org_id=du1.Organization_Id and ors.code='Owner') or (veh.organization_id=du1.Organization_Id)) and du1.function_enum='O'
	                    where ors.state='A'
	                    and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
	                    else COALESCE(end_date,0) =0 end  
                    )
                    --select * from cte_account_vehicle_DynamicOwned
                    ,
                    cte_account_vehicle_DynamicVisible
                    AS (
	                    select distinct 
	                    du2.AlertId
	                    ,du2.VehicleGroupId
	                    ,du2.object_type
	                    ,du2.group_type
	                    ,du2.function_enum
	                    ,du2.Organization_Id
	                    ,du2.VehicleGroupName
	                    ,veh.id as VehicleId
	                    ,veh.name as VehicleName
	                    ,veh.vin as Vin
	                    ,veh.license_plate_number as RegistrationNo
	                    from master.vehicle veh
	                    Inner join master.orgrelationshipmapping  orm
	                    on orm.vehicle_id=veh.id
	                    Inner join master.orgrelationship ors
	                    on ors.id=orm.relationship_id
	                    Inner join cte_account_visibility_for_vehicle_dynamic_unique du2
	                    on orm.target_org_id=du2.Organization_Id and du2.function_enum='V'
	                    where ors.state='A'
	                    and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
	                    else COALESCE(end_date,0) =0 end  
	                    and ors.code<>'Owner'
                    )
                    --select * from cte_account_vehicle_DynamicVisible
                    ,
                    cte_account_vehicle_DynamicOEM
                    AS (
	                    select distinct 
	                     du1.AlertId
	                    ,du1.VehicleGroupId
	                    ,du1.object_type
	                    ,du1.group_type
	                    ,du1.function_enum
	                    ,du1.Organization_Id
	                    ,du1.VehicleGroupName
	                    ,veh.id as VehicleId
	                    ,veh.name as VehicleName
	                    ,veh.vin as Vin
	                    ,veh.license_plate_number as RegistrationNo
	                    from master.vehicle veh
	                    Inner join cte_account_visibility_for_vehicle_dynamic_unique du1
	                    on veh.organization_id=du1.organization_id and du1.function_enum='M'
                    )
                    --select * from cte_account_vehicle_DynamicOEM
                    ,
                    cte_account_vehicle_CompleteList
                    AS (
	                    select distinct * from cte_account_visibility_for_vehicle_single
	                    union
	                    select distinct * from cte_account_visibility_for_vehicle_group
	                    union
	                    select distinct * from cte_account_vehicle_DynamicAll
	                    union
	                    select distinct * from cte_account_vehicle_DynamicOwned
	                    union
	                    select distinct * from cte_account_vehicle_DynamicVisible
	                    union
	                    select distinct * from cte_account_vehicle_DynamicOEM
                    )
                    select  distinct 
		                    AlertId
		                    ,VIN
                    from cte_account_vehicle_CompleteList where  vehicleid>0 order by 1;";

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
                                                    VALUES (@vin, @alertid, @state, @createdat) RETURNING id;";
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
            catch (Exception ex)
            {
                transactionScope.Rollback();
                throw ex;
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
            catch (Exception ex)
            {
                transactionScope.Rollback();
                throw ex;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
            return isSucceed;
        }
        public async Task<bool> DeleteVehicleAlertRef(List<int> alertIds)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alertids", alertIds);
                string queryAlertLevelPull = @"DELETE FROM tripdetail.vehiclealertref
                                               WHERE alert_id=any(@alertids) ";
                int result = await _dataMartdataAccess.ExecuteAsync(queryAlertLevelPull, parameter);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
