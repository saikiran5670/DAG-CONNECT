using System.Collections.Generic;
using System.Linq;
using VehicleBusinessService = net.atos.daf.ct2.vehicleservice;

namespace net.atos.daf.ct2.portalservice.Entity.Vehicle
{
    public class Mapper
    {
        public VehicleBusinessService.VehicleRequest ToVehicle(VehicleRequest request)
        {
            var vehicle = new VehicleBusinessService.VehicleRequest();
            vehicle.Id = request.ID;
            vehicle.Name = request.Name ?? string.Empty;
            vehicle.LicensePlateNumber = request.License_Plate_Number ?? string.Empty;
            vehicle.OrganizationId = request.Organization_Id;
            return vehicle;

        }

        public VehicleRequest ToVehicle(VehicleBusinessService.VehicleRequest request)
        {
            var vehicle = new VehicleRequest();
            vehicle.ID = request.Id;
            vehicle.Name = request.Name;
            vehicle.License_Plate_Number = request.LicensePlateNumber;
            vehicle.Organization_Id = request.OrganizationId;
            return vehicle;

        }

        public VehicleBusinessService.VehicleCreateRequest ToVehicleCreate(VehicleCreateRequest request)
        {
            var vehicle = new VehicleBusinessService.VehicleCreateRequest();
            vehicle.Id = request.ID;
            vehicle.Name = request.Name;
            vehicle.Vin = request.VIN;
            vehicle.LicensePlateNumber = request.License_Plate_Number;
            vehicle.OrganizationId = request.Organization_Id;
            vehicle.Status = request.Status;
            return vehicle;
        }

        public VehicleCreateRequest ToVehicleCreate(VehicleBusinessService.VehicleCreateRequest request)
        {
            var vehicle = new VehicleCreateRequest();
            vehicle.ID = request.Id;
            vehicle.Name = request.Name;
            vehicle.VIN = request.Vin;
            vehicle.License_Plate_Number = request.LicensePlateNumber;
            vehicle.Organization_Id = request.OrganizationId;
            vehicle.Status = request.Status;
            return vehicle;

        }

        public VehicleBusinessService.VehicleFilterRequest ToVehicleFilter(VehicleFilterRequest request)
        {
            var vehicle = new VehicleBusinessService.VehicleFilterRequest();
            vehicle.VehicleId = request.VehicleId;
            vehicle.OrganizationId = request.OrganizationId;
            if (request.VIN != null)
                vehicle.VIN = request.VIN;
            //vehicle.Status = request.Status;
            if (!string.IsNullOrEmpty(request.VehicleIdList))
                vehicle.VehicleIdList = request.VehicleIdList;
            return vehicle;
        }

        public VehicleResponse ToVehicle(VehicleBusinessService.VehicleDetails response)
        {
            var vehicle = new VehicleResponse();
            if (response == null) return vehicle;
            vehicle.ID = response.Id;
            vehicle.Name = response.Name;
            if (!string.IsNullOrEmpty(response.Status)) vehicle.Status = response.Status;
            vehicle.VIN = response.Vin;
            vehicle.License_Plate_Number = response.LicensePlateNumber;
            vehicle.ModelId = response.ModelId;
            vehicle.Organization_Id = response.Organizationid;
            vehicle.Tcu_Brand = response.TcuBrand;
            vehicle.Tcu_Id = response.TcuId;
            vehicle.Tcu_Serial_Number = response.TcuSerialNumber;
            vehicle.Tcu_Version = response.TcuVersion;
            vehicle.Is_Tcu_Register = response.IsTcuRegister;

            return vehicle;
        }

        public List<VehicleResponse> ToVehicles(VehicleBusinessService.VehicleListResponce response)
        {
            var vehicles = new List<VehicleResponse>();
            if (response == null || response.Vehicles == null) return vehicles;
            foreach (var vehicle in response.Vehicles)
            {
                vehicles.Add(ToVehicle(vehicle));
            }
            return vehicles;
        }

