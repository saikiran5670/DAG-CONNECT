
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.subscription.entity;
using net.atos.daf.ct2.utilities;
using System.Net;

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
        public async Task<Package> GetPackageTypeByCode(string PackageCode)
        {
            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@packagecode", PackageCode);
            parameterToGetPackageId.Add("@state", "A");
            var data = await dataAccess.QueryFirstOrDefaultAsync<Package>
                             (@"select id, type, packagecode from master.package where packagecode =@packagecode and state =@state",
                            parameterToGetPackageId);
            return data;
        }

        //forgeting package type By id
        async Task<Package> GetPackageTypeById(int packageId)
        {
            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@id", packageId);
            parameterToGetPackageId.Add("@state", "A");
            var data = await dataAccess.QueryFirstOrDefaultAsync<Package>
                             (@"select id, type,packagecode from master.package where id =@id and state =@state",
                            parameterToGetPackageId);
            return data;
        }

        //forgeting package type and id
        async Task<int> GetOrganizationIdByCode(string OrganizationCode)
        {

            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@org_id", OrganizationCode);
            parameterToGetPackageId.Add("@state", "A");
            string query = @"select id from master.organization where org_id=@org_id and state = @state";
            var data = await dataAccess.ExecuteScalarAsync<int>
                             (query, parameterToGetPackageId);
            return data;
        }

        //to check Subscription Id exits or not
        async Task<IEnumerable<subscriptionIdType>> SubscriptionIdExits(string orderId, int orgId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@subscription_id", Convert.ToInt64(orderId));
            parameterToGetSubscribeId.Add("@organization_id", orgId);
            var data = await dataAccess.QueryAsync<subscriptionIdType>
                             (@"SELECT id, type, state as State FROM master.subscription 
                               where subscription_id =@subscription_id
                               AND organization_id = @organization_id",
                            parameterToGetSubscribeId);
            return data;
        }

        //To check if Organization is already inserted
        async Task<subscriptionIdStatus> SubscriptionExits(int orgId, int packageId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@organization_Id", orgId);
            parameterToGetSubscribeId.Add("@package_id", packageId);
            var data = await dataAccess.QueryFirstOrDefaultAsync<subscriptionIdStatus>
                             (@"select subscription_id, state from master.subscription where organization_Id =@organization_Id and package_id=@package_id",
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
            var data = await dataAccess.QueryFirstOrDefaultAsync<subscriptionIdStatus>
                             (@"select subscription_id, state from master.subscription where organization_Id =@organization_Id and package_id=@package_id and vehicle_id=@vehicle_id",
                            parameterToGetSubscribeId);
            return data;
        }

        async Task<long> GetTopOrderId()
        {
            var data = await dataAccess.ExecuteScalarAsync<long>(@"SELECT max(subscription_id) from master.subscription");
            return ++data;
        }
        public async Task<Tuple<HttpStatusCode, SubscriptionResponse>> Subscribe(SubscriptionActivation objSubscription)
        {
            log.Info("Subscribe Subscription method called in repository");
            try
            {
                // Get package Id and type to insert in Subscription Table
                var packageDetails = await GetPackageTypeByCode(objSubscription.packageId);
                //Get Organization id to insert in subscription Table
                int orgid = await GetOrganizationIdByCode(objSubscription.OrganizationId);
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                if (orgid == 0)
                {
                    return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("ORGANIZATION_NOT_FOUND", objSubscription.OrganizationId));
                }
                if (packageDetails == null)
                {
                    return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("PACKAGE_NOT_FOUND", objSubscription.packageId));
                }
                else if (packageDetails.id > 0 && packageDetails.type.ToLower() == "o")
                {
                    //if package type is organization and has vins in the payload return bad request
                    if (objSubscription.VINs != null && objSubscription.VINs.Count > 0)
                    {
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INCORRECT_PACKAGE_TYPE", objSubscription.packageId));
                    }

                    long SubscriptionId;
                    //return subscriptionid and state from subscription table
                    var response = await SubscriptionExits(orgid, packageDetails.id);
                    if (response != null && response.subscription_id != 0 && response.state.ToUpper() == "A")
                    {// Subscription exists and Active
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INVALID_REQUEST", objSubscription.packageId));
                    }
                    else if (response != null && response.subscription_id != 0 && response.state.ToUpper() == "I")
                    {// Subscription exists and InActive
                        var parameterStatus = new DynamicParameters();
                        parameterStatus.Add("@state", "A");
                        parameterStatus.Add("@subscription_id", response.subscription_id);
                        parameterStatus.Add("@subscription_start_date", objSubscription.StartDateTime);
                        parameterStatus.Add("@subscription_end_date", null);
                        string queryUpdate = @"update master.subscription set state = @state, subscription_start_date = @subscription_start_date,
                                           subscription_end_date = @subscription_end_date where subscription_id = @subscription_id ";

                        int updated = await dataAccess.ExecuteAsync(queryUpdate, parameterStatus);
                        if (updated > 0)
                        {
                            objSubscriptionResponse.Response.orderId = response.subscription_id.ToString();
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, objSubscriptionResponse);
                        }
                    }
                    else if (response == null || response.subscription_id == 0)
                    {//Subscription does'nt exists for this scenerio
                        SubscriptionId = await GetTopOrderId();
                        var parameter = new DynamicParameters();
                        parameter.Add("@organization_id", orgid);
                        parameter.Add("@subscription_id", SubscriptionId);
                        parameter.Add("@package_code", objSubscription.packageId);
                        parameter.Add("@package_id", packageDetails.id);
                        parameter.Add("@type", packageDetails.type);
                        parameter.Add("@subscription_start_date", objSubscription.StartDateTime);
                        parameter.Add("@subscription_end_date", null);
                        parameter.Add("@vehicle_id", null);
                        parameter.Add("@state", "A");
                        parameter.Add("@is_zuora_package", true);
                        string queryInsert = @"insert into master.subscription(organization_id, subscription_id, type, package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, state,is_zuora_package)  
                                     values(@organization_id, @subscription_id, @type, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @state,@is_zuora_package) RETURNING id";

                        long subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                        if (subid > 0)
                            objSubscriptionResponse.Response.orderId = SubscriptionId.ToString();
                    }

                }
                //if package type is vehicle then only check vehicle table
                else if (packageDetails.id > 0 && packageDetails.type.ToLower() == "v")
                {
                    if (objSubscription.VINs == null || objSubscription.VINs.Count == 0)
                    {
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("MISSING_PARAMETER", nameof(objSubscription.VINs)));
                    }
                    if (objSubscription.VINs.Count > 0)
                    {
                        //Get all vehicle Ids corrosponding to VINs and match count with VINs 
                        //for checking VINs outside the provided organization
                        var vehicleIds = await CheckVINsExistInOrg(objSubscription.VINs, orgid);
                        if (objSubscription.VINs.Count != vehicleIds.Count)
                        {
                            var values = objSubscription.VINs.ToArray().Except(vehicleIds.Keys.ToArray()).ToArray();
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("VIN_NOT_FOUND", values));
                        }                            

                        ArrayList objvinList = new ArrayList(); int count = 0;
                        foreach (var vin in objSubscription.VINs)
                        {
                            int vinActivated = 0;
                            var response = await SubscriptionExits(orgid, packageDetails.id, vehicleIds[vin]);
                            if (response != null && response.subscription_id != 0 && response.state.ToUpper() == "A")
                            {
                                return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INVALID_REQUEST", objSubscription.packageId));
                            }
                            else if (response != null && response.subscription_id != 0 && response.state.ToUpper() == "I")
                            {
                                // Subscription exists and InActive
                                var parameterStatus = new DynamicParameters();
                                parameterStatus.Add("@state", "A");
                                parameterStatus.Add("@subscription_id", response.subscription_id);
                                parameterStatus.Add("@subscription_start_date", objSubscription.StartDateTime);
                                parameterStatus.Add("@subscription_end_date", null);
                                parameterStatus.Add("@vehicle_id", vehicleIds[vin]);
                                string queryUpdate = @"update master.subscription set state = @state, subscription_start_date = @subscription_start_date,
                                           subscription_end_date = @subscription_end_date where subscription_id = @subscription_id and vehicle_id=@vehicle_id";

                                vinActivated = await dataAccess.ExecuteAsync(queryUpdate, parameterStatus);
                                if (vinActivated > 0)
                                {
                                    count = ++count;
                                }
                            }
                            //This will get us the list of vins exits on Vehicle Table or Not
                            if (vinActivated == 0)
                            {
                                objvinList.Add(vehicleIds[vin]);
                            }
                            if (response != null && response.subscription_id > 0)
                                objSubscriptionResponse.Response.orderId = response.subscription_id.ToString();
                        }

                        long SubscriptionId = 0;
                        if (string.IsNullOrEmpty(objSubscriptionResponse.Response.orderId) == true)
                        {
                            SubscriptionId = await GetTopOrderId();
                        }
                        else
                        {
                            SubscriptionId = Convert.ToInt64(objSubscriptionResponse.Response.orderId);
                        }
                        var parameter = new DynamicParameters();
                        parameter.Add("@organization_id", orgid);
                        parameter.Add("@subscription_id", SubscriptionId);
                        parameter.Add("@type", packageDetails.type);
                        parameter.Add("@package_code", objSubscription.packageId);
                        parameter.Add("@package_id", packageDetails.id);
                        parameter.Add("@subscription_start_date", objSubscription.StartDateTime);
                        parameter.Add("@subscription_end_date", null);
                        parameter.Add("@state", "A");
                        parameter.Add("@is_zuora_package", true);
                        //This Condition to insert only if all the VIN exits in Vehicle Table
                        foreach (var item in objvinList)
                        {
                            parameter.Add("@vehicle_id", item);
                            string queryInsert = "insert into master.subscription(organization_id, subscription_id, type, package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, state,is_zuora_package) " +
                                         "values(@organization_id, @subscription_id, @type, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @state, @is_zuora_package) RETURNING id";

                            int subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                            count = ++count;
                        }
                        objSubscriptionResponse.Response.numberOfVehicles = count;
                        objSubscriptionResponse.Response.orderId = SubscriptionId.ToString();
                    }
                }
                return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, objSubscriptionResponse);
            }
            catch (Exception ex)
            {
                log.Info("Subscribe Subscription method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(objSubscription));
                log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<Tuple<HttpStatusCode, SubscriptionResponse>> Unsubscribe(UnSubscription objUnSubscription)
        {
            log.Info("Unsubscribe Subscription method called in repository");
            try
            {
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();

                if (!string.IsNullOrEmpty(objUnSubscription.OrganizationID) && Convert.ToInt64(objUnSubscription.OrderID) != 0)// for Organization
                {
                    //To Get Organizationid by orgcode
                    int orgid = await GetOrganizationIdByCode(objUnSubscription.OrganizationID);
                    if (orgid == 0)
                    {
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("ORGANIZATION_NOT_FOUND", objUnSubscription.OrganizationID));
                    }
                    //To Get SubscriptionId and Type
                    var subscriptionDetails = await SubscriptionIdExits(objUnSubscription.OrderID, orgid);

                    if (subscriptionDetails.Count() == 0)
                    {
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("ORDER_NOT_FOUND", objUnSubscription.OrderID));
                    }
                    else if (subscriptionDetails.First().type.ToLower() == "o")
                    {
                        //For type "o", there is only one record to check for unsubscription
                        if (subscriptionDetails.First().State.Equals('I'))
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INVALID_REQUEST", objUnSubscription.OrderID));

                        //if package type is organization and has vins in the payload return bad request
                        if (objUnSubscription.VINs != null && objUnSubscription.VINs.Count > 0)
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INCORRECT_ORDER_PACKAGE_TYPE", objUnSubscription.OrderID));
                        
                        var parameter = new DynamicParameters();
                        parameter.Add("@id", subscriptionDetails.First().id);
                        parameter.Add("@subscription_end_date", objUnSubscription.EndDateTime);
                        parameter.Add("@organization_id", orgid);
                        parameter.Add("@state", "I");
                        string queryUpdate = @"update master.subscription set state=@state, subscription_end_date=@subscription_end_date where id=@id and organization_id=@organization_id";
                        int roweffected = await dataAccess.ExecuteAsync
                            (queryUpdate, parameter);
                        objSubscriptionResponse.Response.orderId = objUnSubscription.OrderID.ToString();
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, objSubscriptionResponse);
                    }

                    //if package type is vehicle then only check vehicle table
                    else if (subscriptionDetails.First().type.ToLower() == "v")
                    {
                        if (objUnSubscription.VINs == null || objUnSubscription.VINs.Count == 0)
                        {
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("MISSING_PARAMETER", nameof(objUnSubscription.VINs)));
                        }
                        if (objUnSubscription.VINs.Count > 0)
                        {
                            //Get all vehicle Ids corrosponding to VINs and match count with VINs 
                            //for checking VINs outside the provided organization
                            var result = await CheckVINsExistInSubscription(objUnSubscription.VINs, objUnSubscription.OrderID, orgid);
                            var nonExistVins = result.Item1;
                            var subscriptionIds = result.Item2;

                            //VINs not found in subscription table
                            if(nonExistVins.Count > 0)
                                return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("VIN_NOT_FOUND", nonExistVins.ToArray()));

                            //Check if order is fully unsubscribed
                            if (result.Item3)
                                return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INVALID_REQUEST", objUnSubscription.OrderID));

                            //Few are subscribed and few are already unsubscribed
                            if (objUnSubscription.VINs.Count != subscriptionIds.Count)
                            {
                                var values = objUnSubscription.VINs.ToArray().Except(subscriptionIds.Keys.ToArray()).ToArray();
                                return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("INVALID_REQUEST", values));
                            }

                            var parameter = new DynamicParameters();
                            parameter.Add("@subscription_end_date", objUnSubscription.EndDateTime);
                            parameter.Add("@state", "I");
                            //This Condition to insert only if all the VIN exits in Vehicle Table
                            foreach (var subscriptionId in subscriptionIds.Values)
                            {
                                parameter.Add("@id", subscriptionId);
                                string queryInsert = "update master.subscription set state=@state,subscription_end_date=@subscription_end_date where id=@id";

                                int rowEffected = await dataAccess.ExecuteAsync(queryInsert, parameter);
                            }
                            objSubscriptionResponse.Response.orderId = objUnSubscription.OrderID.ToString();
                            objSubscriptionResponse.Response.numberOfVehicles = subscriptionIds.Count;
                        }
                    }
                }
                return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, objSubscriptionResponse);
            }
            catch (Exception ex)
            {
                log.Info("Subscribe Subscription method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(objUnSubscription));
                log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<SubscriptionResponse> Create(int orgId, int packageId)
        {
            try
            {
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                //To check if subscription_id exists and to get status
                var response = await SubscriptionExits(orgId, packageId);
                if (response != null && response.subscription_id != 0 && response.state.ToUpper() == "A")
                {
                    objSubscriptionResponse.Response.orderId = response.subscription_id.ToString();
                    return objSubscriptionResponse;
                }
                else if (response != null && response.subscription_id != 0 && response.state.ToUpper() == "I")
                {
                    var parameterStatus = new DynamicParameters();
                    parameterStatus.Add("@state", "A");
                    parameterStatus.Add("@subscription_id", response.subscription_id);
                    parameterStatus.Add("@subscription_start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameterStatus.Add("@subscription_end_date", null);
                    parameterStatus.Add("@is_zuora_package", false);
                    string queryUpdate = @"update master.subscription set state = @state
                                           ,subscription_start_date = @subscription_start_date
                                           ,subscription_end_date = @subscription_end_date
                                           ,is_zuora_package = @is_zuora_package
                                     where subscription_id = @subscription_id";

                    int updated = await dataAccess.ExecuteAsync(queryUpdate, parameterStatus);
                    if (updated > 0)
                    {
                        objSubscriptionResponse.Response.orderId = response.subscription_id.ToString();
                        return objSubscriptionResponse;
                    }
                }
                Package objPackage = await GetPackageTypeById(packageId);
                if (objPackage == null)
                {
                    return null;
                }
                long SubscriptionId = 0;
                SubscriptionId = await GetTopOrderId();
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", orgId);
                parameter.Add("@subscription_id", SubscriptionId);
                parameter.Add("@type", objPackage.type == null ? null : objPackage.type);
                parameter.Add("@package_code", objPackage.packagecode == null ? null : objPackage.packagecode);
                parameter.Add("@package_id", packageId);
                parameter.Add("@vehicle_id", null);
                parameter.Add("@subscription_start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@subscription_end_date", null);
                parameter.Add("@state", "A");
                parameter.Add("@is_zuora_package", false);

                string queryInsert = @"insert into master.subscription(organization_id, subscription_id,type , package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, state, is_zuora_package)
                             values(@organization_id, @subscription_id,@type, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @state, @is_zuora_package) RETURNING id";

                int subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                if (subid > 0)
                {
                    objSubscriptionResponse.Response.orderId = SubscriptionId.ToString();
                }
                return objSubscriptionResponse;

            }
            catch (Exception ex)
            {
                log.Info("Create Subscription by OrganizationId method in repository failed with OrganizationId" + orgId);
                log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<List<SubscriptionDetails>> Get(SubscriptionDetailsRequest objSubscriptionDetailsRequest)
        {
            try
            {
                string query = string.Empty;
                List<SubscriptionDetails> objsubscriptionDetails = new List<SubscriptionDetails>();

                query = @"SELECT sub.subscription_id,org.name as orgname,sub.package_code,pak.name,sub.type,COUNT(veh.id),
                                 sub.subscription_start_date,sub.subscription_end_date,sub.state,sub.organization_id
                            FROM master.Subscription sub
                            Inner JOIN master.organization org on sub.organization_id = org.id
                            Inner JOIN master.package pak on sub.package_id = pak.id
                            Left JOIN master.vehicle veh on sub.vehicle_id = veh.id
                            GROUP BY sub.subscription_id,org.name,sub.package_code,pak.name,sub.type,
                                 sub.subscription_start_date,sub.subscription_end_date,sub.state,sub.organization_id
                            HAVING 1=1";

                var parameter = new DynamicParameters();

                if (objSubscriptionDetailsRequest != null)
                {

                    if (objSubscriptionDetailsRequest.organization_id > 0)
                    {
                        parameter.Add("@organization_id", objSubscriptionDetailsRequest.organization_id);
                        query = $"{query} and sub.organization_id=@organization_id ";

                        if (!string.IsNullOrEmpty(objSubscriptionDetailsRequest.type))
                        {
                            parameter.Add("@type", objSubscriptionDetailsRequest.type.ToUpper());
                            query = $"{query} and sub.type=@type";
                        }

                        else if (objSubscriptionDetailsRequest.state > 0)
                        {
                            parameter.Add("@state", objSubscriptionDetailsRequest.state == StatusType.A ? true : false);
                            query = $"{query} and sub.state=@state";
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
                throw;
            }
        }

        private async Task<Dictionary<string, int>> CheckVINsExistInOrg(List<string> VINs, int orgId)
        {
            int vehicleId;
            DynamicParameters parameters;
            Dictionary<string, int> vehicleIds = new Dictionary<string, int>();
            foreach (var item in VINs)
            {
                vehicleId = 0;
                parameters = new DynamicParameters();
                parameters.Add("@vin", item);
                parameters.Add("@orgId", orgId);
                vehicleId = await dataAccess.ExecuteScalarAsync<int>
                    (@"SELECT id FROM master.vehicle where vin=@vin and organization_id=@orgId", parameters);
                if (vehicleId > 0)
                {
                    if(!vehicleIds.ContainsKey(item)) 
                        vehicleIds.Add(item, vehicleId);
                }
            }
            return vehicleIds;
        }

        private async Task<Tuple<List<string>, Dictionary<string, int>, bool>> CheckVINsExistInSubscription(List<string> VINs, string orderId, int orgId)
        {
            Dictionary<string, int> subscriptionIds = new Dictionary<string, int>();
            List<string> nonExistVins = new List<string>();
            int counter = VINs.Count;
            foreach (var item in VINs)
            {
                var parameterToGetVehicleId = new DynamicParameters();
                parameterToGetVehicleId.Add("@vin", item);
                parameterToGetVehicleId.Add("@subscription_id", Convert.ToInt64(orderId));
                parameterToGetVehicleId.Add("@organization_id", orgId);
                string query = @"SELECT sub.id,sub.state FROM master.subscription sub 
		                                            left JOIN master.vehicle veh ON sub.vehicle_id = veh.id
		                                            WHERE veh.vin= @vin
                                             AND  sub.subscription_id = @subscription_id
                                             AND  sub.organization_id = @organization_id";
                var vinExist = await dataAccess.QueryFirstOrDefaultAsync<UnSubscribeVin>(query, parameterToGetVehicleId);
                if (vinExist == null)
                    nonExistVins.Add(item);

                if (vinExist!= null && vinExist.id > 0 && vinExist.state.ToUpper() == "A")
                {
                    if (!subscriptionIds.ContainsKey(item))
                        subscriptionIds.Add(item, vinExist.id);
                }

                if (vinExist != null && vinExist.state.ToUpper() == "I")
                    counter--;
            }

            if (counter == 0)
                return Tuple.Create(nonExistVins, subscriptionIds, true);

            return Tuple.Create(nonExistVins, subscriptionIds, false);
        }
    }
}
