using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using net.atos.daf.ct2.rfms;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.response;

namespace net.atos.daf.ct2.rfmsdataservice.Entity
{
    internal class Mapper
    {
        internal readonly IHttpContextAccessor _httpContextAccessor;
        internal readonly vehicle.IVehicleManager _vehicleManager;

        internal Mapper(IHttpContextAccessor httpContextAccessor, vehicle.IVehicleManager vehicleManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _vehicleManager = vehicleManager;
        }

        internal VehiclePositionResponseObject MapVehiclePositionResponse(RfmsVehiclePosition rfmsVehiclePosition)
        {
            DateTime currentdatetime = DateTime.Now;

            List<VehiclePositions> vehiclePositions = new List<VehiclePositions>();

            foreach (var vehicle in rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions)
            {
                Entity.TachoDriverIdentification tachoDriverIdentification = new Entity.TachoDriverIdentification()
                {
                    DriverIdentification = vehicle.TriggerType.DriverId.TachoDriverIdentification.DriverIdentification,
                    CardIssuingMemberState = vehicle.TriggerType.DriverId.TachoDriverIdentification.CardIssuingMemberState,
                    CardRenewalIndex = vehicle.TriggerType.DriverId.TachoDriverIdentification.CardRenewalIndex,
                    CardReplacementIndex = vehicle.TriggerType.DriverId.TachoDriverIdentification.CardReplacementIndex,
                    DriverAuthenticationEquipment = vehicle.TriggerType.DriverId.TachoDriverIdentification.DriverAuthenticationEquipment
                };

                Entity.OemDriverIdentification oemDriverIdentification = new Entity.OemDriverIdentification()
                {
                    IdType = vehicle.TriggerType.DriverId.OemDriverIdentification.IdType,
                    OemDriverId = vehicle.TriggerType.DriverId.OemDriverIdentification.DriverIdentification
                };

                DriverIdObject driverIdObject = new DriverIdObject()
                {
                    TachoDriverIdentification = tachoDriverIdentification,
                    OemDriverIdentification = oemDriverIdentification
                };

                TellTaleObject tellTaleObject = new TellTaleObject()
                {
                    State = vehicle.TriggerType.TellTaleInfo.State,
                    TellTale = vehicle.TriggerType.TellTaleInfo.TellTale,
                    OemTellTale = vehicle.TriggerType.TellTaleInfo.OemTellTale
                };

                TriggerObject triggerObject = new TriggerObject()
                {
                    Context = vehicle.TriggerType.Context,
                    PtoId = vehicle.TriggerType.PtoId,
                    TriggerInfo = vehicle.TriggerType.TriggerInfo,
                    DriverId = driverIdObject,
                    TriggerType = vehicle.TriggerType.Type,
                    TellTaleInfo = tellTaleObject
                };

                Entity.GNSSPositionObject gNSSPositionObject = new GNSSPositionObject()
                {
                    Altitude = vehicle.GnssPosition.Altitude,
                    Heading = vehicle.GnssPosition.Heading,
                    Latitude = vehicle.GnssPosition.Latitude,
                    Longitude = vehicle.GnssPosition.Longitude,
                    PositionDateTime = vehicle.GnssPosition.PositionDateTime,// "yyyy-MM-ddThh:mm:ss.fffZ"),
                    Speed = vehicle.GnssPosition.Speed
                };

                VehiclePositions vehiclePosition = new VehiclePositions()
                {
                    CreatedDateTime = vehicle.CreatedDateTime,//"yyyy-MM-ddThh:mm:ss.fffZ"),
                    ReceivedDateTime = vehicle.ReceivedDateTime,//"yyyy-MM-ddThh:mm:ss.fffZ"),
                    TachographSpeed = vehicle.TachographSpeed,
                    WheelBasedSpeed = vehicle.WheelBasedSpeed,
                    TriggerType = triggerObject,
                    Vin = vehicle.Vin,
                    GnssPosition = gNSSPositionObject
                };

                vehiclePositions.Add(vehiclePosition);
            }

            Entity.VehiclePositionResponse vehiclePositionResponse = new Entity.VehiclePositionResponse()
            {
                VehiclePositions = vehiclePositions
            };

            return new VehiclePositionResponseObject()
            {
                RequestServerDateTime = currentdatetime.ToString("yyyy-MM-ddThh:mm:ss.fffZ"),
                MoreDataAvailable = rfmsVehiclePosition.MoreDataAvailable,
                MoreDataAvailableLink = rfmsVehiclePosition.MoreDataAvailableLink,
                VehiclePositionResponse = vehiclePositionResponse
            };
        }

