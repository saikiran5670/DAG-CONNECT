using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.visibility.entity;
//using NpgsqlTypes;

namespace net.atos.daf.ct2.visibility.repository
{
    public class VisibilityRepository : IVisibilityRepository
    {
        private readonly IDataAccess _dataAccess;
        private static readonly log4net.ILog _log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public VisibilityRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }

        public IEnumerable<FeatureSet> GetFeatureSet(int userid, int orgid)
        {
            var featureSet = new List<FeatureSet>();
            var func = "dafconnectmaster.getuserrolefeatures";
            var result = _dataAccess.Query<Feature>(
                            sql: func,
                            param: new { useridinput = userid, orgidinput = orgid },
                            commandType: CommandType.StoredProcedure,
                            commandTimeout: 900) as List<Feature>;

            var parentFeature = result.Where(fe => fe.ParentFeatureId == 0).ToList();
            foreach (var feature in parentFeature)
            {
                if (feature != null)
                {
                    var _featureSet = new FeatureSet();
                    _featureSet.FeatureSetID = feature.RoleFeatureId;
                    _featureSet.FeatureSetName = feature.FeatureDescription;
                    // get child features
                    var childFeatures = result.Where(fe => fe.ParentFeatureId == _featureSet.FeatureSetID).ToList();
                    if (childFeatures != null)
                    {
                        _featureSet.Features = new List<Feature>();
                        _featureSet.Features.AddRange(childFeatures);
                    }
                    featureSet.Add(_featureSet);
                }
            }
            return featureSet;
        }

