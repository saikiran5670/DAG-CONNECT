using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.data;
using Dapper;

namespace net.atos.daf.ct2.vehicle.repository
{
     public class VehicleRepository : IVehicleRepository
    {     
       private readonly IDataAccess dataAccess;   
       public VehicleRepository(IDataAccess _dataAccess) 
        {    
         dataAccess =_dataAccess;
        }
        public async Task<int> AddVehicle(Vehicle vehicle)
        {  
            var QueryStatement = @"INSERT INTO dafconnectmaster.vehicle
                                  (vin,
                                  registrationno,
                                  chassisno,
                                  isactive,
                                  createddate,
                                  createdby) 
                        	VALUES(@vin,
                                  @registrationno,
                                  @chassisno,
                                  @isactive,
                                  @createddate,
                                  @createdby) RETURNING vehicleid";
                
                 var parameter = new DynamicParameters();
                 parameter.Add("@vin", vehicle.VIN);
                 parameter.Add("@registrationno", vehicle.RegistrationNo);
                 parameter.Add("@chassisno", vehicle.ChassisNo);
                 parameter.Add("@isactive", vehicle.IsActive);
                 parameter.Add("@createddate",  DateTime.Now);
                 parameter.Add("@createdby",  vehicle.CreatedBy);
                 parameter.Add("@vehicleidret",dbType:DbType.Int32 ,direction: ParameterDirection.InputOutput);
                 int resultAddVehicle = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                 return resultAddVehicle;          
        }

       
       public async Task<int> AddVehicleGroup(VehicleGroup vehicleGroup)
        {         
          try{
               var QueryStatement = @"INSERT INTO dafconnectmaster.vehiclegroup
                                            (organizationid,
                                            name,
                                            description,
                                            isactive,
                                            isdefaultgroup,
                                            isuserdefinedgroup,
                                            createddate,
                                            createdby) 
	                                VALUES (@organizationid,
                                            @name,
                                            @description,
                                            @isactive,
                                            @isdefaultgroup,
                                            @isuserdefinedgroup,
                                            @createddate,
                                            @createdby) RETURNING vehiclegroupid";
                
                 var parameter = new DynamicParameters();
                 parameter.Add("@organizationid", vehicleGroup.OrganizationID);
                 parameter.Add("@name", vehicleGroup.Name); 
                  parameter.Add("@description", vehicleGroup.Description);              
                 parameter.Add("@isactive", vehicleGroup.IsActive);
                 parameter.Add("@isdefaultgroup", vehicleGroup.IsDefaultGroup);
                 parameter.Add("@isuserdefinedgroup", vehicleGroup.IsUserDefindGroup);
                 parameter.Add("@createddate",  DateTime.Now);
                 parameter.Add("@createdby", vehicleGroup.CreatedBy);    
                
                int resultAddVehicleGroupID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                var vehicleorgIDs=vehicleGroup.VehicleOrgIds;
              
                 if(vehicleorgIDs.Length>0)
                 {
                    int[] vehIDs = Array.ConvertAll(vehicleorgIDs.Split(','), int.Parse);
                    if (vehIDs.Length>0)
                    {
                        int vehOrgId;
                        
                        for(int i=0; i<vehIDs.Length; i++)
                        {
                         vehOrgId = Convert.ToInt32(vehIDs[i]);

                         var QueryStatementMapping = @"INSERT INTO dafconnectmaster.vehiclegroupvehiclemapping
                                            (vehiclegroupid,
                                            vehicleorgid,
                                            isactive,
                                            createddate,createdby)                                     
	                                       VALUES (@vehgroupid,@vehorgid,@isactive,@createddate,@createdby) RETURNING vehiclegroupvehiclemappingid";                        
                          var parameterVehicleOrg = new DynamicParameters();
                          parameterVehicleOrg.Add("@vehgroupid", resultAddVehicleGroupID);
                          parameterVehicleOrg.Add("@vehorgid", vehOrgId);              
                          parameterVehicleOrg.Add("@isactive", vehicleGroup.IsActive);
                          parameterVehicleOrg.Add("@createddate", DateTime.Now);
                          parameterVehicleOrg.Add("@createdby", vehicleGroup.CreatedBy);
                          int resultAddVehicleGroupMappingID = await dataAccess.ExecuteScalarAsync<int>(QueryStatementMapping, parameterVehicleOrg);                       
                         
                        }
                    }
                 }              
                 return resultAddVehicleGroupID;
            }
            catch (System.Exception ex)
            {
              throw ex;             
            }
        }
       
