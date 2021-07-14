using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.reports.entity
{
    public class MapperRepo
    {
        public List<FleetOverviewDetails> GetFleetOverviewDetails(IEnumerable<FleetOverviewResult> fleetOverviewResult)
        {
            List<FleetOverviewDetails> fleetOverviewDetailsList = new List<FleetOverviewDetails>();

            //Lookups are implemeted to avoid inserting duplicate entry of same id into the list
            Dictionary<int, FleetOverviewDetails> fleetOverviewDetailsLookup = new Dictionary<int, FleetOverviewDetails>();
            Dictionary<int, LiveFleetPosition> liveFleetPositionLookup = new Dictionary<int, LiveFleetPosition>();
            Dictionary<int, FleetOverviewAlert> liveFleetAlertLookup = new Dictionary<int, FleetOverviewAlert>();
            foreach (var fleetOverviewItem in fleetOverviewResult)
            {
                if (!fleetOverviewDetailsLookup.TryGetValue(Convert.ToInt32(fleetOverviewItem.Lcts_Id), out FleetOverviewDetails fleetOverviewDetails))
                {
                    fleetOverviewDetailsLookup.Add(Convert.ToInt32(fleetOverviewItem.Lcts_Id), fleetOverviewDetails = ToFleetOverviewDetailsModel(fleetOverviewItem));
                }

                if (fleetOverviewDetails.LiveFleetPositions == null)
                {
                    fleetOverviewDetails.LiveFleetPositions = new List<LiveFleetPosition>();
                }

                if (fleetOverviewItem.Lps_Id > 0 && fleetOverviewItem.Lcts_TripId == fleetOverviewItem.Lps_TripId)
                {
                    if (!liveFleetPositionLookup.TryGetValue(Convert.ToInt32(fleetOverviewItem.Lps_Id), out _))
                    {
                        LiveFleetPosition liveFleetPosition;
                        liveFleetPositionLookup.Add(Convert.ToInt32(fleetOverviewItem.Lps_Id), liveFleetPosition = ToLiveFleetPositionModel(fleetOverviewItem));
                        fleetOverviewDetails.LiveFleetPositions.Add(liveFleetPosition);
                    }
                }
                if (fleetOverviewItem.Lcts_TripId == fleetOverviewItem.Tripal_TripId)
                {
                    if (!liveFleetAlertLookup.TryGetValue(Convert.ToInt32(fleetOverviewItem.Tripal_Id), out _))
                    {
                        FleetOverviewAlert fleetOverviewAlert;
                        liveFleetAlertLookup.Add(Convert.ToInt32(fleetOverviewItem.Tripal_Id), fleetOverviewAlert = ToLiveFleetAlertModel(fleetOverviewItem));
                        fleetOverviewDetails.FleetOverviewAlert.Add(fleetOverviewAlert);
                    }
                }

            }
            foreach (var keyValuePair in fleetOverviewDetailsLookup)
            {
                fleetOverviewDetailsList.Add(keyValuePair.Value);
            }
            return fleetOverviewDetailsList;
        }
        public FleetOverviewDetails ToFleetOverviewDetailsModel(FleetOverviewResult fleetOverviewResult)
        {
            FleetOverviewDetails fleetOverviewDetails = new FleetOverviewDetails
            {
                Id = fleetOverviewResult.Lcts_Id,
                TripId = fleetOverviewResult.Lcts_TripId,
                Vin = fleetOverviewResult.Lcts_Vin,
                StartTimeStamp = fleetOverviewResult.Lcts_StartTimeStamp,
                EndTimeStamp = fleetOverviewResult.Lcts_EndTimeStamp,
                Driver1Id = fleetOverviewResult.Lcts_Driver1Id,
                TripDistance = fleetOverviewResult.Lcts_TripDistance,
                DrivingTime = fleetOverviewResult.Lcts_DrivingTime,
                FuelConsumption = fleetOverviewResult.Lcts_FuelConsumption,
                VehicleDrivingStatusType = fleetOverviewResult.Lcts_VehicleDrivingStatusType,
                OdometerVal = fleetOverviewResult.Lcts_OdometerVal,
                DistanceUntilNextService = fleetOverviewResult.Lcts_DistanceUntilNextService,
                LatestReceivedPositionLattitude = fleetOverviewResult.Lcts_LatestReceivedPositionLattitude,
                LatestReceivedPositionLongitude = fleetOverviewResult.Lcts_LatestReceivedPositionLongitude,
                LatestReceivedPositionHeading = fleetOverviewResult.Lcts_LatestReceivedPositionHeading,
                StartPositionLattitude = fleetOverviewResult.Lcts_StartPositionLattitude,
                StartPositionLongitude = fleetOverviewResult.Lcts_StartPositionLongitude,
                StartPositionHeading = fleetOverviewResult.Lcts_StartPositionHeading,
                LatestProcessedMessageTimeStamp = fleetOverviewResult.Lcts_LatestProcessedMessageTimeStamp,
                VehicleHealthStatusType = fleetOverviewResult.Lcts_VehicleHealthStatusType,
                LatestWarningClass = fleetOverviewResult.Lcts_LatestWarningClass,
                LatestWarningNumber = fleetOverviewResult.Lcts_LatestWarningNumber,
                LatestWarningType = fleetOverviewResult.Lcts_LatestWarningType,
                LatestWarningTimestamp = fleetOverviewResult.Lcts_LatestWarningTimestamp,
                LatestWarningPositionLatitude = fleetOverviewResult.Lcts_LatestWarningPositionLatitude,
                LatestWarningPositionLongitude = fleetOverviewResult.Lcts_LatestWarningPositionLongitude,
                Vid = fleetOverviewResult.Veh_Vid,
                RegistrationNo = fleetOverviewResult.Veh_RegistrationNo,
                DriverFirstName = fleetOverviewResult.Dri_FirstName,
                DriverLastName = fleetOverviewResult.Dri_LastName,
                LatestGeolocationAddressId = fleetOverviewResult.Latgeoadd_LatestGeolocationAddressId,
                LatestGeolocationAddress = fleetOverviewResult.Latgeoadd_LatestGeolocationAddress,
                StartGeolocationAddressId = fleetOverviewResult.Stageoadd_StartGeolocationAddressId,
                StartGeolocationAddress = fleetOverviewResult.Stageoadd_StartGeolocationAddress,
                LatestWarningGeolocationAddressId = fleetOverviewResult.Wangeoadd_LatestWarningGeolocationAddressId,
                LatestWarningGeolocationAddress = fleetOverviewResult.Wangeoadd_LatestWarningGeolocationAddress
            };
            return fleetOverviewDetails;
        }
        public LiveFleetPosition ToLiveFleetPositionModel(FleetOverviewResult fleetOverviewResult)
        {
            LiveFleetPosition liveFleetPosition = new LiveFleetPosition
            {
                Id = fleetOverviewResult.Lps_Id,
                TripId = fleetOverviewResult.Lps_TripId,
                GpsAltitude = fleetOverviewResult.Lps_GpsAltitude,
                GpsHeading = fleetOverviewResult.Lps_GpsHeading,
                GpsLatitude = fleetOverviewResult.Lps_GpsLatitude,
                GpsLongitude = fleetOverviewResult.Lps_GpsLongitude,
                Co2emission = fleetOverviewResult.Lps_Co2Emission,
                Fuelconsumtion = fleetOverviewResult.Lps_FuelConsumption,
            };
            return liveFleetPosition;
        }

        public FleetOverviewAlert ToLiveFleetAlertModel(FleetOverviewResult fleetOverviewResult)
        {
            FleetOverviewAlert liveFleetAlert = new FleetOverviewAlert
            {
                Id = fleetOverviewResult.Tripal_Id,
                AlertName = fleetOverviewResult.AlertName,
                AlertType = fleetOverviewResult.AlertType,
                AlertLocation = fleetOverviewResult.AlertLocation,
                AlertTime = fleetOverviewResult.AlertTime,
                AlertLevel = fleetOverviewResult.AlertLevel,
                CategoryType = fleetOverviewResult.CategoryType,
                AlertLatitude = fleetOverviewResult.AlertLatitude,
                AlertLongitude = fleetOverviewResult.AlertLongitude,
            };
            return liveFleetAlert;
        }
    }
}
