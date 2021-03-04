using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleBusinessService = net.atos.daf.ct2.vehicleservice;

namespace net.atos.daf.ct2.portalservice.Entity.Vehicle
{
    public class Mapper
    {
        public VehicleBusinessService.VehicleRequest ToVehicle(VehicleRequest request)
        {
            var vehicle = new VehicleBusinessService.VehicleRequest();
            vehicle.Id = request.ID;
            vehicle.Name = request.Name;
            vehicle.LicensePlateNumber = request.License_Plate_Number;
            return vehicle;

        }

        public VehicleRequest ToVehicle(VehicleBusinessService.VehicleRequest request)
        {
            var vehicle = new VehicleRequest();
            vehicle.ID = request.Id;
            vehicle.Name = request.Name;
            vehicle.License_Plate_Number = request.LicensePlateNumber;
            return vehicle;

        }

        public VehicleBusinessService.VehicleCreateRequest ToVehicleCreate(VehicleCreateRequest request)
        {
            var vehicle = new VehicleBusinessService.VehicleCreateRequest();
            vehicle.Id = request.ID;
            vehicle.Name = request.Name;
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
            vehicle.License_Plate_Number = request.LicensePlateNumber;
            vehicle.Organization_Id = request.OrganizationId;
            vehicle.Status = request.Status;
            return vehicle;

        }
    }
}
