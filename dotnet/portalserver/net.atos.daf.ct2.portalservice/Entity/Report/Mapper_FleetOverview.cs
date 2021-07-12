using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Report
{
    public partial class Mapper
    {
        public List<VehicleHealthStatus> ToVehicleHealthStatus(reportservice.VehicleHealthStatusListResponse vhsList)
        {
            var hs = new List<VehicleHealthStatus>();
            foreach (var item in vhsList.HealthStatus)
            {
                var vhs = new VehicleHealthStatus()
                {
                    VehicleRegNo = item.VehicleRegNo,
                    VehicleName = item.VehicleName,
                    TripId = item.LctsTripId,
                    Vin = item.LctsVin,
                    TripStartTime = item.LctsTripStartTime,
                    TripEndTime = item.LctsTripEndTime,
                    Driver1Id = item.LctsDriver1Id,
                    TripDistance = item.LctsTripDistance,
                    DrivingTime = item.LctsDrivingTime,
                    FuelConsumption = item.LctsFuelConsumption,
                    VehicleDrivingStatus_type = item.LctsVehicleDrivingStatustype,
                    OdometerVal = item.LctsOdometerVal,
                    DistanceUntilNextService = item.LctsDistanceUntilNextService,
                    LatestReceivedPositionLattitude = item.LctsLatestReceivedPositionLattitude,
                    LatestReceivedPositionLongitude = item.LctsLatestReceivedPositionLongitude,
                    LatestReceivedPositionHeading = item.LctsLatestReceivedPositionHeading,
                    LatestGeolocationAddressId = item.LatgeoaddLatestGeolocationAddressId,
                    LatestGeolocationAddress = item.LatgeoaddLatestGeolocationAddress,
                    StartPositionLattitude = item.LctsStartPositionLattitude,
                    StartPositionLongitude = item.LctsStartPositionLongitude,
                    StartPositionHeading = item.LctsStartPositionHeading,
                    StartGeolocationAddressId = item.StageoaddStartGeolocationAddressId,
                    StartGeolocationAddress = item.StageoaddStartGeolocationAddress,
                    LatestProcessedMessageTimestamp = item.LctsLatestProcessedMessageTimestamp,
                    VehicleHealthStatusType = item.LctsVehicleHealthStatusType,
                    LatestWrningClass = item.LctsLatestWrningClass,
                    LatestWarningNumber = item.LctsLatestWarningNumber,
                    LatestWarningType = item.LctsLatestWarningType,
                    LatestWarningTimestamp = item.LctsLatestWarningTimestamp,
                    LatestWarningPositionLatitude = item.LctsLatestWarningPositionLatitude,
                    LatestWarningPositionLongitude = item.LctsLatestWarningPositionLongitude,
                    LatestWarningGeolocationAddressId = item.WangeoaddLatestWarningGeolocationAddressId,
                    LatestWarningGeolocationAddress = item.WangeoaddLatestWarningGeolocationAddress,
                    WarningId = item.WarningId,
                    DriverName = item.DriverName,
                    WarningTripId = item.WarningTripId,
                    WarningVin = item.WarningVin,
                    WarningTimetamp = item.WarningTimetamp,
                    WarningClass = item.WarningClass,
                    WarningNumber = item.WarningNumber,
                    WarningLat = item.WarningLat,
                    WarningLng = item.WarningLng,
                    WarningHeading = item.WarningHeading,
                    WarningVehicleHealthStatusType = item.WarningVehicleHealthStatusType,
                    WarningVehicleDrivingStatusType = item.WarningVehicleDrivingStatusType,
                    WarningDrivingId = item.WarningDrivingId,
                    WarningType = item.WarningType,
                    WarningDistanceUntilNectService = item.WarningDistanceUntilNectService,
                    WarningOdometerVal = item.WarningOdometerVal,
                    WarningLatestProcessedMessageTimestamp = item.WarningLatestProcessedMessageTimestamp,
                    WarningName = item.WarningName, //from dtcwarning table
                    WarningAdvice = item.WarningAdvice,
                };
                hs.Add(vhs);

            }
            return hs;
        }






    }
}
