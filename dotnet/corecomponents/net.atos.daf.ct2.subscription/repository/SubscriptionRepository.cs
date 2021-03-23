
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
                             (@"select id, type from master.package where packagecode =@packagecode and is_active =@is_active",
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
        async Task<int> SubscriptionIdExits(string orderId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@subscription_id", orderId);
            parameterToGetSubscribeId.Add("@is_active", true);
            int data = await dataAccess.ExecuteScalarAsync<int>
                             (@"select id from master.subscription where subscription_id =@subscription_id and is_active =@is_active",
                            parameterToGetSubscribeId);
            return data;
        }

        //To check if Organization is already inserted
        async Task<string> OrganizationExits(int orgId, int packageId)
        {
            var parameterToGetSubscribeId = new DynamicParameters();
            parameterToGetSubscribeId.Add("@organization_Id", orgId);
            parameterToGetSubscribeId.Add("@package_id", packageId);
            parameterToGetSubscribeId.Add("@is_active", true);
            string data = await dataAccess.ExecuteScalarAsync<string>
                             (@"select subscription_id from master.subscription where organization_Id =@organization_Id and package_id=@package_id and is_active =@is_active",
                            parameterToGetSubscribeId);
            return data;
        }
        public async Task<SubscriptionResponse> Subscribe(SubscriptionActivation objSubscription)
        {
            log.Info("Subscribe Subscription method called in repository");
            try
            {
                var data = await GetPackageTypeByCode(objSubscription.packageId);
                //Get Organization id to insert in subscription Table
                int orgid = await GetOrganizationIdByCode(objSubscription.OrganizationId);
                string SubscriptionId = Guid.NewGuid().ToString();
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                objSubscriptionResponse.orderId = SubscriptionId;
                if (data == null)
                {
                    return null;
                }
                else if ( data.id > 0 && data.type.ToLower() == "o")
                {
                    //if package type is organization and has vins in the payload return bad request
                    if (objSubscription.VINs != null && objSubscription.VINs.Count > 0)
                    {
                        return null;
                    }
                 
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
                            //This will get us the list of vins exits on Vehicle Table or Not
                            objvinList.Add(vinexist);
                        }
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
                    int orderid = await SubscriptionIdExits(objUnSubscription.OrderID);
                    if (orderid == 0)
                    {
                        return null;
                    }
                    else
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("@subscription_id", objUnSubscription.OrderID);
                        parameter.Add("@subscription_end_date", objUnSubscription.EndDateTime);
                        string queryInsert = @"update master.subscription set is_active=false, subscription_end_date=@subscription_end_date where subscription_id=@subscription_id";
                        int roweffected = await dataAccess.ExecuteAsync
                            (queryInsert, parameter);
                        objSubscriptionResponse.numberOfVehicles = roweffected;
                        return objSubscriptionResponse;
                    }
                }

                var data = await GetPackageTypeByCode(objUnSubscription.PackageId);
                //Get Organization id to insert in subscription Table
                int orgid = await GetOrganizationIdByCode(objUnSubscription.OrganizationID);

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
                    parameter.Add("@organization_id", orgid);
                    parameter.Add("@package_id", data.id);
                    parameter.Add("@subscription_end_date", objUnSubscription.EndDateTime);
                    parameter.Add("@is_active", false);
                    int count = await dataAccess.ExecuteAsync
(@"update master.subscription set is_active=@is_active,subscription_end_date=@subscription_end_date where organization_id=@organization_id and package_id=@package_id", parameter);
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
                            int exist = 0;
                            var parameterToGetVehicleId = new DynamicParameters();
                            parameterToGetVehicleId.Add("@vin", item);
                            exist = await dataAccess.ExecuteScalarAsync<int>
                                (@"SELECT id FROM master.vehicle where vin=@vin", parameterToGetVehicleId);
                            if (exist == 0)
                            {//return Bad Request if vin does not exits in vehicle table
                                return null;
                            }

                            int vinexist = 0;
                            //forgeting vehicle id
                            var parametertogetUniqueVin = new DynamicParameters();
                            parametertogetUniqueVin.Add("@vehicle_id", exist);
                            parametertogetUniqueVin.Add("@package_id", data.id);
                            parametertogetUniqueVin.Add("@organization_id", orgid);
                            parametertogetUniqueVin.Add("@is_active", true);
                            string query = @"SELECT id FROM master.subscription where vehicle_id=@vehicle_id and package_id=@package_id and organization_id=@organization_id and is_active=@is_active";
                            vinexist = await dataAccess.ExecuteAsync(query, parametertogetUniqueVin);
                            if (vinexist == 0)
                            {//return Bad Request if vin does not exits in vehicle table
                                return null;
                            }
                            //This will get us the list of vins exits on Vehicle Table or Not
                            objvinList.Add(vinexist);
                        }
                        var parameter = new DynamicParameters();
                        parameter.Add("@subscription_end_date", objUnSubscription.EndDateTime);
                        parameter.Add("@is_active", false);
                        //This Condition to insert only if all the VIN exits in Vehicle Table
                        foreach (var item in objvinList)
                        {
                            parameter.Add("@id", item);
                 string queryInsert = "update master.subscription set is_active=@is_active,subscription_end_date=@subscription_end_date where id=@id";

                            int subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                        }
                        objSubscriptionResponse.numberOfVehicles = objvinList.Count;
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

        public async Task<Subscription> Create(Subscription subscription)
        {
            try
            {
                var parameter = new DynamicParameters();

                //parameter.Add("@id", subscription.Id);
                parameter.Add("@orderid", subscription.OrderId);
                // parameter.Add("@orderid", Subscription.OrderId);
                //parameter.Add("@organizationid", Subscription.OrganizationId);
                // parameter.add("@organizationdate", Subscription.OrganizationDate )

                string query = @"insert into master.subscription(id,orderid,organizationid,organizationdate) " +
                              "values(@id,@orderid,@organizationid,@organizationdate,true) RETURNING id";

                var id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                subscription.Id = id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return subscription;
        }
        public async Task<Subscription> Update(Subscription subscription)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", subscription.Id);
                parameter.Add("@orderid", subscription.OrderId);
                parameter.Add("@organizationid", subscription.OrganizationId);
                parameter.Add("@organizationdate", subscription.OrganizationDate);

                string query = @"update master.subscription set orderid= @orderid, organizationid = @organizationid,
                                organizationdate = @organizationdate,
                                where id = @id RETURNING id";

                subscription.Id = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return subscription;
        }


        //public async Task<Subscription> Get(int subscriptionId)
        //{
        //    try
        //    {
        //        var parameter = new DynamicParameters();
        //        parameter.Add("@id", subscriptionId);

        //        string query = string.Empty;
        //        query = "select * from master.subscription where id=@id";
        //        dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
        //        return Map(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private Subscription Map(dynamic record)
        {
            Subscription subscription = new Subscription();
            subscription.Id = record.id;
            subscription.OrderId = record.orderid;
            subscription.OrganizationId = record.organizationid;
            subscription.OrganizationDate = record.organizationdate;
            return subscription;
        }
        public async Task<Subscription> Get(int OrganizationId, int VehicleId, char Status, DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organizationId", OrganizationId);
                parameter.Add("@vehicleId", VehicleId);
                parameter.Add("@status", Status);
                parameter.Add("@startdate", StartDate);
                parameter.Add("@enddate", EndDate);
                string query = string.Empty;
                query = "select * from master.subscription where organizationid=@organizationid,vehicleid=@vehicleid,status=@status,startdate=@statrtdate,enddate=@enddate";
                dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                return Map(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Subscription Map1(dynamic record)
        {
            Subscription subscription = new Subscription();
            subscription.Id = record.id;
            subscription.OrderId = record.orderid;
            subscription.OrganizationId = record.organizationid;
            subscription.OrganizationDate = record.organizationdate;
            return subscription;
        }
        public async Task<Subscription> Get(char Status, int VehicleGroupID, int VehicleId,  DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vehiclegroupid", VehicleGroupID);
                parameter.Add("@vehicleid", VehicleId);
                parameter.Add("@status", Status);
                parameter.Add("@startdate", StartDate);
                parameter.Add("@enddate", EndDate);
                string query = string.Empty;
                query = "select * from master.subscription where vehiclegroupid=@vehiclegroupid,vehicleid=@vehicleid,status=@status,startdate=@statrtdate,enddate=@enddate";
                dynamic result = await dataAccess.QueryAsync<dynamic>(query, parameter);
                return Map(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Subscription Map2(dynamic record)
        {
            Subscription subscription = new Subscription();
            subscription.Id = record.id;
            subscription.OrderId = record.orderid;
            subscription.OrganizationId = record.organizationid;
            subscription.OrganizationDate = record.organizationdate;
            return subscription;
        }

       public async Task<SubscriptionResponse> Create(int orgId,int packageId)
        {
            try
            {
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                string SubscriptionId = string.Empty;
                SubscriptionId = await OrganizationExits(orgId, packageId);
                if (!string.IsNullOrEmpty(SubscriptionId))
                {
                    objSubscriptionResponse.orderId = SubscriptionId;
                    return objSubscriptionResponse;
                }
                Package objPackage = await GetPackageTypeById(packageId);
                SubscriptionId = Guid.NewGuid().ToString();
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", orgId);
                parameter.Add("@subscription_id", SubscriptionId);
                parameter.Add("@type", objPackage.type == null ? null : objPackage.type);
                parameter.Add("@package_code", objPackage.packagecode);
                parameter.Add("@package_id", packageId);
                parameter.Add("@vehicle_id", null);
                parameter.Add("@subscription_start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@subscription_end_date", null);
                parameter.Add("@is_active", true);
                parameter.Add("@is_zuora_package", false);
                
                string queryInsert = "insert into master.subscription(organization_id, subscription_id,type , package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, is_active,is_zuora_package) " +
                             "values(@organization_id, @subscription_id,@type, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @is_active,@is_zuora_package) RETURNING id";

                int subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);

                
                objSubscriptionResponse.orderId = SubscriptionId;
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
               
                    query = @"SELECT sub.subscription_id,sub.package_code,pak.name,sub.type,COUNT(veh.name),
                                     sub.subscription_start_date,sub.subscription_end_date,sub.is_active,sub.organization_id
                                     FROM master.Subscription sub 
                              JOIN master.package pak on sub.package_id = pak.id 
                              LEFT JOIN master.vehicle veh on sub.vehicle_id = veh.id
                              GROUP BY sub.subscription_id,sub.package_code,pak.name,sub.type,
                                     sub.subscription_start_date,sub.subscription_end_date,sub.is_active,sub.organization_id
                              HAVING 1=1";

                    var parameter = new DynamicParameters();

                    if (objSubscriptionDetailsRequest != null)
                    {
                        
                        if (objSubscriptionDetailsRequest.organization_id > 0)
                        {
                            parameter.Add("@organization_id", objSubscriptionDetailsRequest.organization_id);
                            query =   $"{query} and sub.organization_id=@organization_id ";
                        }
                        
                        else if (!string.IsNullOrEmpty(objSubscriptionDetailsRequest.type))
                        {
                            parameter.Add("@type", objSubscriptionDetailsRequest.type.ToUpper());
                            query = $"{query} and sub.type=@type";
                        }
                        
                        else if(objSubscriptionDetailsRequest.is_active > 0)
                        {
                            parameter.Add("@is_active", objSubscriptionDetailsRequest.is_active == StatusType.True ? true : false);
                            query = $"{query} and sub.is_active=@is_active";
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
