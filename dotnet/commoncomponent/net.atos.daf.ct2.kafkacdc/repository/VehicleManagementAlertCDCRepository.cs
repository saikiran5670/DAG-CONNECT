﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public class VehicleManagementAlertCDCRepository : IVehicleManagementAlertCDCRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;

        public VehicleManagementAlertCDCRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartdataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartdataAccess = dataMartdataAccess;
        }
        public async Task<List<VehicleAlertRef>> GetVehicleAlertRefFromvehicleId(int vehicleId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vin", vehicleId); //TODO: Get VIN from Vehicle Id
                string queryAlertLevelPull = @"select vin,alert_id as AlertId, state  from tripdetail.vehiclealertref where vin = @vin;";

                IEnumerable<VehicleAlertRef> vehicleAlertRefs = await _dataMartdataAccess.QueryAsync<VehicleAlertRef>(queryAlertLevelPull, parameter);
                return vehicleAlertRefs.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<VehicleAlertRef>> GetVehicleAlertByvehicleId(int vehicleId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vehicleid", vehicleId);
                string query = @"with cte_alert_vehicle_groupanddynamic
                            AS (
                            select distinct 
                            cte.id as AlertId,
                            cte.category as Category,
                            cte.type as AlertType,
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
                            on cte.vehicle_group_id = grp.id and grp.object_type='V' 
                            left join master.groupref vgrpref
                            on  grp.id=vgrpref.group_id
                            left join master.vehicle veh
                            on vgrpref.ref_id=veh.id
where veh.id=@vehicleid
                            )
                            --select * from cte_alert_vehicle_groupanddynamic;

                            ,cte_account_visibility_for_vehicle_group
                            AS (
                            select distinct 
                            AlertId
                            ,Category
                            ,AlertType
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
                            where VehicleId = @vehicleid and group_type='G' 
                            )
                            --select * from cte_account_visibility_for_vehicle_group
                            ,cte_account_visibility_for_vehicle_single
                            AS (
                            select distinct 
                            cte.AlertId,
                            cte.Category,
                            cte.AlertType,
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
                            on veh.id = @vehicleid and grp.ref_id=veh.id and grp.group_type='S'
                            where veh.id = @vehicleid and grp.organization_id=cte.organization_id 
                            )
                            --select * from cte_account_visibility_for_vehicle_single
                            ,cte_account_visibility_for_vehicle_dynamic_unique
                            AS (
	                            select distinct 
	                            AlertId
	                            ,Category
	                            ,AlertType
	                            ,VehicleGroupId
	                            ,object_type
	                            ,group_type
	                            ,function_enum
	                            ,Organization_Id
	                            ,VehicleGroupName
	                            From cte_alert_vehicle_groupanddynamic 
where VehicleId = @vehicleid 
	                            group by AlertId
	                            ,Category
	                            ,AlertType
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
	                            ,du1.Category
	                            ,du1.AlertType
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
	                            on orm.vehicle_id=veh.id and veh.id = @vehicleid 
	                            Inner join master.orgrelationship ors
	                            on ors.id=orm.relationship_id
	                            Inner join cte_account_visibility_for_vehicle_dynamic_unique du1
	                            on ((orm.owner_org_id = du1.Organization_Id and ors.code='Owner') 
	                            or (orm.target_org_id= du1.Organization_Id and ors.code<>'Owner'))
	                            and du1.function_enum='A'
	                            --Left join cte_account_visibility_for_vehicle_dynamic_unique du2
	                            --on orm.target_org_id=du2.Organization_Id and ors.code<>'Owner' and du2.function_enum='A'
	                            where veh.id = @vehicleid and ors.state='A' 
	                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
	                            else COALESCE(end_date,0) =0 end  
                            )
                            --select * from cte_account_vehicle_DynamicAll
                            , 
                            cte_account_vehicle_DynamicOwned
                            AS (
	                            select distinct 
	                            du1.AlertId
	                            ,du1.Category
	                            ,du1.AlertType
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
	                            on orm.vehicle_id=veh.id and veh.id = @vehicleid
	                            Inner join master.orgrelationship ors
	                            on ors.id=orm.relationship_id
	                            Inner join cte_account_visibility_for_vehicle_dynamic_unique du1
	                            on ((orm.owner_org_id=du1.Organization_Id and ors.code='Owner') or (veh.organization_id=du1.Organization_Id)) and du1.function_enum='O'
	                            where veh.id = @vehicleid and ors.state='A' 
	                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
	                            else COALESCE(end_date,0) =0 end  
                            )
                            --select * from cte_account_vehicle_DynamicOwned
                            ,
                            cte_account_vehicle_DynamicVisible
                            AS (
	                            select distinct 
	                            du2.AlertId
	                            ,du2.Category
	                            ,du2.AlertType
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
	                            on orm.vehicle_id=veh.id and  veh.id = @vehicleid
	                            Inner join master.orgrelationship ors
	                            on ors.id=orm.relationship_id
	                            Inner join cte_account_visibility_for_vehicle_dynamic_unique du2
	                            on orm.target_org_id=du2.Organization_Id and du2.function_enum='V'
	                            where veh.id = @vehicleid and ors.state='A'
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
	                            ,du1.Category
	                            ,du1.AlertType
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
where veh.id = @vehicleid
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
                            --select * from cte_account_vehicle_CompleteList

                            ,org_veh_subscriptions
                            as (
                            select distinct fea.id as featureid
                                   ,fea.name
                                   ,fea.key
                                   ,fea.level
                                   ,fea.type as featuretype
                                   ,pac.id as packageid
                                   ,pac.packagecode
                                   ,sub.id as subscriptionid
                                   ,sub.organization_id as organizationid
                                   ,sub.type as subscriptiontype
                                   ,sub.subscription_end_date as subscriptionenddate
                                   ,sub.subscription_start_date as subscriptionstartdate
                                   ,sub.is_zuora_package
                                   ,sub.vehicle_id as vehicleid 
	                               ,feasetfea.feature_set_id as featuresetid 
                            from master.subscription sub
                            inner join cte_account_vehicle_CompleteList cavc
                            on sub.organization_id = cavc.Organization_Id 
                            inner join master.package pac
                            on sub.package_id=pac.id and sub.state='A' and pac.state='A'
                            inner join master.featuresetfeature feasetfea
                            on pac.feature_set_id=feasetfea.feature_set_id
                            inner join master.feature fea
                            on feasetfea.feature_id=fea.id and fea.state='A' and fea.type='F'
                            inner join translation.enumtranslation enutra
                            on fea.id=enutra.feature_id and enutra.type='T'
                            and cavc.AlertType= enutra.enum and cavc.Category= enutra.parent_enum and enutra.type='T'
                            where sub.organization_id in(cavc.Organization_Id)
                            and 
                            fea.id= enutra.feature_id
                            and case when COALESCE(subscription_end_date,0) !=0 then to_timestamp(COALESCE(subscription_end_date)/1000)::date>=now()::date
                                else COALESCE(subscription_end_date,0) =0 end
                            order by 1
                            )
                            --select * from org_veh_subscriptions
                            ,
                             org_subscriptions
                             as (
	                             select * from cte_account_vehicle_CompleteList 
	                             where EXISTS (select AlertId from org_veh_subscriptions where subscriptiontype='O')   
                             )
                             -- select * from org_subscriptions
                             ,
                             veh_subscriptions
                             as (
	                             select cavc.* from cte_account_vehicle_CompleteList cavc
	                             inner join org_veh_subscriptions  ovs
	                             on cavc.vehicleid = @vehicleid and cavc.vehicleid = ovs.vehicleid and ovs.subscriptiontype='V'
                             )
                            --select * from veh_subscriptions
                            , 
                            subscription_complete
                            as
                            (
	                            select * from org_subscriptions
	                            union
	                            select * from veh_subscriptions
                            )
                            select distinct alertid,vin from subscription_complete;";

                IEnumerable<VehicleAlertRef> vehicleAlertRefs = await _dataAccess.QueryAsync<VehicleAlertRef>(query, parameter);
                return vehicleAlertRefs.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