        public VehicleBusinessService.VehicleGroupRequest ToVehicleGroup(VehicleGroupRequest request)
        {
            VehicleBusinessService.VehicleGroupRequest group = new VehicleBusinessService.VehicleGroupRequest();

            group.Id = request.Id;
            group.Name = request.Name;
            group.Description = request.Description;
            group.OrganizationId = request.OrganizationId;
            group.RefId = 0;
            if (!string.IsNullOrEmpty(request.GroupType))
            {
                group.GroupType = request.GroupType;
            }
            if (!string.IsNullOrEmpty(request.FunctionEnum))
            {
                group.FunctionEnum = request.FunctionEnum;
            }
            if (request.Vehicles != null)
            {
                //group.GroupRef = new List<AccountBusinessService.AccountGroupRef>();
                foreach (var groupref in request.Vehicles)
                {
                    if (groupref.VehicleId > 0)
                        group.GroupRef.Add(new VehicleBusinessService.VehicleGroupRef() { GroupId = groupref.VehicleGroupId, RefId = groupref.VehicleId });
                }
            }
            return group;
        }

        public VehicleGroupResponse ToVehicleGroup(VehicleBusinessService.VehicleGroupResponce request)
        {
            VehicleGroupResponse group = new VehicleGroupResponse();
            if (request.VehicleGroup == null) return group;

            group.Id = request.VehicleGroup.Id;
            group.Name = request.VehicleGroup.Name;
            group.Description = request.VehicleGroup.Description;
            group.RefId = request.VehicleGroup.RefId;
            group.OrganizationId = request.VehicleGroup.OrganizationId;
            // group type
            if (!string.IsNullOrEmpty(request.VehicleGroup.GroupType))
            {
                group.GroupType = request.VehicleGroup.GroupType;
            }

            if (!string.IsNullOrEmpty(request.VehicleGroup.FunctionEnum))
            {
                group.FunctionEnum = request.VehicleGroup.FunctionEnum;
            }

            if (request.VehicleGroup.GroupRef != null)
            {
                group.GroupRef = new List<GroupRefRequest>();
                foreach (var groupref in request.VehicleGroup.GroupRef)
                {
                    group.GroupRef.Add(new GroupRefRequest() { VehicleGroupId = groupref.GroupId, VehicleId = groupref.RefId });
                }
            }
            return group;
        }

        public VehicleBusinessService.GroupFilterRequest ToVehicleGroupFilter(VehicleGroupFilterRequest request)
        {

            VehicleBusinessService.GroupFilterRequest group = new VehicleBusinessService.GroupFilterRequest();

            group.Id = request.Id;
            group.OrganizationId = request.OrganizationId;
            group.Vehicles = request.Vehicles;
            group.VehiclesGroup = request.VehiclesGroup;
            //group.GroupIds = new List<int>();
            if (request.GroupIds.Count() > 0)
            {
                foreach (var groupref in request.GroupIds)
                {
                    group.GroupIds.Add(groupref);
                }
            }
            return group;
        }

        public VehicleBusinessService.VehicleGroupLandingRequest ToVehicleGroupLandingFilter(int OrganizationId)
        {

            VehicleBusinessService.VehicleGroupLandingRequest group = new VehicleBusinessService.VehicleGroupLandingRequest();

            group.Id = 0;
            group.OrganizationId = OrganizationId;
            group.FunctionEnum = "N";
            group.GroupType = "G";
            return group;
        }


        public VehicleBusinessService.VehicleConnectRequest ToUpdateVehicleConnection(List<VehicleConnectionSettings> poiList)
        {

            var request = new VehicleBusinessService.VehicleConnectRequest();
            request.Vehicles.AddRange(poiList.Select(x => new VehicleBusinessService.VehicleConnection()
            {
                ModifiedBy = x.ModifiedBy,
                OptIn = x.Opt_In.ToString(),
                VehicleId = x.VehicleId
            }).ToList());
            return request;

        }
    }
}
