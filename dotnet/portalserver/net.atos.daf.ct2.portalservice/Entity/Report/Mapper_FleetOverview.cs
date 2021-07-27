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
                    TripId = item.WarningTripId,
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
                    WarningAddress = item.WarningAddress,
                    WarningAddressId = item.WarningAddressId,
                    Icon = item.Icon != null ? item.Icon.ToByteArray() : new Byte[] { },
                    ColorName = item.ColorName,
                    IconName = item.IconName,
                    IconId = item.IconId
                };
                hs.Add(vhs);

            }
            return hs;
        }






    }
}