        public Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibility(int accountId,
                                                                                               int OrganizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_id", OrganizationId);
                #region Query Select Vehicle By Account Visibility
                var query = @"WITH cte_account_visibility_for_vehicle
							AS (
							select distinct ass.vehicle_group_id as vehiclegroupid,ass.access_type,
							case when vgrpref.ref_id is null then  @account_id else vgrpref.ref_id end ref_id
							,grp.organization_id 
							from master.accessrelationship ass
							inner join master.group grp 
							on ass.account_group_id=grp.id and grp.object_type='A' and (((@account_id > 0 and grp.ref_id = @account_id) or (@account_id = 0 and grp.ref_id is not null)) or grp.ref_id is null) 
							left join master.groupref vgrpref
							on  grp.id=vgrpref.group_id and	(( @account_id > 0 and vgrpref.ref_id = @account_id) or (@account_id =0 and 1=1) )
							where ((@organization_id > 0 and grp.organization_id=@organization_id ) or ( @organization_id = 0 and 1=1))							 
							)

							--select * from cte_account_visibility_for_vehicle

							,cte_account_visibility_for_vehicle_groupanddynamic
							AS (
							select distinct grp.id as VehicleGroupId
							,cte.ref_id as accountid
							,grp.object_type
							,grp.group_type
							,grp.function_enum
							,grp.organization_id
							,cte.access_type
							,grp.name as VehicleGroupName
							,veh.id as VehicleId
							,veh.name as VehicleName
							,veh.vin as Vin
							,veh.license_plate_number as RegistrationNo	
							from cte_account_visibility_for_vehicle cte
							inner join master.group grp 
							on cte.vehiclegroupid=grp.id and grp.object_type='V' --and grp.group_type='G'
							left join master.groupref vgrpref
							on  grp.id=vgrpref.group_id
							left join master.vehicle veh
							on vgrpref.ref_id=veh.id
							where grp.organization_id=cte.organization_id 
							)

							--select * from cte_account_visibility_for_vehicle_groupanddynamic

							,cte_account_visibility_for_vehicle_group
							AS (
							select distinct VehicleGroupId
							,accountid
							,object_type
							,group_type
							,function_enum
							,organization_id
							,access_type
							,VehicleGroupName
							,VehicleId
							,VehicleName
							,Vin
							,RegistrationNo	
							from cte_account_visibility_for_vehicle_groupanddynamic
							where group_type='G'
							)

							--select * from cte_account_visibility_for_vehicle_group

							,cte_account_visibility_for_vehicle_single
							AS (
							select distinct grp.id as VehicleGroupId
							,cte.ref_id as accountid
							,grp.object_type
							,grp.group_type
							,grp.function_enum
							,grp.organization_id
							,cte.access_type
							,grp.name as VehicleGroupName
							,veh.id as VehicleId
							,veh.name as VehicleName
							,veh.vin as Vin
							,veh.license_plate_number as RegistrationNo
							from cte_account_visibility_for_vehicle cte
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
								 VehicleGroupId
								,accountid
								,object_type
								,group_type
								,function_enum
								,access_type 
								,Organization_Id
								,VehicleGroupName
								From cte_account_visibility_for_vehicle_groupanddynamic 
								group by VehicleGroupId
								,accountid
								,object_type
								,group_type
								,function_enum
								,access_type
								,Organization_Id
								,VehicleGroupName
								having group_type='D'
							)

							--select * from cte_account_visibility_for_vehicle_dynamic_unique
							,
							cte_account_vehicle_DynamicAll
							AS (
								select distinct 
								 du1.VehicleGroupId
								,du1.accountid
								,du1.object_type
								,du1.group_type
								,du1.function_enum
								,du1.Organization_Id
								,du1.access_type
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
								where ((@organization_id > 0 and veh.organization_id=@organization_id ) or ( @organization_id = 0 and 1=1))
								and ors.state='A'
								and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
								else COALESCE(end_date,0) =0 end  
							)
							--select * from cte_account_vehicle_DynamicAll
							, 
							cte_account_vehicle_DynamicOwned
							AS (
								select distinct 
								 du1.VehicleGroupId
								,du1.accountid
								,du1.object_type
								,du1.group_type
								,du1.function_enum
								,du1.Organization_Id
								,du1.access_type
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
								where ((@organization_id > 0 and veh.organization_id=@organization_id ) or ( @organization_id = 0 and 1=1))
								and ors.state='A'
								and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
								else COALESCE(end_date,0) =0 end  
	
							)
							--select * from cte_account_vehicle_DynamicOwned
							,
							cte_account_vehicle_DynamicVisible
							AS (
								select distinct 
								 du2.VehicleGroupId
								,du2.accountid
								,du2.object_type
								,du2.group_type
								,du2.function_enum
								,du2.Organization_Id
								,du2.access_type
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
								where ((@organization_id > 0 and veh.organization_id=@organization_id  ) or ( @organization_id = 0 and 1=1))
								and ors.state='A'
								and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
								else COALESCE(end_date,0) =0 end  
								and ors.code<>'Owner'
							)
							--select * from cte_account_vehicle_DynamicVisible
							,
							cte_account_vehicle_DynamicOEM
							AS (
								select distinct 
								 du1.VehicleGroupId
								,du1.accountid
								,du1.object_type
								,du1.group_type
								,du1.function_enum
								,du1.Organization_Id
								,du1.access_type
								,du1.VehicleGroupName
								,veh.id as VehicleId
								,veh.name as VehicleName
								,veh.vin as Vin
								,veh.license_plate_number as RegistrationNo
								from master.vehicle veh
								Inner join cte_account_visibility_for_vehicle_dynamic_unique du1
								on veh.organization_id=du1.organization_id and du1.function_enum='M'
								where ((@organization_id > 0 and veh.organization_id=@organization_id) or ( @organization_id = 0 and 1=1))
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
									case when group_type = 'S' then 0 else VehicleGroupId end VehicleGroupId
									,accountid as AccountId
									,object_type as ObjectType
									,group_type as GroupType
									,case when function_enum is null then '' else function_enum end as FunctionEnum
									,Organization_Id as OrganizationId
									,access_type as AccessType
									,case when VehicleGroupName is null or group_type = 'S' then '' else VehicleGroupName end as VehicleGroupName 
									,VehicleId
									,case when VehicleName is null  then '' else VehicleName end as VehicleName
									,case when Vin is null  then '' else Vin end as Vin
									,case when RegistrationNo is null then '' else RegistrationNo end as RegistrationNo 
						 from cte_account_vehicle_CompleteList where ((@organization_id > 0 and organization_id=@organization_id) or ( @organization_id = 0 and 1=1)) AND VehicleId>0 order by 1;";
                #endregion
                return _dataAccess.QueryAsync<VehicleDetailsAccountVisibilty>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<VehicleDetailsFeatureAndSubsction>> GetVehicleByFeatureAndSubscription(int accountId, int organizationId,int roleId,
                                                                                                    string featureName = "Alert")
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@role_id", roleId);
                parameter.Add("@feature_name", featureName + "%");
                #region Query Get Vehicle By Feature And Subsction
                var query = @"
                             with org_veh_subscriptions
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
inner join master.package pac
on sub.package_id=pac.id and sub.state='A' and pac.state='A' and sub.organization_id in(@organization_id) 
inner join master.featuresetfeature feasetfea
on pac.feature_set_id=feasetfea.feature_set_id
inner join master.feature fea
on feasetfea.feature_id=fea.id and fea.state='A' and fea.type='F'
where sub.organization_id in(@organization_id)
and 
fea.name like @feature_name
and case when COALESCE(subscription_end_date,0) !=0 then to_timestamp(COALESCE(subscription_end_date)/1000)::date>=now()::date
    else COALESCE(subscription_end_date,0) =0 end
order by 1
)
--select * from org_veh_subscriptions
,
 org_subscriptions
 as (
     select * from  org_veh_subscriptions where subscriptiontype='O'
 )
 -- select * from org_subscriptions
 ,
 veh_subscriptions
 as (
     select * from  org_veh_subscriptions where subscriptiontype='V'
 )
-- select * from veh_subscriptions
,
matching_org_veh_features
as
(
	select featureid,key,name,organizationid,featuresetid from org_subscriptions
	intersect
	select featureid,key,name,organizationid,featuresetid from veh_subscriptions 
)
--select * from matching_org_veh_features
,
veh_features_not_in_org
as
(
	select *  
	from veh_subscriptions 
	where featureid not in ( select featureid from matching_org_veh_features) 	
)
--select * from veh_features_not_in_org
,
org_veh_subscribe_features
as
(
	select distinct  movf.featureid,movf.key,movf.name,0 as vehicleid, movf.organizationid , 'O' as subscriptiontype,
	case when featuresetid is null then '0' else featuresetid end as featuresetid 
	from matching_org_veh_features movf
	union
	select distinct  featureid,key,name, case when vehicleid is null then 0 else vehicleid end  as vehicleid , organizationid , subscriptiontype,
	case when featuresetid is null then 0 else featuresetid end as featuresetid 
	from  veh_features_not_in_org
	union
	select distinct featureid,key,name, case when vehicleid is null then 0 else vehicleid end  as vehicleid, organizationid , subscriptiontype,case when featuresetid is null then 0 else featuresetid end as featuresetid  from  org_subscriptions
)

--select distinct ovsf.featureid, ovsf.key as featurekey,ovsf.name,ovsf.vehicleid,ovsf.organizationid,ovsf.subscriptiontype,enutra.key as enumkey,ovsf.featuresetid
select ovsf.featureid as FeatureId, 
case when ovsf.key is null  then '' else ovsf.key end as key,
case when ovsf.name is null then '' else ovsf.name end as Name,ovsf.vehicleid as VehicleId,ovsf.organizationid as OrganizationId,
case when ovsf.subscriptiontype is null then '' else subscriptiontype end as SubscriptionType,
case when enutra.key is null then '' else enutra.key end as FeatureEnum  
FROM master.Account acc
INNER JOIN master.AccountRole accrol ON acc.id = accrol.account_id AND acc.id = @account_id  AND accrol.organization_id = @organization_id AND accrol.role_id = @role_id AND acc.state = 'A'
INNER JOIN master.Role rol ON accrol.role_id = rol.id AND rol.state = 'A'
inner join org_veh_subscribe_features ovsf ON rol.feature_set_id=ovsf.featuresetid
left join translation.enumtranslation enutra
on   ovsf.featureid = enutra.feature_id
and enutra.type='T' 
order by 1 desc
";
                #endregion
                
                var list = _dataAccess.QueryAsync<VehicleDetailsFeatureAndSubsction>(query, parameter);
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}