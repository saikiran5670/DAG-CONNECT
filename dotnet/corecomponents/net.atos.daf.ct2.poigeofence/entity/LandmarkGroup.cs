using System;
using System.Collections.Generic;
using System.Text;
using static net.atos.daf.ct2.utilities.CommonEnums;

namespace net.atos.daf.ct2.poigeofence.entity
{
    public class LandmarkGroup
    {
        public int id { get; set; }
        public int organization_id { get; set; }
        public string name { get; set; }
        public int icon_id { get; set; }
        public string state { get; set; }
        public long created_at { get; set; }
        public int created_by { get; set; }
        public long modified_at { get; set; }
        public int modified_by { get; set; }
        public List<POI> poilist { get; set; }
    }

    public class LandmarkgroupRef
    {
        public int id { get; set; }
        public int landmark_group_id { get; set; }
        public LandmarkType type { get; set; }
        public int ref_id { get; set; }
    }
}