        internal RfmsVehiclePositionRequest MapVehiclePositionRequest(RfmsVehiclePositionStatusFilter requestFilter)
        {
            var vehiclePositionRequest = new RfmsVehiclePositionRequest() { RfmsVehiclePositionFilter = requestFilter };
            vehiclePositionRequest.RfmsVehiclePositionFilter.Type = string.IsNullOrEmpty(requestFilter.Type) ? DateType.Received.ToString() : DateType.Created.ToString();
            return vehiclePositionRequest;
        }

        internal VehicleResponseObject MapVehiclesRecord(RfmsVehicles rfmsVehicles)
        {
            VehicleResponseObject responseObject = new VehicleResponseObject();
            VehicleResponse vehicleObject = new VehicleResponse();
            vehicleObject.Vehicles = new List<Entity.Vehicle>();
            int vehicleCnt = 0;

            //validate authorize paths
            List<string> lstAuthorizedPaths = new List<string>();
            if (_httpContextAccessor.HttpContext.Items["AuthorizedPaths"] != null)
            {
                lstAuthorizedPaths = (List<string>)_httpContextAccessor.HttpContext.Items["AuthorizedPaths"];
            }

            foreach (var item in rfmsVehicles.Vehicles)
            {
                Entity.Vehicle vehicleObj = new Entity.Vehicle();
                vehicleObj.Vin = item.Vin;
                vehicleObj.CustomerVehicleName = item.CustomerVehicleName;
                vehicleObj.Brand = item.Brand;

                if (item.ProductionDate != null)
                {
                    Entity.ProductionDate prdDate = new Entity.ProductionDate();
                    prdDate.Day = item.ProductionDate.Day;
                    prdDate.Month = item.ProductionDate.Month;
                    prdDate.Year = item.ProductionDate.Year;
                    if (prdDate.Day != 0 && prdDate.Month != 0 && prdDate.Year != 0)
                        vehicleObj.ProductionDate = prdDate;
                    else vehicleObj.ProductionDate = null;
                }
                //Below commented fields as due to no db mapping provided by database team and is currently seeks clarification from DAF
                vehicleObj.Type = item.Type;
                vehicleObj.Model = item.Model;
                vehicleObj.PossibleFuelType = item.PossibleFuelType;
                vehicleObj.EmissionLevel = item.EmissionLevel;
                //vehicleObj.TellTaleCode = item.TellTaleCode;
                vehicleObj.ChassisType = item.ChassisType;
                vehicleObj.NoOfAxles = item.NoOfAxles;
                vehicleObj.TotalFuelTankVolume = item.TotalFuelTankVolume ;

                //vehicleObj.TachographType = item.TachographType;
                vehicleObj.GearboxType = item.GearboxType;
                //vehicleObj.BodyType = item.BodyType;
                //vehicleObj.DoorConfiguration = item.DoorConfiguration;
                //vehicleObj.HasRampOrLift = item.HasRampOrLift;
                vehicleObj.AuthorizedPaths = lstAuthorizedPaths;
                vehicleObject.Vehicles.Add(vehicleObj);
                vehicleCnt++;
            }
            if (rfmsVehicles.MoreDataAvailable)
            {
                responseObject.MoreDataAvailable = true;
                responseObject.MoreDataAvailableLink = "/rfms/vehicles?lastVin='" + rfmsVehicles.Vehicles.Last().Vin + "'";
            }
            else
            {
                responseObject.MoreDataAvailable = false;
            }

            responseObject.VehicleResponse = vehicleObject;

            return responseObject;
        }

        internal RfmsVehicleStatusRequest MapVehicleStatusRequest(RfmsVehiclePositionStatusFilter request)
        {
            var vehicleStatusRequest = new RfmsVehicleStatusRequest()
            {
                RfmsVehicleStatusFilter = request
            };
            vehicleStatusRequest.RfmsVehicleStatusFilter.Type = string.IsNullOrEmpty(request.Type) ? DateType.Received.ToString() : DateType.Created.ToString();
            return vehicleStatusRequest;
        }
        #region Validators
        //internal bool ValidateVehiclePositionParameters(ref RfmsVehiclePositionRequest vehiclePositionRequest, out string field)
        //{
        //    field = string.Empty;

