using System;
using net.atos.daf.ct2.map.entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.map.repository;
using net.atos.daf.ct2.map.geocode;
using System.Linq;

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
        public async Task<List<LookupAddress>> AddLookupAddress(List<LookupAddress> lookupAddresses)
        {
            lookupAddresses.Select(x => x.Address = GetAddress(x.Latitude,x.Longitude)).ToList();
            return await _mapRepository.AddLookupAddress(lookupAddresses);
        }

        public async Task<List<LookupAddress>> GetLookupAddress(List<LookupAddress> lookupAddresses)
        {
            return await _mapRepository.GetLookupAddress(lookupAddresses);
        }

        public void InitializeMapGeocoder(string appId, string appCode)
        {
            _geocoder.InitializeMapGeocoder(appId, appCode);
        }



        private string GetAddress(double lat, double lan)
        {
            var address = _geocoder.ReverseGeocodeAsync(lat, lan).Result;
            return address;
        }
    }
}
