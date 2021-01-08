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
         dataAccess =_dataAccess;
        }

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
                                      ,termination_date) 
                            	VALUES(
                                       @organization_id 
                                      ,@name
                                      ,@vin
                                      ,@license_plate_number
                                      ,@status
                                      ,@status_changed_date
                                      ,@termination_date) RETURNING id";

                     var parameter = new DynamicParameters();
                     parameter.Add("@organization_id", vehicle.OrganizationId);
                     parameter.Add("@name", vehicle.Name);
                     parameter.Add("@vin", vehicle.VIN);
                     parameter.Add("@license_plate_number", vehicle.RegistrationNo);
                     parameter.Add("@status", (char)vehicle.Status);
                     parameter.Add("@status_changed_date", UTCHandling.GetUTCFromDateTime(vehicle.StatusDate.ToString()));
                     parameter.Add("@termination_date", UTCHandling.GetUTCFromDateTime(vehicle.TerminationDate.ToString()));
           
                     parameter.Add("@id",dbType:DbType.Int32 ,direction: ParameterDirection.InputOutput);
                     int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                     vehicle.ID = vehicleID;
                     return vehicle;    
        }

        public async Task<VehicleProperty> CreateProperty(VehicleProperty vehicleproperty)
        {
            var QueryStatement = @"INSERT INTO master.vehicleproperty
                                      (
                                       vehicle_id
                                      ,manufacture_date
                                      ,registration_date
                                      ,delivery_date
                                      ,make
                                      ,model
                                      ,series
                                      ,type
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
                                      ,chasis_side_skirts
                                      ,chasis_side_collar
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
                                      ,@model
                                      ,@series
                                      ,@type
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
                                      ,@chasis_side_skirts
                                      ,@chasis_side_collar
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

             var parameter = new DynamicParameters();
                     parameter.Add("@vehicle_id", vehicleproperty.VehicleId);
                     parameter.Add("@manufacture_date", vehicleproperty.ManufactureDate);
                     parameter.Add("@registration_date", vehicleproperty.RegistrationDateTime);
                     parameter.Add("@delivery_date", vehicleproperty.DeliveryDate);
                     parameter.Add("@make", vehicleproperty.Classification_Make);
                     parameter.Add("@model", vehicleproperty.Classification_Model);
                     parameter.Add("@series", vehicleproperty.Classification_Series); 
                     parameter.Add("@type", vehicleproperty.Classification_Type);
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
                     parameter.Add("@chasis_side_skirts", vehicleproperty.SideSkirts);
                     parameter.Add("@chasis_side_collar", vehicleproperty.SideCollars);
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
                     parameter.Add("@id",dbType:DbType.Int32 ,direction: ParameterDirection.InputOutput);
                     int vehiclePropertyID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                     vehicleproperty.ID = vehiclePropertyID;
                     return vehicleproperty;
        }

        public async Task<List<Vehicle>> Get(VehicleFilter vehiclefilter)
        {
           
            var QueryStatement = @"select organization_id
                                            ,name
                                            ,vin
                                            ,license_plate_number
                                            ,Status
                                            ,status_changed_date
                                            ,termination_date
                                     from master.vehicle
                                     where 1=1";
            var parameter = new DynamicParameters();

            if (vehiclefilter.VehicleId > 0)
            {
                QueryStatement = QueryStatement + "id  = @id";
                parameter.Add("@id", vehiclefilter.VehicleId);
            }
            // organization id filter
            if (vehiclefilter.OrganizationId > 0)
            {
                QueryStatement = QueryStatement + "organization_id  = @organization_id";
                parameter.Add("@organization_id", vehiclefilter.OrganizationId);
            }

            if (vehiclefilter.VIN != null)
            {
                QueryStatement = QueryStatement + "vin  = ANY(@vin)";
                parameter.Add("@vin", vehiclefilter.VIN);
            }
            
            // if (vehiclefilter.VehicleIdList <> null)
            // {
            //     QueryStatement = QueryStatement + "vin  = ANY(@vin)";
            //     parameter.Add("@vin", vehiclefilter.VehicleIdList);
            // }
            IEnumerable<Vehicle> VehicleDetails = await dataAccess.QueryAsync<Vehicle>(QueryStatement, parameter);
            return VehicleDetails.ToList();
        }

        public async Task<Vehicle> Update(Vehicle vehicle)
        {
           var QueryStatement = @" UPDATE master.vehicle
                                        SET 
                                        organization_id=@organization_id
                                        ,name=@name
                                        ,vin=@vin
        	                            ,license_plate_number=@license_plate_number
                                    	,status=@status
                                    	,status_changed_date=@status_changed_date
        	                            ,termination_date=@termination_date,
                                        WHERE id = @id;";

                     var parameter = new DynamicParameters();
                     parameter.Add("@id", vehicle.ID);
                     parameter.Add("@organization_id", vehicle.OrganizationId);
                     parameter.Add("@name", vehicle.Name);
                     parameter.Add("@vin", vehicle.VIN);
                     parameter.Add("@license_plate_number", vehicle.RegistrationNo);
                     parameter.Add("@status", vehicle.Status);
                     parameter.Add("@status_changed_date", vehicle.StatusDate);
                     parameter.Add("@termination_date", vehicle.TerminationDate);

                     int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                     return vehicle;
        }

        public async Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty)
        {

              var QueryStatement = @" UPDATE master.vehicleproperty
                                    SET
                                      ,manufacture_date=@manufacture_date
                                      ,registration_date=@registration_date
                                      ,delivery_date=@delivery_date
                                      ,make=@make
                                      ,model=@model
                                      ,series=@series
                                      ,type=@type
                                      ,length=@length
                                      ,widht=@widht
                                      ,height=@height
                                      ,weight=@weight
                                      ,engine_id=@engine_id
                                      ,engine_type=@engine_type
                                      ,engine_power=@engine_power
                                      ,engine_coolant=@engine_coolant
                                      ,engine_emission_level=@engine_emission_level
                                      ,chasis_id=@chasis_id
                                      ,chasis_side_skirts=@chasis_side_skirts
                                      ,chasis_side_collar=@chasis_side_collar
                                      ,chasis_rear_overhang=@chasis_rear_overhang
                                      ,chasis_fuel_tank_number=@chasis_fuel_tank_number
                                      ,chasis_fuel_tank_volume=@chasis_fuel_tank_volume
                                      ,driveline_axle_configuration=@driveline_axle_configuration
                                      ,driveline_wheel_base=@driveline_wheel_base
                                      ,driveline_tire_size=@driveline_tire_size
                                      ,driveline_front_axle_position=@driveline_front_axle_position
                                      ,driveline_front_axle_load=@driveline_front_axle_load
                                      ,driveline_rear_axle_position=@driveline_rear_axle_position
                                      ,driveline_rear_axle_load=@driveline_rear_axle_load
                                      ,driveline_rear_axle_ratio=@driveline_rear_axle_ratio
                                      ,transmission_gearbox_id=@transmission_gearbox_id
                                      ,transmission_gearbox_type=@transmission_gearbox_type
                                      ,cabin_id=@cabin_id
                                      ,cabin_color_id=@cabin_color_id
                                      ,cabin_color_value=@cabin_color_value
                                       WHERE vehicle_id = @vehicle_id";
                     var parameter = new DynamicParameters();
                     parameter.Add("@vehicle_id", vehicleproperty.VehicleId);
                     parameter.Add("@manufacture_date", vehicleproperty.ManufactureDate);
                     parameter.Add("@registration_date", vehicleproperty.RegistrationDateTime);
                     parameter.Add("@delivery_date", vehicleproperty.DeliveryDate);
                     parameter.Add("@make", vehicleproperty.Classification_Make);
                     parameter.Add("@model", vehicleproperty.Classification_Model);
                     parameter.Add("@series", vehicleproperty.Classification_Series); 
                     parameter.Add("@type", vehicleproperty.Classification_Type);
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
                     parameter.Add("@chasis_side_skirts", vehicleproperty.SideSkirts);
                     parameter.Add("@chasis_side_collar", vehicleproperty.SideCollars);
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
                     parameter.Add("@id",dbType:DbType.Int32 ,direction: ParameterDirection.InputOutput);
                   int vehicleID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                   return vehicleproperty;
        }

        public async Task<Vehicle> UpdateStatus(Vehicle vehicle)
        {
            var QueryStatement = @" UPDATE master.vehicle
                                                SET
                                                status=@status
                                                ,status_changed_date=@status_changed_date
                                                WHERE id = @id;";

            var parameter = new DynamicParameters();
            parameter.Add("@id", vehicle.ID);
            if (Enum.IsDefined(typeof(VehicleStatusType), (VehicleStatusType)vehicle.Status))
            {
                VehicleStatusType vehicleStatusCode = (VehicleStatusType)vehicle.Status;
                parameter.Add("@status", vehicleStatusCode);
            }
            parameter.Add("@status_changed_date", vehicle.StatusDate);
            int vehiclepropertyId = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            
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
                     OptInOptOutparameter.Add("@ref_id", vehicle.ID);
                     OptInOptOutparameter.Add("@account_id", vehicle.Account_Id);
                    if (Enum.IsDefined(typeof(VehicleStatusType), (VehicleStatusType)vehicle.Status))
                    {
                        VehicleStatusType vehicleStatusCode = (VehicleStatusType)vehicle.Status;
                        OptInOptOutparameter.Add("@status", vehicleStatusCode);
                    }
                     OptInOptOutparameter.Add("@status_changed_date", vehicle.StatusDate);

                    if (Enum.IsDefined(typeof(OptInOptOutType), (OptInOptOutType)vehicle.Type))
                    {
                        OptInOptOutType OptInOptOutStatusCode = (OptInOptOutType)vehicle.Type;
                        OptInOptOutparameter.Add("@type", vehicle.Status);
                    }
                    
            int Id = await dataAccess.ExecuteScalarAsync<int>(InsertQueryStatement, OptInOptOutparameter);

            return vehicle;
        }


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


