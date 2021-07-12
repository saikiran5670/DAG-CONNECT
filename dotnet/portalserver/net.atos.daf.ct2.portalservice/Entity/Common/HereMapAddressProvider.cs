
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
            return lookupAddress.LookupAddresses.Address ?? string.Empty;


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
        public TripDetails UpdateTripReportAddress(TripDetails tripDetails)
        {


            if (string.IsNullOrEmpty(tripDetails.StartPosition) && string.IsNullOrEmpty(tripDetails.EndPosition))
            {

                tripDetails.StartPosition = GetAddress(tripDetails.StartPositionLattitude, tripDetails.StartPositionLongitude);
                tripDetails.EndPosition = GetAddress(tripDetails.EndPositionLattitude, tripDetails.EndPositionLongitude);
                if (!string.IsNullOrEmpty(tripDetails.StartPosition) && !string.IsNullOrEmpty(tripDetails.EndPosition))
                {
                    var updtetrip = new AddTripAddressRequest() { Id = tripDetails.Id, StartAddress = tripDetails.StartPosition, EndAddress = tripDetails.EndPosition };
                    var result = _poiServiceClient.UpdateTripAddress(updtetrip);
                }
            }

            return tripDetails;
        }
        private bool ValidateLatLng(double lat, double lng)
        {
            return ((lng < -180 || lng > 180) && (lat < -90 || lat > 90));

        }

    }
}
