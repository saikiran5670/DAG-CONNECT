using net.atos.daf.ct2.vehicle.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Group = net.atos.daf.ct2.group;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle;

namespace net.atos.daf.ct2.vehicleservice.Entity
{
    public class Mapper
    {
        public Vehicle ToVehicleEntity(VehicleCreateRequest request)
        {
            Vehicle vehicle = new Vehicle();
            vehicle.ID = request.Id;
            vehicle.Name = request.Name;
            vehicle.VIN = request.Vin;
            vehicle.License_Plate_Number = request.LicensePlateNumber;
            vehicle.Vid = null;
            vehicle.ModelId = null;
            vehicle.Tcu_Id = null;
            vehicle.Tcu_Serial_Number = null;
            vehicle.Tcu_Brand = null;
            vehicle.Tcu_Version = null;
            vehicle.Is_Tcu_Register = false;
            vehicle.Reference_Date = null;
            vehicle.Organization_Id = request.OrganizationId;
           // vehicle.VehiclePropertiesId = null;
            return vehicle;
        }

        public Vehicle ToVehicleEntity(VehicleRequest request)
        {
            Vehicle vehicle = new Vehicle();
            vehicle.ID = request.Id;
            vehicle.Name = request.Name;
            vehicle.VIN = null;
            vehicle.License_Plate_Number = request.LicensePlateNumber;
            vehicle.Vid = null;
            vehicle.ModelId = null;
            vehicle.Tcu_Id = null;
            vehicle.Tcu_Serial_Number = null;
            vehicle.Tcu_Brand = null;
            vehicle.Tcu_Version = null;
            vehicle.Is_Tcu_Register = false;
            vehicle.Reference_Date = null;
            vehicle.Organization_Id = request.OrganizationId;
            return vehicle;
        }

        public VehicleFilter ToVehicleFilterEntity(VehicleFilterRequest request)
        {
            VehicleFilter vehicleFilter = new VehicleFilter();
            vehicleFilter.VehicleId = request.VehicleId;
            vehicleFilter.OrganizationId = request.OrganizationId;
            vehicleFilter.VehicleIdList = request.VehicleIdList;
            vehicleFilter.VIN = request.VIN;
            //if (!string.IsNullOrEmpty(request.Status))
            //{
            //    char type = Convert.ToChar(request.Status);
            //    if (type == 'I' || type == 'i')
            //    {
            //        vehicleFilter.Status = vehicle.VehicleStatusType.OptIn;
            //    }
            //    else if (type == 'U' || type == 'u')
            //    {
            //        vehicleFilter.Status = vehicle.VehicleStatusType.OptOut;
            //    }
            //    else if (type == 'T' || type == 't')
            //    {
            //        vehicleFilter.Status = vehicle.VehicleStatusType.Terminate;
            //    }
            //    else if (type == 'o' || type == 'O')
            //    {
            //        vehicleFilter.Status = vehicle.VehicleStatusType.Ota;
            //    }
            //    else
            //    {
            //        vehicleFilter.Status = vehicle.VehicleStatusType.OptIn;
            //    }
            //}
            //else
            //{
            //    vehicleFilter.Status = vehicle.VehicleStatusType.None;
            //}
            vehicleFilter.Status = vehicle.VehicleStatusType.None;
            return vehicleFilter;
        }

        public VehicleDetails ToVehicle(Vehicle vehicle)
        {
            VehicleDetails vehicledetails = new VehicleDetails();
            vehicledetails.Id = vehicle.ID;
            if(Convert.ToInt32(vehicle.Organization_Id)>0)
            vehicledetails.Organizationid = vehicle.Organization_Id;
            vehicledetails.Vin = vehicle.VIN;
            if (!string.IsNullOrEmpty(vehicle.License_Plate_Number))
                vehicledetails.LicensePlateNumber = vehicle.License_Plate_Number;
            if (!string.IsNullOrEmpty(vehicle.ModelId))
                vehicledetails.ModelId = vehicle.ModelId;
            if (!string.IsNullOrEmpty(vehicle.Name))
                vehicledetails.Name = vehicle.Name;

            if (vehicle.Status == VehicleCalculatedStatus.Connected)
            {
                vehicledetails.Status = "C";
            }
            else if (vehicle.Status == VehicleCalculatedStatus.Connected_OTA)
            {
                vehicledetails.Status = "N";
            }
            else if (vehicle.Status == VehicleCalculatedStatus.Off)
            {
                vehicledetails.Status = "O";
            }
            else if (vehicle.Status == VehicleCalculatedStatus.OTA)
            {
                vehicledetails.Status = "A";
            }
            else if (vehicle.Status == VehicleCalculatedStatus.Terminate)
            {
                vehicledetails.Status = "T";
            }

            vehicledetails.OemId = vehicle.Oem_id;
            vehicledetails.OemOrganisationId = vehicle.Oem_Organisation_id;
            vehicledetails.IsOta = vehicle.Is_Ota;
            if (vehicle.Opt_In.ToString() == VehicleStatusType.OptIn.ToString())
            {
                vehicledetails.OptIn = "I";
            }
            else if (vehicle.Opt_In.ToString() == VehicleStatusType.OptOut.ToString())
            {
                vehicledetails.OptIn = "U";
            }
            else if (vehicle.Opt_In.ToString() == VehicleStatusType.Inherit.ToString())
            {
                vehicledetails.OptIn = "H";
            }
            vehicledetails.RelationShip = vehicle.RelationShip;
            vehicledetails.AssociatedGroups = vehicle.AssociatedGroups;

            return vehicledetails;
        }

