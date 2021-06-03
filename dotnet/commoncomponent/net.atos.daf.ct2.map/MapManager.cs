using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.map.entity;
using net.atos.daf.ct2.map.geocode;
using net.atos.daf.ct2.map.repository;

namespace net.atos.daf.ct2.map
{
    public class MapManager : IMapManager
    {
        private readonly IMapRepository _mapRepository;
        private readonly Geocoder _geocoder;
        public MapManager(IMapRepository mapRepository)
        {
            _mapRepository = mapRepository;
            _geocoder = new Geocoder();

        }      
        public void InitializeMapGeocoder(HereMapConfiguration  hereMapConfiguration) => _geocoder.InitializeMapGeocoder(hereMapConfiguration.AppId, hereMapConfiguration.AppCode);


        public async Task<LookupAddress> GetMapAddress(LookupAddress lookupAddress)
        {

            LookupAddress result = await _mapRepository.GetMapAddress(lookupAddress);
            if (result == null)
            {
                lookupAddress = AddMapAddress(lookupAddress).Result;
                return lookupAddress;

            }

            return result;

        }
        private async Task<LookupAddress> AddMapAddress(LookupAddress lookupAddress)
        {
            lookupAddress.Address = GetAddress(lookupAddress.Latitude, lookupAddress.Longitude);
            return await _mapRepository.AddMapAddress(lookupAddress);
        }

        private string GetAddress(double lat, double lan)
        {
            string address = _geocoder.ReverseGeocodeAsync(lat, lan).Result;
            return address;
        }


        //public async Task<List<LookupAddress>> AddLookupAddress(List<LookupAddress> lookupAddresses)
        //{
        //    lookupAddresses.Select(x => x.Address = GetAddress(x.Latitude, x.Longitude)).ToList();
        //    return await _mapRepository.AddLookupAddress(lookupAddresses);
        //}


        //public async Task<List<LookupAddress>> GetLookupAddress(List<LookupAddress> lookupAddresses)
        //{
        //    return await _mapRepository.GetLookupAddress(lookupAddresses);
        //}


    }
}
