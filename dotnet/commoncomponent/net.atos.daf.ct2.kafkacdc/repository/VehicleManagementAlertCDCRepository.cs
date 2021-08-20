using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
        public async Task<List<VehicleAlertRef>> GetVehicleAlertRefFromvehicleId(IEnumerable<int> alertIds)
        {
            try
            {
                //parameter.Add("@vin", await GetVINsByIds(vehicleIds));
                var parameter = new DynamicParameters();
                parameter.Add("@alertids", alertIds.ToArray());
                string queryAlertLevelPull = @"select vin,alert_id as AlertId, state  from tripdetail.vehiclealertref where alert_id = ANY(@alertIds);";

                IEnumerable<VehicleAlertRef> vehicleGroupAlertRefs = await _dataMartdataAccess.QueryAsync<VehicleAlertRef>(queryAlertLevelPull, parameter);
                return vehicleGroupAlertRefs.AsList();

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<VehicleAlertRef>> GetVehicleAlertByvehicleId(IEnumerable<int> vehicleIds, int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@context_org_id", organizationId);
                parameter.Add("@vehicleIds", vehicleIds.ToArray());
                string query = @"with Vehicle_Group_of_Type_Group
as(
select distinct 
	 grp.id as vehicle_group_id
	,grp.object_type
	,grp.group_type
	,grp.function_enum
	,grp.organization_id
	,grp.name as vehicle_group_name
	,grp.ref_id
from master.group grp
inner join master.groupref grpref
on grp.id=grpref.group_id and grp.object_type='V'
and grpref.ref_id = Any(@vehicleIds)
)
--select * from Vehicle_Group_of_Type_Group;
, Vehicle_Group_of_Type_Single
as(
select distinct 
	 grp.id as vehicle_group_id
	,grp.object_type
	,grp.group_type
	,grp.function_enum
	,grp.organization_id
	,grp.name as vehicle_group_name
	,grp.ref_id
from master.group grp
where  grp.ref_id = Any(@vehicleIds) and grp.object_type='V'
)
--select * from Vehicle_Group_of_Type_Single;
, Vehicle_Group_of_Type_Dynamic
as(
select distinct
	 grp.id as vehicle_group_id
	,grp.object_type
	,grp.group_type
	,grp.function_enum
	,grp.organization_id
	,grp.name as vehicle_group_name
	,grp.ref_id
from master.group grp
where grp.group_type='D' and grp.object_type='V' and  grp.organization_id = @context_org_id
)
--select * from Vehicle_Group_of_Type_Dynamic;
, Vehicle_Group_of_Type_All
as(
select * from Vehicle_Group_of_Type_Group
union
select * from Vehicle_Group_of_Type_Single
union
select * from Vehicle_Group_of_Type_Dynamic
)
--select * from Vehicle_Group_of_Type_All
, Alerts_Associated_To_Groups
	AS (
	select distinct 
	ale.id as AlertId
	,ale.category as Category
	,ale.type as AlertType
	,cte.vehicle_group_id
	,cte.object_type
	,cte.group_type
	,cte.function_enum
	,cte.organization_id
	,cte.vehicle_group_name	
	,cte.ref_id
	from master.alert ale
	inner join Vehicle_Group_of_Type_All cte 
	on ale.vehicle_group_id = cte.vehicle_group_id
)
--select * from Alerts_Associated_To_Groups;
,Alerts_Associated_To_Single_Vehicle
AS (
	select distinct 
	cte.AlertId
	,cte.Category
	,cte.AlertType
	,cte.vehicle_group_id
	,cte.object_type
	,cte.group_type
	,cte.function_enum
	,cte.organization_id
	,cte.vehicle_group_name
	,veh.id as VehicleId
	,veh.name as VehicleName
	,veh.vin as Vin
	,veh.license_plate_number as RegistrationNo
	from Alerts_Associated_To_Groups cte	
	inner join master.vehicle veh
	on cte.ref_id=veh.id and cte.group_type='S' and veh.opt_in='I'
	union
	select distinct 
	cte.AlertId
	,cte.Category
	,cte.AlertType
	,cte.vehicle_group_id
	,cte.object_type
	,cte.group_type
	,cte.function_enum
	,cte.organization_id
	,cte.vehicle_group_name
	,veh.id as VehicleId
	,veh.name as VehicleName
	,veh.vin as Vin
	,veh.license_plate_number as RegistrationNo
	from Alerts_Associated_To_Groups cte	
	inner join master.vehicle veh
	on cte.ref_id=veh.id and cte.group_type='S' and veh.opt_in='H'
	inner join master.organization org
	on org.id = veh.organization_id  and org.vehicle_default_opt_in = 'I'
)
--select * from Alerts_Associated_To_Single_Vehicle
,Alerts_Associated_To_Group_Vehicle
AS (
	select distinct 
	cte.AlertId
	,cte.Category
	,cte.AlertType
	,cte.vehicle_group_id
	,cte.object_type
	,cte.group_type
	,cte.function_enum
	,cte.organization_id
	,cte.vehicle_group_name
	,veh.id as VehicleId
	,veh.name as VehicleName
	,veh.vin as Vin
	,veh.license_plate_number as RegistrationNo
	from Alerts_Associated_To_Groups cte	
	inner join master.groupref grpref
	on cte.vehicle_group_id=grpref.group_id and cte.group_type='G'
	inner join master.vehicle veh
	on veh.id=grpref.ref_id and veh.opt_in='I'
	union
	select distinct 
	cte.AlertId
	,cte.Category
	,cte.AlertType
	,cte.vehicle_group_id
	,cte.object_type
	,cte.group_type
	,cte.function_enum
	,cte.organization_id
	,cte.vehicle_group_name
	,veh.id as VehicleId
	,veh.name as VehicleName
	,veh.vin as Vin
	,veh.license_plate_number as RegistrationNo
	from Alerts_Associated_To_Groups cte	
	inner join master.groupref grpref
	on cte.vehicle_group_id=grpref.group_id and cte.group_type='G'
	inner join master.vehicle veh
	on veh.id=grpref.ref_id and veh.opt_in='H'
	inner join master.organization org
	on org.id = veh.organization_id  and org.vehicle_default_opt_in = 'I'
)
--select * from Alerts_Associated_To_Group_Vehicle
,Alerts_Associated_To_Dynamic_Unique_Groups
AS (
		select distinct 
		AlertId
		,Category
		,AlertType
		,vehicle_group_id
		,object_type
		,group_type
		,function_enum
		,Organization_Id
		,vehicle_group_name
		From Alerts_Associated_To_Groups 
		group by AlertId
		,Category
		,AlertType
		,vehicle_group_id
		,object_type
		,group_type
		,function_enum
		,Organization_Id
		,vehicle_group_name
		having group_type='D'
	)
	-- select * from Alerts_Associated_To_Dynamic_Unique_Groups
	,
	Alerts_Associated_To_Dynamic_Vehicle
	AS (
		select distinct 
		 du1.AlertId
		,du1.Category
		,du1.AlertType
		,du1.vehicle_group_id
		,du1.object_type
		,du1.group_type
		,du1.function_enum
		,du1.Organization_Id
		,du1.vehicle_group_name
		,veh.id as VehicleId
		,veh.name as VehicleName
		,veh.vin as Vin
		,veh.license_plate_number as RegistrationNo
		from master.vehicle veh
		Inner join master.orgrelationshipmapping  orm
		on orm.vehicle_id=veh.id and veh.id = Any(@vehicleIds) and veh.opt_in='I'
		Inner join master.orgrelationship ors
		on ors.id=orm.relationship_id
		Inner join Alerts_Associated_To_Dynamic_Unique_Groups du1
		on ((orm.owner_org_id = du1.Organization_Id and ors.code='Owner') 
		or (orm.target_org_id= du1.Organization_Id and ors.code<>'Owner'))
		and du1.function_enum='A'
		--Left join cte_account_visibility_for_vehicle_dynamic_unique du2
		--on orm.target_org_id=du2.Organization_Id and ors.code<>'Owner' and du2.function_enum='A'
		where veh.id = Any(@vehicleIds) and ors.state='A' 
		and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
		else COALESCE(end_date,0) =0 end 
		union
		select distinct 
		 du1.AlertId
		,du1.Category
		,du1.AlertType
		,du1.vehicle_group_id
		,du1.object_type
		,du1.group_type
		,du1.function_enum
		,du1.Organization_Id
		,du1.vehicle_group_name
		,veh.id as VehicleId
		,veh.name as VehicleName
		,veh.vin as Vin
		,veh.license_plate_number as RegistrationNo
		from master.vehicle veh
		Inner join master.orgrelationshipmapping  orm
		on orm.vehicle_id=veh.id and veh.id = Any(@vehicleIds) and veh.opt_in='H'
		inner join master.organization org
		on org.id = veh.organization_id  and org.vehicle_default_opt_in = 'I'
		Inner join master.orgrelationship ors
		on ors.id=orm.relationship_id
		Inner join Alerts_Associated_To_Dynamic_Unique_Groups du1
		on ((orm.owner_org_id = du1.Organization_Id and ors.code='Owner') 
		or (orm.target_org_id= du1.Organization_Id and ors.code<>'Owner'))
		and du1.function_enum='A'
		--Left join cte_account_visibility_for_vehicle_dynamic_unique du2
		--on orm.target_org_id=du2.Organization_Id and ors.code<>'Owner' and du2.function_enum='A'
		where veh.id = Any(@vehicleIds) and ors.state='A' 
		and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
		else COALESCE(end_date,0) =0 end
	)
	--select * from Alerts_Associated_To_Dynamic_Vehicle
	, 
	Alerts_Associated_To_Dynamic_Owned_Vehicle
	AS (
		select distinct 
		du1.AlertId
		,du1.Category
		,du1.AlertType
		,du1.vehicle_group_id
		,du1.object_type
		,du1.group_type
		,du1.function_enum
		,du1.Organization_Id
		,du1.vehicle_group_name
		,veh.id as VehicleId
		,veh.name as VehicleName
		,veh.vin as Vin
		,veh.license_plate_number as RegistrationNo
		from master.vehicle veh
		Inner join master.orgrelationshipmapping  orm
		on orm.vehicle_id=veh.id and veh.id = Any(@vehicleIds) and veh.opt_in='I'
		Inner join master.orgrelationship ors
		on ors.id=orm.relationship_id
		Inner join Alerts_Associated_To_Dynamic_Unique_Groups du1
		on ((orm.owner_org_id=du1.Organization_Id and ors.code='Owner') or (veh.organization_id=du1.Organization_Id)) and du1.function_enum='O'
		where veh.id = Any(@vehicleIds) and ors.state='A' 
		and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
		else COALESCE(end_date,0) =0 end  
		union
		select distinct 
		du1.AlertId
		,du1.Category
		,du1.AlertType
		,du1.vehicle_group_id
		,du1.object_type
		,du1.group_type
		,du1.function_enum
		,du1.Organization_Id
		,du1.vehicle_group_name
		,veh.id as VehicleId
		,veh.name as VehicleName
		,veh.vin as Vin
		,veh.license_plate_number as RegistrationNo
		from master.vehicle veh
		Inner join master.orgrelationshipmapping  orm
		on orm.vehicle_id=veh.id and veh.id = Any(@vehicleIds) and veh.opt_in='H'
		inner join master.organization org
		on org.id = veh.organization_id  and org.vehicle_default_opt_in = 'I'
		Inner join master.orgrelationship ors
		on ors.id=orm.relationship_id
		Inner join Alerts_Associated_To_Dynamic_Unique_Groups du1
		on ((orm.owner_org_id=du1.Organization_Id and ors.code='Owner') or (veh.organization_id=du1.Organization_Id)) and du1.function_enum='O'
		where veh.id = Any(@vehicleIds) and ors.state='A' 
		and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
		else COALESCE(end_date,0) =0 end 
	)
	-- select * from Alerts_Associated_To_Dynamic_Owned_Vehicle
	,
	Alerts_Associated_To_Dynamic_visible_Vehicle
	AS (
		select distinct 
		du2.AlertId
		,du2.Category
		,du2.AlertType
		,du2.vehicle_group_id
		,du2.object_type
		,du2.group_type
		,du2.function_enum
		,du2.Organization_Id
		,du2.vehicle_group_name
		,veh.id as VehicleId
		,veh.name as VehicleName
		,veh.vin as Vin
		,veh.license_plate_number as RegistrationNo
		from master.vehicle veh
		Inner join master.orgrelationshipmapping  orm
		on orm.vehicle_id=veh.id and  veh.id = Any(@vehicleIds) and veh.opt_in='I'
		Inner join master.orgrelationship ors
		on ors.id=orm.relationship_id
		Inner join Alerts_Associated_To_Dynamic_Unique_Groups du2
		on orm.target_org_id=du2.Organization_Id and du2.function_enum='V'
		where veh.id = Any(@vehicleIds) and ors.state='A'
		and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
		else COALESCE(end_date,0) =0 end  
		and ors.code<>'Owner'
		union
		select distinct 
		du2.AlertId
		,du2.Category
		,du2.AlertType
		,du2.vehicle_group_id
		,du2.object_type
		,du2.group_type
		,du2.function_enum
		,du2.Organization_Id
		,du2.vehicle_group_name
		,veh.id as VehicleId
		,veh.name as VehicleName
		,veh.vin as Vin
		,veh.license_plate_number as RegistrationNo
		from master.vehicle veh
		Inner join master.orgrelationshipmapping  orm
		on orm.vehicle_id=veh.id and  veh.id = Any(@vehicleIds) and veh.opt_in='H'
		inner join master.organization org
		on org.id = veh.organization_id  and org.vehicle_default_opt_in = 'I'
		Inner join master.orgrelationship ors
		on ors.id=orm.relationship_id
		Inner join Alerts_Associated_To_Dynamic_Unique_Groups du2
		on orm.target_org_id=du2.Organization_Id and du2.function_enum='V'
		where veh.id = Any(@vehicleIds) and ors.state='A'
		and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
		else COALESCE(end_date,0) =0 end  
		and ors.code<>'Owner'
	)
	--select * from Alerts_Associated_To_Dynamic_visible_Vehicle
	,
	Alerts_Associated_To_Dynamic_OEM_Vehicle
	AS (
		select distinct 
		 du1.AlertId
		,du1.Category
		,du1.AlertType
		,du1.vehicle_group_id
		,du1.object_type
		,du1.group_type
		,du1.function_enum
		,du1.Organization_Id
		,du1.vehicle_group_name
		,veh.id as VehicleId
		,veh.name as VehicleName
		,veh.vin as Vin
		,veh.license_plate_number as RegistrationNo
		from master.vehicle veh
		Inner join Alerts_Associated_To_Dynamic_Unique_Groups du1
		on veh.organization_id=du1.organization_id and du1.function_enum='M' and veh.opt_in='I'
		union
		select distinct 
		 du1.AlertId
		,du1.Category
		,du1.AlertType
		,du1.vehicle_group_id
		,du1.object_type
		,du1.group_type
		,du1.function_enum
		,du1.Organization_Id
		,du1.vehicle_group_name
		,veh.id as VehicleId
		,veh.name as VehicleName
		,veh.vin as Vin
		,veh.license_plate_number as RegistrationNo
		from master.vehicle veh
		Inner join Alerts_Associated_To_Dynamic_Unique_Groups du1
		on veh.organization_id=du1.organization_id and du1.function_enum='M' and veh.opt_in='H'
		inner join master.organization org
		on org.id = veh.organization_id  and org.vehicle_default_opt_in = 'I'
	)
	--select * from Alerts_Associated_To_Dynamic_OEM_Vehicle
	,
	cte_account_vehicle_CompleteList
	AS (
		select distinct * from Alerts_Associated_To_Single_Vehicle
		union
		select distinct * from Alerts_Associated_To_Group_Vehicle
		union
		select distinct * from Alerts_Associated_To_Dynamic_Vehicle
		union
		select distinct * from Alerts_Associated_To_Dynamic_Owned_Vehicle
		union
		select distinct * from Alerts_Associated_To_Dynamic_visible_Vehicle
		union
		select distinct * from Alerts_Associated_To_Dynamic_OEM_Vehicle
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
-- select * from org_veh_subscriptions
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
		 on cavc.vehicleid = Any(@vehicleIds) and cavc.vehicleid = ovs.vehicleid and ovs.subscriptiontype='V'
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
select distinct alertid as AlertId,vin as VIN from subscription_complete;";

                IEnumerable<VehicleAlertRef> vehicleAlertRefs = await _dataAccess.QueryAsync<VehicleAlertRef>(query, parameter);
                return vehicleAlertRefs.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Task<IEnumerable<string>> GetVINsByIds(IEnumerable<int> vehicleIds)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vehicleid", vehicleIds.ToArray());
            string queryAlertLevelPull = @"select vin  from master.vehicle where id = Any(@vehicleid) and vin is not null;";

            return _dataAccess.QueryAsync<string>(queryAlertLevelPull, parameter);
        }
    }
}
