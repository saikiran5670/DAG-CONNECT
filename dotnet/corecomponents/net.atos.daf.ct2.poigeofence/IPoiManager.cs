﻿using net.atos.daf.ct2.poigeofence.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public interface IPoiManager
    {
        Task<List<POIEntityResponce>> GetAllPOI(POIEntityRequest objPOIEntityRequest);
    }
}