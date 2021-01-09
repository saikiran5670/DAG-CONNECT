using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.vehicleservice;
using net.atos.daf.ct2.vehiclerepository;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle;

namespace net.atos.daf.ct2.vehicleservice.Services
{
    public class VehicleManagementService : VehicleService.VehicleServiceBase
    {
        private readonly ILogger<VehicleManagementService> _logger;
        private readonly IVehicleManagement _vehicelManagement;

        public VehicleManagementService(ILogger<VehicleManagementService> logger, IVehicleManagement vehicelManagement)
        {
            _logger = logger;
            _vehicelManagement = vehicelManagement;

        }

        public override Task<VehicleResponce> CreateVehicle(VehicleRequest request, ServerCallContext context)
        {
            Vehicle Objvehicle = new Vehicle();
            Vehicle ObjvehicleResponse = new Vehicle();

            Objvehicle.OrganizationId = request.Id;
            Objvehicle.Name = request.Name;
            Objvehicle.VIN = request.Vin;
            Objvehicle.RegistrationNo = "123";
            Objvehicle.ManufactureDate = DateTime.Now;
            Objvehicle.ChassisNo = "123545";
            Objvehicle.StatusDate = DateTime.Now;
            Objvehicle.Status = VehicleStatusType.OptIn;
            Objvehicle.TerminationDate = DateTime.Now;

            ObjvehicleResponse = _vehicelManagement.Create(Objvehicle).Result;
            return Task.FromResult(new VehicleResponce
            {
                Id = ObjvehicleResponse.ID
                // Organizationid = ObjvehicleResponse.OrganizationId,
                // Name = ObjvehicleResponse.Name,
                // VIN = ObjvehicleResponse.VIN

            });
        }

    }
}
