using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.map;
using net.atos.daf.ct2.map.entity;

namespace net.atos.daf.ct2.reportscheduler.helper
{
    public class MapHelper
    {
        private readonly IMapManager _mapManager;

        public MapHelper(IMapManager mapManager)
        {
            _mapManager = mapManager;
        }
        public async Task<string> GetAddress(double lat, double lng)
        {
            if (lat != 0 && lng != 0)
            {
                try
                {
                    var lookupAddress = await _mapManager.GetMapAddress(new LookupAddress() { Latitude = lat, Longitude = lng });
                    return lookupAddress != null ? (lookupAddress.Address ?? string.Empty) : string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