        public async Task<int>  UpdateVehicle(Vehicle vehicle)
        {
             var QueryStatement = @" UPDATE dafconnectmaster.vehicle
	                                SET vin=@vin,
		                            registrationno=@registrationno,
	                            	chassisno=@chassisno,
	                            	terminationdate=@terminationdate,
		                            isactive=@isactive,
		                            updatedby=@updatedby,
		                            updateddate=@updateddate
	                                WHERE vehicleid = @vehicleid;";
          
                 var parameter = new DynamicParameters();
                 parameter.Add("@vehicleid", vehicle.VehicleID);
                 parameter.Add("@vin", vehicle.VIN);
                 parameter.Add("@registrationno", vehicle.RegistrationNo);
                 parameter.Add("@chassisno", vehicle.ChassisNo);
                 parameter.Add("@terminationdate", vehicle.TerminationDate);
                 parameter.Add("@isactive", vehicle.IsActive);
                 parameter.Add("@updatedby", vehicle.UpdatedBy);
                 parameter.Add("@updateddate", DateTime.Now);
                 
                 int resultupdateVehicle = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                 return resultupdateVehicle;         
        }

        public async Task<int> UpdateVehicleGroup(VehicleGroup vehicleGroup)
         {

                  var QueryStatement = @" UPDATE dafconnectmaster.vehicleGroup
	                                    SET name=@grpname,
                                        description=@description,
		                                organizationid=@orgid,
		                                isactive=@active,
	                                	updatedby=@updated,		
	                                	updateddate=@updateddate,
	                                	isdefaultgroup=@defaultgroup,
		                                isuserdefinedgroup=@userdefinedgroup                                        
	                                    WHERE vehiclegroupid = @vehiclegrpid;";
                 var parameter = new DynamicParameters();
                 parameter.Add("@vehiclegrpid", vehicleGroup.VehicleGroupID);
                 parameter.Add("@orgid", vehicleGroup.OrganizationID);
                 parameter.Add("@grpname", vehicleGroup.Name); 
                 parameter.Add("@description", vehicleGroup.Description);                        
                 parameter.Add("@active", vehicleGroup.IsActive);
                 parameter.Add("@updated", vehicleGroup.UpdatedBy);
                 parameter.Add("@defaultgroup", vehicleGroup.IsDefaultGroup);
                 parameter.Add("@userdefinedgroup", vehicleGroup.IsUserDefindGroup);
                 parameter.Add("@updateddate", DateTime.Now);

                 int resultUpdateVehicleGroupID = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);   
                
                 var vehicleorgIDs=vehicleGroup.VehicleOrgIds;
                 if(vehicleorgIDs.Length>0)
                 {

                    int[] vehIDs = Array.ConvertAll(vehicleorgIDs.Split(','), int.Parse);
                    if (vehIDs.Length>0)
                    {                     
                                                   
                        //   var parameterDeleteVehicMapping = new DynamicParameters();
                        //   parameterDeleteVehicMapping.Add("@vehgroupid", vehicleGroup.VehicleGroupID);
                        //   parameterDeleteVehicMapping.Add("@orgid", vehicleGroup.OrganizationID);    
                        //   parameterDeleteVehicMapping.Add("@val",dbType:DbType.Int32 ,direction: ParameterDirection.InputOutput);
                        //   dataAccess.QuerySingle<int>(@"CALL dafconnectmaster.deletevehiclegroupvehiclemapping(@vehgroupid,@orgid,@val)",parameterDeleteVehicMapping);
                    
                        for(int i=0; i<vehIDs.Length; i++)
                        {
                          int vehOrgId = Convert.ToInt32(vehIDs[i]);
                          var QueryStatementMapping = @" INSERT INTO dafconnectmaster.vehiclegroupvehiclemapping
                                            (vehiclegroupid,
                                            vehicleorgid,
                                            isactive,
                                            createddate,
                                            createdby) 
	                                 VALUES (@vehgroupid,
                                            @vehorgid,
                                            @isactive,
                                            @createddate,
                                            @createdby) RETURNING vehiclegroupvehiclemappingid";                        
                          var parameterVehicleOrg = new DynamicParameters();
                          parameterVehicleOrg.Add("@vehgroupid", resultUpdateVehicleGroupID);
                          parameterVehicleOrg.Add("@vehorgid", vehOrgId);              
                          parameterVehicleOrg.Add("@isactive", vehicleGroup.IsActive);
                          parameterVehicleOrg.Add("@createddate", DateTime.Now);
                          parameterVehicleOrg.Add("@createdby", vehicleGroup.CreatedBy);
                         int resultUpdateVehicleGroupMappingID = await dataAccess.ExecuteScalarAsync<int>(QueryStatementMapping, parameterVehicleOrg);                       
                      
                        }
                    }
                 }
                 return resultUpdateVehicleGroupID;                        
        }
     
