
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using net.atos.daf.ct2.mapservice;
using net.atos.daf.ct2.poiservice;
using net.atos.daf.ct2.reportservice;
using static net.atos.daf.ct2.mapservice.MapService;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class HereMapAddressProvider
    {
        private readonly MapServiceClient _mapServiceClient;
        private readonly POIService.POIServiceClient _poiServiceClient;

        public HereMapAddressProvider(MapServiceClient mapServiceClient, POIService.POIServiceClient poiServiceClient)
        {
            _mapServiceClient = mapServiceClient;
            _poiServiceClient = poiServiceClient;
        }

        public string GetAddress(double lat, double lng)
        {
            var mapRequest = new GetMapRequest() { Latitude = lat, Longitude = lng };
            var lookupAddress = _mapServiceClient.GetMapAddressAsync(mapRequest).GetAwaiter().GetResult();
            return lookupAddress.LookupAddresses != null ? (lookupAddress.LookupAddresses.Address ?? string.Empty) : string.Empty;
        }

        public GetMapRequest GetAddressObject(double lat, double lng)
        {

            var mapRequest = new GetMapRequest() { Latitude = lat, Longitude = lng };
            var lookupAddress = _mapServiceClient.GetMapAddressAsync(mapRequest).GetAwaiter().GetResult();
            return lookupAddress.LookupAddresses;
        }

        public TripData UpdateTripAddress(TripData tripData)
        {


            if (string.IsNullOrEmpty(tripData.StartAddress) && string.IsNullOrEmpty(tripData.EndAddress))
            {

                tripData.StartAddress = GetAddress(tripData.StartPositionlattitude, tripData.StartPositionLongitude);
                tripData.EndAddress = GetAddress(tripData.StartPositionlattitude, tripData.EndPositionLongitude);
                if (!string.IsNullOrEmpty(tripData.StartAddress) && !string.IsNullOrEmpty(tripData.EndAddress))
                {
                    var updtetrip = new AddTripAddressRequest() { Id = tripData.Id, StartAddress = tripData.StartAddress, EndAddress = tripData.EndAddress };
                    var result = _poiServiceClient.UpdateTripAddress(updtetrip);
                }
            }

            return tripData;
        }
        public void UpdateTripReportAddress(TripDetails trip)
        {
            if (string.IsNullOrEmpty(trip.EndPosition) && trip.EndPositionLattitude != 255 && trip.EndPositionLongitude != 255)
            {

                {
                    GetMapRequest getMapRequestWarning = GetAddressObject(trip.EndPositionLattitude, trip.EndPositionLongitude);
                    trip.EndPosition = getMapRequestWarning.Address;
                    var updtetrip = new AddTripAddressRequest() { Id = trip.Id, EndAddress = trip.EndPosition ?? string.Empty };
                    var result = _poiServiceClient.UpdateTripAddress(updtetrip);
                }

            }
            else if (string.IsNullOrEmpty(trip.StartPosition) && trip.StartPositionLattitude != 255 && trip.StartPositionLongitude != 255)
            {
                GetMapRequest getMapRequestWarning = GetAddressObject(trip.StartPositionLattitude, trip.StartPositionLongitude);
                trip.StartPosition = getMapRequestWarning.Address;
                var updtetrip = new AddTripAddressRequest() { Id = trip.Id, StartAddress = trip.StartPosition ?? string.Empty };
                var result = _poiServiceClient.UpdateTripAddress(updtetrip); // to update address in trip_statistics table
            }

        }
        private bool ValidateLatLng(double lat, double lng)
        {
            return ((lng < -180 || lng > 180) && (lat < -90 || lat > 90));

        }
    }
}