        public Group.Group ToGroup(VehicleGroupRequest request)
        {
            Group.Group entity = new Group.Group();
            entity.Id = request.Id;
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Argument = "";//request.Argument;                
            entity.FunctionEnum = group.FunctionEnum.None;
            entity.RefId = null;
            if (request.RefId > 0) entity.RefId = request.RefId;

            if (!string.IsNullOrEmpty(request.GroupType))
            {
                char type = Convert.ToChar(request.GroupType);
                if (type == 'd' || type == 'D')
                {
                    entity.GroupType = Group.GroupType.Dynamic;
                }
                else if (type == 's' || type == 'S')
                {
                    entity.GroupType = Group.GroupType.Single;
                }
                else if (type == 'g' || type == 'G')
                {
                    entity.GroupType = Group.GroupType.Group;
                }
                else if (type == 'n' || type == 'N')
                {
                    entity.GroupType = Group.GroupType.None;
                }
            }
            else
            {
                entity.GroupType = Group.GroupType.Group;
            }
            entity.ObjectType = group.ObjectType.VehicleGroup;
            entity.OrganizationId = request.OrganizationId;
            if (request.CreatedAt > 0)
            {
                entity.CreatedAt = (long)request.CreatedAt;
            }
            else
            {
                entity.CreatedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            }

            if (!string.IsNullOrEmpty(request.FunctionEnum))
            {
                char type = Convert.ToChar(request.FunctionEnum);
                if (type == 'a' || type == 'A')
                {
                    entity.FunctionEnum = Group.FunctionEnum.All;
                }
                else if (type == 'o' || type == 'O')
                {
                    entity.FunctionEnum = Group.FunctionEnum.OwnedVehicles;
                }
                else if (type == 'v' || type == 'V')
                {
                    entity.FunctionEnum = Group.FunctionEnum.VisibleVehicles;
                }
                else
                {
                    entity.FunctionEnum = Group.FunctionEnum.None;
                }
            }
            else
            {
                entity.FunctionEnum = Group.FunctionEnum.All;
            }

            entity.GroupRef = new List<Group.GroupRef>();
            return entity;
        }

        public Group.GroupFilter ToGroupFilterEntity(GroupFilterRequest request)
        {
            Group.GroupFilter GroupFilter = new Group.GroupFilter();
            GroupFilter.Id = request.Id;
            GroupFilter.OrganizationId = request.OrganizationId;
            GroupFilter.FunctionEnum = Group.FunctionEnum.None;
            GroupFilter.ObjectType = Group.ObjectType.VehicleGroup;
            GroupFilter.GroupType = Group.GroupType.None;
            GroupFilter.GroupRef = request.Vehicles;
            GroupFilter.GroupRefCount =true;
            GroupFilter.GroupIds = new List<int>();
            foreach (int item in request.GroupIds)
            {
                if (item > 0)
                    GroupFilter.GroupIds.Add(item);
            }
            return GroupFilter;
        }

        public OrgVehicleGroupDetails ToOrgVehicleGroup(net.atos.daf.ct2.vehicle.entity.VehicleGroupRequest request)
        {
            OrgVehicleGroupDetails vehicleGroupResonse = new OrgVehicleGroupDetails();
            vehicleGroupResonse.VehicleGroupId = request.VehicleGroupId;
            vehicleGroupResonse.VehicleGroupName = request.VehicleGroupName;
            vehicleGroupResonse.VehicleCount = request.VehicleCount;
            vehicleGroupResonse.UserCount = request.UserCount;
            vehicleGroupResonse.IsGroup = request.IsGroup;
            return vehicleGroupResonse;
        }

        public VehicleGroupDetails ToVehicleGroupDetails(net.atos.daf.ct2.vehicle.entity.VehicleGroup request)
        {
            VehicleGroupDetails vehicleGroupResonse = new VehicleGroupDetails();
            vehicleGroupResonse.Id = request.Id;
            vehicleGroupResonse.GroupId = request.Group_Id;
            vehicleGroupResonse.Name = request.Name;
            return vehicleGroupResonse;
        }

        public Group.GroupFilter ToVehicleGroupLandingFilterEntity(VehicleGroupLandingRequest request)
        {
            Group.GroupFilter GroupFilter = new Group.GroupFilter();
            GroupFilter.Id = request.Id;
            GroupFilter.OrganizationId = request.OrganizationId;
         //   GroupFilter.FunctionEnum = Group.FunctionEnum.None;
            GroupFilter.ObjectType = Group.ObjectType.VehicleGroup;
            GroupFilter.GroupType = Group.GroupType.None;
         //   GroupFilter.GroupRef = request.Vehicles;
         //   GroupFilter.GroupRefCount = true;
            //GroupFilter.GroupIds = new List<int>();
            //foreach (int item in request.GroupIds)
            //{
            //    if (item > 0)
            //        GroupFilter.GroupIds.Add(item);
            //}
            return GroupFilter;
        }

        public VehicleGroupList MapVehicleGroup(net.atos.daf.ct2.vehicle.entity.VehicleGroupList vehiclegroup)
        {
            VehicleGroupList objvehiclegroup = new VehicleGroupList();
            objvehiclegroup.VehicleGroupId = vehiclegroup.VehicleGroupId;
            objvehiclegroup.VehicleGroupName = string.IsNullOrEmpty(vehiclegroup.VehicleGroupName) ? string.Empty : vehiclegroup.VehicleGroupName;
            objvehiclegroup.VehicleId = vehiclegroup.VehicleId;
            objvehiclegroup.VehicleName = string.IsNullOrEmpty(vehiclegroup.VehicleName) ? string.Empty : vehiclegroup.VehicleName;
            objvehiclegroup.Vin = string.IsNullOrEmpty(vehiclegroup.Vin) ? string.Empty : vehiclegroup.Vin;
            objvehiclegroup.SubcriptionStatus = vehiclegroup.SubcriptionStatus;
            return objvehiclegroup;
        }

    }
}
