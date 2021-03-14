
using System;
using System.Collections;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.subscription.entity;

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
            var data = await dataAccess.QueryFirstOrDefaultAsync<Package>
                             (@"select id, type from master.package where packagecode = @packagecode and is_active = true",
                            parameterToGetPackageId);
            return data;
        }

        //forgeting package type and id
        async Task<int> GetOrganizationIdByCode(string OrganizationCode)
        {
            
            var parameterToGetPackageId = new DynamicParameters();
            parameterToGetPackageId.Add("@org_id", OrganizationCode);
            var data = await dataAccess.ExecuteScalarAsync<int>
                             (@"select id from master.organization where org_id = and is_active = true",
                            parameterToGetPackageId);
            return data;
        }

        public async Task<SubscriptionResponse> Subscribe(Subscription objSubscription)
        {
            log.Info("Subscribe Subscription method called in repository");
            try
            {
                var data = await GetPackageTypeByCode(objSubscription.packageId);
                //Get Organization id to insert in subscription Table
                int orgid = await GetOrganizationIdByCode(objSubscription.OrganizationId);
                string SubscriptionId = new Guid().ToString();
                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                objSubscriptionResponse.orderId = SubscriptionId;
                if (data.id > 0 && data.type.ToLower() == "o")
                {
                    //if package type is organization and has vins in the payload return bad request
                    if (objSubscription.VINs.Length > 0)
                    {
                        return null;
                    }
                 
                    var parameter = new DynamicParameters();
                    parameter.Add("@organization_id", orgid);
                    parameter.Add("@subscription_id", SubscriptionId);
                    parameter.Add("@package_code", objSubscription.packageId);
                    parameter.Add("@package_id", data.id);
                    parameter.Add("@subscription_start_date", objSubscription.StartDateTime);
                    parameter.Add("@subscription_end_date", null);
                    parameter.Add("@vehicle_id", null);
                    parameter.Add("@is_active", true);
                    parameter.Add("@is_zuora_package", true);
                    string queryInsert = "insert into master.subscription(organization_id, subscription_id, package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, is_active,is_zuora_package) " +
                                 "values(@organization_id, @subscription_id, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @is_active,@is_zuora_package) RETURNING id";

                   int subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);

                }
                //if package type is vehicle then only check vehicle table
                else if (data.id > 0 && data.type.ToLower() == "v")
                {
                    if (objSubscription.VINs.Length == 0)
                    {
                        return null;
                    }
                    if (objSubscription.VINs.Length > 0)
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
                            string queryInsert = "insert into master.subscription(organization_id, subscription_id, package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, is_active,is_zuora_package) " +
                                         "values(@organization_id, @subscription_id, @package_code, @package_id, @vehicle_id, @subscription_start_date, @subscription_end_date, @is_active, @is_zuora_package) RETURNING id";

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
                var data = await GetPackageTypeByCode(objUnSubscription.PackageId);
                //Get Organization id to insert in subscription Table
                int orgid = await GetOrganizationIdByCode(objUnSubscription.OrganizationID);

                var parameter = new DynamicParameters();
                parameter.Add("@subscription_id", objUnSubscription.OrderID);

                await dataAccess.ExecuteScalarAsync<int>(@"update master.subscription set is_active=false where subscription_id=@subscription_id", parameter);

                SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                objSubscriptionResponse.orderId = "";
                objSubscriptionResponse.numberOfVehicles = 1;
                return objSubscriptionResponse;
            }
            catch (Exception ex)
            {
                log.Info("Subscribe Subscription method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(objUnSubscription));
                log.Error(ex.ToString());
                throw ex;
            }
        }
    }
}
