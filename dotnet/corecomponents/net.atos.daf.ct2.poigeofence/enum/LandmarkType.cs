using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence
{
    public enum LandmarkType
    {
        None = 'N',
        POI = 'P',
        CircularGeofence = 'C',
        PolygonGeofence = 'O',
        ExistingTripCorridor = 'E',
        RouteCorridor = 'R'

    }
}
