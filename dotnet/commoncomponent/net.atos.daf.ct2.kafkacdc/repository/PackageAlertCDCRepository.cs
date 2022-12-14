using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public class PackageAlertCdcRepository : IPackageAlertCdcRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;

        public PackageAlertCdcRepository(IDataAccess dataAccess, IDataMartDataAccess dataMartdataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartdataAccess = dataMartdataAccess;
        }

        public async Task<List<VehicleAlertRef>> GetVehicleAlertRefByAlertIds(List<int> alertId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@alertid", alertId);
                string queryAlertLevelPull = @"select vin,alert_id as AlertId, state  from tripdetail.vehiclealertref where alert_id = ANY(@alertid);";

                IEnumerable<VehicleAlertRef> vehicleAlertRefs = await _dataMartdataAccess.QueryAsync<VehicleAlertRef>(queryAlertLevelPull, parameter);
                return vehicleAlertRefs.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<VehicleAlertRef>> GetVehiclesAndAlertFromPackageConfiguration(int packageId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@packageId", packageId);
                string query = @"with cte_package_alert
                            AS (
                                   select distinct 
                                    ale.id as alertid
                                   ,ale.vehicle_group_id as vehicle_group_id
                                   ,fea.id as featureid
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
                            from master.package pac
                            inner join master.subscription sub
                            on sub.package_id=pac.id and sub.state='A'
                            inner join master.featuresetfeature feasetfea
                            on pac.feature_set_id=feasetfea.feature_set_id
                            inner join master.feature fea
                            on feasetfea.feature_id=fea.id and fea.state='A' and fea.type='F'
                            inner join translation.enumtranslation enutra
                            on fea.id=enutra.feature_id and enutra.type='T'
							inner join master.alert ale
                            on ale.type= enutra.enum and ale.category= enutra.parent_enum and enutra.type='T' AND ale.state ='A'
                            where pac.id =@packageId and sub.organization_id in(ale.organization_id)
                            and sub.state ='A' and fea.id= enutra.feature_id
                            and case when COALESCE(subscription_end_date,0) !=0 then to_timestamp(COALESCE(subscription_end_date)/1000)::date>now()::date
                                else COALESCE(subscription_end_date,0) =0 end
                            order by 1
								),

                            --select * from cte_package_alert
                            cte_alert_vehicle_groupanddynamic
                            AS (
                            select distinct 
                            cte.alertid as AlertId,                            
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
                            from cte_package_alert cte
                            inner join master.group grp 
                            on cte.vehicle_group_id = grp.id and grp.object_type='V' 
                            left join master.groupref vgrpref
                            on  grp.id=vgrpref.group_id
                            left join master.vehicle veh
                            on vgrpref.ref_id=veh.id AND case when veh.opt_in='H' 
							THEN 
							Case when (select count(vehicle_default_opt_in) as vehcount
											from master.organization where vehicle_default_opt_in='I' AND id =veh.organization_id)>0 
											then
											veh.opt_in='H'
											ELSE
											veh.opt_in='I'
											END	
							ELSE veh.opt_in='I' END 
                            )
                            --select * from cte_alert_vehicle_groupanddynamic
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
                            where grp.organization_id=cte.organization_id AND
								CASE WHEN veh.opt_in='H' 
								THEN 
								CASE WHEN (select count(vehicle_default_opt_in) as vehcount
												from master.organization where vehicle_default_opt_in='I' AND id =veh.organization_id)>0 
												THEN
												veh.opt_in='H'
												ELSE
												veh.opt_in='I'
												END	
								ELSE veh.opt_in='I' END 
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
	                            on ((orm.owner_org_id = du1.Organization_Id and lower(ors.code)='owner') 
	                            or (orm.target_org_id= du1.Organization_Id and lower(ors.code) NOT IN ('owner','oem')))
	                            and du1.function_enum='A'
	                            --Left join cte_account_visibility_for_vehicle_dynamic_unique du2
	                            --on orm.target_org_id=du2.Organization_Id and lower(ors.code) NOT IN ('owner','oem') and du2.function_enum='A'
	                            where ors.state='A'
	                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date 
	                            else COALESCE(end_date,0) =0 end
								AND
								CASE WHEN veh.opt_in='H' 
								THEN 
								CASE WHEN (select count(vehicle_default_opt_in) as vehcount
												from master.organization where vehicle_default_opt_in='I' AND id =veh.organization_id)>0 
												THEN
												veh.opt_in='H'
												ELSE
												veh.opt_in='I'
												END	
								ELSE veh.opt_in='I' END 
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
	                            on ((orm.owner_org_id=du1.Organization_Id and lower(ors.code)='owner') or (veh.organization_id=du1.Organization_Id)) and du1.function_enum='O'
	                            where ors.state='A'
	                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date 
	                            else COALESCE(end_date,0) =0 end 
								AND
								CASE WHEN veh.opt_in='H' 
								THEN 
								CASE WHEN (select count(vehicle_default_opt_in) as vehcount
												from master.organization where vehicle_default_opt_in='I' AND id =veh.organization_id)>0 
												THEN
												veh.opt_in='H'
												ELSE
												veh.opt_in='I'
												END	
								ELSE veh.opt_in='I' END 
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
	                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>now()::date 
	                            else COALESCE(end_date,0) =0 end  
	                            and lower(ors.code) NOT IN ('owner','oem')
								AND
								CASE WHEN veh.opt_in='H' 
								THEN 
								CASE WHEN (select count(vehicle_default_opt_in) as vehcount
												from master.organization where vehicle_default_opt_in='I' AND id =veh.organization_id)>0 
												THEN
												veh.opt_in='H'
												ELSE
												veh.opt_in='I'
												END	
								ELSE veh.opt_in='I' END 
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
								AND
								CASE WHEN veh.opt_in='H' 
								THEN 
								CASE WHEN (select count(vehicle_default_opt_in) as vehcount
												from master.organization where vehicle_default_opt_in='I' AND id =veh.organization_id)>0 
												THEN
												veh.opt_in='H'
												ELSE
												veh.opt_in='I'
												END	
								ELSE veh.opt_in='I' END 
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
							select distinct alertid,vin from cte_account_vehicle_CompleteList;";

                IEnumerable<VehicleAlertRef> vehicleAlertRefs = await _dataAccess.QueryAsync<VehicleAlertRef>(query, parameter);
                return vehicleAlertRefs.AsList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<AlertFromPackage>> GetAlertFromPackageConfiguration(int packageId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@packageId", packageId);
                string query = @"select distinct 
                                    ale.id as alertid
                                   ,ale.vehicle_group_id as vehicle_group_id
                                   ,fea.id as featureid
								   ,ale.organization_id as organizationid
                            from master.package pac
                            inner join master.subscription sub
                            on sub.package_id = pac.id and sub.state = 'A'
                            inner join master.featuresetfeature feasetfea
                            on pac.feature_set_id = feasetfea.feature_set_id
                            inner join master.feature fea
                            on feasetfea.feature_id = fea.id and fea.state = 'A' and fea.type = 'F'
                            inner join translation.enumtranslation enutra
                            on fea.id = enutra.feature_id and enutra.type = 'T'
                            inner join master.alert ale
                            on ale.type = enutra.enum and ale.category= enutra.parent_enum and enutra.type='T' AND ale.state ='A'
                            where pac.id = @packageId and sub.organization_id in (ale.organization_id)
                             and sub.state = 'A' and fea.id= enutra.feature_id 
                             and case when COALESCE(subscription_end_date,0) !=0 then to_timestamp(COALESCE(subscription_end_date)/1000)::date>now()::date
                                else COALESCE(subscription_end_date,0) =0 end
                            order by 1";
                IEnumerable<AlertFromPackage> alertDetails = await _dataAccess.QueryAsync<AlertFromPackage>(query, parameter);
                return alertDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<AlertGroupId>> GetAlertIdsandVGIds(List<int> groupIds, List<int> featureIds)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@featureIds", featureIds);
                var queryStatementFeature = @"select enum from translation.enumtranslation where feature_id = ANY(@featureIds)";
                List<string> resultFeaturEnum = (List<string>)await _dataAccess.QueryAsync<string>(queryStatementFeature, parameter);
                parameter.Add("@featureEnums", resultFeaturEnum);
                parameter.Add("@groupIds", groupIds);
                string query = @"SELECT ale.id as Alertid,ale.vehicle_group_id as GroupId
								FROM master.alert ale
								WHERE ale.vehicle_group_id= ANY(@groupIds) AND ale.type = ANY(@featureEnums) AND ale.state<>'D';";

                IEnumerable<AlertGroupId> vehicleAlertRefs = await _dataAccess.QueryAsync<AlertGroupId>(query, parameter);

                return vehicleAlertRefs.AsList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<int>> GetAlertPackageIds(int orgContextId, int packageId, List<int> featureIds)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@packageId", packageId);
                parameter.Add("@featureIds", featureIds);
                parameter.Add("@organisationId", orgContextId);

                string query = @"SELECT distinct f.id
                                    FROM master.Package pkg
                                    INNER JOIN master.Subscription s ON s.package_id = pkg.id AND pkg.id =@packageId
                                    AND s.organization_id = @organisationId AND s.state = 'A' AND pkg.state = 'A'
                                    INNER JOIN master.FeatureSet fset ON pkg.feature_set_id = fset.id AND fset.state = 'A'
                                    INNER JOIN master.FeatureSetFeature fsf ON fsf.feature_set_id = fset.id
                                    INNER JOIN master.Feature f ON f.id = fsf.feature_id AND f.state = 'A' AND f.type <> 'D' AND f.name like 'Alerts%'
                                    WHERE f.id =ANY(@featureIds);";

                IEnumerable<int> featureids = await _dataAccess.QueryAsync<int>(query, parameter);
                return featureids;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
