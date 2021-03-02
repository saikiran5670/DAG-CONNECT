using net.atos.daf.ct2.package.ENUM;

namespace net.atos.daf.ct2.package.entity
{
    public class PackageFilter
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public PackageType Type { get; set; }
        public int FeatureSetId { get; set; }
        public PackageStatus  Status { get; set; }
        public string PackageCodes { get; set; }
    }
}
