using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class POIEntityResponce
    {
        public string poiName { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string category { get; set; }
        public string city { get; set; }
    }

    public class POIEntityRequest
    {
        public int organization_id { get; set; }
        public int category_id { get; set; }
        public int sub_category_id { get; set; }
        public int roleIdlevel { get; set; }
        //public string Category { get; set; }
        //public string City { get; set; }
    }
}
