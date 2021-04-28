using System.Collections.Generic;
namespace net.atos.daf.ct2.portalservice.Entity.POI
{
    public class UploadPOIExcel
    {
        public List<POI> PoiUploadedList { get; set; }
        public List<POI> PoiDuplicateList { get; set; }
        public List<POI> PoiExcelList { get; set; }
    }
}
