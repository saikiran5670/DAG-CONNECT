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

namespace net.atos.daf.ct2.vehicle.repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly IDataAccess dataAccess;
        public VehicleRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }

        #region Vehicle component methods

        public async Task<Vehicle> Create(Vehicle vehicle)
        {
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
                                      ,model
                                      ,tcu_id
                                      ,tcu_serial_number
                                      ,tcu_brand
                                      ,tcu_version
                                      ,is_tcu_register
                                      ,reference_date) 
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
                                      ,@model
                                      ,@tcu_id
                                      ,@tcu_serial_number
                                      ,@tcu_brand
                                      ,@tcu_version
                                      ,@is_tcu_register
                                      ,@reference_date) RETURNING id";


            var parameter = new DynamicParameters();
            if (vehicle.Organization_Id > 0)
            {
                parameter.Add("@organization_id", vehicle.Organization_Id);
            }
            else
            {
                parameter.Add("@organization_id", null);
            }
            parameter.Add("@name", vehicle.Name);
            parameter.Add("@vin", vehicle.VIN);
            parameter.Add("@license_plate_number", vehicle.License_Plate_Number);
            //parameter.Add("@status", ((char)vehicle.Status).ToString() != null ? (char)vehicle.Status:'P');
            if (vehicle.Organization_Id > 0)
            {
                bool QrgOptStatus = await dataAccess.QuerySingleAsync<bool>("SELECT optout_status FROM master.organization where id=@id", new { id = vehicle.Organization_Id });
                parameter.Add("@status", QrgOptStatus == false ? (char)VehicleStatusType.OptIn : (char)VehicleStatusType.OptOut);
            }
            else
            {
                parameter.Add("@status", (char)VehicleStatusType.OptIn);
            }
            parameter.Add("@status_changed_date", (vehicle.Status_Changed_Date != null && DateTime.Compare(DateTime.MinValue, vehicle.Status_Changed_Date) > 0) ? UTCHandling.GetUTCFromDateTime(vehicle.Status_Changed_Date.ToString()) : 0);
            parameter.Add("@termination_date", (vehicle.Termination_Date != null && DateTime.Compare(DateTime.MinValue, vehicle.Termination_Date) > 0)  ? UTCHandling.GetUTCFromDateTime(vehicle.Termination_Date.ToString()) : 0);
            parameter.Add("@vid", vehicle.Vid);
            parameter.Add("@type", (char)vehicle.Type);
            parameter.Add("@model", vehicle.Model);
            parameter.Add("@tcu_id", vehicle.Tcu_Id);
            parameter.Add("@tcu_serial_number", vehicle.Tcu_Serial_Number);
            parameter.Add("@tcu_brand", vehicle.Tcu_Brand);
            parameter.Add("@tcu_version", vehicle.Tcu_Version);
            parameter.Add("@is_tcu_register", vehicle.Is_Tcu_Register);
            parameter.Add("@reference_date", (vehicle.Reference_Date != null && DateTime.Compare(DateTime.MinValue, vehicle.Termination_Date) > 0)  ? UTCHandling.GetUTCFromDateTime(vehicle.Reference_Date.ToString()) : 0);
           
            parameter.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            vehicle.ID = vehicleID;
            return vehicle;
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
                                    ,model
                                    ,vid
                                    ,type
                                    ,tcu_id
                                    ,tcu_serial_number
                                    ,tcu_brand
                                    ,tcu_version
                                    ,is_tcu_register
                                    ,reference_date
                                    from master.vehicle 
                                    where 1=1";
            var parameter = new DynamicParameters();

            // Vehicle Id Filter
            if (vehiclefilter.VehicleId > 0)
            {
                parameter.Add("@id", vehiclefilter.VehicleId);
                QueryStatement = QueryStatement + " and id  = @id";

            }
            // organization id filter
            if (vehiclefilter.OrganizationId > 0)
            {
                parameter.Add("@organization_id", vehiclefilter.OrganizationId);
                QueryStatement = QueryStatement + " and organization_id  = @organization_id";

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
                QueryStatement = QueryStatement + " and status  = @status";

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
            vehicle.Status = (VehicleStatusType)(Convert.ToChar(record.status));
            if (record.status_changed_date != null)
                vehicle.Status_Changed_Date = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.status_changed_date,"America/New_York", "yyyy-MM-ddTHH:mm:ss"));
            if (record.termination_date != null)
                vehicle.Termination_Date = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.termination_date, "Africa/Mbabane", "yyyy-MM-ddTHH:mm:ss"));
            vehicle.Model = record.model;
            vehicle.Vid = record.vid;
            vehicle.Type = (VehicleType)(Convert.ToChar(record.type == null ? 'N' : record.type));
            vehicle.Tcu_Id = record.tcu_id;
            vehicle.Tcu_Serial_Number = record.tcu_serial_number;
            vehicle.Tcu_Brand = record.tcu_brand;
            vehicle.Tcu_Version = record.tcu_version;
            vehicle.Is_Tcu_Register = record.is_tcu_register == null ? false : record.is_tcu_register;
            if (record.reference_date != null)
                vehicle.Reference_Date = Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(record.reference_date, "Asia/Dubai", "yyyy-MM-ddTHH:mm:ss"));
            return vehicle;
        }

        public async Task<Vehicle> Update(Vehicle vehicle)
        {
            if (vehicle.Tcu_Id == null || vehicle.Tcu_Id.Length == 0)
            {
                var QueryStatement = @" UPDATE master.vehicle
                                        SET 
                                         name=@name                                        
        	                            ,license_plate_number=@license_plate_number
                                        WHERE id = @id
                                         RETURNING id;";

                var parameter = new DynamicParameters();
                parameter.Add("@id", vehicle.ID);
                // parameter.Add("@organization_id", vehicle.Organization_Id);
                parameter.Add("@name", vehicle.Name);
                // parameter.Add("@vin", vehicle.VIN);
                parameter.Add("@license_plate_number", vehicle.License_Plate_Number);
                // parameter.Add("@status", (char)vehicle.Status);
                // parameter.Add("@status_changed_date", vehicle.Status_Changed_Date != null ? UTCHandling.GetUTCFromDateTime(vehicle.Status_Changed_Date.ToString()) : 0);
                // parameter.Add("@termination_date", vehicle.Termination_Date != null ? UTCHandling.GetUTCFromDateTime(vehicle.Termination_Date.ToString()) : 0);
                int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            }
            else
            {
                var QueryStatement = @" UPDATE master.vehicle
                                        SET 
                                        vid=@vid
                                      ,tcu_id=@tcu_id
                                      ,tcu_serial_number=@tcu_serial_number
                                      ,tcu_brand=@tcu_brand
                                      ,tcu_version=@tcu_version
                                      ,is_tcu_register=@is_tcu_register
                                      ,reference_date=@reference_date
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
                string VIN = await dataAccess.ExecuteScalarAsync<string>(QueryStatement, parameter);
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
                                                WHERE organization_id = @id
                                                RETURNING id;";

                parameter.Add("@id", vehicleOptInOptOut.RefId);
            }

            parameter.Add("@status", (char)vehicleOptInOptOut.Status);
            parameter.Add("@status_changed_date", vehicleOptInOptOut.Date != null ? UTCHandling.GetUTCFromDateTime(vehicleOptInOptOut.Date) : 0);
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
                OptInOptOutparameter.Add("@status_changed_date", vehicleOptInOptOut.Date != null ? UTCHandling.GetUTCFromDateTime(vehicleOptInOptOut.Date) : 0);
                OptInOptOutparameter.Add("@type", (char)vehicleOptInOptOut.Type);

                int Id = await dataAccess.ExecuteScalarAsync<int>(InsertQueryStatement, OptInOptOutparameter);
            }
            return vehicleOptInOptOut;
        }

        #endregion



        #region Vehicle Data Interface Methods

        public async Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty)
        {
            Vehicle objVeh=new Vehicle();
            var InsertQueryStatement = string.Empty;
            var UpdateQueryStatement = string.Empty;
            int VehicleId = await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.vehicle where vin=@vin), 0)", new { vin = vehicleproperty.VIN });
            int OrgId= await dataAccess.QuerySingleAsync<int>("select coalesce((SELECT id FROM master.organization where org_id=@org_id), null)", new { org_id = vehicleproperty.Org_Id });

            vehicleproperty.VehicleId=VehicleId;
            objVeh.Organization_Id=OrgId;
            objVeh.VIN=vehicleproperty.VIN;
            objVeh.Model=vehicleproperty.Classification_Model;
            objVeh.License_Plate_Number=vehicleproperty.License_Plate_Number;
            objVeh.Type=(VehicleType)vehicleproperty.Classification_Type;
            objVeh.ID=VehicleId;

            var parameter = new DynamicParameters();
            if (VehicleId > 0)
            {
                
              

                UpdateQueryStatement = @" UPDATE master.vehicleproperties
                                    SET
                                      manufacture_date = @manufacture_date
                                      ,registration_date = @registration_date
                                      ,delivery_date = @delivery_date
                                      ,make = @make                                     
                                      ,series = @series                                      
                                      ,length = @length
                                      ,widht = @widht
                                      ,height = @height
                                      ,weight = @weight
                                      ,engine_id = @engine_id
                                      ,engine_type = @engine_type
                                      ,engine_power = @engine_power
                                      ,engine_coolant = @engine_coolant
                                      ,engine_emission_level = @engine_emission_level
                                      ,chasis_id = @chasis_id
                                      ,is_chasis_side_skirts = @is_chasis_side_skirts
                                      ,is_chasis_side_collar = @is_chasis_side_collar
                                      ,chasis_rear_overhang = @chasis_rear_overhang
                                      ,chasis_fuel_tank_number = @chasis_fuel_tank_number
                                      ,chasis_fuel_tank_volume = @chasis_fuel_tank_volume
                                      ,driveline_axle_configuration = @driveline_axle_configuration
                                      ,driveline_wheel_base = @driveline_wheel_base
                                      ,driveline_tire_size = @driveline_tire_size
                                      ,driveline_front_axle_position = @driveline_front_axle_position
                                      ,driveline_front_axle_load = @driveline_front_axle_load
                                      ,driveline_rear_axle_position = @driveline_rear_axle_position
                                      ,driveline_rear_axle_load = @driveline_rear_axle_load
                                      ,driveline_rear_axle_ratio = @driveline_rear_axle_ratio
                                      ,transmission_gearbox_id = @transmission_gearbox_id
                                      ,transmission_gearbox_type = @transmission_gearbox_type
                                      ,cabin_id = @cabin_id
                                      ,cabin_color_id = @cabin_color_id
                                      ,cabin_color_value = @cabin_color_value
                                       WHERE vehicle_id = @vehicle_id
                                       RETURNING vehicle_id";
            }
            else
            {

                objVeh = await Create(objVeh);

                InsertQueryStatement = @"INSERT INTO master.vehicleproperties
                                      (
                                       vehicle_id
                                      ,manufacture_date
                                      ,registration_date
                                      ,delivery_date
                                      ,make                                      
                                      ,series                                      
                                      ,length
                                      ,widht
                                      ,height
                                      ,weight
                                      ,engine_id
                                      ,engine_type
                                      ,engine_power
                                      ,engine_coolant
                                      ,engine_emission_level
                                      ,chasis_id
                                      ,is_chasis_side_skirts
                                      ,is_chasis_side_collar
                                      ,chasis_rear_overhang
                                      ,chasis_fuel_tank_number
                                      ,chasis_fuel_tank_volume
                                      ,driveline_axle_configuration
                                      ,driveline_wheel_base
                                      ,driveline_tire_size
                                      ,driveline_front_axle_position
                                      ,driveline_front_axle_load
                                      ,driveline_rear_axle_position
                                      ,driveline_rear_axle_load
                                      ,driveline_rear_axle_ratio
                                      ,transmission_gearbox_id
                                      ,transmission_gearbox_type
                                      ,cabin_id
                                      ,cabin_color_id
                                      ,cabin_color_value) 
                            	VALUES(
                                       @vehicle_id
                                      ,@manufacture_date
                                      ,@registration_date
                                      ,@delivery_date
                                      ,@make                                      
                                      ,@series                                     
                                      ,@length
                                      ,@widht
                                      ,@height
                                      ,@weight
                                      ,@engine_id
                                      ,@engine_type
                                      ,@engine_power
                                      ,@engine_coolant
                                      ,@engine_emission_level
                                      ,@chasis_id
                                      ,@is_chasis_side_skirts
                                      ,@is_chasis_side_collar
                                      ,@chasis_rear_overhang
                                      ,@chasis_fuel_tank_number
                                      ,@chasis_fuel_tank_volume
                                      ,@driveline_axle_configuration
                                      ,@driveline_wheel_base
                                      ,@driveline_tire_size
                                      ,@driveline_front_axle_position
                                      ,@driveline_front_axle_load
                                      ,@driveline_rear_axle_position
                                      ,@driveline_rear_axle_load
                                      ,@driveline_rear_axle_ratio
                                      ,@transmission_gearbox_id
                                      ,@transmission_gearbox_type
                                      ,@cabin_id
                                      ,@cabin_color_id
                                      ,@cabin_color_value) RETURNING id";

            }
           
            parameter.Add("@vehicle_id", objVeh.ID);
            parameter.Add("@manufacture_date", vehicleproperty.ManufactureDate != null ? UTCHandling.GetUTCFromDateTime(vehicleproperty.ManufactureDate.ToString()) : 0);
            parameter.Add("@registration_date", vehicleproperty.RegistrationDateTime != null ? UTCHandling.GetUTCFromDateTime(vehicleproperty.RegistrationDateTime.ToString()) : 0);
            parameter.Add("@delivery_date", vehicleproperty.DeliveryDate != null ? UTCHandling.GetUTCFromDateTime(vehicleproperty.DeliveryDate.ToString()) : 0);
            parameter.Add("@make", vehicleproperty.Classification_Make);
            // parameter.Add("@model", vehicleproperty.Classification_Model);
            parameter.Add("@series", vehicleproperty.Classification_Series);
            // parameter.Add("@type", vehicleproperty.Classification_Type);
            parameter.Add("@length", vehicleproperty.Dimensions_Size_Length);
            parameter.Add("@widht", vehicleproperty.Dimensions_Size_Width);
            parameter.Add("@height", vehicleproperty.Dimensions_Size_Height);
            parameter.Add("@weight", vehicleproperty.Dimensions_Size_Weight);
            parameter.Add("@engine_id", vehicleproperty.Engine_ID);
            parameter.Add("@engine_type", vehicleproperty.Engine_Type);
            parameter.Add("@engine_power", vehicleproperty.Engine_Power);
            parameter.Add("@engine_coolant", vehicleproperty.Engine_Coolant);
            parameter.Add("@engine_emission_level", vehicleproperty.Engine_EmissionLevel);
            parameter.Add("@chasis_id", vehicleproperty.Chasis_Id);
            parameter.Add("@is_chasis_side_skirts", vehicleproperty.SideSkirts);
            parameter.Add("@is_chasis_side_collar", vehicleproperty.SideCollars);
            parameter.Add("@chasis_rear_overhang", vehicleproperty.RearOverhang);
            parameter.Add("@chasis_fuel_tank_number", vehicleproperty.Tank_Nr);
            parameter.Add("@chasis_fuel_tank_volume", vehicleproperty.Tank_Volume);
            parameter.Add("@driveline_axle_configuration", vehicleproperty.DriverLine_AxleConfiguration);
            parameter.Add("@driveline_wheel_base", vehicleproperty.DriverLine_Wheelbase);
            parameter.Add("@driveline_tire_size", vehicleproperty.DriverLine_Tire_Size);
            parameter.Add("@driveline_front_axle_position", vehicleproperty.DriverLine_FrontAxle_Position);
            parameter.Add("@driveline_front_axle_load", vehicleproperty.DriverLine_FrontAxle_Load);
            parameter.Add("@driveline_rear_axle_position", vehicleproperty.DriverLine_RearAxle_Position);
            parameter.Add("@driveline_rear_axle_load", vehicleproperty.DriverLine_RearAxle_Load);
            parameter.Add("@driveline_rear_axle_ratio", vehicleproperty.DriverLine_RearAxle_Ratio);
            parameter.Add("@transmission_gearbox_id", vehicleproperty.GearBox_Id);
            parameter.Add("@transmission_gearbox_type", vehicleproperty.GearBox_Type);
            parameter.Add("@cabin_id", vehicleproperty.DriverLine_Cabin_ID);
            parameter.Add("@cabin_color_id", vehicleproperty.DriverLine_Cabin_Color_ID);
            parameter.Add("@cabin_color_value", vehicleproperty.DriverLine_Cabin_Color_Value);
            parameter.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

            
            if (VehicleId > 0)
            {
                await dataAccess.ExecuteAsync("UPDATE master.vehicle SET model = @model ,type = @type,license_plate_number = @license_plate_number WHERE vin = @vin",new {model=objVeh.Model, type=(char)objVeh.Type,license_plate_number=objVeh.License_Plate_Number,vin=objVeh.VIN});
                vehicleproperty.ID = await dataAccess.ExecuteScalarAsync<int>(UpdateQueryStatement, parameter);
            }
            else
            {
                vehicleproperty.ID = await dataAccess.ExecuteScalarAsync<int>(InsertQueryStatement, parameter);
            }
            return vehicleproperty;
        }

        


        #endregion


        //     public async Task<int> AddVehicle(Vehicle vehicle)
        //     {  
        //         var QueryStatement = @"INSERT INTO dafconnectmaster.vehicle
        //                               (vin,
        //                               registrationno,
        //                               chassisno,
        //                               isactive,
        //                               createddate,
        //                               createdby) 
        //                     	VALUES(@vin,
        //                               @registrationno,
        //                               @chassisno,
        //                               @isactive,
        //                               @createddate,
        //                               @createdby) RETURNING vehicleid";

        //              var parameter = new DynamicParameters();
        //              parameter.Add("@vin", vehicle.VIN);
        //              parameter.Add("@registrationno", vehicle.RegistrationNo);
        //              parameter.Add("@chassisno", vehicle.ChassisNo);
        //              parameter.Add("@isactive", vehicle.IsActive);
        //              parameter.Add("@createddate",  DateTime.Now);
        //              parameter.Add("@createdby",  vehicle.CreatedBy);
        //              parameter.Add("@vehicleidret",dbType:DbType.Int32 ,direction: ParameterDirection.InputOutput);
        //              int resultAddVehicle = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
        //              return resultAddVehicle;          
        //     }


        //    public async Task<int> AddVehicleGroup(VehicleGroup vehicleGroup)
        //     {         
        //       try{
        //            var QueryStatement = @"INSERT INTO dafconnectmaster.vehiclegroup
        //                                         (organizationid,
        //                                         name,
        //                                         description,
        //                                         isactive,
        //                                         isdefaultgroup,
        //                                         isuserdefinedgroup,
        //                                         createddate,
        //                                         createdby) 
        //                                 VALUES (@organizationid,
        //                                         @name,
        //                                         @description,
        //                                         @isactive,
        //                                         @isdefaultgroup,
        //                                         @isuserdefinedgroup,
        //                                         @createddate,
        //                                         @createdby) RETURNING vehiclegroupid";

        //              var parameter = new DynamicParameters();
        //              parameter.Add("@organizationid", vehicleGroup.OrganizationID);
        //              parameter.Add("@name", vehicleGroup.Name); 
        //               parameter.Add("@description", vehicleGroup.Description);              
        //              parameter.Add("@isactive", vehicleGroup.IsActive);
        //              parameter.Add("@isdefaultgroup", vehicleGroup.IsDefaultGroup);
        //              parameter.Add("@isuserdefinedgroup", vehicleGroup.IsUserDefindGroup);
        //              parameter.Add("@createddate",  DateTime.Now);
        //              parameter.Add("@createdby", vehicleGroup.CreatedBy);    

        //             int resultAddVehicleGroupID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
        //             var vehicleorgIDs=vehicleGroup.VehicleOrgIds;

        //              if(vehicleorgIDs.Length>0)
        //              {
        //                 int[] vehIDs = Array.ConvertAll(vehicleorgIDs.Split(','), int.Parse);
        //                 if (vehIDs.Length>0)
        //                 {
        //                     int vehOrgId;

        //                     for(int i=0; i<vehIDs.Length; i++)
        //                     {
        //                      vehOrgId = Convert.ToInt32(vehIDs[i]);

        //                      var QueryStatementMapping = @"INSERT INTO dafconnectmaster.vehiclegroupvehiclemapping
        //                                         (vehiclegroupid,
        //                                         vehicleorgid,
        //                                         isactive,
        //                                         createddate,createdby)                                     
        //                                        VALUES (@vehgroupid,@vehorgid,@isactive,@createddate,@createdby) RETURNING vehiclegroupvehiclemappingid";                        
        //                       var parameterVehicleOrg = new DynamicParameters();
        //                       parameterVehicleOrg.Add("@vehgroupid", resultAddVehicleGroupID);
        //                       parameterVehicleOrg.Add("@vehorgid", vehOrgId);              
        //                       parameterVehicleOrg.Add("@isactive", vehicleGroup.IsActive);
        //                       parameterVehicleOrg.Add("@createddate", DateTime.Now);
        //                       parameterVehicleOrg.Add("@createdby", vehicleGroup.CreatedBy);
        //                       int resultAddVehicleGroupMappingID = await dataAccess.ExecuteScalarAsync<int>(QueryStatementMapping, parameterVehicleOrg);                       

        //                     }
        //                 }
        //              }              
        //              return resultAddVehicleGroupID;
        //         }
        //         catch (System.Exception ex)
        //         {
        //           throw ex;             
        //         }
        //     }

        //     public async Task<int>  UpdateVehicle(Vehicle vehicle)
        //     {
        //          var QueryStatement = @" UPDATE dafconnectmaster.vehicle
        //                                 SET vin=@vin,
        // 	                            registrationno=@registrationno,
        //                             	chassisno=@chassisno,
        //                             	terminationdate=@terminationdate,
        // 	                            isactive=@isactive,
        // 	                            updatedby=@updatedby,
        // 	                            updateddate=@updateddate
        //                                 WHERE vehicleid = @vehicleid;";

        //              var parameter = new DynamicParameters();
        //              parameter.Add("@vehicleid", vehicle.VehicleID);
        //              parameter.Add("@vin", vehicle.VIN);
        //              parameter.Add("@registrationno", vehicle.RegistrationNo);
        //              parameter.Add("@chassisno", vehicle.ChassisNo);
        //              parameter.Add("@terminationdate", vehicle.TerminationDate);
        //              parameter.Add("@isactive", vehicle.IsActive);
        //              parameter.Add("@updatedby", vehicle.UpdatedBy);
        //              parameter.Add("@updateddate", DateTime.Now);

        //              int resultupdateVehicle = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
        //              return resultupdateVehicle;         
        //     }

        //     public async Task<int> UpdateVehicleGroup(VehicleGroup vehicleGroup)
        //      {

        //               var QueryStatement = @" UPDATE dafconnectmaster.vehicleGroup
        //                                     SET name=@grpname,
        //                                     description=@description,
        // 	                                organizationid=@orgid,
        // 	                                isactive=@active,
        //                                 	updatedby=@updated,		
        //                                 	updateddate=@updateddate,
        //                                 	isdefaultgroup=@defaultgroup,
        // 	                                isuserdefinedgroup=@userdefinedgroup                                        
        //                                     WHERE vehiclegroupid = @vehiclegrpid;";
        //              var parameter = new DynamicParameters();
        //              parameter.Add("@vehiclegrpid", vehicleGroup.VehicleGroupID);
        //              parameter.Add("@orgid", vehicleGroup.OrganizationID);
        //              parameter.Add("@grpname", vehicleGroup.Name); 
        //              parameter.Add("@description", vehicleGroup.Description);                        
        //              parameter.Add("@active", vehicleGroup.IsActive);
        //              parameter.Add("@updated", vehicleGroup.UpdatedBy);
        //              parameter.Add("@defaultgroup", vehicleGroup.IsDefaultGroup);
        //              parameter.Add("@userdefinedgroup", vehicleGroup.IsUserDefindGroup);
        //              parameter.Add("@updateddate", DateTime.Now);

        //              int resultUpdateVehicleGroupID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);   

        //              var vehicleorgIDs=vehicleGroup.VehicleOrgIds;
        //              if(vehicleorgIDs.Length>0)
        //              {

        //                 int[] vehIDs = Array.ConvertAll(vehicleorgIDs.Split(','), int.Parse);
        //                 if (vehIDs.Length>0)
        //                 {                     

        //                     //   var parameterDeleteVehicMapping = new DynamicParameters();
        //                     //   parameterDeleteVehicMapping.Add("@vehgroupid", vehicleGroup.VehicleGroupID);
        //                     //   parameterDeleteVehicMapping.Add("@orgid", vehicleGroup.OrganizationID);    
        //                     //   parameterDeleteVehicMapping.Add("@val",dbType:DbType.Int32 ,direction: ParameterDirection.InputOutput);
        //                     //   dataAccess.QuerySingle<int>(@"CALL dafconnectmaster.deletevehiclegroupvehiclemapping(@vehgroupid,@orgid,@val)",parameterDeleteVehicMapping);

        //                     for(int i=0; i<vehIDs.Length; i++)
        //                     {
        //                       int vehOrgId = Convert.ToInt32(vehIDs[i]);
        //                       var QueryStatementMapping = @" INSERT INTO dafconnectmaster.vehiclegroupvehiclemapping
        //                                         (vehiclegroupid,
        //                                         vehicleorgid,
        //                                         isactive,
        //                                         createddate,
        //                                         createdby) 
        //                                  VALUES (@vehgroupid,
        //                                         @vehorgid,
        //                                         @isactive,
        //                                         @createddate,
        //                                         @createdby) RETURNING vehiclegroupvehiclemappingid";                        
        //                       var parameterVehicleOrg = new DynamicParameters();
        //                       parameterVehicleOrg.Add("@vehgroupid", resultUpdateVehicleGroupID);
        //                       parameterVehicleOrg.Add("@vehorgid", vehOrgId);              
        //                       parameterVehicleOrg.Add("@isactive", vehicleGroup.IsActive);
        //                       parameterVehicleOrg.Add("@createddate", DateTime.Now);
        //                       parameterVehicleOrg.Add("@createdby", vehicleGroup.CreatedBy);
        //                      int resultUpdateVehicleGroupMappingID = await dataAccess.ExecuteScalarAsync<int>(QueryStatementMapping, parameterVehicleOrg);                       

        //                     }
        //                 }
        //              }
        //              return resultUpdateVehicleGroupID;                        
        //     }

        //      public async Task<int> DeleteVehicle(int vehicleid, int updatedby)
        //     {
        //            var QueryStatement = @"UPDATE dafconnectmaster.vehicle
        //                                          SET isactive=@isactive,
        // 	                                     updatedby=@updatedby,
        // 	                                     updateddate=@updateddate  
        //                                          WHERE vehicleid = @vehicleid
        //                                          RETURNING vehicleid;";
        //         var parameter = new DynamicParameters();
        //         parameter.Add("@vehicleid", vehicleid);
        //         parameter.Add("@isactive", false);
        //         parameter.Add("@updatedby", updatedby);
        //         parameter.Add("@updateddate", DateTime.Now);

        //         int resultDeleteVehicle = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
        //         return resultDeleteVehicle;
        //     }       
        //     public async Task<int> DeleteVehicleGroup(int vehicleGroupid, int updatedby)
        //     {
        //         var parameter = new DynamicParameters();
        //         parameter.Add("@vehgroupid", vehicleGroupid);
        //         parameter.Add("@isactive", false);
        //         parameter.Add("@updatedby", updatedby);
        //         parameter.Add("@updateddate", DateTime.Now);
        //         var QueryStatement = @"UPDATE dafconnectmaster.vehicleGroup
        //                                 SET isactive=@isactive,
        // 	                            updatedby=@updatedby,
        // 	                            updateddate=@updateddate
        //                                 WHERE vehiclegroupid = @vehgroupid
        //                                 RETURNING vehiclegroupid;";          

        //         int resultDeleteVehicleGroup = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
        //         return resultDeleteVehicleGroup;             
        //     }
        //     public async Task<IEnumerable<Vehicle>> GetVehicleByID(int vehicleid,int orgid)
        //     {
        //         var QueryStatement = @"select veh.vin,veh.registrationno,veh.chassisno,veh.model,vorg.name
        //                              from dafconnectmaster.vehiclegroupvehiclemapping vgvm
        //                              left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
        //                              left join dafconnectmaster.vehicle veh on vorg.vehicleid=veh.vehicleid
        //                              where vorg.organizationid=@orgid and veh.vehicleid=@vehicleid and vorg.optoutstatus=false and vorg.isactive=true;";                    
        //         var parameter = new DynamicParameters();
        //         parameter.Add("@vehicleid", vehicleid);
        //         parameter.Add("@orgid", orgid);
        //         IEnumerable<Vehicle> VehicleDetails = await dataAccess.QueryAsync<Vehicle>(QueryStatement, parameter);
        //         return VehicleDetails;
        //     }

        //     public async Task<IEnumerable<VehicleGroup>> GetVehicleGroupByID(int vehicleGroupID,int orgid)
        //     {
        //         var QueryStatement = @"select veh.vin,veh.registrationno,veh.chassisno,veh.model,vorg.name 
        //                              from dafconnectmaster.vehiclegroupvehiclemapping vgvm
        //                              left join dafconnectmaster.vehiclegroup vehgrp on vgvm.vehiclegroupid=vehgrp.vehiclegroupid
        //                              left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
        //                              left join dafconnectmaster.vehicle veh on vorg.vehicleid=veh.vehicleid
        //                              where vehgrp.vehiclegroupid=@vehicleGroupID and vorg.organizationid=@orgid
        //                              and vorg.optoutstatus=false and vorg.isactive=true;";

        //         var parameter = new DynamicParameters();
        //         parameter.Add("@vehicleGroupID", vehicleGroupID);
        //         parameter.Add("@orgid", orgid);
        //         IEnumerable<VehicleGroup> VehicleOrgDetails = await dataAccess.QueryAsync<VehicleGroup>(QueryStatement, parameter);
        //         return VehicleOrgDetails;
        //     }

        //     // public async Task<IEnumerable<VehicleGroup>> GetAllVehicleGroups(int orgid)
        //     // {
        //     //     var QueryStatement = @" SELECT vehiclegrp.vehiclegroupid, 
        // 	// 			 vehiclegrp.organizationid,
        // 	// 			 vehiclegrp.name,
        //     //              vehiclegrp.description,
        // 	// 			 vehiclegrp.isactive,
        // 	// 			 vehiclegrp.createddate,
        // 	// 			 vehiclegrp.createdby,
        // 	// 			 vehiclegrp.updateddate,
        // 	// 			 vehiclegrp.updatedby,
        // 	// 			 vehiclegrp.isdefaultgroup,
        // 	// 			 vehiclegrp.isuserdefinedgroup					 
        // 	// 			FROM dafconnectmaster.vehiclegroup vehiclegrp
        // 	// 			WHERE vehiclegrp.organizationid=@orgid
        // 	// 			and vehiclegrp.isactive=true;";                    
        //     //     var parameter = new DynamicParameters();
        //     //     parameter.Add("@orgid", orgid);
        //     //     IEnumerable<VehicleGroup> VehicleOrgDetails = await dataAccess.QueryAsync<VehicleGroup>(QueryStatement, parameter);
        //     //     return VehicleOrgDetails;
        //     // }
        //     // public async Task<IEnumerable<Vehicle>> GetAllVehicles(int orgid)
        //     // {
        //     //     var QueryStatement = @"select 
        //     //     veh.vin,
        //     //     veh.registrationno,
        //     //     veh.chassisno,
        //     //     veh.model,
        //     //     vorg.name
        //     //     from dafconnectmaster.vehiclegroupvehiclemapping vgvm
        //     //     left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
        //     //     left join dafconnectmaster.vehicle veh on vorg.vehicleid=veh.vehicleid
        //     //     where vorg.organizationid=@orgid and vorg.optoutstatus=false and vorg.isactive=true;";                    
        //     //     var parameter = new DynamicParameters();
        //     //     parameter.Add("@orgid", orgid);
        //     //     IEnumerable<Vehicle> VehicleDetails = await dataAccess.QueryAsync<Vehicle>(QueryStatement, parameter);
        //     //     return VehicleDetails;
        //     // }



        //     public async Task<IEnumerable<VehicleGroup>> GetVehicleGroupByOrgID(int orgid)
        //     {
        //         var QueryStatement = @" select vg.vehiclegroupid,vg.name,count(vorg.vehicleorgid) 
        //                             from dafconnectmaster.vehiclegroupvehiclemapping vgvm
        //                             left join dafconnectmaster.vehiclegroup vg on vg.vehiclegroupid=vgvm.vehiclegroupid
        //                             left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
        //                             group by vg.vehiclegroupid
        // 				            WHERE vehiclegrp.organizationid=@orgid
        // 				            and vehiclegrp.isactive=true;";                    
        //         var parameter = new DynamicParameters();
        //         parameter.Add("@orgid", orgid);
        //         IEnumerable<VehicleGroup> VehicleOrgDetails = await dataAccess.QueryAsync<VehicleGroup>(QueryStatement, parameter);
        //         return VehicleOrgDetails;
        //     }     

        //     public async Task<IEnumerable<Vehicle>> GetVehiclesByOrgID(int orgid)
        //     {
        //        var QueryStatement = @"select veh.vin,veh.registrationno,veh.chassisno,veh.model,vorg.name
        //                             from dafconnectmaster.vehiclegroupvehiclemapping vgvm
        //                             left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
        //                             left join dafconnectmaster.vehicle veh on vorg.vehicleid=veh.vehicleid
        //                             where vorg.organizationid=@orgid and vorg.optoutstatus=false and vorg.isactive=true;";                    
        //         var parameter = new DynamicParameters();
        //         parameter.Add("@orgid", orgid);
        //         IEnumerable<Vehicle> VehicleDetails = await dataAccess.QueryAsync<Vehicle>(QueryStatement, parameter);
        //         return VehicleDetails;
        //     }

        //     public async Task<IEnumerable<ServiceSubscribers>> GetServiceSubscribersByOrgID(int orgid)
        //     {
        //        var QueryStatement = @"select vg.vehiclegroupid,vg.name as vehiclegroup,count(vorg.vehicleorgid) as vehicles,count(usr.userid) as users
        //                             from dafconnectmaster.vehiclegroupvehiclemapping vgvm
        //                             left join dafconnectmaster.vehiclegroup vg on vg.vehiclegroupid=vgvm.vehiclegroupid
        //                             left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
        //                             left join dafconnectmaster.usergroupvehiclegroup ugvg on vgvm.vehiclegroupid=ugvg.vehiclegroupid
        //                             left join dafconnectmaster.usergroup usrgrp on usrgrp.usergroupid=ugvg.usergroupid
        //                             left join dafconnectmaster.userorg usrorg on usrorg.userorgid=usrgrp.organizationid
        //                             left join dafconnectmaster.user usr on usr.userid=usrorg.userid
        //                             where vgvm.vehicleorgid=@orgid 
        //                             group by vg.vehiclegroupid;";                    
        //         var parameter = new DynamicParameters();
        //         parameter.Add("@orgid", orgid);
        //         IEnumerable<ServiceSubscribers> ServiceSubscribersDetails = await dataAccess.QueryAsync<ServiceSubscribers>(QueryStatement, parameter);
        //         return ServiceSubscribersDetails;
        //     }   

        //     // public async Task<IEnumerable<User>> GetUsersDetailsByGroupID(int orgid,int usergroupid)
        //     // {
        //     //    var QueryStatement = @"select usr.userid,usr.emailid,usr.salutation,usr.firstname,usr.lastname,usr.dob,usr.usertypeid
        //     //                         from dafconnectmaster.usergroupvehiclegroup ugvg
        //     //                         left join dafconnectmaster.usergroup usrgrp on usrgrp.usergroupid=ugvg.usergroupid
        //     //                         left join dafconnectmaster.userorg usrorg on usrorg.userorgid=usrgrp.organizationid
        //     //                         left join dafconnectmaster.user usr on usr.userid=usrorg.userid
        //     //                         where usrorg.userorgid=@orgid and usrgrp.usergroupid=@usergroupid and usrgrp.isactive=true and ugvg.isactive=true;";                    
        //     //     var parameter = new DynamicParameters();
        //     //     parameter.Add("@orgid", orgid);
        //     //     parameter.Add("@usergroupid", usergroupid);
        //     //     IEnumerable<User> UsersDetail = await dataAccess.QueryAsync<User>(QueryStatement, parameter);
        //     //     return UsersDetail;
        //     // }          
    }
}


