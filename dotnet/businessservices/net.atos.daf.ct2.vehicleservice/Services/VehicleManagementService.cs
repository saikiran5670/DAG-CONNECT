using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.vehicleservice;
using net.atos.daf.ct2.vehiclerepository;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle;
using System.Collections.Generic;

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

        public override Task<VehicleResponce> Create(VehicleRequest request, ServerCallContext context)
        {
            Vehicle Objvehicle = new Vehicle();
            Vehicle ObjvehicleResponse = new Vehicle();

            Objvehicle.OrganizationId = request.Organizationid;
            Objvehicle.Name = request.Name;
            Objvehicle.VIN = request.Vin;
            Objvehicle.RegistrationNo = request.Registrationno;
            //Objvehicle.ManufactureDate = request.ManufactureDate;
            //Objvehicle.ChassisNo = request.ChassisNo;
            Objvehicle.StatusDate = DateTime.Now;
            Objvehicle.Status = VehicleStatusType.OptIn;
            Objvehicle.TerminationDate = DateTime.Now;

            ObjvehicleResponse = _vehicelManagement.Create(Objvehicle).Result;
            return Task.FromResult(new VehicleResponce
            {
                Id = ObjvehicleResponse.ID,
                Organizationid = ObjvehicleResponse.OrganizationId,
                Name = ObjvehicleResponse.Name,
                VIN = ObjvehicleResponse.VIN,
                RegistrationNo = ObjvehicleResponse.RegistrationNo
            });
        }

        public override Task<VehicleResponce> Update(VehicleRequest request, ServerCallContext context)
        {
            Vehicle Objvehicle = new Vehicle();
            Vehicle ObjvehicleResponse = new Vehicle();
            Objvehicle.ID = request.Id;
            Objvehicle.Name = request.Name;
            Objvehicle.VIN = request.Vin;
            Objvehicle.RegistrationNo = request.Registrationno;
            //Objvehicle.ManufactureDate = request.ManufactureDate;
            //Objvehicle.ChassisNo = request.ChassisNo;
            Objvehicle.StatusDate = DateTime.Now;
            Objvehicle.Status = VehicleStatusType.OptIn;
            Objvehicle.TerminationDate = DateTime.Now;

            ObjvehicleResponse = _vehicelManagement.Update(Objvehicle).Result;
            return Task.FromResult(new VehicleResponce
            {
                Id = ObjvehicleResponse.ID,
                Organizationid = ObjvehicleResponse.OrganizationId,
                Name = ObjvehicleResponse.Name,
                VIN = ObjvehicleResponse.VIN,
                RegistrationNo = ObjvehicleResponse.RegistrationNo
            });
        }

        public override Task<VehicleList> Get(VehicleFilterRequest request, ServerCallContext context)
        {
            VehicleFilter ObjVehicleFilter = new VehicleFilter();
            VehicleList ObjList = new VehicleList();

            ObjVehicleFilter.VehicleId = request.VehicleId;
            ObjVehicleFilter.OrganizationId = request.OrganizationId;
            ObjVehicleFilter.AccountId = request.AccountId;
            ObjVehicleFilter.VehicleGroupId = request.VehicleGroupId;
            ObjVehicleFilter.AccountGroupId = request.AccountGroupId;
            ObjVehicleFilter.FeatureId = request.FeatureId;
            ObjVehicleFilter.VehicleIdList = request.VehicleIdList;
            ObjVehicleFilter.VIN = request.VIN;

            IEnumerable<Vehicle> ObjRetrieveVehicleList = _vehicelManagement.Get(ObjVehicleFilter).Result;
            foreach (var item in ObjRetrieveVehicleList)
            {
                VehicleResponce ObjResponce=new VehicleResponce();
                ObjResponce.Id=item.ID;
                ObjResponce.Organizationid=item.OrganizationId;
                ObjResponce.Name=item.Name;

                ObjList.Vehicles.Add(ObjResponce);
            }

           

            return Task.FromResult(ObjList);

        }

    }
}