        //    //Validate StartTime
        //    if (!string.IsNullOrEmpty(vehiclePositionRequest.StartTime))
        //    {
        //        if (!DateTime.TryParse(vehiclePositionRequest.StartTime, out _))
        //        {
        //            field = nameof(vehiclePositionRequest.StartTime);
        //            return false;
        //        }
        //    }

        //    //Validate StopTime
        //    if (!string.IsNullOrEmpty(vehiclePositionRequest.StopTime))
        //    {
        //        if (!DateTime.TryParse(vehiclePositionRequest.StopTime, out _) || string.IsNullOrEmpty(vehiclePositionRequest.StartTime))
        //        {
        //            field = nameof(vehiclePositionRequest.StopTime);
        //            return false;
        //        }
        //    }

        //    //Validate VIN
        //    if (!string.IsNullOrEmpty(vehiclePositionRequest.Vin))
        //    {
        //        string vin = vehiclePositionRequest.Vin;
        //        Task<int> vinNo = Task.Run<int>(async () => await _vehicleManager.IsVINExists(vin));
        //        if (vinNo.Result == 0)
        //        {
        //            field = nameof(vehiclePositionRequest.Vin);
        //            return false;
        //        }
        //    }

        //    //Validate Last VIN
        //    if (!string.IsNullOrEmpty(vehiclePositionRequest.LastVin))
        //    {
        //        string vin = vehiclePositionRequest.LastVin;
        //        Task<int> vinNo = Task.Run<int>(async () => await _vehicleManager.IsVINExists(vin));
        //        if (vinNo.Result == 0)
        //        {
        //            field = nameof(vehiclePositionRequest.LastVin);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        internal bool ValidateVehicleStatusParameters(RfmsVehiclePositionStatusFilter rfmsVehicleStatusRequest, out string field)
        {
            field = string.Empty;

            //Validate StartTime
            if (!string.IsNullOrEmpty(rfmsVehicleStatusRequest.StartTime))
            {
                if (!DateTime.TryParse(rfmsVehicleStatusRequest.StartTime, out _))
                {
                    field = nameof(rfmsVehicleStatusRequest.StartTime);
                    return false;
                }
            }

            //Validate StopTime
            if (!string.IsNullOrEmpty(rfmsVehicleStatusRequest.StopTime))
            {
                if (!DateTime.TryParse(rfmsVehicleStatusRequest.StopTime, out _) || string.IsNullOrEmpty(rfmsVehicleStatusRequest.StartTime))
                {
                    field = nameof(rfmsVehicleStatusRequest.StopTime);
                    return false;
                }
            }

            //Validate VIN
            if (!string.IsNullOrEmpty(rfmsVehicleStatusRequest.Vin))
            {
                string vin = rfmsVehicleStatusRequest.Vin;
                Task<int> vinNo = Task.Run<int>(async () => await _vehicleManager.IsVINExists(vin));
                if (vinNo.Result == 0)
                {
                    field = nameof(rfmsVehicleStatusRequest.Vin);
                    return false;
                }
            }

            //Validate Last VIN
            if (!string.IsNullOrEmpty(rfmsVehicleStatusRequest.LastVin))
            {
                string vin = rfmsVehicleStatusRequest.LastVin;
                Task<int> vinNo = Task.Run<int>(async () => await _vehicleManager.IsVINExists(vin));
                if (vinNo.Result == 0)
                {
                    field = nameof(rfmsVehicleStatusRequest.LastVin);
                    return false;
                }
            }
            return true;
        }
        internal bool ValidateParameter(ref string lastVin, out bool moreData)
        {
            moreData = true;
            if (string.IsNullOrEmpty(lastVin))
            {
                moreData = false;
            }
            else
            {
                //Validate Vin no from Db
                string lastVinNo = lastVin;
                Task<int> vinNo = Task.Run<int>(async () => await _vehicleManager.IsVINExists(lastVinNo));
                if (vinNo.Result == 0)
                {
                    moreData = false;
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
