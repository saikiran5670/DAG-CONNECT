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
            var featureSets = new List<FeatureSet>();
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
                    var featureSet = new FeatureSet();
                    featureSet.FeatureSetID = feature.RoleFeatureId;
                    featureSet.FeatureSetName = feature.FeatureDescription;
                    // get child features
                    var childFeatures = result.Where(fe => fe.ParentFeatureId == featureSet.FeatureSetID).ToList();
                    if (childFeatures != null)
                    {
                        featureSet.Features = new List<Feature>();
                        featureSet.Features.AddRange(childFeatures);
                    }
                    featureSets.Add(featureSet);
                }
            }
            return featureSets;
        }

        public Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleByAccountVisibility(int accountId,
                                                                                               int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_id", organizationId);
                // Added and veh.opt_in != 'U' to avoide opt out vehicle in below query for bug no. 15983
                #region Query Select Vehicle By Account Visibility
                var query = @"WITH cte_account_visibility_for_vehicle
                            AS (
                            select distinct ass.vehicle_group_id as vehiclegroupid,ass.access_type,
                            case when vgrpref.ref_id is null then  @account_id else vgrpref.ref_id end ref_id
                            ,grp.organization_id 
                            from master.accessrelationship ass
                            inner join master.group grp 
                            on ass.account_group_id=grp.id and grp.object_type='A' and 
                            (grp.ref_id = @account_id  or grp.ref_id is null)
                            inner join master.accountorg accorg
                            on accorg.account_id=@account_id and accorg.organization_id=@organization_id
                            left join master.groupref vgrpref
                            on  grp.id=vgrpref.group_id and	vgrpref.ref_id = @account_id
                            where grp.organization_id=@organization_id
                            and (grp.ref_id = @account_id  or  vgrpref.ref_id = @account_id or (grp.group_type='D'  ))
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
                            on vgrpref.ref_id=veh.id and veh.opt_in != 'U'
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
                            on grp.ref_id=veh.id and grp.group_type='S' and veh.opt_in != 'U'
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
	                            on orm.vehicle_id=veh.id and veh.opt_in != 'U'
	                            Inner join master.orgrelationship ors
	                            on ors.id=orm.relationship_id
	                            Inner join cte_account_visibility_for_vehicle_dynamic_unique du1
	                            on ((orm.owner_org_id = du1.Organization_Id and lower(ors.code)='owner') 
	                            or (orm.target_org_id= du1.Organization_Id and lower(ors.code) NOT IN ('owner','oem')))
	                            and du1.function_enum='A'
	                            --Left join cte_account_visibility_for_vehicle_dynamic_unique du2
	                            --on orm.target_org_id=du2.Organization_Id and lower(ors.code) NOT IN ('owner','oem') and du2.function_enum='A'
	                            where ((@organization_id > 0 and veh.organization_id=@organization_id ) or ( @organization_id = 0 and 1=1))
	                            and ors.state='A'
	                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date 
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
	                            on orm.vehicle_id=veh.id  and veh.opt_in != 'U'
	                            Inner join master.orgrelationship ors
	                            on ors.id=orm.relationship_id
	                            Inner join cte_account_visibility_for_vehicle_dynamic_unique du1
	                            on ((orm.owner_org_id=du1.Organization_Id and lower(ors.code)='owner') or (veh.organization_id=du1.Organization_Id)) and du1.function_enum='O'
	                            where ((@organization_id > 0 and veh.organization_id=@organization_id ) or ( @organization_id = 0 and 1=1))
	                            and ors.state='A'
	                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date 
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
	                            on orm.vehicle_id=veh.id  and veh.opt_in != 'U'
	                            Inner join master.orgrelationship ors
	                            on ors.id=orm.relationship_id
	                            Inner join cte_account_visibility_for_vehicle_dynamic_unique du2
	                            on orm.target_org_id=du2.Organization_Id and du2.function_enum='V'
	                            where ((@organization_id > 0 and veh.organization_id=@organization_id  ) or ( @organization_id = 0 and 1=1))
	                            and ors.state='A'
	                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date 
	                            else COALESCE(end_date,0) =0 end  
	                            and lower(ors.code) NOT IN ('owner','oem')
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
	                            on veh.organization_id=du1.organization_id and du1.function_enum='M'  and veh.opt_in != 'U'
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
                            from cte_account_vehicle_CompleteList where  organization_id=@organization_id and vehicleid>0 order by 1;";
                #endregion
                return _dataAccess.QueryAsync<VehicleDetailsAccountVisibility>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<IEnumerable<VehicleDetailsFeatureAndSubsction>> GetVehicleByFeatureAndSubscription(int accountId, int orgId, int contextOrgId, int roleId,
                                                                                                    string featureName = "Alert")
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_id", orgId);
                parameter.Add("@context_org_id", contextOrgId);
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
                            on sub.package_id=pac.id and sub.state='A' and pac.state='A' and sub.organization_id in(@context_org_id) 
                            inner join master.featuresetfeature feasetfea
                            on pac.feature_set_id=feasetfea.feature_set_id
                            inner join master.feature fea
                            on feasetfea.feature_id=fea.id and fea.state='A' and fea.type='F'
                            where sub.organization_id in(@context_org_id)
                            and 
                            fea.name like @feature_name
                            and case when COALESCE(subscription_end_date,0) !=0 then to_timestamp(COALESCE(subscription_end_date)/1000)::date>now()::date
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
	                            select distinct  movf.featureid,movf.key,movf.name,0 as vehicleid, movf.organizationid , 'O' as subscriptiontype
	                            --case when featuresetid is null then '0' else featuresetid end as featuresetid 
	                            from matching_org_veh_features movf
	                            union
	                            select distinct  featureid,key,name, case when vehicleid is null then 0 else vehicleid end  as vehicleid , organizationid , subscriptiontype
	                            --case when featuresetid is null then 0 else featuresetid end as featuresetid 
	                            from  veh_features_not_in_org
	                            union
	                            select distinct featureid,key,name, case when vehicleid is null then 0 else vehicleid end  as vehicleid, organizationid , subscriptiontype
	                            --,case when featuresetid is null then 0 else featuresetid end as featuresetid  
	                            from  org_subscriptions
                            )
                            ,user_specific_features
                            as
                            (
                            select FSF.feature_id as FeatureId
                            FROM master.Account acc
                            INNER JOIN master.AccountRole accrol ON acc.id = accrol.account_id AND acc.id = @account_id  AND accrol.organization_id = @organization_id AND accrol.role_id = @role_id AND acc.state = 'A'
                            INNER JOIN master.Role rol ON accrol.role_id = rol.id AND rol.state = 'A'
                            INNER JOIN master.FeatureSetFeature FSF on FSF.feature_set_id=rol.feature_set_id
                            --INNER join org_veh_subscribe_features ovsf ON FSF.feature_id=ovsf.featureid
                            --left join translation.enumtranslation enutra
                            --on   ovsf.featureid = enutra.feature_id
                            --and enutra.type='T'
                            --order by 1 desc
                            )
                            select distinct ovsf.featureid as FeatureId, 
                            case when ovsf.key is null  then '' else ovsf.key end as key,
                            case when ovsf.name is null then '' else ovsf.name end as Name,ovsf.vehicleid as VehicleId,ovsf.organizationid as OrganizationId,
                            case when ovsf.subscriptiontype is null then '' else subscriptiontype end as SubscriptionType,
                            case when enutra.key is null then '' else enutra.key end as FeatureEnum   from org_veh_subscribe_features ovsf
                            inner join
                            user_specific_features usf
                            on ovsf.FeatureId=usf.FeatureId
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

        public async Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleVisibilityDetails(int[] vehicleIds, int accountId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@VehicleIds", vehicleIds);
                parameter.Add("@AccountId", accountId);

                var queryStatement = @"SELECT DISTINCT @AccountId as AccountId, v.id as VehicleId, v.name as VehicleName, vin as Vin, license_plate_number as RegistrationNo, 
                                            grp.id as VehicleGroupId, grp.group_type as GroupType,
                                            object_type as ObjectType, v.organization_id as OrganizationId,
		                                    case when function_enum is null then '' else function_enum end as FunctionEnum,
                                            case when grp.name is null or group_type = 'S' then '' else grp.name end as VehicleGroupName
                                    FROM master.vehicle v
                                    LEFT OUTER JOIN master.groupref gref ON v.id=gref.ref_id
                                    INNER JOIN master.group grp ON (gref.group_id=grp.id OR grp.ref_id=v.id OR grp.group_type='D') AND grp.object_type='V'
                                    WHERE v.organization_id=grp.organization_id AND v.id = ANY(@VehicleIds)";

                return await _dataAccess.QueryAsync<VehicleDetailsAccountVisibility>(queryStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleVisibilityDetailsTemp(int[] vehicleIds)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@VehicleIds", vehicleIds);

                var queryStatement = @"SELECT DISTINCT v.id as VehicleId, v.name as VehicleName, vin as Vin, license_plate_number as RegistrationNo, 
                                            v.organization_id as OrganizationId,
		                                    string_agg(distinct concat(grp.id::text,'~',grp.name::text,'~',grp.group_type::text), ',') as VehicleGroupDetails
                                    FROM master.vehicle v
                                    LEFT OUTER JOIN master.groupref gref ON v.id=gref.ref_id
                                    INNER JOIN master.group grp ON (gref.group_id=grp.id OR grp.ref_id=v.id OR grp.group_type='D') AND grp.object_type='V'
                                    WHERE v.organization_id=grp.organization_id AND v.id = ANY(@VehicleIds)
                                    GROUP BY v.id,v.name,v.vin, v.organization_id, v.license_plate_number";

                return await _dataAccess.QueryAsync<VehicleDetailsAccountVisibility>(queryStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> GetReportFeatureId(int reportId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@report_id", reportId);

                var query = @"SELECT feature_id FROM master.report WHERE id = @report_id";

                return await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VehiclePackage>> GetSubscribedVehicleByFeature(int featureId, int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@featureid", featureId);
                parameter.Add("@organizationid", organizationId);

                var queryStatement = @"select type as Packagetype,coalesce(HasOwned,true) as HasOwned, array_agg(distinct coalesce(vehicle_id, 0)) as VehicleIds
                                            from
                                            (
                                                select p.type, s.vehicle_id, case when s.organization_id=v.organization_id 
	                                                then true else false end as HasOwned
                                                from master.subscription s
                                                inner join master.package p on p.id=s.package_id
                                                inner join master.featureset fset on fset.id=p.feature_set_id AND fset.state = 'A'
                                                inner join master.featuresetfeature ff on ff.feature_set_id=fset.id
                                                inner join master.feature f on f.id=ff.feature_id AND f.state = 'A'
                                                left join master.vehicle v on s.vehicle_id = v.id
                                                where (s.organization_id=@organizationid or s.type  ='N')  and f.id=@featureid and p.type In ('V','O','N') AND s.state = 'A'
                                            ) temp
                                            group by HasOwned,type";
                var result = await _dataAccess.QueryAsync<VehiclePackage>(queryStatement, parameter);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int[]> GetRelationshipVehiclesByFeature(int featureId, int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@featureid", featureId);
                parameter.Add("@organizationid", organizationId);

                var queryStatement =
                    @"with cte_org_rels as 
                    (
	                    select distinct orm.owner_org_id, orm.vehicle_group_id, grp.group_type
	                    from master.orgrelationshipmapping orm
	                    inner join master.orgrelationship ors on orm.relationship_id = ors.id and orm.target_org_id=@organizationid and lower(ors.code) not in ('owner','oem')
	                    inner join master.group grp on orm.vehicle_group_id = grp.id and grp.organization_id = orm.owner_org_id
	                    inner join master.featureset fset on fset.id=ors.feature_set_id
	                    inner join master.featuresetfeature ff on ff.feature_set_id=fset.id
	                    inner join master.feature f on f.id=ff.feature_id
	                    where f.id= @featureid and 
                            case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                        else COALESCE(end_date,0) = 0 end
	                    group by orm.owner_org_id, orm.vehicle_group_id, grp.group_type
                    ),
                    cte_g_vehicles as
                    (
	                    select rels.owner_org_id, array_agg(gref.ref_id) as vehicleIds
	                    from master.group grp
	                    inner join master.groupref gref on grp.id = gref.group_id and grp.object_type = 'V' and grp.group_type = 'G'
	                    inner join cte_org_rels rels on grp.id = rels.vehicle_group_id and grp.organization_id = rels.owner_org_id and rels.group_type = grp.group_type
	                    group by rels.owner_org_id
                    ),
                    cte_d_vehicles as
                    (
	                    select rels.owner_org_id, array_agg(v.id) as vehicleIds
	                    from master.group grp
	                    inner join cte_org_rels rels on grp.id = rels.vehicle_group_id and grp.organization_id = rels.owner_org_id and rels.group_type = grp.group_type and grp.group_type = 'D'
	                    inner join master.vehicle v on v.organization_id = rels.owner_org_id
	                    group by rels.owner_org_id
                    )
                    select array_agg(vehicleIds) as vehicleIds
                    from
                    (
	                    select owner_org_id, unnest(vehicleIds) as vehicleIds from cte_g_vehicles
	                    UNION
	                    select owner_org_id, unnest(vehicleIds) as vehicleIds from cte_d_vehicles
                    ) veh
                    group by veh.owner_org_id";

                var result = await _dataAccess.ExecuteScalarAsync<int[]>(queryStatement, parameter);
                return result ?? new int[] { };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<VehicleDetailsVisibiltyAndFeatureTemp>> GetSubscribedVehicleByAlertFeature(List<int> featureId, int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@featureid", featureId.ToArray());
                parameter.Add("@organizationid", organizationId);

                var queryStatement = @"select distinct (Case when s.type ='N' Then 'V' ELSE s.type end) as SubscriptionType, s.vehicle_id as VehicleId,e.key as FeatureKey
                                    from master.subscription s
                                    inner join master.package p on p.id=s.package_id 
                                    inner join master.featureset fset on fset.id=p.feature_set_id AND fset.state = 'A'
                                    inner join master.featuresetfeature ff on ff.feature_set_id=fset.id
                                    inner join master.feature f on f.id=ff.feature_id AND f.state = 'A'
                                    left join translation.enumtranslation e
                                    on f.id = e.feature_id and e.type='T'
                                    where (s.organization_id=@organizationid or s.type  ='N') AND f.id= ANY(@featureid) AND s.state = 'A'";
                var result = await _dataAccess.QueryAsync<VehicleDetailsVisibiltyAndFeatureTemp>(queryStatement, parameter);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<int>> GetAccountsForOTA(string vin)
        {
            try
            {
                var parameter = new DynamicParameters();
                int vehicleId = await _dataAccess.QuerySingleAsync<int>("SELECT id FROM master.vehicle where vin=@vin", new { vin = vin });
                parameter.Add("@vehicleId", vehicleId);

                var queryStatement = @"SELECT distinct acc.id as AccountIds
                                                FROM master.account acc
                                                INNER JOIN master.accountorg ar on acc.id=ar.account_id and acc.state='A' and ar.state='A'
                                                LEFT OUTER JOIN master.groupref gref ON acc.id=gref.ref_id
                                                INNER JOIN master.group grp ON (gref.group_id=grp.id OR grp.ref_id=acc.id OR grp.group_type='D') AND grp.object_type='A' AND grp.organization_id = ar.organization_id
                                                INNER JOIN master.accessrelationship arship ON arship.account_group_id=grp.id
                                                INNER JOIN 
                                                (
                                                -- Vehicle Owner Org groups S/G/D
                                                SELECT grp.id
                                                FROM master.vehicle v
                                                LEFT OUTER JOIN master.groupref gref ON v.id=gref.ref_id
                                                INNER JOIN master.group grp ON (gref.group_id=grp.id OR grp.ref_id=v.id OR (grp.group_type='D' and grp.function_enum IN ('A','O'))) and v.organization_id=grp.organization_id AND grp.object_type='V'
                                                WHERE v.id = @vehicleId
                                                UNION
                                                -- Vehicle Owner shared vehicle via vehicle group type 'G' 
                                                SELECT grp.id
                                                FROM master.vehicle v
                                                INNER JOIN master.groupref gref ON v.id = @vehicleId and v.id=gref.ref_id
                                                INNER JOIN master.group grp ON gref.group_id=grp.id AND grp.object_type='V' AND grp.group_type = 'G'
                                                INNER JOIN master.orgrelationshipmapping as orm on orm.target_org_id=grp.organization_id and orm.owner_org_id=v.organization_id
                                                INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id and ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                                                WHERE 
	                                                case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                                                else COALESCE(end_date,0) = 0 end
                                                UNION
                                                -- Vehicle Owner shared vehicle via vehicle group type 'D' 
                                                SELECT grp.id
                                                FROM master.vehicle v
                                                INNER JOIN master.group grp ON grp.object_type='V' AND grp.group_type = 'D' AND grp.function_enum IN ('A','V') AND v.id = @vehicleId
                                                INNER JOIN master.orgrelationshipmapping as orm on orm.target_org_id=grp.organization_id and orm.owner_org_id=v.organization_id
                                                INNER JOIN master.orgrelationship as ors on orm.relationship_id=ors.id and ors.state='A' AND lower(ors.code) NOT IN ('owner','oem')
                                                WHERE 
	                                                case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date
	                                                else COALESCE(end_date,0) = 0 end
                                                ) temp on arship.vehicle_group_id = temp.id";
                var result = (List<int>)await _dataAccess.QueryAsync<int>(queryStatement, parameter);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
