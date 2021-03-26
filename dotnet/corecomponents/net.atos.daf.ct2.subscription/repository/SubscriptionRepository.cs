
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.subscription.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.subscription.repository
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IDataAccess dataAccess;

        private static readonly log4net.ILog log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SubscriptionRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
        //forgeting package type and id
        async Task<Package> GetPackageTypeByCode(string PackageCode)
        {
            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@packagecode", PackageCode);
            parameterToGetPackageId.Add("@is_active", true);
            var data = await dataAccess.QueryFirstOrDefaultAsync<Package>
                             (@"select id, type, packagecode from master.package where packagecode =@packagecode and is_active =@is_active",
                            parameterToGetPackageId);
            return data;
        }

        //forgeting package type By id
        async Task<Package> GetPackageTypeById(int packageId)
        {
            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@id", packageId);
            parameterToGetPackageId.Add("@is_active", true);
            var data = await dataAccess.QueryFirstOrDefaultAsync<Package>
                             (@"select id, type,packagecode from master.package where id =@id and is_active =@is_active",
                            parameterToGetPackageId);
            return data;
        }

        //forgeting package type and id
        async Task<int> GetOrganizationIdByCode(string OrganizationCode)
        {
            
            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@org_id", OrganizationCode);
            parameterToGetPackageId.Add("@is_active", true);
            string query = @"select id from master.organization where org_id=@org_id and is_active = @is_active";
            var data = await dataAccess.ExecuteScalarAsync<int>
                             (query,parameterToGetPackageId);
            return data;
        }

        //to check Subscription Id exits or not
        async Task<subscriptionIdType> SubscriptionIdExits(string orderId, int orgId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@subscription_id", orderId); 
                parameterToGetSubscribeId.Add("@organization_id", orgId);
            parameterToGetSubscribeId.Add("@is_active", true);
            var data = await dataAccess.QueryFirstOrDefaultAsync<subscriptionIdType>
                             (@"SELECT id, type FROM master.subscription 
                               where subscription_id =@subscription_id AND is_active =@is_active
                               AND  organization_id = @organization_id",
                            parameterToGetSubscribeId);
            return data;
        }

        //To check if Organization is already inserted
        async Task<subscriptionIdStatus> SubscriptionExits(int orgId, int packageId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@organization_Id", orgId);
            parameterToGetSubscribeId.Add("@package_id", packageId);
            //parameterToGetSubscribeId.Add("@is_active", true);
            var data = await dataAccess.QueryFirstOrDefaultAsync<subscriptionIdStatus>
                             (@"select subscription_id, is_active from master.subscription where organization_Id =@organization_Id and package_id=@package_id",
                            parameterToGetSubscribeId);
            return data;
        }

        //To check if Subscription exits with orgid, packid and vinid is already inserted
        async Task<subscriptionIdStatus> SubscriptionExits(int orgId, int packageId, int vinId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@organization_Id", orgId);
            parameterToGetSubscribeId.Add("@package_id", packageId);
            parameterToGetSubscribeId.Add("@vehicle_id", vinId);
            //parameterToGetSubscribeId.Add("@is_active", true);
            var data = await dataAccess.QueryFirstOrDefaultAsync<subscriptionIdStatus>
                             (@"select subscription_id, is_active from master.subscription where organization_Id =@organization_Id and package_id=@package_id and vehicle_id=@vehicle_id",
                            parameterToGetSubscribeId);
            return data;
        }
        public async Task<SubscriptionResponse> Subscribe(SubscriptionActivation objSubscription)
        {
            log.Info("Subscribe Subscription method called in repository");
            try
            {
                // Get package Id and type to insert in Subscription Table
                var data = await GetPackageTypeByCode(objSubscription.packageId);
                //Get Organization id to insert in subscription Table
                int orgid = await GetOrganizationIdByCode(objSubscription.OrganizationId);
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                if (data == null || orgid == 0)
                {
                    return null;
                }
                else if (data.id > 0 && data.type.ToLower() == "o")
                {
                    //if package type is organization and has vins in the payload return bad request
                    if (objSubscription.VINs != null && objSubscription.VINs.Count > 0)
                    {
                        return null;
                    }

                    string SubscriptionId = string.Empty;
                    //return subscriptionid and is_active from subscription table
                    var responce  = await SubscriptionExits(orgid, data.id);
                    if (responce != null && !string.IsNullOrEmpty(responce.subscription_id) && responce.is_active)
                    {// Subscription exists and Active
                        return null;
                    }
                    else if (responce != null && !string.IsNullOrEmpty(responce.subscription_id) && !responce.is_active)
                    {// Subscription exists and InActive
                        var parameterStatus = new DynamicParameters();
                        parameterStatus.Add("@is_active", true);
                        parameterStatus.Add("@subscription_id", responce.subscription_id);
                        parameterStatus.Add("@subscription_start_date", objSubscription.StartDateTime);
                        parameterStatus.Add("@subscription_end_date", null);
                        string queryUpdate = @"update master.subscription set is_active = @is_active, subscription_start_date = @subscription_start_date,
                                           subscription_end_date = @subscription_end_date where subscription_id = @subscription_id and is_active = false";

                        int updated = await dataAccess.ExecuteAsync(queryUpdate, parameterStatus);
                        if (updated > 0)
                        {
                            objSubscriptionResponse.orderId = responce.subscription_id;
                            return objSubscriptionResponse;
                        }
                    }
                    else if (responce == null && string.IsNullOrEmpty(responce.subscription_id))
                    {//Subscription does'nt exists for this scenerio
                        SubscriptionId = Guid.NewGuid().ToString();
                        var parameter = new DynamicParameters();
                        parameter.Add("@organization_id", orgid);
                        parameter.Add("@subscription_id", SubscriptionId);
                        parameter.Add("@package_code", objSubscription.packageId);
                        parameter.Add("@package_id", data.id);
                        parameter.Add("@type", data.type);
                        parameter.Add("@subscription_start_date", objSubscription.StartDateTime);
                        parameter.Add("@subscription_end_date", null);
                        parameter.Add("@vehicle_id", null);
                        parameter.Add("@is_active", true);
                        parameter.Add("@is_zuora_package", true);
                        string queryInsert = "insert into master.subscription(organization_id, subscription_id, type, package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, is_active,is_zuora_package) " +
                                     "values(@organization_id, @subscription_id, @type, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @is_active,@is_zuora_package) RETURNING id";

                        int subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                        if (subid > 0)
                            objSubscriptionResponse.orderId = SubscriptionId;
                    }
                         
                }
                //if package type is vehicle then only check vehicle table
                else if (data.id > 0 && data.type.ToLower() == "v")
                {
                    if (objSubscription.VINs == null || objSubscription.VINs.Count == 0)
                    {
                        return null;
                    }
                    if (objSubscription.VINs.Count > 0)
                    {
                        ArrayList objvinList = new ArrayList();
                        foreach (var item in objSubscription.VINs)
                        {
                            int vinexist = 0;
                            //forgeting vehicle id
                            var parameterToGetVehicleId = new DynamicParameters();
                            parameterToGetVehicleId.Add("@vin", item);
                            vinexist = await dataAccess.ExecuteScalarAsync<int>
                                (@"SELECT id FROM master.vehicle where vin=@vin", parameterToGetVehicleId);
                            if (vinexist == 0)
                            {//return Bad Request if vin does not exits in vehicle table
                                return null;
                            }
                            int vinActivated = 0;
                            var response = await SubscriptionExits(orgid, data.id, vinexist);
                            if (response != null && !string.IsNullOrEmpty(response.subscription_id) && response.is_active)
                            {
                                return null;
                            }
                            else if (response != null && !string.IsNullOrEmpty(response.subscription_id) && !response.is_active)
                            {
                                // Subscription exists and InActive
                                var parameterStatus = new DynamicParameters();
                                parameterStatus.Add("@is_active", true);
                                parameterStatus.Add("@subscription_id", response.subscription_id);
                                parameterStatus.Add("@subscription_start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                                parameterStatus.Add("@subscription_end_date", null);
                                parameterStatus.Add("@vehicle_id", vinexist); 
                                string queryUpdate = @"update master.subscription set is_active = @is_active, subscription_start_date = @subscription_start_date,
                                           subscription_end_date = @subscription_end_date where subscription_id = @subscription_id and vehicle_id=@vehicle_id and is_active = false";

                                vinActivated = await dataAccess.ExecuteAsync(queryUpdate, parameterStatus);
                                if (vinActivated > 0)
                                {
                                }
                            }
                            //This will get us the list of vins exits on Vehicle Table or Not
                            if (vinActivated == 0)
                            {
                                objvinList.Add(vinexist);
                            }

                        }

                        string SubscriptionId = string.Empty;
                        SubscriptionId = Guid.NewGuid().ToString();
                        var parameter = new DynamicParameters();
                        parameter.Add("@organization_id", orgid);
                        parameter.Add("@subscription_id", SubscriptionId);
                        parameter.Add("@type", data.type);
                        parameter.Add("@package_code", objSubscription.packageId);
                        parameter.Add("@package_id", data.id);
                        parameter.Add("@subscription_start_date", objSubscription.StartDateTime);
                        parameter.Add("@subscription_end_date", null);
                        parameter.Add("@is_active", true);
                        parameter.Add("@is_zuora_package", true);
                        //This Condition to insert only if all the VIN exits in Vehicle Table
                        foreach (var item in objvinList)
                        {
                            parameter.Add("@vehicle_id", item);
                            string queryInsert = "insert into master.subscription(organization_id, subscription_id, type, package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, is_active,is_zuora_package) " +
                                         "values(@organization_id, @subscription_id, @type, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @is_active, @is_zuora_package) RETURNING id";

                            int subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                        }
                        objSubscriptionResponse.numberOfVehicles = objvinList.Count;
                        objSubscriptionResponse.orderId = SubscriptionId;
                    }
                }
                return objSubscriptionResponse;
            }
            catch (Exception ex)
            {
                log.Info("Subscribe Subscription method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(objSubscription));
                log.Error(ex.ToString());
                throw ex;
            }
        }

        public async Task<SubscriptionResponse> Unsubscribe(UnSubscription objUnSubscription)
        {
            log.Info("Unsubscribe Subscription method called in repository");
            try
            {
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();

                if (!string.IsNullOrEmpty(objUnSubscription.OrderID))// for Organization
                {
                    //To Get Organizationid by orgcode
                    int orgid = await GetOrganizationIdByCode(objUnSubscription.OrganizationID);
                    if (orgid == 0)
                    {
                        return null;
                    }
                    //To Get SubscriptionId and Type
                    var data = await SubscriptionIdExits(objUnSubscription.OrderID, orgid);
                    
                    if (data == null)
                    {
                        return null;
                    }
                    else if (data.id > 0 && data.type.ToLower() == "o")
                    {
                        //if package type is organization and has vins in the payload return bad request
                        if (objUnSubscription.VINs != null && objUnSubscription.VINs.Count > 0)
                        {
                            return null;
                        }
                        var parameter = new DynamicParameters();
                        parameter.Add("@subscription_id", objUnSubscription.OrderID);
                        parameter.Add("@subscription_end_date", objUnSubscription.EndDateTime);
                        parameter.Add("@organization_id", orgid);
                        string queryUpdate = @"update master.subscription set is_active=false, subscription_end_date=@subscription_end_date where subscription_id=@subscription_id and organization_id=@organization_id";
                        int roweffected = await dataAccess.ExecuteAsync
                            (queryUpdate, parameter);
                        objSubscriptionResponse.orderId = objUnSubscription.OrderID;
                        return objSubscriptionResponse;
                    }
                
                //if package type is vehicle then only check vehicle table
                else if (data.id > 0 && data.type.ToLower() == "v")
                {
                    if (objUnSubscription.VINs == null || objUnSubscription.VINs.Count == 0)
                    {
                        return null;
                    }
                    if (objUnSubscription.VINs.Count > 0)
                    {
                        ArrayList objvinList = new ArrayList();
                        foreach (var item in objUnSubscription.VINs)
                        {
                            
                            var parameterToGetVehicleId = new DynamicParameters();
                            parameterToGetVehicleId.Add("@vin", item);
                                parameterToGetVehicleId.Add("@subscription_id", objUnSubscription.OrderID);
                                parameterToGetVehicleId.Add("@organization_id", orgid);
                                string query = @"SELECT sub.id,sub.is_active FROM master.subscription sub 
		                                            left JOIN master.vehicle veh ON sub.vehicle_id = veh.id
		                                            WHERE veh.vin= @vin
                                             AND  sub.subscription_id = @subscription_id
                                             AND  sub.organization_id = @organization_id";
                            var vinExist = await dataAccess.QueryFirstOrDefaultAsync<UnSubscribeVin>
                                (query, parameterToGetVehicleId);
                            if (vinExist == null || vinExist.id == 0 || !vinExist.is_active)
                            {//return Bad Request if vin does not exits and is_active is already false in Subscription table
                                return null;
                            }
                           
                            //This will get us the list of vins exits on Vehicle Table or Not
                            objvinList.Add(vinExist.id);
                        }
                        var parameter = new DynamicParameters();
                        parameter.Add("@subscription_end_date", objUnSubscription.EndDateTime);
                        parameter.Add("@is_active", false); 
                            //This Condition to insert only if all the VIN exits in Vehicle Table
                            foreach (var item in objvinList)
                        {
                            parameter.Add("@id", item);
                 string queryInsert = "update master.subscription set is_active=@is_active,subscription_end_date=@subscription_end_date where id=@id";

                            int rowEffected = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                        }
                        objSubscriptionResponse.numberOfVehicles = objvinList.Count;
                    }
                }
                }
                return objSubscriptionResponse;
            }
            catch (Exception ex)
            {
                log.Info("Subscribe Subscription method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(objUnSubscription));
                log.Error(ex.ToString());
                throw ex;
            }
        }

        public async Task<SubscriptionResponse> Create(int orgId,int packageId)
        {
            try
            {
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                //To check if subscription_id exists and to get status
                var response = await SubscriptionExits(orgId, packageId);
                if (response != null && !string.IsNullOrEmpty(response.subscription_id) && response.is_active)
                {
                    objSubscriptionResponse.orderId = response.subscription_id;
                    return objSubscriptionResponse;
                }
                else if (response != null && !string.IsNullOrEmpty(response.subscription_id) && !response.is_active)
                {
                    var parameterStatus = new DynamicParameters();
                    parameterStatus.Add("@is_active", true);
                    parameterStatus.Add("@subscription_id", response.subscription_id);
                    parameterStatus.Add("@subscription_start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameterStatus.Add("@subscription_end_date", null);
                    parameterStatus.Add("@is_zuora_package", false);
                    string queryUpdate = @"update master.subscription set is_active = @is_active
                                           ,subscription_start_date = @subscription_start_date
                                           ,subscription_end_date = @subscription_end_date
                                           ,is_zuora_package = @is_zuora_package
                                     where subscription_id = @subscription_id and is_active = false";

                    int updated = await dataAccess.ExecuteAsync(queryUpdate, parameterStatus);
                    if (updated > 0)
                    {
                        objSubscriptionResponse.orderId = response.subscription_id;
                        return objSubscriptionResponse;
                    }
                }
                Package objPackage = await GetPackageTypeById(packageId);
                string SubscriptionId = string.Empty;
                SubscriptionId = Guid.NewGuid().ToString();
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", orgId);
                parameter.Add("@subscription_id", SubscriptionId);
                parameter.Add("@type", objPackage.type == null ? null : objPackage.type);
                parameter.Add("@package_code", objPackage.packagecode == null ? null : objPackage.packagecode);
                parameter.Add("@package_id", packageId);
                parameter.Add("@vehicle_id", null);
                parameter.Add("@subscription_start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@subscription_end_date", null);
                parameter.Add("@is_active", true);
                parameter.Add("@is_zuora_package", false);
                
                string queryInsert = "insert into master.subscription(organization_id, subscription_id,type , package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, is_active,is_zuora_package) " +
                             "values(@organization_id, @subscription_id,@type, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @is_active,@is_zuora_package) RETURNING id";

                int subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                if (subid > 0)
                {
                    objSubscriptionResponse.orderId = SubscriptionId;
                }
                return objSubscriptionResponse;

            }
            catch (Exception ex)
            {
                log.Info("Create Subscription by OrganizationId method in repository failed with OrganizationId" + orgId);
                log.Error(ex.ToString());
                throw ex;
            }
        }

        public async Task<List<SubscriptionDetails>> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest)
        {
            try
            {
                string query = string.Empty;
                List<SubscriptionDetails> objsubscriptionDetails = new List<SubscriptionDetails>();
               
                    query = @"SELECT sub.subscription_id,org.name as orgname,sub.package_code,pak.name,sub.type,COUNT(veh.name),
                                     sub.subscription_start_date,sub.subscription_end_date,sub.is_active,sub.organization_id
                                     FROM master.Subscription sub 
                              JOIN master.package pak on sub.package_id = pak.id 
                              LEFT JOIN master.vehicle veh on sub.vehicle_id = veh.id
                              LEFT JOIN master.organization org on sub.organization_id = org.id
                              GROUP BY sub.subscription_id,org.name,sub.package_code,pak.name,sub.type,
                                     sub.subscription_start_date,sub.subscription_end_date,sub.is_active,sub.organization_id
                              HAVING 1=1";

                    var parameter = new DynamicParameters();

                    if (objSubscriptionDetailsRequest != null)
                    {
                        
                        if (objSubscriptionDetailsRequest.organization_id > 0)
                        {
                            parameter.Add("@organization_id", objSubscriptionDetailsRequest.organization_id);
                            query =   $"{query} and sub.organization_id=@organization_id ";

                        if (!string.IsNullOrEmpty(objSubscriptionDetailsRequest.type))
                        {
                            parameter.Add("@type", objSubscriptionDetailsRequest.type.ToUpper());
                            query = $"{query} and sub.type=@type";
                        }

                        else if (objSubscriptionDetailsRequest.is_active > 0)
                        {
                            parameter.Add("@is_active", objSubscriptionDetailsRequest.is_active == StatusType.True ? true : false);
                            query = $"{query} and sub.is_active=@is_active";
                        }
                    }
                        
                        
                    }
                    var data = await dataAccess.QueryAsync<SubscriptionDetails>(query, parameter);
                if (data == null)
                {
                    return null;
                }
                    objsubscriptionDetails = data.ToList();
                
                return objsubscriptionDetails;
            }
            catch (Exception ex)
            {
                log.Info("Subscribe Get method in repository failed ");
                log.Error(ex.ToString());
                throw ex;
            }
        }
    }
}
