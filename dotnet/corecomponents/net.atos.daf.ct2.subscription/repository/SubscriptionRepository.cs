
using System;
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
        public async Task<SubscriptionResponse> Subscribe(Subscription objSubscription)
        {
            log.Info("Subscribe Subscription method called in repository");
            try
            {
                //for geting OrganizationId
                var parameterToGetOrgId = new DynamicParameters();
                parameterToGetOrgId.Add("@org_id", objSubscription.OrganizationId);
                int orgexist = await dataAccess.ExecuteScalarAsync<int>
                               (@"SELECT id FROM master.organization where org_id=@org_id",
                               parameterToGetOrgId);

                if (orgexist > 0)
                {
                    //
                }
                //forgeting packageid
                var parameterToGetPackageId = new DynamicParameters();
                parameterToGetPackageId.Add("@packagecode", objSubscription.packageId);
                int packexist = await dataAccess.ExecuteScalarAsync<int>
                                (@"SELECT id FROM master.package where packagecode=@packagecode",
                                parameterToGetPackageId);

                if (packexist > 0)
                {
                   //
                }

                    var parameter = new DynamicParameters();
                    parameter.Add("@organization_id", orgexist);
                    parameter.Add("@subscription_id", new Guid());
                    parameter.Add("@package_code", objSubscription.packageId);
                    parameter.Add("@package_id", packexist);
                    parameter.Add("@subscription_start_date", "");
                    parameter.Add("@subscription_end_date", "");
                    parameter.Add("@is_active", true);
                    int vinexist = 0; int subid = 0;int vincount = 0;
                    if (objSubscription.VINs.Length > 0)
                    {
                        foreach (var item in objSubscription.VINs)
                        {
                            //forgeting vehicle id
                            var parameterToGetVehicleId = new DynamicParameters();
                            parameterToGetVehicleId.Add("@vin", item);
                            vinexist = await dataAccess.ExecuteScalarAsync<int>
                                (@"SELECT id FROM master.vehicle where vin=@vin", parameterToGetVehicleId);
                            parameter.Add("@vehicle_id", vinexist);

                            string queryInsert = "insert into master.subscription(organization_id, subscription_id, package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, is_active) " +
                                 "values(organization_id, subscription_id, package_code, package_id, vehicle_id, NULL, NULL, true) RETURNING id";

                            subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                            if (subid>0)
                            {
                                vincount = ++vincount;
                            }
                        }
                    }
                    else
                    {
                        parameter.Add("@vehicle_id", null);

                        string queryInsert = "insert into master.subscription(organization_id, subscription_id, package_code, package_id, vehicle_id, subscription_start_date, subscription_end_date, is_active) " +
                             "values(organization_id, subscription_id, package_code, package_id, NULL, NULL, NULL, NULL) RETURNING id";

                        subid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                    }
                    SubscriptionResponse objSubscriptionResponse = new SubscriptionResponse();
                    objSubscriptionResponse.orderId = subid.ToString();
                    objSubscriptionResponse.numberOfVehicles = vincount;
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
              
                var parameter = new DynamicParameters();
                parameter.Add("@subscription_id", objUnSubscription.serviceSubscriberId);

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