         public async Task<int> DeleteVehicle(int vehicleid, int updatedby)
        {
               var QueryStatement = @"UPDATE dafconnectmaster.vehicle
	                                         SET isactive=@isactive,
		                                     updatedby=@updatedby,
		                                     updateddate=@updateddate  
	                                         WHERE vehicleid = @vehicleid
                                             RETURNING vehicleid;";
            var parameter = new DynamicParameters();
            parameter.Add("@vehicleid", vehicleid);
            parameter.Add("@isactive", false);
            parameter.Add("@updatedby", updatedby);
            parameter.Add("@updateddate", DateTime.Now);
           
            int resultDeleteVehicle = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return resultDeleteVehicle;
        }       
        public async Task<int> DeleteVehicleGroup(int vehicleGroupid, int updatedby)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@vehgroupid", vehicleGroupid);
            parameter.Add("@isactive", false);
            parameter.Add("@updatedby", updatedby);
            parameter.Add("@updateddate", DateTime.Now);
            var QueryStatement = @"UPDATE dafconnectmaster.vehicleGroup
	                                SET isactive=@isactive,
		                            updatedby=@updatedby,
		                            updateddate=@updateddate
	                                WHERE vehiclegroupid = @vehgroupid
	                                RETURNING vehiclegroupid;";          

