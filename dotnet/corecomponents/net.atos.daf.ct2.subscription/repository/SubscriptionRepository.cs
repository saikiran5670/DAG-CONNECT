﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.subscription.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.subscription.repository
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IDataAccess _dataAccess;

        private static readonly log4net.ILog _log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SubscriptionRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        //forgeting package type and id
        public async Task<Package> GetPackageTypeByCode(string packageCode)
        {
            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@packagecode", packageCode);
            parameterToGetPackageId.Add("@state", "A");
            var data = await _dataAccess.QueryFirstOrDefaultAsync<Package>
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
            var data = await _dataAccess.QueryFirstOrDefaultAsync<Package>
                             (@"select id, type,packagecode from master.package where id =@id and state =@state",
                            parameterToGetPackageId);
            return data;
        }

        //forgeting package type and id
        public async Task<int> GetOrganizationIdByCode(string organizationCode)
        {

            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@org_id", organizationCode);
            parameterToGetPackageId.Add("@state", "A");
            string query = @"select id from master.organization where org_id=@org_id and state = @state";
            var data = await _dataAccess.ExecuteScalarAsync<int>
                             (query, parameterToGetPackageId);
            return data;
        }

        //to check Subscription Id exits or not
        async Task<IEnumerable<SubscriptionIdType>> SubscriptionIdExits(string orderId, int orgId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@subscription_id", Convert.ToInt64(orderId));
            parameterToGetSubscribeId.Add("@organization_id", orgId);
            var data = await _dataAccess.QueryAsync<SubscriptionIdType>
                             (@"SELECT id, type, state as State FROM master.subscription 
                               where subscription_id =@subscription_id
                               AND organization_id = @organization_id",
                            parameterToGetSubscribeId);
            return data;
        }

        //To check if Organization is already inserted
        async Task<SubscriptionIdStatus> SubscriptionExits(int orgId, int packageId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@organization_Id", orgId);
            parameterToGetSubscribeId.Add("@package_id", packageId);
            var data = await _dataAccess.QueryFirstOrDefaultAsync<SubscriptionIdStatus>
                             (@"select subscription_id, state from master.subscription where organization_Id =@organization_Id and package_id=@package_id",
                            parameterToGetSubscribeId);
            return data;
        }

        //To check if Subscription exits with orgid, packid and vinid is already inserted
        async Task<SubscriptionIdStatus> SubscriptionExits(int orgId, int packageId, int vinId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@organization_Id", orgId);
            parameterToGetSubscribeId.Add("@package_id", packageId);
            parameterToGetSubscribeId.Add("@vehicle_id", vinId);
            var data = await _dataAccess.QueryFirstOrDefaultAsync<SubscriptionIdStatus>
                             (@"select subscription_id, state from master.subscription where organization_Id =@organization_Id and package_id=@package_id and vehicle_id=@vehicle_id",
                            parameterToGetSubscribeId);
            return data;
        }

        async Task<long> GetTopOrderId()
        {
            var data = await _dataAccess.ExecuteScalarAsync<long>(@"SELECT max(subscription_id) from master.subscription");
            return ++data;
        }
        public async Task<Tuple<HttpStatusCode, SubscriptionResponse>> Subscribe(SubscriptionActivation objSubscription, IEnumerable<string> visibleVINs)
        {
            _log.Info("Subscribe Subscription method called in repository");
            try
            {
                // Get package Id and type to insert in Subscription Table
                var packageDetails = await GetPackageTypeByCode(objSubscription.PackageId);
                //Get Organization id to insert in subscription Table
                int orgid = await GetOrganizationIdByCode(objSubscription.OrganizationId);
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                if (orgid == 0)
                {
                    return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("ORGANIZATION_NOT_FOUND", objSubscription.OrganizationId));
                }
                if (packageDetails == null)
                {
                    return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("PACKAGE_NOT_FOUND", objSubscription.PackageId));
                }
                else if (packageDetails.Id > 0 && packageDetails.Type.ToLower() == "o")
                {
                    //if package type is organization and has vins in the payload return bad request
                    if (objSubscription.VINs != null && objSubscription.VINs.Count > 0)
                    {
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INCORRECT_PACKAGE_TYPE", objSubscription.PackageId));
                    }

                    long subscriptionId;
                    //return subscriptionid and state from subscription table
                    var response = await SubscriptionExits(orgid, packageDetails.Id);
                    if (response != null && response.Subscription_Id != 0 && response.State.ToUpper() == "A")
                    {// Subscription exists and Active
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INVALID_REQUEST", objSubscription.PackageId));
                    }
                    else if (response != null && response.Subscription_Id != 0 && response.State.ToUpper() == "I")
                    {// Subscription exists and InActive
                        var parameterStatus = new DynamicParameters();
                        parameterStatus.Add("@state", "A");
                        parameterStatus.Add("@subscription_id", response.Subscription_Id);
                        parameterStatus.Add("@subscription_start_date", objSubscription.StartDateTime);
                        parameterStatus.Add("@subscription_end_date", null);
                        string queryUpdate = @"update master.subscription set state = @state, subscription_start_date = @subscription_start_date,
                                           subscription_end_date = @subscription_end_date where subscription_id = @subscription_id ";

                        int updated = await _dataAccess.ExecuteAsync(queryUpdate, parameterStatus);
                        if (updated > 0)
                        {
                            objSubscriptionResponse.Response.OrderId = response.Subscription_Id.ToString();
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, objSubscriptionResponse);
                        }
                    }
                    else if (response == null || response.Subscription_Id == 0)
                    {//Subscription does'nt exists for this scenerio
                        subscriptionId = await GetTopOrderId();
                        var parameter = new DynamicParameters();
                        parameter.Add("@organization_id", orgid);
                        parameter.Add("@subscription_id", subscriptionId);
                        parameter.Add("@package_code", objSubscription.PackageId);
                        parameter.Add("@package_id", packageDetails.Id);
                        parameter.Add("@type", packageDetails.Type);
                        parameter.Add("@subscription_start_date", objSubscription.StartDateTime);
                        parameter.Add("@subscription_end_date", null);
                        parameter.Add("@vehicle_id", null);
                        parameter.Add("@state", "A");
                        parameter.Add("@is_zuora_package", true);
                        string queryInsert = @"insert into master.subscription(organization_id, subscription_id, type, package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, state,is_zuora_package)  
                                     values(@organization_id, @subscription_id, @type, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @state,@is_zuora_package) RETURNING id";

                        long subid = await _dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                        if (subid > 0)
                            objSubscriptionResponse.Response.OrderId = subscriptionId.ToString();
                    }

                }
                //if package type is vehicle then only check vehicle table
                else if (packageDetails.Id > 0 && packageDetails.Type.ToLower() == "v")
                {
                    // Keep Owned VINs in objSubscription.VINs list
                    objSubscription.VINs = objSubscription.VINs.Except(visibleVINs).ToList();

                    if (objSubscription.VINs.Count == 0 && visibleVINs.Count() == 0)
                    {
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("MISSING_PARAMETER", nameof(objSubscription.VINs)));
                    }

                    Dictionary<string, int> vehicleIds = new Dictionary<string, int>();
                    if (objSubscription.VINs.Count > 0)
                    {
                        //Get all vehicle Ids corrosponding to VINs and match count with VINs 
                        //for checking VINs outside the provided organization
                        vehicleIds = await CheckVINsExistInOrg(objSubscription.VINs, orgid);
                        if (objSubscription.VINs.Count != vehicleIds.Count)
                        {
                            var values = objSubscription.VINs.ToArray().Except(vehicleIds.Keys.ToArray()).ToArray();
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("VIN_NOT_FOUND", values));
                        }
                    }

                    if (visibleVINs.Count() > 0)
                    {
                        //Get all vehicle Ids corrosponding to VINs and match count with VINs 
                        //for checking VINs outside the provided organization
                        var visible_vehicleIds = await CheckVINsExistInOrg(visibleVINs.ToList(), orgid, isVisible: true);
                        if (visibleVINs.Count() != visible_vehicleIds.Count)
                        {
                            var values = visibleVINs.ToArray().Except(visible_vehicleIds.Keys.ToArray()).ToArray();
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.NotFound, new SubscriptionResponse("VIN_NOT_FOUND", values));
                        }

                        foreach (var kv in visible_vehicleIds)
                        {
                            if (!vehicleIds.ContainsKey(kv.Key))
                                vehicleIds.Add(kv.Key, kv.Value);
                        }
                    }

                    ArrayList objvinList = new ArrayList(); int count = 0;
                    foreach (var vin in objSubscription.VINs.Concat(visibleVINs))
                    {
                        int vinActivated = 0;
                        var response = await SubscriptionExits(orgid, packageDetails.Id, vehicleIds[vin]);
                        if (response != null && response.Subscription_Id != 0 && response.State.ToUpper() == "A")
                        {
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INVALID_REQUEST", objSubscription.PackageId));
                        }
                        else if (response != null && response.Subscription_Id != 0 && response.State.ToUpper() == "I")
                        {
                            // Subscription exists and InActive
                            var parameterStatus = new DynamicParameters();
                            parameterStatus.Add("@state", "A");
                            parameterStatus.Add("@subscription_id", response.Subscription_Id);
                            parameterStatus.Add("@subscription_start_date", objSubscription.StartDateTime);
                            parameterStatus.Add("@subscription_end_date", null);
                            parameterStatus.Add("@vehicle_id", vehicleIds[vin]);
                            string queryUpdate = @"update master.subscription set state = @state, subscription_start_date = @subscription_start_date,
                                        subscription_end_date = @subscription_end_date where subscription_id = @subscription_id and vehicle_id=@vehicle_id";

                            vinActivated = await _dataAccess.ExecuteAsync(queryUpdate, parameterStatus);
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
                        if (response != null && response.Subscription_Id > 0)
                            objSubscriptionResponse.Response.OrderId = response.Subscription_Id.ToString();
                    }

                    long subscriptionId = 0;
                    if (string.IsNullOrEmpty(objSubscriptionResponse.Response.OrderId) == true)
                    {
                        subscriptionId = await GetTopOrderId();
                    }
                    else
                    {
                        subscriptionId = Convert.ToInt64(objSubscriptionResponse.Response.OrderId);
                    }
                    var parameter = new DynamicParameters();
                    parameter.Add("@organization_id", orgid);
                    parameter.Add("@subscription_id", subscriptionId);
                    parameter.Add("@type", packageDetails.Type);
                    parameter.Add("@package_code", objSubscription.PackageId);
                    parameter.Add("@package_id", packageDetails.Id);
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

                        int subid = await _dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                        count = ++count;
                    }
                    objSubscriptionResponse.Response.NumberOfVehicles = count;
                    objSubscriptionResponse.Response.OrderId = subscriptionId.ToString();
                }
                return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, objSubscriptionResponse);
            }
            catch (Exception ex)
            {
                _log.Info("Subscribe Subscription method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(objSubscription));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<Tuple<HttpStatusCode, SubscriptionResponse>> Unsubscribe(UnSubscription objUnSubscription)
        {
            _log.Info("Unsubscribe Subscription method called in repository");
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
                    else if (subscriptionDetails.First().Type.ToLower() == "o")
                    {
                        //For type "o", there is only one record to check for unsubscription
                        if (subscriptionDetails.First().State.Equals('I'))
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INVALID_REQUEST", objUnSubscription.OrderID));

                        //if package type is organization and has vins in the payload return bad request
                        if (objUnSubscription.VINs != null && objUnSubscription.VINs.Count > 0)
                            return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.BadRequest, new SubscriptionResponse("INCORRECT_ORDER_PACKAGE_TYPE", objUnSubscription.OrderID));

                        var parameter = new DynamicParameters();
                        parameter.Add("@id", subscriptionDetails.First().Id);
                        parameter.Add("@subscription_end_date", objUnSubscription.EndDateTime);
                        parameter.Add("@organization_id", orgid);
                        parameter.Add("@state", "I");
                        string queryUpdate = @"update master.subscription set state=@state, subscription_end_date=@subscription_end_date where id=@id and organization_id=@organization_id";
                        int roweffected = await _dataAccess.ExecuteAsync
                            (queryUpdate, parameter);
                        objSubscriptionResponse.Response.OrderId = objUnSubscription.OrderID.ToString();
                        return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, objSubscriptionResponse);
                    }

                    //if package type is vehicle then only check vehicle table
                    else if (subscriptionDetails.First().Type.ToLower() == "v")
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
                            if (nonExistVins.Count > 0)
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

                                int rowEffected = await _dataAccess.ExecuteAsync(queryInsert, parameter);
                            }
                            objSubscriptionResponse.Response.OrderId = objUnSubscription.OrderID.ToString();
                            objSubscriptionResponse.Response.NumberOfVehicles = subscriptionIds.Count;
                        }
                    }
                }
                return new Tuple<HttpStatusCode, SubscriptionResponse>(HttpStatusCode.OK, objSubscriptionResponse);
            }
            catch (Exception ex)
            {
                _log.Info("Subscribe Subscription method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(objUnSubscription));
                _log.Error(ex.ToString());
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
                if (response != null && response.Subscription_Id != 0 && response.State.ToUpper() == "A")
                {
                    objSubscriptionResponse.Response.OrderId = response.Subscription_Id.ToString();
                    return objSubscriptionResponse;
                }
                else if (response != null && response.Subscription_Id != 0 && response.State.ToUpper() == "I")
                {
                    var parameterStatus = new DynamicParameters();
                    parameterStatus.Add("@state", "A");
                    parameterStatus.Add("@subscription_id", response.Subscription_Id);
                    parameterStatus.Add("@subscription_start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                    parameterStatus.Add("@subscription_end_date", null);
                    parameterStatus.Add("@is_zuora_package", false);
                    string queryUpdate = @"update master.subscription set state = @state
                                           ,subscription_start_date = @subscription_start_date
                                           ,subscription_end_date = @subscription_end_date
                                           ,is_zuora_package = @is_zuora_package
                                     where subscription_id = @subscription_id";

                    int updated = await _dataAccess.ExecuteAsync(queryUpdate, parameterStatus);
                    if (updated > 0)
                    {
                        objSubscriptionResponse.Response.OrderId = response.Subscription_Id.ToString();
                        return objSubscriptionResponse;
                    }
                }
                Package objPackage = await GetPackageTypeById(packageId);
                if (objPackage == null)
                {
                    return null;
                }
                long subscriptionId = 0;
                subscriptionId = await GetTopOrderId();
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", orgId);
                parameter.Add("@subscription_id", subscriptionId);
                parameter.Add("@type", objPackage.Type ?? null);
                parameter.Add("@package_code", objPackage.PackageCode ?? null);
                parameter.Add("@package_id", packageId);
                parameter.Add("@vehicle_id", null);
                parameter.Add("@subscription_start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@subscription_end_date", null);
                parameter.Add("@state", "A");
                parameter.Add("@is_zuora_package", false);

                string queryInsert = @"insert into master.subscription(organization_id, subscription_id,type , package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, state, is_zuora_package)
                             values(@organization_id, @subscription_id,@type, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @state, @is_zuora_package) RETURNING id";

                int subid = await _dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                if (subid > 0)
                {
                    objSubscriptionResponse.Response.OrderId = subscriptionId.ToString();
                }
                return objSubscriptionResponse;

            }
            catch (Exception ex)
            {
                _log.Info("Create Subscription by OrganizationId method in repository failed with OrganizationId" + orgId);
                _log.Error(ex.ToString());
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

                    if (objSubscriptionDetailsRequest.Organization_Id > 0)
                    {
                        parameter.Add("@organization_id", objSubscriptionDetailsRequest.Organization_Id);
                        query = $"{query} and sub.organization_id=@organization_id ";

                        if (!string.IsNullOrEmpty(objSubscriptionDetailsRequest.Type))
                        {
                            parameter.Add("@type", objSubscriptionDetailsRequest.Type.ToUpper());
                            query = $"{query} and sub.type=@type";
                        }

                        else if (objSubscriptionDetailsRequest.State > 0)
                        {
                            parameter.Add("@state", objSubscriptionDetailsRequest.State == StatusType.A ? 'A' : 'I');
                            query = $"{query} and sub.state=@state";
                        }
                    }
                }
                var data = await _dataAccess.QueryAsync<SubscriptionDetails>(query, parameter);
                if (data == null)
                {
                    return null;
                }
                objsubscriptionDetails = data.ToList();

                return objsubscriptionDetails;
            }
            catch (Exception ex)
            {
                _log.Info("Subscribe Get method in repository failed ");
                _log.Error(ex.ToString());
                throw;
            }
        }

        private async Task<Dictionary<string, int>> CheckVINsExistInOrg(List<string> vins, int orgId, bool isVisible = false)
        {
            int vehicleId;
            DynamicParameters parameters;
            Dictionary<string, int> vehicleIds = new Dictionary<string, int>();

            var query = @"SELECT id FROM master.vehicle where vin=@vin";

            foreach (var item in vins)
            {
                vehicleId = 0;
                parameters = new DynamicParameters();
                parameters.Add("@vin", item);

                if (!isVisible)
                {
                    parameters.Add("@orgId", orgId);
                    query += " and organization_id=@orgId";
                }

                vehicleId = await _dataAccess.ExecuteScalarAsync<int>(query, parameters);
                if (vehicleId > 0)
                {
                    if (!vehicleIds.ContainsKey(item))
                        vehicleIds.Add(item, vehicleId);
                }
            }
            return vehicleIds;
        }

        private async Task<Tuple<List<string>, Dictionary<string, int>, bool>> CheckVINsExistInSubscription(List<string> vins, string orderId, int orgId)
        {
            Dictionary<string, int> subscriptionIds = new Dictionary<string, int>();
            List<string> nonExistVins = new List<string>();
            int counter = vins.Count;
            foreach (var item in vins)
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
                var vinExist = await _dataAccess.QueryFirstOrDefaultAsync<UnSubscribeVin>(query, parameterToGetVehicleId);
                if (vinExist == null)
                    nonExistVins.Add(item);

                if (vinExist != null && vinExist.Id > 0 && vinExist.State.ToUpper() == "A")
                {
                    if (!subscriptionIds.ContainsKey(item))
                        subscriptionIds.Add(item, vinExist.Id);
                }

                if (vinExist != null && vinExist.State.ToUpper() == "I")
                    counter--;
            }

            if (counter == 0)
                return Tuple.Create(nonExistVins, subscriptionIds, true);

            return Tuple.Create(nonExistVins, subscriptionIds, false);
        }
    }
}
