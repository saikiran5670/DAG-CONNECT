﻿using net.atos.daf.ct2.vehicle.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Group = net.atos.daf.ct2.group;

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
            return vehicle;
        }

        public VehicleFilter ToVehicleFilterEntity(VehicleFilterRequest request)
        {
            VehicleFilter vehicleFilter = new VehicleFilter();
            vehicleFilter.VehicleId = request.VehicleId;
            vehicleFilter.OrganizationId = request.OrganizationId;
            vehicleFilter.VehicleIdList = request.VehicleIdList;
            vehicleFilter.VIN = request.VIN;
            vehicleFilter.Status = (vehicle.VehicleStatusType)Enum.Parse(typeof(vehicle.VehicleStatusType), request.Status.ToString());
            return vehicleFilter;
        }

        public VehicleDetails ToVehicle(Vehicle vehicle)
        {
            VehicleDetails vehicledetails = new VehicleDetails();
            vehicledetails.Id = vehicle.ID;
            vehicledetails.Organizationid = vehicle.Organization_Id;
            vehicledetails.Vin = vehicle.VIN;
            vehicledetails.LicensePlateNumber = vehicle.License_Plate_Number;
            vehicledetails.ModelId = vehicle.ModelId;
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
                else
                {
                    entity.GroupType = Group.GroupType.Group;
                }
            }
            else
            {
                entity.GroupType = Group.GroupType.Group;
            }
            entity.ObjectType = group.ObjectType.VehicleGroup;
            entity.OrganizationId = request.OrganizationId;
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
            GroupFilter.GroupType = Group.GroupType.Group;
            GroupFilter.GroupRef = request.Vehicles;
            GroupFilter.GroupRefCount =true;
            foreach (var item in request.GroupIds)
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
    }
}
