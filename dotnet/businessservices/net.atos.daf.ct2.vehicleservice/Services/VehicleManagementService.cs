using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.vehicleservice;
using net.atos.daf.ct2.vehiclerepository;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.vehicleservice.Services
{
    public class VehicleManagementService : VehicleService.VehicleServiceBase
    {
        private readonly ILogger<VehicleManagementService> _logger;
        private readonly IVehicleManager _vehicelManager;

        public VehicleManagementService(ILogger<VehicleManagementService> logger, IVehicleManager vehicelManager)
        {
            _logger = logger;
            _vehicelManager = vehicelManager;

        }

        public override Task<VehicleResponce> Create(VehicleRequest request, ServerCallContext context)
        {
            Vehicle Objvehicle = new Vehicle();
            Vehicle ObjvehicleResponse = new Vehicle();

            Objvehicle.Organization_Id = request.Organizationid;
            Objvehicle.Name = request.Name;
            Objvehicle.VIN = request.Vin;
            Objvehicle.License_Plate_Number = request.LicensePlateNumber;
            //Objvehicle.ManufactureDate = request.ManufactureDate;
            //Objvehicle.ChassisNo = request.ChassisNo;
            Objvehicle.Status_Changed_Date = DateTime.Now;
            Objvehicle.Status = VehicleStatusType.OptIn;
            Objvehicle.Termination_Date = DateTime.Now;

            ObjvehicleResponse = _vehicelManager.Create(Objvehicle).Result;
            return Task.FromResult(new VehicleResponce
            {
                Id = ObjvehicleResponse.ID,
                Organizationid = ObjvehicleResponse.Organization_Id,
                Name = ObjvehicleResponse.Name,
                VIN = ObjvehicleResponse.VIN,
                LicensePlateNumber = ObjvehicleResponse.License_Plate_Number
            });
        }

        public override Task<VehicleResponce> Update(VehicleRequest request, ServerCallContext context)
        {
            Vehicle Objvehicle = new Vehicle();
            Vehicle ObjvehicleResponse = new Vehicle();
            Objvehicle.ID = request.Id;
            Objvehicle.Name = request.Name;
            //Objvehicle.VIN = request.Vin;
            Objvehicle.License_Plate_Number = request.LicensePlateNumber;
            //Objvehicle.ManufactureDate = request.ManufactureDate;
            //Objvehicle.ChassisNo = request.ChassisNo;
            //Objvehicle.Status_Changed_Date = DateTime.Now;
            //Objvehicle.Status = VehicleStatusType.OptIn;
            //Objvehicle.Termination_Date = DateTime.Now;

            ObjvehicleResponse = _vehicelManager.Update(Objvehicle).Result;
            return Task.FromResult(new VehicleResponce
            {
                Id = request.Id,
                Organizationid = request.Organizationid,
                Name = ObjvehicleResponse.Name,
                VIN = request.Vin,
                LicensePlateNumber = ObjvehicleResponse.License_Plate_Number
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

            IEnumerable<Vehicle> ObjRetrieveVehicleList = _vehicelManager.Get(ObjVehicleFilter).Result;
            foreach (var item in ObjRetrieveVehicleList)
            {
                VehicleResponce ObjResponce=new VehicleResponce();
                ObjResponce.Id=item.ID;
                ObjResponce.Organizationid=item.Organization_Id;
                ObjResponce.Name=item.Name;

                ObjList.Vehicles.Add(ObjResponce);
            }
            //return Task.FromResult(JsonConvert.DeserializeObject<VehicleList>(Convert.ToString(ObjList)));
            return Task.FromResult(ObjList);

        }

          public override Task<VehicleOptInOptOutResponce> UpdateStatus(VehicleOptInOptOutRequest request, ServerCallContext context)
        {
            VehicleOptInOptOut ObjvehicleOptInOptOut = new VehicleOptInOptOut();
            VehicleOptInOptOut ObjvehicleOptInOptOutResponce = new VehicleOptInOptOut();
            ObjvehicleOptInOptOutResponce.RefId = request.Refid;
            ObjvehicleOptInOptOutResponce.AccountId = request.Accountid;
            ObjvehicleOptInOptOutResponce.Status = VehicleStatusType.OptIn;
            ObjvehicleOptInOptOutResponce.Date=DateTime.Now;
            ObjvehicleOptInOptOutResponce.Type=OptInOptOutType.VehicleLevel;
            ObjvehicleOptInOptOutResponce = _vehicelManager.UpdateStatus(ObjvehicleOptInOptOutResponce).Result;
            return Task.FromResult(new VehicleOptInOptOutResponce
            {
                Refid = ObjvehicleOptInOptOutResponce.RefId,
                Accountid = ObjvehicleOptInOptOutResponce.AccountId
            });
        }

    }
}
