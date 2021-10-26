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

    public class VehiclePackageForAlert
    {
        public string PackageType { get; set; }
        public bool HasOwned { get; set; }
        public int Vehicle_Id { get; set; }
        public long[] FeatureIds { get; set; }
    }

    public class VehicleRelationshipForAlert
    {
        public int Vehicle_Id { get; set; }
        public long[] FeatureIds { get; set; }
    }
}