            int resultDeleteVehicleGroup = await dataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
            return resultDeleteVehicleGroup;             
        }
        public async Task<IEnumerable<Vehicle>> GetVehicleByID(int vehicleid,int orgid)
        {
            var QueryStatement = @"select veh.vin,veh.registrationno,veh.chassisno,veh.model,vorg.name
                                 from dafconnectmaster.vehiclegroupvehiclemapping vgvm
                                 left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
                                 left join dafconnectmaster.vehicle veh on vorg.vehicleid=veh.vehicleid
                                 where vorg.organizationid=@orgid and veh.vehicleid=@vehicleid and vorg.optoutstatus=false and vorg.isactive=true;";                    
            var parameter = new DynamicParameters();
            parameter.Add("@vehicleid", vehicleid);
            parameter.Add("@orgid", orgid);
            IEnumerable<Vehicle> VehicleDetails = await dataAccess.QueryAsync<Vehicle>(QueryStatement, parameter);
            return VehicleDetails;
        }

        public async Task<IEnumerable<VehicleGroup>> GetVehicleGroupByID(int vehicleGroupID,int orgid)
        {
            var QueryStatement = @"select veh.vin,veh.registrationno,veh.chassisno,veh.model,vorg.name 
                                 from dafconnectmaster.vehiclegroupvehiclemapping vgvm
                                 left join dafconnectmaster.vehiclegroup vehgrp on vgvm.vehiclegroupid=vehgrp.vehiclegroupid
                                 left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
                                 left join dafconnectmaster.vehicle veh on vorg.vehicleid=veh.vehicleid
                                 where vehgrp.vehiclegroupid=@vehicleGroupID and vorg.organizationid=@orgid
                                 and vorg.optoutstatus=false and vorg.isactive=true;";
                    
            var parameter = new DynamicParameters();
            parameter.Add("@vehicleGroupID", vehicleGroupID);
            parameter.Add("@orgid", orgid);
            IEnumerable<VehicleGroup> VehicleOrgDetails = await dataAccess.QueryAsync<VehicleGroup>(QueryStatement, parameter);
            return VehicleOrgDetails;
        }

        // public async Task<IEnumerable<VehicleGroup>> GetAllVehicleGroups(int orgid)
        // {
        //     var QueryStatement = @" SELECT vehiclegrp.vehiclegroupid, 
		// 			 vehiclegrp.organizationid,
		// 			 vehiclegrp.name,
        //              vehiclegrp.description,
		// 			 vehiclegrp.isactive,
		// 			 vehiclegrp.createddate,
		// 			 vehiclegrp.createdby,
		// 			 vehiclegrp.updateddate,
		// 			 vehiclegrp.updatedby,
		// 			 vehiclegrp.isdefaultgroup,
		// 			 vehiclegrp.isuserdefinedgroup					 
		// 			FROM dafconnectmaster.vehiclegroup vehiclegrp
		// 			WHERE vehiclegrp.organizationid=@orgid
		// 			and vehiclegrp.isactive=true;";                    
        //     var parameter = new DynamicParameters();
        //     parameter.Add("@orgid", orgid);
        //     IEnumerable<VehicleGroup> VehicleOrgDetails = await dataAccess.QueryAsync<VehicleGroup>(QueryStatement, parameter);
        //     return VehicleOrgDetails;
        // }
        // public async Task<IEnumerable<Vehicle>> GetAllVehicles(int orgid)
        // {
        //     var QueryStatement = @"select 
        //     veh.vin,
        //     veh.registrationno,
        //     veh.chassisno,
        //     veh.model,
        //     vorg.name
        //     from dafconnectmaster.vehiclegroupvehiclemapping vgvm
        //     left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
        //     left join dafconnectmaster.vehicle veh on vorg.vehicleid=veh.vehicleid
        //     where vorg.organizationid=@orgid and vorg.optoutstatus=false and vorg.isactive=true;";                    
        //     var parameter = new DynamicParameters();
        //     parameter.Add("@orgid", orgid);
        //     IEnumerable<Vehicle> VehicleDetails = await dataAccess.QueryAsync<Vehicle>(QueryStatement, parameter);
        //     return VehicleDetails;
        // }


      
        public async Task<IEnumerable<VehicleGroup>> GetVehicleGroupByOrgID(int orgid)
        {
            var QueryStatement = @" select vg.vehiclegroupid,vg.name,count(vorg.vehicleorgid) 
                                from dafconnectmaster.vehiclegroupvehiclemapping vgvm
                                left join dafconnectmaster.vehiclegroup vg on vg.vehiclegroupid=vgvm.vehiclegroupid
                                left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
                                group by vg.vehiclegroupid
					            WHERE vehiclegrp.organizationid=@orgid
					            and vehiclegrp.isactive=true;";                    
            var parameter = new DynamicParameters();
            parameter.Add("@orgid", orgid);
            IEnumerable<VehicleGroup> VehicleOrgDetails = await dataAccess.QueryAsync<VehicleGroup>(QueryStatement, parameter);
            return VehicleOrgDetails;
        }     

        public async Task<IEnumerable<Vehicle>> GetVehiclesByOrgID(int orgid)
        {
           var QueryStatement = @"select veh.vin,veh.registrationno,veh.chassisno,veh.model,vorg.name
                                from dafconnectmaster.vehiclegroupvehiclemapping vgvm
                                left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
                                left join dafconnectmaster.vehicle veh on vorg.vehicleid=veh.vehicleid
                                where vorg.organizationid=@orgid and vorg.optoutstatus=false and vorg.isactive=true;";                    
            var parameter = new DynamicParameters();
            parameter.Add("@orgid", orgid);
            IEnumerable<Vehicle> VehicleDetails = await dataAccess.QueryAsync<Vehicle>(QueryStatement, parameter);
            return VehicleDetails;
        }

        public async Task<IEnumerable<ServiceSubscribers>> GetServiceSubscribersByOrgID(int orgid)
        {
           var QueryStatement = @"select vg.vehiclegroupid,vg.name as vehiclegroup,count(vorg.vehicleorgid) as vehicles,count(usr.userid) as users
                                from dafconnectmaster.vehiclegroupvehiclemapping vgvm
                                left join dafconnectmaster.vehiclegroup vg on vg.vehiclegroupid=vgvm.vehiclegroupid
                                left join dafconnectmaster.vehicleorg vorg on vgvm.vehicleorgid=vorg.vehicleorgid
                                left join dafconnectmaster.usergroupvehiclegroup ugvg on vgvm.vehiclegroupid=ugvg.vehiclegroupid
                                left join dafconnectmaster.usergroup usrgrp on usrgrp.usergroupid=ugvg.usergroupid
                                left join dafconnectmaster.userorg usrorg on usrorg.userorgid=usrgrp.organizationid
                                left join dafconnectmaster.user usr on usr.userid=usrorg.userid
                                where vgvm.vehicleorgid=@orgid 
                                group by vg.vehiclegroupid;";                    
            var parameter = new DynamicParameters();
            parameter.Add("@orgid", orgid);
            IEnumerable<ServiceSubscribers> ServiceSubscribersDetails = await dataAccess.QueryAsync<ServiceSubscribers>(QueryStatement, parameter);
            return ServiceSubscribersDetails;
        }   

        // public async Task<IEnumerable<User>> GetUsersDetailsByGroupID(int orgid,int usergroupid)
        // {
        //    var QueryStatement = @"select usr.userid,usr.emailid,usr.salutation,usr.firstname,usr.lastname,usr.dob,usr.usertypeid
        //                         from dafconnectmaster.usergroupvehiclegroup ugvg
        //                         left join dafconnectmaster.usergroup usrgrp on usrgrp.usergroupid=ugvg.usergroupid
        //                         left join dafconnectmaster.userorg usrorg on usrorg.userorgid=usrgrp.organizationid
        //                         left join dafconnectmaster.user usr on usr.userid=usrorg.userid
        //                         where usrorg.userorgid=@orgid and usrgrp.usergroupid=@usergroupid and usrgrp.isactive=true and ugvg.isactive=true;";                    
        //     var parameter = new DynamicParameters();
        //     parameter.Add("@orgid", orgid);
        //     parameter.Add("@usergroupid", usergroupid);
        //     IEnumerable<User> UsersDetail = await dataAccess.QueryAsync<User>(QueryStatement, parameter);
        //     return UsersDetail;
        // }          
    }
}


