using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.visibility.entity
{
    public class VehiclePackage
    {
        public string PackageType { get; set; }
        public bool HasOwned { get; set; }
        public int[] VehicleIds { get; set; }
    }
}
