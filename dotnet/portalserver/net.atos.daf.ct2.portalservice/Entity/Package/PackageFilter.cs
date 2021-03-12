
namespace net.atos.daf.ct2.portalservice.Entity.Package
{
    public class PackageFilter
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int FeatureSetId { get; set; }
        public PackageStatus  Status { get; set; }
        public string PackageCodes { get; set; }
    }
}
