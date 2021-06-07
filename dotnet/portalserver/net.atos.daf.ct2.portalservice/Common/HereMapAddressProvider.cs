﻿
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using net.atos.daf.ct2.mapservice;
using net.atos.daf.ct2.poiservice;
using static net.atos.daf.ct2.mapservice.MapService;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class HereMapAddressProvider
    {
        private readonly MapServiceClient _mapServiceClient;
        private POIService.POIServiceClient _poiServiceClient;



        public HereMapAddressProvider(MapServiceClient mapServiceClient, POIService.POIServiceClient poiServiceClient)
        {
            _mapServiceClient = mapServiceClient;
            _poiServiceClient = poiServiceClient;
        }

        public string GetAddress(double lat, double lng)
        {
           // if (ValidateLatLng(lat,lng))
            {
                var mapRequest = new GetMapRequest() { Latitude = lat, Longitude = lng };
                var lookupAddress = _mapServiceClient.GetMapAddressAsync(mapRequest).GetAwaiter().GetResult();
                return lookupAddress.LookupAddresses.Address ?? string.Empty;
            }
            return string.Empty;

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
        private bool ValidateLatLng(double lat, double lng)
        {
            return ((lng < -180 || lng > 180) && (lat < -90 || lat > 90));

        }

    }
}
