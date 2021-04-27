using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class UploadPOIExcel
    {
        public List<POI> PoiUploadedList { get; set; }
        public List<POI> PoiExistingList { get; set; }
        public List<POI> PoiExcelList { get; set; }
    }
}
