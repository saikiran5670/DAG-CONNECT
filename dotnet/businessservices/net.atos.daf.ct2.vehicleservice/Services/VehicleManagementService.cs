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
            try
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
                Objvehicle.Status = (vehicle.VehicleStatusType)Enum.Parse(typeof(vehicle.VehicleStatusType), request.Status.ToString().ToUpper()); //GetVehicleStatusEnum((int)request.Status);
                Objvehicle.Termination_Date = DateTime.Now;

                ObjvehicleResponse = _vehicelManager.Create(Objvehicle).Result;

                return Task.FromResult(new VehicleResponce
                {
                    Message = "Vehicle created with id:- " + ObjvehicleResponse.ID,
                    Code = Responcecode.Success

                });

            }
            catch (Exception ex)
            {
                return Task.FromResult(new VehicleResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public override Task<VehicleResponce> Update(VehicleRequest request, ServerCallContext context)
        {
            try
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
                    Message = "Vehicle updated for id:- " + ObjvehicleResponse.ID,
                    Code = Responcecode.Success
                });

            }
            catch (Exception ex)
            {
                return Task.FromResult(new VehicleResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public async override Task<VehicleListResponce> Get(VehicleFilterRequest request, ServerCallContext context)
        {
            try
            {
                VehicleFilter ObjVehicleFilter = new VehicleFilter();
                VehicleListResponce ObjVehicleList = new VehicleListResponce();

                ObjVehicleFilter.VehicleId = request.VehicleId;
                ObjVehicleFilter.OrganizationId = request.OrganizationId;
                ObjVehicleFilter.AccountId = request.AccountId;
                ObjVehicleFilter.VehicleGroupId = request.VehicleGroupId;
                ObjVehicleFilter.AccountGroupId = request.AccountGroupId;
                ObjVehicleFilter.FeatureId = request.FeatureId;
                ObjVehicleFilter.VehicleIdList = request.VehicleIdList;
                ObjVehicleFilter.VIN = request.VIN;
                ObjVehicleFilter.Status = (vehicle.VehicleStatusType)Enum.Parse(typeof(vehicle.VehicleStatusType), request.Status.ToString().ToUpper());

                IEnumerable<Vehicle> ObjRetrieveVehicleList = _vehicelManager.Get(ObjVehicleFilter).Result;
                foreach (var item in ObjRetrieveVehicleList)
                {
                    VehicleRequest ObjResponce = new VehicleRequest();
                    ObjResponce.Id = item.ID;
                    ObjResponce.Organizationid = item.Organization_Id;
                    ObjResponce.Name = item.Name;
                    ObjResponce.Vin = item.VIN;
                    ObjResponce.LicensePlateNumber = item.License_Plate_Number;
                    ObjResponce.Status = (VehicleStatusType)(char)item.Status;

                    ObjVehicleList.Vehicles.Add(ObjResponce);
                }
                ObjVehicleList.Message = "Vehicles data retrieved";
                ObjVehicleList.Code = Responcecode.Success;
                return await Task.FromResult(ObjVehicleList);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new VehicleListResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }


        }

        public override Task<VehicleOptInOptOutResponce> UpdateStatus(VehicleOptInOptOutRequest request, ServerCallContext context)
        {
            try
            {
                VehicleOptInOptOut ObjvehicleOptInOptOut = new VehicleOptInOptOut();
                VehicleOptInOptOut ObjvehicleOptInOptOutResponce = new VehicleOptInOptOut();
                ObjvehicleOptInOptOutResponce.RefId = request.Refid;
                ObjvehicleOptInOptOutResponce.AccountId = request.Accountid;
                ObjvehicleOptInOptOutResponce.Status = (vehicle.VehicleStatusType)Enum.Parse(typeof(vehicle.VehicleStatusType), request.Status.ToString().ToUpper()); //GetVehicleStatusEnum((int)request.Status);
                ObjvehicleOptInOptOutResponce.Date = DateTime.Now;
                ObjvehicleOptInOptOutResponce.Type = (vehicle.OptInOptOutType)Enum.Parse(typeof(vehicle.OptInOptOutType), request.OptInOptOutType.ToString()); //GetOptInOptOutEnum((int)request.OptInOptOutType);
                ObjvehicleOptInOptOutResponce = _vehicelManager.UpdateStatus(ObjvehicleOptInOptOutResponce).Result;
                return Task.FromResult(new VehicleOptInOptOutResponce
                {
                    Message = "Status updated for " + ObjvehicleOptInOptOutResponce.RefId,
                     Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new VehicleOptInOptOutResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }
    }
}
