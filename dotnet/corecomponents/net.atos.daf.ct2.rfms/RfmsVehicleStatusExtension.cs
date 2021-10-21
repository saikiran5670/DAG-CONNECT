using System.Linq;
using net.atos.daf.ct2.rfms.common;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.response;

namespace net.atos.daf.ct2.rfms
{
    public class RfmsVehicleStatusExtension
    {
        private readonly RfmsVehicleMasterTableCache _rfmsVehicleMasterTableCache;

        public RfmsVehicleStatusExtension(RfmsVehicleMasterTableCache rfmsVehicleMasterTableCache)
        {
            _rfmsVehicleMasterTableCache = rfmsVehicleMasterTableCache;
        }


        internal void GetLatestOnlyVehicleStatus(RfmsVehicleStatus rfmsVehicleStatus, int thresholdValue)
        {
            rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses = rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses.Take(thresholdValue).ToList();
            string lastVin = rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses.Last().Vin;
            string lastReceivedDateTime = rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses.Last().ReceivedDateTime;//.ToString("yyyy-MM-ddThh:mm:ss.fffZ");

            if (!rfmsVehicleStatus.MoreDataAvailable)
            {
                rfmsVehicleStatus.MoreDataAvailableLink = "/vehiclestatus?LatestOnly=true&lastVin=" + lastVin;
                rfmsVehicleStatus.MoreDataAvailable = true;
            }
            else
            {
                rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses = rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses.Skip(thresholdValue).ToList();
                rfmsVehicleStatus.MoreDataAvailable = false;
                rfmsVehicleStatus.MoreDataAvailableLink = string.Empty;

            }
            // return rfmsVehicleStatus;
        }
        internal void GetStartDateTimeVehicleStatus(RfmsVehicleStatus rfmsVehicleStatus, int thresholdValue)
        {
            rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses = rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses.Take(thresholdValue).ToList();
            string lastVin = rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses.Last().Vin;
            string lastReceivedDateTime = rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses.Last().ReceivedDateTime;//.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
            rfmsVehicleStatus.MoreDataAvailableLink = "//vehiclestatus?starttime=" + lastReceivedDateTime + "&lastVin=" + lastVin;
            rfmsVehicleStatus.MoreDataAvailable = true;
        }
        internal void SetStatusMoreDataAvailableLink(RfmsVehicleStatusRequest rfmsVehicleStatusRequest, RfmsVehicleStatus rfmsVehicleStatus)
        {

            if (rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.Type == DateType.Created.ToString())
            {
                rfmsVehicleStatus.MoreDataAvailableLink += "&datetype=" + DateType.Created;
            }
            if (!string.IsNullOrEmpty(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StopTime))
            {
                rfmsVehicleStatus.MoreDataAvailableLink += "&stoptime=" + rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.StopTime;
            }
            if (!string.IsNullOrEmpty(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.Vin))
            {
                rfmsVehicleStatus.MoreDataAvailableLink += "&vin=" + rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.Vin;
            }
            if (!string.IsNullOrEmpty(rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.TriggerFilter))
            {
                rfmsVehicleStatus.MoreDataAvailableLink += "&triggerFilter=" + _rfmsVehicleMasterTableCache.GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TRIGGER_TYPE, rfmsVehicleStatusRequest.RfmsVehicleStatusFilter.TriggerFilter, false);
            }
        }
        internal void SetVehicleStatusTriggerData(RfmsVehicleStatus rfmsVehicleStatus)
        {
            int vehicleCnt = 0;

            foreach (var vehiclePos in rfmsVehicleStatus.VehicleStatusResponse.VehicleStatuses)
            {
                if (!string.IsNullOrEmpty(vehiclePos.TriggerType.Type))
                {
                    string triggerName = _rfmsVehicleMasterTableCache.GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TRIGGER_TYPE, vehiclePos.TriggerType.Type, false);
                    vehiclePos.TriggerType.Type = triggerName;
                }
                if (!string.IsNullOrEmpty(vehiclePos.TriggerType.DriverId.TachoDriverIdentification.DriverAuthenticationEquipment))
                {
                    string driverAuthId = _rfmsVehicleMasterTableCache.GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.DRIVER_AUTH_EQUIPMENT, vehiclePos.TriggerType.DriverId.TachoDriverIdentification.DriverAuthenticationEquipment, false);
                    vehiclePos.TriggerType.DriverId.TachoDriverIdentification.DriverAuthenticationEquipment = driverAuthId;
                }
                if (!string.IsNullOrEmpty(vehiclePos.TriggerType.TellTaleInfo.TellTale))
                {
                    string tellTale = _rfmsVehicleMasterTableCache.GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TALE_TELL, vehiclePos.TriggerType.TellTaleInfo.TellTale, false);
                    vehiclePos.TriggerType.TellTaleInfo.TellTale = tellTale;
                }
                if (!string.IsNullOrEmpty(vehiclePos.TriggerType.TellTaleInfo.State))
                {
                    string state = _rfmsVehicleMasterTableCache.GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TALE_TELL_STATE, vehiclePos.TriggerType.TellTaleInfo.State, false);
                    vehiclePos.TriggerType.TellTaleInfo.State = state;
                }
                vehicleCnt++;
            }
        }
    }
}

