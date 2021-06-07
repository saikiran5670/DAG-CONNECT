
using System.Threading.Tasks;
using net.atos.daf.ct2.mapservice;
using static net.atos.daf.ct2.mapservice.MapService;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class HereMapAddressProvider
    {
        private readonly MapServiceClient _mapServiceClient;
        public HereMapAddressProvider(MapServiceClient mapServiceClient)
        {
            _mapServiceClient = mapServiceClient;
        }
        public  string GetAddress(double lat, double lng)
        {
            var mapRequest = new GetMapRequest() { Latitude = lat, Longitude = lng };
            var lookupAddress =  _mapServiceClient.GetMapAddressAsync(mapRequest).GetAwaiter().GetResult();
            return lookupAddress.LookupAddresses.Address;
        }
    }
}
