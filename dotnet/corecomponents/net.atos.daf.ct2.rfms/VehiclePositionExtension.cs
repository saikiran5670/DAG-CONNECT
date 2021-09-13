using System.Linq;
using net.atos.daf.ct2.rfms.common;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.response;

namespace net.atos.daf.ct2.rfms
{
    public class VehiclePositionExtension
    {
        private readonly RfmsVehicleMasterTableCache _rfmsVehicleMasterTableCache;

        public VehiclePositionExtension(RfmsVehicleMasterTableCache rfmsVehicleMasterTableCache)
        {
            _rfmsVehicleMasterTableCache = rfmsVehicleMasterTableCache;
        }


        internal void GetLatestOnlyVehiclePostion(RfmsVehiclePosition rfmsVehiclePosition, int thresholdValue)
        {
            rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Take(thresholdValue).ToList();
            string lastVin = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Last().Vin;
            string lastReceivedDateTime = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Last().ReceivedDateTime;//.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
            if (!rfmsVehiclePosition.MoreDataAvailable)
            {
                rfmsVehiclePosition.MoreDataAvailableLink = "/vehiclepositions?LatestOnly=true&lastVin=" + lastVin;
                rfmsVehiclePosition.MoreDataAvailable = true;
            }
            else
            {
                rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Skip(thresholdValue).ToList();
                rfmsVehiclePosition.MoreDataAvailable = false;
                rfmsVehiclePosition.MoreDataAvailableLink = string.Empty;

            }
            // return rfmsVehiclePosition;
        }
        internal void GetStartDateTimeVehiclePostion(RfmsVehiclePosition rfmsVehiclePosition, int thresholdValue)
        {
            rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Take(thresholdValue).ToList();
            string lastVin = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Last().Vin;
            string lastReceivedDateTime = rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions.Last().ReceivedDateTime;//.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
            rfmsVehiclePosition.MoreDataAvailableLink = "/vehiclepositions?starttime=" + lastReceivedDateTime + "&lastVin=" + lastVin;
            rfmsVehiclePosition.MoreDataAvailable = true;
        }
        internal void SetPostionMoreDataAvailableLink(RfmsVehiclePositionRequest rfmsVehiclePositionRequest, RfmsVehiclePosition rfmsVehiclePosition)
        {

            if (rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.Type == DateType.Created.ToString())
            {
                rfmsVehiclePosition.MoreDataAvailableLink += "&datetype=" + DateType.Created;
            }
            if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StopTime))
            {
                rfmsVehiclePosition.MoreDataAvailableLink += "&stoptime=" + rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.StopTime;
            }
            if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.Vin))
            {
                rfmsVehiclePosition.MoreDataAvailableLink += "&vin=" + rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.Vin;
            }
            if (!string.IsNullOrEmpty(rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.TriggerFilter))
            {
                rfmsVehiclePosition.MoreDataAvailableLink += "&triggerFilter=" + _rfmsVehicleMasterTableCache.GetMasterDataValueFromCache(MasterMemoryObjectCacheConstants.TRIGGER_TYPE, rfmsVehiclePositionRequest.RfmsVehiclePositionFilter.TriggerFilter, false);
            }
        }
        internal void SetVehiclePostionTriggerData(RfmsVehiclePosition rfmsVehiclePosition)
        {
            int vehicleCnt = 0;

            foreach (var vehiclePos in rfmsVehiclePosition.VehiclePositionResponse.VehiclePositions)
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
