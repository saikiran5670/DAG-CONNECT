using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.data;
using Dapper;
using net.atos.daf.ct2.utilities;
using System.Transactions;
using System.Configuration;

namespace net.atos.daf.ct2.vehicle.repository
{
    public class VehicleRepository : IVehicleRepository
    {

        private readonly IDataAccess dataAccess;
        private readonly IDataMartDataAccess DataMartdataAccess;
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //public VehicleRepository(IEnumerable<IDataAccess> _dataAccess)
        //{
        //    DataMartdataAccess = _dataAccess.Where(x => x is PgSQLDataMartDataAccess).First();
        //    dataAccess = _dataAccess.Where(x => x is PgSQLDataAccess).First();

        //}

        public VehicleRepository(IDataAccess _dataAccess, IDataMartDataAccess _DataMartdataAccess)
        {
            DataMartdataAccess = _DataMartdataAccess;
            dataAccess = _dataAccess;

        }

        #region Vehicle component methods

        public async Task<List<VehiclesBySubscriptionId>> GetVehicleBySubscriptionId(int subscriptionId)
        {
            log.Info("GetVehicleBySubscriptionId Vehicle method called in repository");
            try
            {
                List<VehiclesBySubscriptionId> objVehiclesBySubscriptionId = new List<VehiclesBySubscriptionId>();
                var parameter = new DynamicParameters();
                parameter.Add("@subscription_id", subscriptionId);
                var query = @"select veh.id, sub.subscription_id as orderId, veh.name, veh.vin, veh.license_plate_number
                              from master.Subscription sub 
                              join master.vehicle veh on sub.vehicle_id = veh.id
                              where subscription_id = @subscription_id";
                var data = await dataAccess.QueryAsync<VehiclesBySubscriptionId>(query, parameter);
                return objVehiclesBySubscriptionId = data.Cast<VehiclesBySubscriptionId>().ToList();
            }
            catch (Exception ex)
            {
                log.Info("GetVehicleBySubscriptionId Vehicle method in repository failed.");
                log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<Vehicle> Create(Vehicle vehicle)
        {
            try
            {
                char org_status = await GetOrganisationStatusofVehicle(Convert.ToInt32(vehicle.Organization_Id));
                vehicle.Opt_In = VehicleStatusType.Inherit;
                vehicle.Is_Ota = false;
                vehicle.Status = (VehicleCalculatedStatus)await GetCalculatedVehicleStatus(org_status, vehicle.Is_Ota);
                dynamic oiedetail = await GetOEM_Id(vehicle.VIN);
                if (oiedetail != null)
                {
                    vehicle.Oem_id = oiedetail[0].id;
                    vehicle.Oem_Organisation_id = oiedetail[0].oem_organisation_id;
                }

                var QueryStatement = @"INSERT INTO master.vehicle
                                      (
                                       organization_id
                                       ,name 
                                       ,vin
                                       ,license_plate_number 
                                       ,status 
                                       ,status_changed_date 
                                       ,termination_date 
                                       ,vid 
                                       ,type 
                                       ,tcu_id
                                       ,tcu_serial_number 
                                       ,tcu_brand 
                                       ,tcu_version 
                                       ,is_tcu_register 
                                       ,reference_date 
                                       ,vehicle_property_id                                       
                                       ,created_at 
                                       ,model_id
                                       ,oem_id
                                       ,oem_organisation_id
                                       ,opt_in
                                       ,is_ota
                                       ,fuel_type) 
                            	VALUES(
                                       @organization_id 
                                      ,@name
                                      ,@vin
                                      ,@license_plate_number
                                      ,@status
                                      ,@status_changed_date
                                      ,@termination_date
                                      ,@vid
                                      ,@type                                      
                                      ,@tcu_id
                                      ,@tcu_serial_number
                                      ,@tcu_brand
                                      ,@tcu_version
                                      ,@is_tcu_register
                                      ,@reference_date
                                      ,@vehicle_property_id                                      
                                      ,@created_at
                                      ,@model_id
                                      ,@oem_id
                                      ,@oem_organisation_id
                                      ,@opt_in
                                      ,@is_ota
                                      ,@fuel_type
                                      ) RETURNING id";


                var parameter = new DynamicParameters();
                if (vehicle.Organization_Id > 0)
                {
                    parameter.Add("@organization_id", vehicle.Organization_Id);
                }
                else
                {
                    parameter.Add("@organization_id", null);
                }
                parameter.Add("@name", string.IsNullOrEmpty(vehicle.Name) ? null : vehicle.Name);
                parameter.Add("@vin", string.IsNullOrEmpty(vehicle.VIN) ? null : vehicle.VIN);
                parameter.Add("@license_plate_number", string.IsNullOrEmpty(vehicle.License_Plate_Number) ? null : vehicle.License_Plate_Number);
                parameter.Add("@status", (char)vehicle.Status);
                parameter.Add("@status_changed_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@termination_date", vehicle.Termination_Date != null ? UTCHandling.GetUTCFromDateTime(vehicle.Termination_Date.ToString()) : (long?)null);
                parameter.Add("@vid", string.IsNullOrEmpty(vehicle.Vid) ? null : vehicle.Vid);
                parameter.Add("@type", null);
                parameter.Add("@tcu_id", string.IsNullOrEmpty(vehicle.Tcu_Id) ? null : vehicle.Tcu_Id);
                parameter.Add("@tcu_serial_number", string.IsNullOrEmpty(vehicle.Tcu_Serial_Number) ? null : vehicle.Tcu_Serial_Number);
                parameter.Add("@tcu_brand", string.IsNullOrEmpty(vehicle.Tcu_Brand) ? null : vehicle.Tcu_Brand);
                parameter.Add("@tcu_version", string.IsNullOrEmpty(vehicle.Tcu_Version) ? null : vehicle.Tcu_Version);
                parameter.Add("@is_tcu_register", vehicle.Is_Tcu_Register);
                parameter.Add("@reference_date", vehicle.Reference_Date != null ? UTCHandling.GetUTCFromDateTime(vehicle.Reference_Date.ToString()) : (long?)null);
                parameter.Add("@vehicle_property_id", vehicle.VehiclePropertiesId != 0 ? vehicle.VehiclePropertiesId : null);
                parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));
                parameter.Add("@model_id", string.IsNullOrEmpty(vehicle.ModelId) ? null : vehicle.ModelId);
                parameter.Add("@oem_id", vehicle.Oem_id);
                parameter.Add("@oem_organisation_id", vehicle.Oem_Organisation_id);
                parameter.Add("@opt_in", (char)vehicle.Opt_In);
                parameter.Add("@is_ota", vehicle.Is_Ota);
                parameter.Add("@fuel_type", vehicle.Fuel);
                parameter.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                vehicle.ID = vehicleID;


                if (vehicle.IPPS == false)
                {
                    // create data into data mart
                    VehicleDataMart vehicleDataMart = new VehicleDataMart();
                    vehicleDataMart.VIN = vehicle.VIN;
                    vehicleDataMart.Name = vehicle.Name;
                    vehicleDataMart.Registration_No = vehicle.License_Plate_Number;
                    vehicleDataMart.Vid = vehicle.Vid;
                    vehicleDataMart.IsIPPS = false;
                    await CreateAndUpdateVehicleInDataMart(vehicleDataMart);
                }

                return vehicle;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> Get(VehicleFilter vehiclefilter)
        {

            var QueryStatement = @"select id
                                   ,organization_id 
                                   ,name 
                                   ,vin 
                                   ,license_plate_number 
                                   ,status 
                                   ,status_changed_date 
                                   ,termination_date 
                                   ,vid 
                                   ,type 
                                   ,tcu_id 
                                   ,tcu_serial_number 
                                   ,tcu_brand 
                                   ,tcu_version 
                                   ,is_tcu_register 
                                   ,reference_date 
                                   ,vehicle_property_id                                   
                                   ,created_at 
                                   ,model_id
                                   ,opt_in
                                   ,is_ota
                                   ,oem_id
                                   ,oem_organisation_id
                                   from master.vehicle 
                                   where 1=1";
            var parameter = new DynamicParameters();

            // Vehicle Id Filter
            if (vehiclefilter.VehicleId > 0)
            {
                parameter.Add("@id", vehiclefilter.VehicleId);
                QueryStatement = QueryStatement + " and id=@id";

            }
            // organization id filter
            if (vehiclefilter.OrganizationId > 0)
            {
                parameter.Add("@organization_id", vehiclefilter.OrganizationId);
                QueryStatement = QueryStatement + " and organization_id=@organization_id";

            }

            // VIN Id Filter
            if (vehiclefilter.VIN != null && Convert.ToInt32(vehiclefilter.VIN.Length) > 0)
            {
                parameter.Add("@vin", "%" + vehiclefilter.VIN + "%");
                QueryStatement = QueryStatement + " and vin LIKE @vin";

            }

            // Vehicle Id list Filter
            if (vehiclefilter.VehicleIdList != null && Convert.ToInt32(vehiclefilter.VehicleIdList.Length) > 0)
            {
                List<int> VehicleIds = vehiclefilter.VehicleIdList.Split(',').Select(int.Parse).ToList();
                QueryStatement = QueryStatement + " and id  = ANY(@VehicleIds)";
                parameter.Add("@VehicleIds", VehicleIds);
            }

            if (vehiclefilter.Status != VehicleStatusType.None && vehiclefilter.Status != 0)
            {
                parameter.Add("@status", (char)vehiclefilter.Status);
                QueryStatement = QueryStatement + " and status=@status";

            }

            List<Vehicle> vehicles = new List<Vehicle>();
            dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            foreach (dynamic record in result)
            {
                vehicles.Add(Map(record));
            }
            return vehicles.AsEnumerable();
        }

        private Vehicle Map(dynamic record)
        {
            Vehicle vehicle = new Vehicle();
            vehicle.ID = record.id;
            vehicle.Organization_Id = record.organization_id;
            vehicle.Name = record.name;
            vehicle.VIN = record.vin;
            vehicle.License_Plate_Number = record.license_plate_number;
            if (record.status != null)
                vehicle.Status = (VehicleCalculatedStatus)(Convert.ToChar(record.status));
            if (record.status_changed_date != null)
                vehicle.Status_Changed_Date = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.status_changed_date, "America/New_York", "yyyy-MM-ddTHH:mm:ss"));
            if (record.termination_date != null)
                vehicle.Termination_Date = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.termination_date, "Africa/Mbabane", "yyyy-MM-ddTHH:mm:ss"));
            vehicle.ModelId = record.model_id;
            vehicle.Vid = record.vid;
            vehicle.Type = (VehicleType)(Convert.ToChar(record.type == null ? 'N' : record.type));
            vehicle.Tcu_Id = record.tcu_id;
            vehicle.Tcu_Serial_Number = record.tcu_serial_number;
            vehicle.Tcu_Brand = record.tcu_brand;
            vehicle.Tcu_Version = record.tcu_version;
            vehicle.Is_Tcu_Register = record.is_tcu_register == null ? false : record.is_tcu_register;
            if (record.reference_date != null)
                vehicle.Reference_Date = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.reference_date, "Asia/Dubai", "yyyy-MM-ddTHH:mm:ss"));
            vehicle.Oem_id = record.oem_id;
            vehicle.Oem_Organisation_id = record.oem_organisation_id;
            if (record.is_ota != null)
                vehicle.Is_Ota = record.is_ota;
            if (record.opt_in != null)
                vehicle.Opt_In = (VehicleStatusType)(Convert.ToChar(record.opt_in));
            if (record.created_at != null)
                // vehicle.CreatedAt = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.created_at, "Africa/Mbabane", "yyyy-MM-ddTHH:mm:ss"));
                vehicle.CreatedAt = record.created_at;
            if (record.relationship != null)
                vehicle.RelationShip = record.relationship;
            return vehicle;
        }

        public async Task<Vehicle> Update(Vehicle vehicle)
        {
            if (vehicle.Tcu_Id == null || vehicle.Tcu_Id.Length == 0 || vehicle.Tcu_Id == "string")
            {
                vehicle = await VehicleNameExists(vehicle);
                vehicle = await VehicleLicensePlateNumberExists(vehicle);

                // duplicate vehicle Name
                if (vehicle.VehicleNameExists)
                {
                    return vehicle;
                }

                // duplicate License Plate Number
                if (vehicle.VehicleLicensePlateNumberExists)
                {
                    return vehicle;
                }

                var QueryStatement = @" UPDATE master.vehicle
                                        SET 
                                         name=@name                                        
        	                            ,license_plate_number=@license_plate_number
                                        ,modified_at=@modified_at
                                        WHERE id = @id
                                         RETURNING id;";

                var parameter = new DynamicParameters();
                parameter.Add("@id", vehicle.ID);
                // parameter.Add("@organization_id", vehicle.Organization_Id);
                parameter.Add("@name", vehicle.Name);
                // parameter.Add("@vin", vehicle.VIN);
                parameter.Add("@license_plate_number", vehicle.License_Plate_Number);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                // parameter.Add("@status_changed_date", vehicle.Status_Changed_Date != null ? UTCHandling.GetUTCFromDateTime(vehicle.Status_Changed_Date.ToString()) : 0);
                // parameter.Add("@termination_date", vehicle.Termination_Date != null ? UTCHandling.GetUTCFromDateTime(vehicle.Termination_Date.ToString()) : 0);
                int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

                VehicleDataMart vehicleDataMart = new VehicleDataMart();
                vehicleDataMart.VIN = await dataAccess.QuerySingleAsync<string>("SELECT vin FROM master.vehicle where id=@id", new { id = vehicle.ID });
                vehicleDataMart.Registration_No = vehicle.License_Plate_Number;
                vehicleDataMart.Name = vehicle.Name;
                vehicleDataMart.Vid = "";
                await CreateAndUpdateVehicleInDataMart(vehicleDataMart);

            }
            else
            {
                //using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                //{
                await CheckUnknownOEM(vehicle.VIN);

                var QueryStatement = @" UPDATE master.vehicle
                                        SET 
                                        vid=@vid
                                      ,tcu_id=@tcu_id
                                      ,tcu_serial_number=@tcu_serial_number
                                      ,tcu_brand=@tcu_brand
                                      ,tcu_version=@tcu_version
                                      ,is_tcu_register=@is_tcu_register
                                      ,reference_date=@reference_date
                                      ,modified_at=@modified_at
                                       WHERE vin = @vin
                                       RETURNING vin;";

                var parameter = new DynamicParameters();
                parameter.Add("@vid", vehicle.Vid);
                parameter.Add("@tcu_id", vehicle.Tcu_Id);
                parameter.Add("@tcu_serial_number", vehicle.Tcu_Serial_Number);
                parameter.Add("@tcu_brand", vehicle.Tcu_Brand);
                parameter.Add("@tcu_version", vehicle.Tcu_Version);
                parameter.Add("@is_tcu_register", vehicle.Is_Tcu_Register);
                parameter.Add("@reference_date", vehicle.Reference_Date != null ? UTCHandling.GetUTCFromDateTime(vehicle.Reference_Date.ToString()) : 0);
                parameter.Add("@vin", vehicle.VIN);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                string VIN = await dataAccess.ExecuteScalarAsync<string>(QueryStatement, parameter);


                VehicleDataMart vehicleDataMart = new VehicleDataMart();
                vehicleDataMart.VIN = vehicle.VIN;
                vehicleDataMart.Vid = vehicle.Vid;
                await CreateAndUpdateVehicleInDataMart(vehicleDataMart);


                //    transactionScope.Complete();
                //}
            }
            return vehicle;
        }

        public async Task<VehicleOptInOptOut> UpdateStatus(VehicleOptInOptOut vehicleOptInOptOut)
        {
            var parameter = new DynamicParameters();
            var QueryStatement = string.Empty;
            if ((char)vehicleOptInOptOut.Type == 'V')
            {
                QueryStatement = @" UPDATE master.vehicle
                                                SET
                                                status=@status
                                                ,status_changed_date=@status_changed_date
                                                ,termination_date=@termination_date
                                                WHERE id = @id
                                                RETURNING id;";

                parameter.Add("@id", vehicleOptInOptOut.RefId);
            }

            if ((char)vehicleOptInOptOut.Type == 'O')
            {
                QueryStatement = @" UPDATE master.vehicle
                                                SET
                                                status=@status
                                                ,status_changed_date=@status_changed_date
                                                ,termination_date=@termination_date
                                                WHERE organization_id = @id
                                                RETURNING id;";

                parameter.Add("@id", vehicleOptInOptOut.RefId);
            }

            parameter.Add("@status", (char)vehicleOptInOptOut.Status);
            parameter.Add("@status_changed_date", vehicleOptInOptOut.Date != null ? UTCHandling.GetUTCFromDateTime(vehicleOptInOptOut.Date.ToString()) : 0);
            if (vehicleOptInOptOut.Status.ToString() == VehicleCalculatedStatus.Terminate.ToString())
            {
                parameter.Add("@termination_date", vehicleOptInOptOut.Date != null ? UTCHandling.GetUTCFromDateTime(vehicleOptInOptOut.Date.ToString()) : 0);
            }
            else
            {
                parameter.Add("@termination_date", null);
            }
            int vehiclepropertyId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            if (vehiclepropertyId > 0)
            {
                var InsertQueryStatement = @"INSERT INTO master.vehicleoptinoptout
                                      (
                                       ref_id
                                      ,account_id
                                      ,status
                                      ,status_changed_date
                                      ,type) 
                            	VALUES(
                                       @ref_id 
                                      ,@account_id
                                      ,@status
                                      ,@status_changed_date
                                      ,@type) RETURNING id";

                var OptInOptOutparameter = new DynamicParameters();
                OptInOptOutparameter.Add("@ref_id", vehicleOptInOptOut.RefId);
                OptInOptOutparameter.Add("@account_id", vehicleOptInOptOut.AccountId);
                OptInOptOutparameter.Add("@status", (char)vehicleOptInOptOut.Status);
                OptInOptOutparameter.Add("@status_changed_date", vehicleOptInOptOut.Date != null ? UTCHandling.GetUTCFromDateTime(vehicleOptInOptOut.Date.ToString()) : 0);
                OptInOptOutparameter.Add("@type", (char)vehicleOptInOptOut.Type);

                int Id = await dataAccess.ExecuteScalarAsync<int>(InsertQueryStatement, OptInOptOutparameter);
            }
            return vehicleOptInOptOut;
        }

        public async Task<IEnumerable<VehicleGroupRequest>> GetOrganizationVehicleGroupdetails(long OrganizationId)
        {

            var QueryStatement = @"select vehiclegroupid,VehicleGroupName,vehicleCount,count(account) as usercount, true as isgroup from 
                                   (select grp.id as vehiclegroupid,grp.name as VehicleGroupName,grp.object_type
                                    ,count(distinct vgrpref.ref_id) as vehicleCount 
                                    ,agrpref.ref_id as account
                                    from master.group grp 
                                    Left join master.groupref vgrpref
                                    on  grp.id=vgrpref.group_id and grp.object_type='V'
                                    left join master.accessrelationship accrel
                                    on  accrel.vehicle_group_id=grp.id
                                    left join master.groupref agrpref
                                    on  accrel.account_group_id=agrpref.group_id
                                    where grp.organization_id = @organization_id and grp.object_type='V'
                                    group by grp.id,grp.name,accrel.account_group_id,agrpref.ref_id,grp.object_type) vdetail
                                    group by vehiclegroupid,VehicleGroupName,vehicleCount,object_type 
                                    union all 
									select id as vehiclegroupid, name as VehicleGroupName,0 as vehicleCount, 0 as usercount,false as isgroup
									 from master.vehicle where organization_id=@organization_id";

            var parameter = new DynamicParameters();

            parameter.Add("@organization_id", OrganizationId);
            IEnumerable<VehicleGroupRequest> OrgVehicleGroupDetails = await dataAccess.QueryAsync<VehicleGroupRequest>(QueryStatement, parameter);
            return OrgVehicleGroupDetails;
        }

        public async Task<Vehicle> GetVehicle(int Vehicle_Id)
        {
            try
            {
                var QueryStatement = @"select id
                                   ,organization_id 
                                   ,name 
                                   ,vin 
                                   ,license_plate_number 
                                   ,status 
                                   ,status_changed_date 
                                   ,termination_date 
                                   ,vid 
                                   ,type 
                                   ,tcu_id 
                                   ,tcu_serial_number 
                                   ,tcu_brand 
                                   ,tcu_version 
                                   ,is_tcu_register 
                                   ,reference_date 
                                   ,vehicle_property_id                                   
                                   ,created_at 
                                   ,model_id
                                   ,opt_in
                                   ,is_ota
                                   ,oem_id
                                   ,oem_organisation_id
                                   from master.vehicle 
                                   where ";
                var parameter = new DynamicParameters();

                // Vehicle Id 
                if (Vehicle_Id > 0)
                {
                    parameter.Add("@id", Vehicle_Id);
                    QueryStatement = QueryStatement + " id=@id";
                }
                dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
                Vehicle vehicle = new Vehicle();
                foreach (dynamic record in result)
                {
                    vehicle = Map(record);
                }
                return vehicle;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Vehicle> UpdateOrgVehicleDetails(Vehicle vehicle)
        {

            vehicle.ID = await IsVINExists(vehicle.VIN);
            await VehicleOptInOptOutHistory(vehicle.ID);
            char VehOptIn = await dataAccess.QuerySingleAsync<char>("select coalesce((select vehicle_default_opt_in FROM master.organization where id=@id), 'U')", new { id = vehicle.Organization_Id });
            vehicle.Status = (VehicleCalculatedStatus)await GetCalculatedVehicleStatus(VehOptIn, false);

            var QueryStatement = @" UPDATE master.vehicle
                                    SET 
                                     organization_id=@organization_id                                        
                                    ,opt_in=@opt_in
                                    ,modified_at=@modified_at
                                    ,modified_by=@modified_by
                                    ,status_changed_date=@status_changed_date
                                    ,status=@status
                                    ,reference_date=@reference_date
                                    ,is_ota=@is_ota
                                     WHERE id = @id
                                     RETURNING id;";

            var parameter = new DynamicParameters();
            parameter.Add("@id", vehicle.ID);
            parameter.Add("@organization_id", vehicle.Organization_Id);
            parameter.Add("@opt_in", (char)VehicleStatusType.Inherit);
            parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            parameter.Add("@modified_by", vehicle.Modified_By);
            parameter.Add("@status", (char)vehicle.Status);
            parameter.Add("@status_changed_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            parameter.Add("@reference_date", vehicle.Reference_Date != null ? UTCHandling.GetUTCFromDateTime(vehicle.Reference_Date.ToString()) : 0);
            parameter.Add("@is_ota", vehicle.Is_Ota);
            int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

            return vehicle;
        }

        public async Task<int> IsVINExists(string VIN)
        {
            int VehicleId = await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.vehicle where vin=@vin), 0)", new { vin = VIN });
            return VehicleId;
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicAllVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId)
        {

            var QueryStatement = @"select distinct orm.relationship_id
	                               ,veh.id
	                               ,orm.target_org_id
                                   ,veh.organization_id
	                               ,veh.vin
	                               ,veh.license_plate_number
	                               ,veh.name	                               
                                   ,veh.status 
                                   ,veh.status_changed_date 
                                   ,veh.termination_date 
                                   ,veh.vid 
                                   ,veh.type 
                                   ,veh.tcu_id 
                                   ,veh.tcu_serial_number 
                                   ,veh.tcu_brand 
                                   ,veh.tcu_version 
                                   ,veh.is_tcu_register 
                                   ,veh.reference_date 
                                   ,veh.vehicle_property_id                                   
                                   ,veh.created_at 
                                   ,veh.model_id
                                   ,veh.opt_in
                                   ,veh.is_ota
                                   ,veh.oem_id
                                   ,veh.oem_organisation_id
	                               from master.vehicle veh
                            Inner join master.orgrelationshipmapping  orm
                            on orm.vehicle_id=veh.id
                            Inner join master.orgrelationship ors
                            on ors.id=orm.relationship_id
                            where  1=1
                            and ors.state='A'
                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
                            else COALESCE(end_date,0) =0 end ";
            var parameter = new DynamicParameters();


            // organization id filter
            if (OrganizationId > 0)
            {
                parameter.Add("@organization_id", OrganizationId);
                QueryStatement = QueryStatement + @" AND ((orm.owner_org_id=@organization_id AND ors.code='Owner')
                                                        OR(orm.target_org_id=@organization_id AND ors.code<>'Owner'))";

            }

            // RelationShip Id Filter
            if (RelationShipId > 0)
            {
                parameter.Add("@id", RelationShipId);
                QueryStatement = QueryStatement + " and ors.id=@id";

            }

            List<Vehicle> vehicles = new List<Vehicle>();
            dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            foreach (dynamic record in result)
            {
                vehicles.Add(Map(record));
            }
            return vehicles.AsEnumerable();
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicVisibleVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId)
        {
            var QueryStatement = @"select distinct 
	                               orm.relationship_id
	                               ,veh.id
	                               ,orm.target_org_id
                                   ,veh.organization_id
	                               ,veh.vin
	                               ,veh.license_plate_number
	                               ,veh.name	                               
                                   ,veh.status 
                                   ,veh.status_changed_date 
                                   ,veh.termination_date 
                                   ,veh.vid 
                                   ,veh.type 
                                   ,veh.tcu_id 
                                   ,veh.tcu_serial_number 
                                   ,veh.tcu_brand 
                                   ,veh.tcu_version 
                                   ,veh.is_tcu_register 
                                   ,veh.reference_date 
                                   ,veh.vehicle_property_id                                   
                                   ,veh.created_at 
                                   ,veh.model_id
                                   ,veh.opt_in
                                   ,veh.is_ota
                                   ,veh.oem_id
                                   ,veh.oem_organisation_id
	                               from master.vehicle veh
                            Inner join master.orgrelationshipmapping  orm
                            on orm.vehicle_id=veh.id
                            Inner join master.orgrelationship ors
                            on ors.id=orm.relationship_id
                            where 1=1
                            and ors.state='A'
                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
                            else COALESCE(end_date,0) =0 end ";
            var parameter = new DynamicParameters();

            // Organization Id filter
            if (OrganizationId > 0)
            {
                parameter.Add("@organization_id", OrganizationId);
                QueryStatement = QueryStatement + " and orm.target_org_id=@organization_id AND ors.code<>'Owner'";
            }

            // RelationShip Id Filter
            if (RelationShipId > 0)
            {
                parameter.Add("@id", RelationShipId);
                QueryStatement = QueryStatement + " and ors.id=@id";

            }

            List<Vehicle> vehicles = new List<Vehicle>();
            dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            foreach (dynamic record in result)
            {
                vehicles.Add(Map(record));
            }
            return vehicles.AsEnumerable();
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicOwnedVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId)
        {

            var QueryStatement = @"select distinct 
	                               orm.relationship_id
	                               ,veh.id
	                               ,orm.target_org_id
                                   ,veh.organization_id
	                               ,veh.vin
	                               ,veh.license_plate_number
	                               ,veh.name	                               
                                   ,veh.status 
                                   ,veh.status_changed_date 
                                   ,veh.termination_date 
                                   ,veh.vid 
                                   ,veh.type 
                                   ,veh.tcu_id 
                                   ,veh.tcu_serial_number 
                                   ,veh.tcu_brand 
                                   ,veh.tcu_version 
                                   ,veh.is_tcu_register 
                                   ,veh.reference_date 
                                   ,veh.vehicle_property_id                                   
                                   ,veh.created_at 
                                   ,veh.model_id
                                   ,veh.opt_in
                                   ,veh.is_ota
                                   ,veh.oem_id
                                   ,veh.oem_organisation_id
	                               from master.vehicle veh
                            Left join master.orgrelationshipmapping  orm
                            on orm.vehicle_id=veh.id
                            Inner join master.orgrelationship ors
                            on ors.id=orm.relationship_id
                             where 1=1  
                            and ors.state='A'
                            and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
                            else COALESCE(end_date,0) =0 end ";


            var parameter = new DynamicParameters();

            // Organization Id filter
            if (OrganizationId > 0)
            {
                parameter.Add("@organization_id", OrganizationId);
                QueryStatement = QueryStatement + " and ((orm.owner_org_id=@organization_id AND ors.code='Owner') or veh.organization_id=@organization_id)";

            }

            // RelationShip Id Filter
            if (RelationShipId > 0)
            {
                parameter.Add("@id", RelationShipId);
                QueryStatement = QueryStatement + " and ors.id=@id";

            }

            List<Vehicle> vehicles = new List<Vehicle>();
            dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            foreach (dynamic record in result)
            {
                vehicles.Add(Map(record));
            }
            return vehicles.AsEnumerable();
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicOEMVehicles(int vehicleGroupId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vehicleGroupId", vehicleGroupId);
            var QueryStatement = @"select 
	                               veh.id
                                   ,veh.organization_id
	                               ,veh.vin
	                               ,veh.license_plate_number
	                               ,veh.name	                               
                                   ,veh.status 
                                   ,veh.status_changed_date 
                                   ,veh.termination_date 
                                   ,veh.vid 
                                   ,veh.type 
                                   ,veh.tcu_id 
                                   ,veh.tcu_serial_number 
                                   ,veh.tcu_brand 
                                   ,veh.tcu_version 
                                   ,veh.is_tcu_register 
                                   ,veh.reference_date 
                                   ,veh.vehicle_property_id                                   
                                   ,veh.created_at 
                                   ,veh.model_id
                                   ,veh.opt_in
                                   ,veh.is_ota
                                   ,veh.oem_id
                                   ,veh.oem_organisation_id
	                               from master.vehicle veh
                            INNER JOIN master.group grp ON grp.object_type='V' AND grp.id=@vehicleGroupId
                            WHERE veh.oem_organisation_id=grp.organization_id";

            List<Vehicle> vehicles = new List<Vehicle>();
            dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            foreach (dynamic record in result)
            {
                vehicles.Add(Map(record));
            }
            return vehicles.AsEnumerable();
        }

        public async Task<IEnumerable<Vehicle>> GetRelationshipVehicles(VehicleFilter vehiclefilter)
        {

            var QueryStatement = @"select distinct v.id
                                   ,v.organization_id 
                                   ,v.name 
                                   ,v.vin 
                                   ,v.license_plate_number 
                                   ,v.status 
                                   ,v.status_changed_date 
                                   ,v.termination_date 
                                   ,v.vid 
                                   ,v.type 
                                   ,v.tcu_id 
                                   ,v.tcu_serial_number 
                                   ,v.tcu_brand 
                                   ,v.tcu_version 
                                   ,v.is_tcu_register 
                                   ,v.reference_date 
                                   ,v.vehicle_property_id                                   
                                   ,v.created_at 
                                   ,v.model_id
                                   ,v.opt_in
                                   ,v.is_ota
                                   ,v.oem_id
                                   ,v.oem_organisation_id
                                   ,os.name as relationship
                                   from master.vehicle v
                                   inner join master.orgrelationshipmapping as om on v.id = om.vehicle_id
                                   inner join master.orgrelationship as os on om.relationship_id=os.id 
                                   where 1=1
                                   and os.state='A'
                                   and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
								    else COALESCE(end_date,0) =0 end";
            var parameter = new DynamicParameters();

            // Vehicle Id Filter
            if (vehiclefilter.VehicleId > 0)
            {
                parameter.Add("@id", vehiclefilter.VehicleId);
                QueryStatement = QueryStatement + " and v.id=@id";

            }
            // organization id filter
            if (vehiclefilter.OrganizationId > 0)
            {
                parameter.Add("@organization_id", vehiclefilter.OrganizationId);
                QueryStatement = QueryStatement + " and ((v.organization_id=@organization_id and om.owner_org_id=@organization_id and os.code='Owner') or (om.target_org_id=@organization_id and os.code<>'Owner'))";

            }

            // VIN Id Filter
            if (vehiclefilter.VIN != null && Convert.ToInt32(vehiclefilter.VIN.Length) > 0)
            {
                parameter.Add("@vin", "%" + vehiclefilter.VIN + "%");
                QueryStatement = QueryStatement + " and v.vin LIKE @vin";

            }

            // Vehicle Id list Filter
            if (vehiclefilter.VehicleIdList != null && Convert.ToInt32(vehiclefilter.VehicleIdList.Length) > 0)
            {
                List<int> VehicleIds = vehiclefilter.VehicleIdList.Split(',').Select(int.Parse).ToList();
                QueryStatement = QueryStatement + " and v.id  = ANY(@VehicleIds)";
                parameter.Add("@VehicleIds", VehicleIds);
            }

            if (vehiclefilter.Status != VehicleStatusType.None && vehiclefilter.Status != 0)
            {
                parameter.Add("@status", (char)vehiclefilter.Status);
                QueryStatement = QueryStatement + " and v.status=@status";

            }

            List<Vehicle> vehicles = new List<Vehicle>();
            Vehicle vehicle = new Vehicle();
            dynamic result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);
            foreach (dynamic record in result)
            {
                vehicle = Map(record);
                vehicle.AssociatedGroups = GetVehicleAssociatedGroup(vehicle.ID, Convert.ToInt32(vehicle.Organization_Id)).Result;
                vehicles.Add(vehicle);
            }

            return vehicles.AsEnumerable();
        }

        private async Task<string> GetVehicleAssociatedGroup(int vehicleId, int organizationId)
        {
            try
            {
                var QueryStatement = @"select string_agg(grp.name::text, ',')
                                    from master.vehicle veh left join
                                    master.groupref gref on veh.id= gref.ref_id Left join 
                                    master.group grp on grp.id= gref.group_id
                                    where 1=1 ";
                var parameter = new DynamicParameters();

                // Vehicle Id Filter
                if (vehicleId > 0)
                {
                    parameter.Add("@id", vehicleId);
                    QueryStatement = QueryStatement + " and veh.id  = @id";
                }
                // organization id filter
                if (organizationId > 0)
                {
                    parameter.Add("@organization_id", organizationId);
                    QueryStatement = QueryStatement + " and veh.organization_id = @organization_id";
                }
                string result = await dataAccess.ExecuteScalarAsync<string>(QueryStatement, parameter);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<Vehicle> VehicleNameExists(Vehicle vehicle)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select id from master.vehicle where 1=1 ";
                if (vehicle != null)
                {

                    // id
                    if (Convert.ToInt32(vehicle.ID) > 0)
                    {
                        parameter.Add("@id", vehicle.ID);
                        query = query + " and id!=@id";
                    }
                    // name
                    parameter.Add("@name", vehicle.Name);
                    query = query + " and name=@name and (name!=null Or name!='')";

                    // License Plate Number
                    //if (!string.IsNullOrEmpty(vehicle.License_Plate_Number))
                    //{
                    //    parameter.Add("@license_plate_number", vehicle.License_Plate_Number);
                    //    query = query + " or license_plate_number=@license_plate_number";
                    //}

                    //organization id filter
                    if (vehicle.Organization_Id > 0)
                    {
                        parameter.Add("@organization_id", vehicle.Organization_Id);
                        query = query + " and organization_id=@organization_id ";
                    }
                }
                var vehicleid = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (vehicleid > 0)
                {
                    vehicle.VehicleNameExists = true;
                    vehicle.ID = vehicleid;
                }
                return vehicle;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<Vehicle> VehicleLicensePlateNumberExists(Vehicle vehicle)
        {
            try
            {
                var parameter = new DynamicParameters();
                var query = @"select id from master.vehicle where 1=1 ";
                if (vehicle != null)
                {

                    // id
                    if (Convert.ToInt32(vehicle.ID) > 0)
                    {
                        parameter.Add("@id", vehicle.ID);
                        query = query + " and id!=@id";
                    }
                    // License Plate Number

                    parameter.Add("@license_plate_number", vehicle.License_Plate_Number);
                    query = query + " and license_plate_number=@license_plate_number and (license_plate_number!=null Or license_plate_number!='')";


                }
                var vehicleid = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (vehicleid > 0)
                {
                    vehicle.VehicleLicensePlateNumberExists = true;
                    vehicle.ID = vehicleid;
                }
                return vehicle;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VehicleGroupList>> GetVehicleGroupbyAccountId(int accountid, int orgnizationid)
        {
            try
            {
                //       var QueryStatement = @"select grp.id as VehicleGroupId,grp.name as VehicleGroupName,veh.id as VehicleId,veh.name as VehicleName,veh.vin as Vin
                //                           from master.group grp 
                //inner join master.groupref vgrpref
                //on  grp.id=vgrpref.group_id and grp.object_type='V'                                    
                //inner join master.vehicle veh
                //on vgrpref.ref_id=veh.id
                //where grp.id in( 									
                //select ass.vehicle_group_id from master.accessrelationship ass
                //inner join master.group grp 
                //on ass.account_group_id=grp.id and grp.object_type='A' 
                //inner join master.groupref vgrpref
                //on  grp.id=vgrpref.group_id
                //where vgrpref.ref_id=@accountid)";

                var QueryStatement = @"select grp.id as VehicleGroupId,grp.name as VehicleGroupName,veh.id as VehicleId,veh.name as VehicleName,veh.vin as Vin,veh.license_plate_number as RegNo,
                                    (CASE WHEN sub.vehicle_id >0 AND sub.state='A' THEN true ELSE false END )as SubcriptionStatus
									from master.vehicle veh
                                    left join master.groupref vgrpref
									on vgrpref.ref_id=veh.id 									
									left join master.group grp 
									on  grp.id=vgrpref.group_id and grp.object_type='V'	
									left join master.subscription sub
									on veh.id= sub.vehicle_id
									where veh.id not in (select vgrpref.ref_id from master.groupref vgrpref)
									OR grp.id in( 									
									select ass.vehicle_group_id from master.accessrelationship ass
									inner join master.group grp 
									on ass.account_group_id=grp.id and grp.object_type='A' 
									inner join master.groupref vgrpref
									on  grp.id=vgrpref.group_id
									where vgrpref.ref_id=@accountid) AND veh.status <>'T'
									AND veh.organization_id =@orgnizationid AND sub.state='A'";
                //Start date and end date need to be discuss in subscription

                var parameter = new DynamicParameters();

                parameter.Add("@accountid", accountid);
                parameter.Add("@orgnizationid", orgnizationid);


                IEnumerable<VehicleGroupList> vehiclegrouplist = await dataAccess.QueryAsync<VehicleGroupList>(QueryStatement, parameter);

                return vehiclegrouplist;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<AccountVehicleEntity>> GetORGRelationshipVehicleGroupVehicles(int organizationId, bool is_vehicle)
        {
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                List<AccountVehicleEntity> response = new List<AccountVehicleEntity>();
                // org filter
                if (organizationId > 0 && is_vehicle)
                {
                    query = @"select distinct id,name,count,true as is_group from (
	                                     select vg.id,vg.name,
			                                    case when (vg.group_type ='D' and vg.function_enum='A') then 
						                                    (select count(veh.id) 
									                                    from master.vehicle veh 
									                                    inner join master.orgrelationshipmapping org 
									                                    on veh.id=org.vehicle_id 
									                                    Inner join master.orgrelationship ors
									                                     on ors.id=org.relationship_id
									                                    and ((org.owner_org_id=@organization_id and ors.code='Owner') 
									                                    or (org.target_org_id=@organization_id and ors.code<>'Owner'))
									                                    and ors.state='A'
									                                    and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
									                                    else COALESCE(end_date,0) =0 end)
				                                     when (vg.group_type ='D' and vg.function_enum='V') then 
						                                    (select count(veh.id) 
									                                    from master.vehicle veh 
									                                    inner join master.orgrelationshipmapping org 
									                                    on veh.id=org.vehicle_id 
									                                    inner join master.orgrelationship ors
									                                     on ors.id=org.relationship_id
									                                    and (org.target_org_id=@organization_id and ors.code<>'Owner')
									                                    and ors.state='A'
									                                    and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
									                                    else COALESCE(end_date,0) =0 end)
				                                    when (vg.group_type ='D' and vg.function_enum='O') then 
						                                    (select count(veh.id) 
									                                    from master.vehicle veh 
									                                    inner join master.orgrelationshipmapping org 
									                                    on veh.id=org.vehicle_id 
									                                    inner join master.orgrelationship ors
									                                     on ors.id=org.relationship_id
									                                    and ((org.owner_org_id=@organization_id AND ors.code='Owner') or veh.organization_id=@organization_id)
									                                    and ors.state='A'
									                                    and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
									                                    else COALESCE(end_date,0) =0 end)
		                                    --	else (select count(gr.group_id) from master.groupref gr where gr.group_id=vg.id or gr.group_id=om.vehicle_group_id and ((om.owner_org_id=@organization_id and os.code='Owner') or (om.target_org_id=@organization_id and os.code<>'Owner'))) end as count
			                                    else (select count(gr.group_id) from master.groupref gr where gr.group_id=vg.id or gr.group_id=om.vehicle_group_id  and ((om.owner_org_id=@organization_id and os.code='Owner') or (om.target_org_id=@organization_id and os.code<>'Owner'))) end as count
			                                    from master.group vg 
			                                    left join master.orgrelationshipmapping as om on vg.id = om.vehicle_group_id
			                                    left join master.orgrelationship as os on om.relationship_id=os.id 
			                                    left join master.vehicle v on vg.organization_id=v.organization_id
			                                    where (vg.organization_id=@organization_id or om.target_org_id=@organization_id)
			                                    and vg.object_type='V' and vg.group_type in ('G','D')
			                                    ) vehicleGroup";
                }
                else
                {
                    query = @"select distinct id,name,0 as count,true as is_group from (
                                select vg.id,vg.name
                                from master.group vg
								inner join master.orgrelationshipmapping as om on vg.id = om.vehicle_group_id
								inner join master.orgrelationship as os on om.relationship_id=os.id 
                                where (vg.organization_id=@organization_id or ((om.owner_org_id=@organization_id and os.code='Owner') or (om.target_org_id=@organization_id and os.code<>'Owner')))  and vg.object_type='V' and vg.group_type in ('G','D') 
                                ) vehicleGroup";
                }
                parameter.Add("@organization_id", organizationId);
                IEnumerable<AccountVehicleEntity> accessRelationship = await dataAccess.QueryAsync<AccountVehicleEntity>(query, parameter);
                response = accessRelationship.ToList();
                VehicleFilter vehiclefilter = new VehicleFilter();
                vehiclefilter.OrganizationId = organizationId;
                IEnumerable<Vehicle> vehicles = await GetRelationshipVehicles(vehiclefilter);
                foreach (var item in vehicles.ToList())
                {
                    AccountVehicleEntity accountVehicleEntity = new AccountVehicleEntity();
                    accountVehicleEntity.id = item.ID;
                    accountVehicleEntity.name = item.Name;
                    accountVehicleEntity.count = 0;
                    accountVehicleEntity.is_group = false;
                    accountVehicleEntity.VIN = item.VIN;
                    accountVehicleEntity.RegistrationNo = item.License_Plate_Number;
                    response.Add(accountVehicleEntity);
                }
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Vehicle Data Interface Methods

        public async Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty)
        {
            try
            {
                Vehicle objVeh = new Vehicle();
                var InsertQueryStatement = string.Empty;
                var UpdateQueryStatement = string.Empty;
                int VehiclePropertiesId = await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT vehicle_property_id FROM master.vehicle where vin=@vin), 0)", new { vin = vehicleproperty.VIN });
                int vehicleId = await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.vehicle where vin=@vin), 0)", new { vin = vehicleproperty.VIN });
                int OrgId = await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.organization where lower(name)=@name), null)", new { name = "daf-paccar" });

                vehicleproperty.ID = VehiclePropertiesId;
                objVeh.Organization_Id = OrgId;
                objVeh.VIN = vehicleproperty.VIN;
                objVeh.ModelId = vehicleproperty.Classification_Model_Id;
                objVeh.License_Plate_Number = vehicleproperty.License_Plate_Number;
                objVeh.VehiclePropertiesId = VehiclePropertiesId;
                objVeh.Fuel = vehicleproperty.Fuel;
                objVeh.IPPS = true;
                //dynamic oiedetail = await GetOEM_Id(vehicleproperty.VIN);
                //if (oiedetail != null)
                //{
                //    objVeh.Oem_id = oiedetail[0].id;
                //    objVeh.Oem_Organisation_id = oiedetail[0].oem_organisation_id;
                //}
                //char org_status = await GetOrganisationStatusofVehicle(OrgId);
                //objVeh.Opt_In = VehicleStatusType.Inherit;
                //objVeh.Is_Ota = false;
                //objVeh.Status = (VehicleCalculatedStatus)await GetCalculatedVehicleStatus(org_status, objVeh.Is_Ota);
                if (VehiclePropertiesId > 0)
                {
                    UpdateQueryStatement = @"UPDATE master.vehicleproperties
                                    SET
                                      manufacture_date = @manufacture_date 
                                      ,delivery_date = @delivery_date 
                                      ,make = @make 
                                      ,length = @length 
                                      ,height = @height 
                                      ,weight = @weight 
                                      ,engine_id = @engine_id 
                                      ,engine_type = @engine_type 
                                      ,engine_power = @engine_power 
                                      ,engine_coolant = @engine_coolant 
                                      ,engine_emission_level = @engine_emission_level 
                                      ,chasis_id = @chasis_id 
                                      ,chasis_rear_overhang = @chasis_rear_overhang 
                                      ,driveline_axle_configuration = @driveline_axle_configuration 
                                      ,driveline_wheel_base = @driveline_wheel_base 
                                      ,driveline_tire_size = @driveline_tire_size 
                                      ,transmission_gearbox_id = @transmission_gearbox_id 
                                      ,transmission_gearbox_type = @transmission_gearbox_type 
                                      ,cabin_id = @cabin_id 
                                      ,series_id = @series_id 
                                      ,series_vehicle_range = @series_vehicle_range 
                                      ,model_year = @model_year                                      
                                      ,cabin_type = @cabin_type 
                                      ,cabin_roofspoiler = @cabin_roofspoiler 
                                      ,electronic_control_unit_type = @electronic_control_unit_type 
                                      ,electronic_control_unit_name = @electronic_control_unit_name 
                                      ,weight_type = @weight_type 
                                      ,chasis_side_skirts = @chasis_side_skirts 
                                      ,chasis_side_collar = @chasis_side_collar 
                                      ,width = @width
                                      ,type_id = @type_id
                                      WHERE id = @id
                                      RETURNING id";
                }
                else
                {

                    InsertQueryStatement = @"INSERT INTO master.vehicleproperties
                                      (
                                        manufacture_date 
                                        ,delivery_date 
                                        ,make 
                                        ,length 
                                        ,height 
                                        ,weight 
                                        ,engine_id 
                                        ,engine_type 
                                        ,engine_power 
                                        ,engine_coolant 
                                        ,engine_emission_level 
                                        ,chasis_id 
                                        ,chasis_rear_overhang 
                                        ,driveline_axle_configuration 
                                        ,driveline_wheel_base 
                                        ,driveline_tire_size 
                                        ,transmission_gearbox_id 
                                        ,transmission_gearbox_type 
                                        ,cabin_id 
                                        ,series_id 
                                        ,series_vehicle_range 
                                        ,model_year                                        
                                        ,cabin_type 
                                        ,cabin_roofspoiler 
                                        ,electronic_control_unit_type 
                                        ,electronic_control_unit_name 
                                        ,weight_type 
                                        ,chasis_side_skirts 
                                        ,chasis_side_collar 
                                        ,width
                                        ,type_id) 
                            	VALUES(
                                        @manufacture_date 
                                        ,@delivery_date 
                                        ,@make 
                                        ,@length 
                                        ,@height 
                                        ,@weight 
                                        ,@engine_id 
                                        ,@engine_type 
                                        ,@engine_power 
                                        ,@engine_coolant 
                                        ,@engine_emission_level 
                                        ,@chasis_id 
                                        ,@chasis_rear_overhang 
                                        ,@driveline_axle_configuration 
                                        ,@driveline_wheel_base 
                                        ,@driveline_tire_size 
                                        ,@transmission_gearbox_id 
                                        ,@transmission_gearbox_type 
                                        ,@cabin_id 
                                        ,@series_id 
                                        ,@series_vehicle_range 
                                        ,@model_year                                        
                                        ,@cabin_type 
                                        ,@cabin_roofspoiler 
                                        ,@electronic_control_unit_type 
                                        ,@electronic_control_unit_name 
                                        ,@weight_type 
                                        ,@chasis_side_skirts 
                                        ,@chasis_side_collar 
                                        ,@width
                                        ,@type_id) RETURNING id";

                }

                var parameter = new DynamicParameters();
                if (VehiclePropertiesId > 0)
                {
                    vehicleproperty.ID = VehiclePropertiesId;
                    parameter.Add("@id", vehicleproperty.ID);
                }

                parameter.Add("@manufacture_date", vehicleproperty.ManufactureDate != null && vehicleproperty.ManufactureDate != Convert.ToDateTime("01 - 01 - 0001 00:00:00") ? UTCHandling.GetUTCFromDateTime(vehicleproperty.ManufactureDate.ToString()) : 0);
                parameter.Add("@delivery_date", vehicleproperty.DeliveryDate != null && vehicleproperty.DeliveryDate != Convert.ToDateTime("01 - 01 - 0001 00:00:00") ? UTCHandling.GetUTCFromDateTime(vehicleproperty.DeliveryDate.ToString()) : 0);
                parameter.Add("@make", vehicleproperty.Classification_Make);
                parameter.Add("@length", vehicleproperty.Dimensions_Size_Length);
                parameter.Add("@height", vehicleproperty.Dimensions_Size_Height);
                parameter.Add("@weight", vehicleproperty.Dimensions_Size_Weight_Value);
                parameter.Add("@engine_id", vehicleproperty.Engine_ID);
                parameter.Add("@engine_type", vehicleproperty.Engine_Type);
                parameter.Add("@engine_power", vehicleproperty.Engine_Power);
                parameter.Add("@engine_coolant", vehicleproperty.Engine_Coolant);
                parameter.Add("@engine_emission_level", vehicleproperty.Engine_EmissionLevel);
                parameter.Add("@chasis_id", vehicleproperty.Chassis_Id);
                parameter.Add("@chasis_rear_overhang", vehicleproperty.Chassis_RearOverhang);
                parameter.Add("@driveline_axle_configuration", vehicleproperty.DriverLine_AxleConfiguration);
                parameter.Add("@driveline_wheel_base", vehicleproperty.DriverLine_Wheelbase);
                parameter.Add("@driveline_tire_size", vehicleproperty.DriverLine_Tire_Size);
                parameter.Add("@transmission_gearbox_id", vehicleproperty.GearBox_Id);
                parameter.Add("@transmission_gearbox_type", vehicleproperty.GearBox_Type);
                parameter.Add("@cabin_id", vehicleproperty.DriverLine_Cabin_ID);
                parameter.Add("@series_id", vehicleproperty.Classification_Series_Id);
                parameter.Add("@series_vehicle_range", vehicleproperty.Classification_Series_VehicleRange);
                parameter.Add("@model_year", vehicleproperty.Classification_ModelYear);
                parameter.Add("@cabin_type", vehicleproperty.DriverLine_Cabin_Type);
                parameter.Add("@cabin_roofspoiler", vehicleproperty.DriverLine_Cabin_RoofSpoiler);
                parameter.Add("@electronic_control_unit_type", vehicleproperty.DriverLine_ElectronicControlUnit_Type);
                parameter.Add("@electronic_control_unit_name", vehicleproperty.DriverLine_ElectronicControlUnit_Name);
                parameter.Add("@weight_type", vehicleproperty.Dimensions_Size_Weight_Type);
                parameter.Add("@chasis_side_skirts", vehicleproperty.Chassis_SideSkirts);
                parameter.Add("@chasis_side_collar", vehicleproperty.Chassis_SideCollars);
                parameter.Add("@width", vehicleproperty.Dimensions_Size_Width);
                parameter.Add("@type_id", vehicleproperty.Classification_Type_Id);
                //parameter.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);


                if (VehiclePropertiesId > 0 && vehicleId > 0)
                {
                    await CheckUnknownOEM(objVeh.VIN);
                    await dataAccess.ExecuteAsync("UPDATE master.vehicle SET model_id = @model_id , license_plate_number = @license_plate_number, fuel_type=@fuel_type , modified_at=@modified_at WHERE vin = @vin", new { model_id = objVeh.ModelId, license_plate_number = objVeh.License_Plate_Number, fuel_type = objVeh.Fuel, vin = objVeh.VIN, modified_at = UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()) });
                    vehicleproperty.ID = await dataAccess.ExecuteScalarAsync<int>(UpdateQueryStatement, parameter);
                    objVeh.ID = await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.vehicle where vehicle_property_id=@id), 0)", new { id = vehicleproperty.ID });
                    vehicleproperty.VehicleId = objVeh.ID;
                }
                else if (vehicleId == 0)
                {
                    vehicleproperty.ID = await dataAccess.ExecuteScalarAsync<int>(InsertQueryStatement, parameter);
                    objVeh.VehiclePropertiesId = vehicleproperty.ID;
                    objVeh = await Create(objVeh);
                    vehicleproperty.VehicleId = objVeh.ID;
                }
                else
                {
                    objVeh.ID = vehicleId;
                    vehicleproperty.VehicleId = vehicleId;
                    vehicleproperty.ID = await dataAccess.ExecuteScalarAsync<int>(InsertQueryStatement, parameter);
                    await dataAccess.ExecuteAsync("UPDATE master.vehicle SET vehicle_property_id = @vehicle_property_id, model_id = @model_id , license_plate_number = @license_plate_number, fuel_type=@fuel_type , modified_at=@modified_at WHERE id = @id", new { vehicle_property_id = vehicleproperty.ID, id = objVeh.ID, model_id = objVeh.ModelId, license_plate_number = objVeh.License_Plate_Number, fuel_type = objVeh.Fuel, modified_at = UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()) });

                }

                if (objVeh.ID > 0)
                {
                    if (vehicleproperty.VehicleAxelInformation != null)
                    {
                        //Create axelproperties                
                        await CreateVehicleAxelInformation(vehicleproperty.VehicleAxelInformation, objVeh.ID);
                    }
                    if (vehicleproperty.VehicleFuelTankProperties != null)
                    {
                        //Create Fuel Tank Properties
                        await CreateVehicleFuelTank(vehicleproperty.VehicleFuelTankProperties, objVeh.ID);
                    }

                    VehicleDataMart vehicleDataMart = new VehicleDataMart();
                    vehicleDataMart.VIN = vehicleproperty.VIN;
                    vehicleDataMart.Name = "";
                    vehicleDataMart.Registration_No = vehicleproperty.License_Plate_Number;
                    vehicleDataMart.Vid = "";
                    vehicleDataMart.Engine_Type = vehicleproperty.Engine_Type;
                    vehicleDataMart.Type = vehicleproperty.Classification_Type_Id;
                    vehicleDataMart.Model_Type = vehicleproperty.Classification_Model_Id;
                    vehicleDataMart.IsIPPS = true;
                    await CreateAndUpdateVehicleInDataMart(vehicleDataMart);
                }

                return vehicleproperty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> CreateVehicleAxelInformation(List<VehicleAxelInformation> vehicleaxelinfo, int vehicleId)
        {
            bool is_result = false;
            int VehicleId = await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT count(vehicle_id) FROM master.vehicleaxleproperties where vehicle_id=@vehicle_id), 0)", new { vehicle_id = vehicleId });

            if (VehicleId > 0)
            {
                await DeleteVehicleAxelInformation(vehicleId);
            }
            var QueryStatement = @"INSERT INTO master.vehicleaxleproperties
                                      (
                                        vehicle_id 
                                       ,axle_type 
                                       ,position 
                                       ,type 
                                       ,springs 
                                       ,load 
                                       ,ratio
                                       ,is_wheel_tire_size_replaced
                                       ,size) 
                            	VALUES(                                       
                                        @vehicle_id 
                                       ,@axle_type 
                                       ,@position 
                                       ,@type 
                                       ,@springs 
                                       ,@load 
                                       ,@ratio
                                       ,@is_wheel_tire_size_replaced
                                       ,@size
                                      ) RETURNING id";

            foreach (var axelInfo in vehicleaxelinfo)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vehicle_id", vehicleId);
                parameter.Add("@axle_type", (char)axelInfo.AxelType);
                parameter.Add("@position", axelInfo.Position);
                parameter.Add("@type", axelInfo.Type);
                parameter.Add("@springs", axelInfo.Springs);
                parameter.Add("@load", axelInfo.Load);
                parameter.Add("@ratio", axelInfo.Ratio);
                parameter.Add("@is_wheel_tire_size_replaced", axelInfo.Is_Wheel_Tire_Size_Replaced);
                parameter.Add("@size", axelInfo.Size);
                int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                is_result = true;
            }

            return is_result;
        }

        public async Task<int> DeleteVehicleAxelInformation(int vehicleId)
        {

            var QueryStatement = @"DELETE FROM master.vehicleaxleproperties
                                    Where vehicle_id= @vehicle_id
                                    RETURNING vehicle_id";
            var parameter = new DynamicParameters();
            parameter.Add("@vehicle_id", vehicleId);
            int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

            return vehicleID;
        }

        public async Task<bool> CreateVehicleFuelTank(List<VehicleFuelTankProperties> vehiclefuelTank, int vehicleId)
        {
            bool is_result = false;
            int VehicleId = await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT count(vehicle_id) FROM master.vehiclefueltankproperties where vehicle_id=@vehicle_id), 0)", new { vehicle_id = vehicleId });

            if (VehicleId > 0)
            {
                await DeleteVehicleFuelTank(vehicleId);
            }
            var QueryStatement = @"INSERT INTO master.vehiclefueltankproperties
                                      (
                                        vehicle_id 
                                       ,chasis_fuel_tank_number 
                                       ,chasis_fuel_tank_volume 
                                       ) 
                            	VALUES(                                       
                                        @vehicle_id 
                                       ,@chasis_fuel_tank_number 
                                       ,@chasis_fuel_tank_volume                                        
                                      ) RETURNING id";

            foreach (var fuelTank in vehiclefuelTank)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vehicle_id", vehicleId);
                parameter.Add("@chasis_fuel_tank_number", fuelTank.Chassis_Tank_Nr);
                parameter.Add("@chasis_fuel_tank_volume", fuelTank.Chassis_Tank_Volume);

                int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                is_result = true;
            }

            return is_result;
        }

        public async Task<int> DeleteVehicleFuelTank(int vehicleId)
        {
            var QueryStatement = @"DELETE FROM master.vehiclefueltankproperties
                                    Where vehicle_id= @vehicle_id
                                    RETURNING vehicle_id";
            var parameter = new DynamicParameters();
            parameter.Add("@vehicle_id", vehicleId);
            int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

            return vehicleID;
        }

        private async Task<dynamic> GetOEM_Id(string vin)
        {
            string vin_prefix = vin.Substring(0, 3);
            dynamic result = null;
            var QueryStatement = @"SELECT id, oem_organisation_id
	                             FROM master.oem
                                 where vin_prefix=@vin_prefix";

            var parameter = new DynamicParameters();
            parameter.Add("@vin_prefix", vin_prefix);

            result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

            if (((System.Collections.Generic.List<object>)result).Count == 0)
            {
                var UnkQueryStatement = @"SELECT id, oem_organisation_id
	                             FROM master.oem
                                 where name=@name";
                vin_prefix = "Unknown";
                parameter.Add("@name", vin_prefix);
                result = await dataAccess.QueryAsync<dynamic>(UnkQueryStatement, parameter);
            }


            return result;
        }
        public async Task<char> GetOrganisationStatusofVehicle(int org_id)
        {

            char optin = await dataAccess.QuerySingleAsync<char>("SELECT vehicle_default_opt_in FROM master.organization where id=@id", new { id = org_id });

            return optin;
        }

        public async Task<char> GetCalculatedVehicleStatus(char opt_in, bool is_ota)
        {
            char calVehicleStatus = 'I';
            //Connected
            if (opt_in == (char)VehicleStatusType.OptIn && !is_ota)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.Connected;
            }
            //Off 
            if (opt_in == (char)VehicleStatusType.OptOut && !is_ota)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.Off;
            }
            //Connected + OTA
            if (opt_in == (char)VehicleStatusType.OptIn && is_ota)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.Connected_OTA;
            }
            //OTA only
            if (opt_in == (char)VehicleStatusType.OptOut && is_ota)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.OTA;
            }
            //Terminated
            if (opt_in == (char)VehicleStatusType.Terminate)
            {
                calVehicleStatus = (char)VehicleCalculatedStatus.Terminate;
            }
            return await Task.FromResult(calVehicleStatus);
        }

        public async Task<IEnumerable<VehicleGroup>> GetVehicleGroup(int organizationId, int vehicleId)
        {

            var QueryStatement = @"select veh.id,grp.id as group_id,grp.name 
                                    from master.vehicle veh left join
                                    master.groupref gref on veh.id= gref.ref_id Left join 
                                    master.group grp on grp.id= gref.group_id
                                    where 1=1 ";
            var parameter = new DynamicParameters();

            // Vehicle Id Filter
            if (vehicleId > 0)
            {
                parameter.Add("@id", vehicleId);
                QueryStatement = QueryStatement + " and veh.id  = @id";
            }
            // organization id filter
            if (organizationId > 0)
            {
                parameter.Add("@organization_id", organizationId);
                QueryStatement = QueryStatement + " and veh.organization_id = @organization_id";
            }

            IEnumerable<VehicleGroup> result = await dataAccess.QueryAsync<VehicleGroup>(QueryStatement, parameter);
            return result;
        }

        private async Task<string> CheckUnknownOEM(string VIN)
        {
            dynamic result;
            string VehVIN = string.Empty;
            var QueryStatement = @" select oem.id,oem.oem_organisation_id from master.vehicle veh
                                    Inner join master.oem oem
                                    on veh.oem_id=oem.id
                                    where veh.vin=@vin
                                    and LOWER(oem.name)=LOWER('Unknown')";
            var parameter = new DynamicParameters();
            parameter.Add("@vin", VIN);
            result = await dataAccess.QueryAsync<dynamic>(QueryStatement, parameter);

            if (((System.Collections.Generic.List<object>)result).Count > 0)
            {
                dynamic oiedetail = await GetOEM_Id(VIN);
                if (oiedetail != null)
                {
                    if (oiedetail[0].id != result[0].id)
                    {
                        var OemQueryStatement = @" UPDATE master.vehicle
                                        SET 
                                        oem_id=@oem_id
                                       ,oem_organisation_id=@oem_organisation_id
                                       WHERE vin = @vin
                                       RETURNING vin;";

                        var oemparameter = new DynamicParameters();
                        oemparameter.Add("@oem_id", oiedetail[0].id);
                        oemparameter.Add("@oem_organisation_id", oiedetail[0].oem_organisation_id);
                        oemparameter.Add("@vin", VIN);
                        VehVIN = await dataAccess.ExecuteScalarAsync<string>(OemQueryStatement, oemparameter);
                    }
                }
            }
            return VehVIN;
        }

        #endregion

        #region Vehicle OptIn Opt Out Methods

        public async Task<bool> SetOTAStatus(bool Is_Ota, int Modified_By, int Vehicle_Id)
        {
            bool OTABck = await VehicleOptInOptOutHistory(Vehicle_Id);

            if (OTABck)
            {
                var QueryStatement = @" UPDATE master.vehicle
                                        SET 
                                        is_ota=@is_ota                                        
        	                           ,modified_by=@modified_by
                                        WHERE id = @id
                                        RETURNING id;";

                var parameter = new DynamicParameters();
                parameter.Add("@id", Vehicle_Id);
                parameter.Add("@is_ota", Is_Ota);
                parameter.Add("@modified_by", Modified_By);
                int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                if (vehicleID > 0)
                {

                    char VehOptIn = await dataAccess.QuerySingleAsync<char>("select coalesce((select opt_in FROM master.vehicle where id=@id), 'H')", new { id = Vehicle_Id });

                    if (VehOptIn == 'H')
                    {
                        int VehOrgId = await dataAccess.QuerySingleAsync<int>("select organization_id FROM master.vehicle where id=@id", new { id = Vehicle_Id });
                        VehOptIn = await dataAccess.QuerySingleAsync<char>("select coalesce((select vehicle_default_opt_in FROM master.organization where id=@id), 'U')", new { id = VehOrgId });

                    }

                    char calStatus = await GetCalculatedVehicleStatus(VehOptIn, Is_Ota);
                    await SetConnectionStatus(calStatus, Vehicle_Id);

                }
            }

            return true;
        }

        public async Task<bool> VehicleOptInOptOutHistory(int VehicleId)
        {
            var QueryStatement = @" INSERT INTO master.vehicleoptinoptout(
	                                status
	                                ,organization_id
	                                ,vehicle_id
	                                ,opt_in
	                                ,created_at
	                                ,modified_at
	                                ,is_ota
	                                ,modified_by)
                                SELECT 
	                                status
	                                ,organization_id
	                                ,id
	                                ,opt_in 
	                                ,created_at
	                                ,modified_at  
	                                ,is_ota
	                                ,modified_by
	                                FROM master.vehicle
                                where id=@id
                                RETURNING id";

            var parameter = new DynamicParameters();
            parameter.Add("@id", VehicleId);
            int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            if (vehicleID > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> Terminate(bool Is_Terminate, int Modified_By, int Vehicle_Id)
        {
            bool terminate = await VehicleOptInOptOutHistory(Vehicle_Id);

            if (terminate)
            {
                var QueryStatement = @" UPDATE master.vehicle
                                        SET 
                                        status=@status                                        
        	                           ,modified_by=@modified_by
                                       ,termination_date=@termination_date
                                        WHERE id = @id
                                        RETURNING id;";

                var parameter = new DynamicParameters();
                parameter.Add("@id", Vehicle_Id);
                parameter.Add("@status", (char)VehicleCalculatedStatus.Terminate);
                parameter.Add("@modified_by", Modified_By);
                parameter.Add("@termination_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

            }
            return true;

        }

        public async Task<bool> SetOptInStatus(char Is_OptIn, int Modified_By, int Vehicle_Id)
        {
            await VehicleOptInOptOutHistory(Vehicle_Id);

            var QueryStatement = @" UPDATE master.vehicle
                                        SET 
                                        opt_in=@opt_in                                        
        	                           ,modified_by=@modified_by
                                       ,modified_at=@modified_at
                                        WHERE id = @id
                                        RETURNING id;";

            var parameter = new DynamicParameters();
            parameter.Add("@id", Vehicle_Id);
            parameter.Add("@opt_in", Is_OptIn);
            parameter.Add("@modified_by", Modified_By);
            parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            if (vehicleID > 0)
            {
                bool Is_Ota = await dataAccess.QuerySingleAsync<bool>("select coalesce((SELECT is_ota FROM master.vehicle where id=@id), false)", new { id = Vehicle_Id });

                char calStatus = await GetCalculatedVehicleStatus(Is_OptIn, Is_Ota);

                await SetConnectionStatus(calStatus, Vehicle_Id);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SetConnectionStatus(char Status, int vehicle_Id)
        {
            var QueryStatement = @" UPDATE master.vehicle
                                        SET 
                                        status= @status                                        
        	                           ,modified_by= @modified_by
                                       ,status_changed_date= @status_changed_date
                                        WHERE id = @id
                                        RETURNING id;";

            var parameter = new DynamicParameters();
            parameter.Add("@id", vehicle_Id);
            parameter.Add("@status", Status);
            parameter.Add("@status_changed_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
            int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            if (vehicleID > 0)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Vehicle Data Mart methods

        public async Task<VehicleDataMart> CreateAndUpdateVehicleInDataMart(VehicleDataMart vehicledatamart)
        {
            try
            {
                int VehicleDataMartID = await DataMartdataAccess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.vehicle where vin=@vin), 0)", new { vin = vehicledatamart.VIN });
                var QueryStatement = "";
                bool isNameOrRegUpdated = false;
                var parameter = new DynamicParameters();

                parameter.Add("@name", string.IsNullOrEmpty(vehicledatamart.Name) ? "" : vehicledatamart.Name);
                parameter.Add("@vin", string.IsNullOrEmpty(vehicledatamart.VIN) ? "" : vehicledatamart.VIN);
                parameter.Add("@registration_no", string.IsNullOrEmpty(vehicledatamart.Registration_No) ? "" : vehicledatamart.Registration_No);
                parameter.Add("@type", string.IsNullOrEmpty(vehicledatamart.Type) ? "" : vehicledatamart.Type);
                parameter.Add("@engine_type", string.IsNullOrEmpty(vehicledatamart.Engine_Type) ? "" : vehicledatamart.Engine_Type);
                parameter.Add("@model_type", string.IsNullOrEmpty(vehicledatamart.Model_Type) ? "" : vehicledatamart.Model_Type);
                parameter.Add("@vid", string.IsNullOrEmpty(vehicledatamart.Vid) ? "" : vehicledatamart.Vid);
                parameter.Add("@created_modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now));

                if (VehicleDataMartID == 0)
                {
                    QueryStatement = @"INSERT INTO master.vehicle
                                      (
                                        vin
                                       ,name
                                       ,vid
                                       ,registration_no 
                                       ,type 
                                       ,engine_type
                                       ,model_type
                                       ,created_at
                                       ,modified_at) 
                            	VALUES(
                                       @vin
                                       ,@name
                                       ,@vid
                                       ,@registration_no 
                                       ,@type 
                                       ,@engine_type
                                       ,@model_type
                                       ,@created_modified_at
                                       ,@created_modified_at
                                      ) RETURNING id";
                }
                else if (VehicleDataMartID > 0 && vehicledatamart.IsIPPS == true)
                {
                    //check for old values for vehicle namelist modified_at update
                    parameter.Add("@id", VehicleDataMartID);
                    var vehicleNamelist = await DataMartdataAccess.QueryFirstAsync<VehicleNamelist>
                        ("Select name as Name, registration_no as RegistrationNo from master.vehicle where id=@id", parameter);

                    if (!string.IsNullOrEmpty(vehicleNamelist.RegistrationNo))
                        isNameOrRegUpdated = !vehicleNamelist.RegistrationNo.Equals(vehicledatamart.Registration_No);
                    if (string.IsNullOrEmpty(vehicleNamelist.RegistrationNo) && !string.IsNullOrEmpty(vehicledatamart.Registration_No))
                        isNameOrRegUpdated = true;

                    QueryStatement = @" UPDATE master.vehicle
                                        SET                                  
                                        registration_no=@registration_no
                                        ,type=@type
                                        ,engine_type=@engine_type
                                        ,model_type=@model_type
                                            WHERE id = @id
                                            RETURNING id;";
                }
                else if (VehicleDataMartID > 0 && vehicledatamart.IsIPPS == false && vehicledatamart.Vid == "")
                {
                    //check for old values for vehicle namelist modified_at update
                    parameter.Add("@id", VehicleDataMartID);
                    var vehicleNamelist = await DataMartdataAccess.QueryFirstAsync<VehicleNamelist>
                        ("Select name as Name, registration_no as RegistrationNo from master.vehicle where id=@id", parameter);

                    if (!string.IsNullOrEmpty(vehicleNamelist.RegistrationNo) &&
                        !string.IsNullOrEmpty(vehicleNamelist.Name))
                        isNameOrRegUpdated = (!vehicleNamelist.RegistrationNo.Equals(vehicledatamart.Registration_No) ||
                                          !vehicleNamelist.Name.Equals(vehicledatamart.Name));

                    if ((string.IsNullOrEmpty(vehicleNamelist.RegistrationNo) && !string.IsNullOrEmpty(vehicledatamart.Registration_No)) ||
                        string.IsNullOrEmpty(vehicleNamelist.Name) && !string.IsNullOrEmpty(vehicledatamart.Name))
                        isNameOrRegUpdated = true;

                    QueryStatement = @"UPDATE master.vehicle
                                        SET registration_no=@registration_no, name=@name
                                        WHERE id = @id RETURNING id;";
                }
                else
                {
                    //TCU condition
                    parameter.Add("@id", VehicleDataMartID);
                    QueryStatement = @"UPDATE master.vehicle
                                         SET vid=@vid
                                         WHERE id = @id RETURNING id;";
                }

                int vehicleID = await DataMartdataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);

                if (isNameOrRegUpdated)
                    await DataMartdataAccess.ExecuteScalarAsync<int>(
                                     @"UPDATE master.vehicle
                                     SET                                  
                                     modified_at=@created_modified_at
                                     WHERE id = @id", parameter);

                vehicledatamart.ID = vehicleID;
                return vehicledatamart;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Vehicle Mileage Data
        public async Task<IEnumerable<dtoVehicleMileage>> GetVehicleMileage(long startDate, long endDate, bool noFilter)
        {
            try
            {
                var QueryStatement = @"select 
                                         id                                       
                                        ,modified_at 
                                        ,evt_timestamp
                                        ,odo_mileage
                                        ,odo_distance
                                        ,real_distance
                                        ,vin
                                        from mileage.vehiclemileage";

                DynamicParameters parameter = null;
                if (!noFilter)
                {
                    parameter = new DynamicParameters();
                    parameter.Add("@start_at", startDate);
                    parameter.Add("@end_at", endDate);
                    QueryStatement += " where modified_at >= @start_at AND modified_at <= @end_at";
                }

                IEnumerable<dtoVehicleMileage> mileageData = await DataMartdataAccess.QueryAsync<dtoVehicleMileage>(QueryStatement, parameter);

                return mileageData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<string> GetVehicleVin(int vehicle_id)
        //{
        //    try
        //    {
        //       string vin = await DataMartdataAccess.QuerySingleAsync<string>("select coalesce((select vin FROM master.vehicle where id=@id))", new { id = vehicle_id });
        //       return vin;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //private Vehicles MapMileage(dynamic record)
        //{
        //    string sTimezone = "UTC";
        //    string targetdateformat = "yyyy-MM-ddTHH:mm:ss.fffz";          

        //    Vehicles vehicles=new Vehicles();
        //    vehicles.EvtDateTime = UTCHandling.GetConvertedDateTimeFromUTC(record.evt_timestamp, sTimezone, targetdateformat); 
        //    vehicles.VIN =record.vin;
        //    vehicles.TachoMileage =record.odo_distance;
        //    vehicles.GPSMileage =record.real_distance;
        //    vehicles.RealMileage =record.real_distance;
        //    vehicles.RealMileageAlgorithmVersion ="1.2";
        //    return vehicles;
        //}

        #endregion

        #region Vehicle Namelist Data
        public async Task<IEnumerable<dtoVehicleNamelist>> GetVehicleNamelist(long startDate, long endDate, bool noFilter)
        {
            try
            {
                var QueryStatement = @"select 
                                         vin 
                                        ,name
                                        ,registration_no as regno
                                        from master.vehicle";

                DynamicParameters parameter = null;
                if (!noFilter)
                {
                    parameter = new DynamicParameters();
                    parameter.Add("@start_at", startDate);
                    parameter.Add("@end_at", endDate);
                    QueryStatement += " where modified_at >= @start_at AND modified_at <= @end_at";
                }

                IEnumerable<dtoVehicleNamelist> namelistData = await DataMartdataAccess.QueryAsync<dtoVehicleNamelist>(QueryStatement, parameter);

                return namelistData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Vehicle Visibility

        public async Task<VisibilityVehicle> GetVehicleForVisibility(int Vehicle_Id)
        {
            try
            {
                var QueryStatement = @"select id, vin from master.vehicle where ";
                var parameter = new DynamicParameters();

                // Vehicle Id 
                if (Vehicle_Id > 0)
                {
                    parameter.Add("@id", Vehicle_Id);
                    QueryStatement += " id=@id";
                }
                return await dataAccess.QueryFirstAsync<VisibilityVehicle>(QueryStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VisibilityVehicle>> GetDynamicAllVehicleForVisibility(int OrganizationId)
        {
            try
            {
                var QueryStatement = @"select distinct veh.id, veh.vin	                               
	                                from master.vehicle veh
                                    Inner join master.orgrelationshipmapping  orm
                                    on orm.vehicle_id=veh.id
                                    Inner join master.orgrelationship ors
                                    on ors.id=orm.relationship_id
                                    where  1=1
                                    and ors.state='A'
                                    and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
                                    else COALESCE(end_date,0) =0 end ";
                var parameter = new DynamicParameters();

                // organization id filter
                if (OrganizationId > 0)
                {
                    parameter.Add("@organization_id", OrganizationId);
                    QueryStatement += @" AND ((orm.owner_org_id=@organization_id AND ors.code='Owner')
                                                        OR(orm.target_org_id=@organization_id AND ors.code<>'Owner'))";

                }

                return await dataAccess.QueryAsync<VisibilityVehicle>(QueryStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VisibilityVehicle>> GetDynamicVisibleVehicleForVisibility(int OrganizationId)
        {
            try
            {
                var QueryStatement = @"select distinct veh.id, veh.vin	                               
	                               from master.vehicle veh
                                    Inner join master.orgrelationshipmapping  orm
                                    on orm.vehicle_id=veh.id
                                    Inner join master.orgrelationship ors
                                    on ors.id=orm.relationship_id
                                    where 1=1
                                    and ors.state='A'
                                    and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
                                    else COALESCE(end_date,0) =0 end ";
                var parameter = new DynamicParameters();

                // Organization Id filter
                if (OrganizationId > 0)
                {
                    parameter.Add("@organization_id", OrganizationId);
                    QueryStatement += " and orm.target_org_id=@organization_id AND ors.code<>'Owner'";
                }

                return await dataAccess.QueryAsync<VisibilityVehicle>(QueryStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VisibilityVehicle>> GetDynamicOwnedVehicleForVisibility(int OrganizationId)
        {
            try
            { 
                var QueryStatement = @"select distinct veh.id, veh.vin
	                                    from master.vehicle veh
                                        Left join master.orgrelationshipmapping  orm
                                        on orm.vehicle_id=veh.id
                                        Inner join master.orgrelationship ors
                                        on ors.id=orm.relationship_id
                                            where 1=1  
                                        and ors.state='A'
                                        and case when COALESCE(end_date,0) !=0 then to_timestamp(COALESCE(end_date)/1000)::date>=now()::date 
                                        else COALESCE(end_date,0) =0 end ";

                var parameter = new DynamicParameters();

                // Organization Id filter
                if (OrganizationId > 0)
                {
                    parameter.Add("@organization_id", OrganizationId);
                    QueryStatement += " and ((orm.owner_org_id=@organization_id AND ors.code='Owner') or veh.organization_id=@organization_id)";

                }

                return await dataAccess.QueryAsync<VisibilityVehicle>(QueryStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VisibilityVehicle>> GetDynamicOEMVehiclesForVisibility(int vehicleGroupId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vehicleGroupId", vehicleGroupId);
                var QueryStatement = @"select veh.id, veh.vin	                               
	                                   from master.vehicle veh
                                       INNER JOIN master.group grp ON grp.object_type='V' AND grp.id=@vehicleGroupId
                                       WHERE veh.oem_organisation_id=grp.organization_id";

                return await dataAccess.QueryAsync<VisibilityVehicle>(QueryStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VehicleGroupDetails>> GetVehicleGroupsViaAccessRelationship(int accountId, int orgId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@account_id", accountId);
                parameter.Add("@organization_id", orgId);

                string query =
                    @"SELECT id, group_type as GroupType, function_enum as GroupMethod, ref_id as RefId FROM master.group
                      WHERE id IN
                      (
	                      SELECT arship.vehicle_group_id FROM master.account acc
	                      INNER JOIN master.groupref gref ON acc.id=gref.ref_id AND acc.state='A'
	                      INNER JOIN master.group grp ON (gref.group_id=grp.id OR grp.ref_id=acc.id OR grp.group_type='D') AND grp.object_type='A'
	                      INNER JOIN master.accessrelationship arship ON arship.account_group_id=grp.id 
	                      WHERE acc.id=@account_id AND grp.organization_id=@organization_id
                      )";

                return await dataAccess.QueryAsync<VehicleGroupDetails>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VisibilityVehicle>> GetGroupTypeVehicles(int vehicleGroupId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vehicleGroupId", vehicleGroupId);

                string query =
                                @"select veh.id, veh.vin	                               
	                               from master.vehicle veh 
                                   INNER JOIN master.group grp ON grp.object_type='V' AND grp.id=@vehicleGroupId
                                   INNER JOIN master.groupref gref ON gref.group_id=grp.id AND veh.id=gref.ref_id";

                return await dataAccess.QueryAsync<VisibilityVehicle>(query, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}


