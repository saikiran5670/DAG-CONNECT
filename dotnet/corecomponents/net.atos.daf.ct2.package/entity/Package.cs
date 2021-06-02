using System.Collections.Generic;

namespace net.atos.daf.ct2.package.entity
{
    public class Package
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int FeatureSetID { get; set; }
        public List<int> FeatureIds { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public long CreatedAt { get; set; }
    }
    public class PackageMaster
    {
        public List<Package> Packages { get; set; }
    }

}
