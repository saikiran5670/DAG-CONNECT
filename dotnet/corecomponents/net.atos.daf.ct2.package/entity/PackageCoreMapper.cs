using net.atos.daf.ct2.package.ENUM;
using System.Collections.Generic;

namespace net.atos.daf.ct2.package.entity
{
    public class PackageCoreMapper
    {
        public Package Map(dynamic record)
        {
            Package package = new Package();
            package.Id = record.id;
            package.Code = !string.IsNullOrEmpty(record.packagecode) ? record.packagecode : string.Empty;
            package.IsActive = record.is_active;
            package.Status = !string.IsNullOrEmpty(record.status) ? MapCharToPackageStatus(record.status) : string.Empty;
            package.Type = !string.IsNullOrEmpty(record.type) ? MapCharToPackageType(record.type) : string.Empty;
            package.Name = !string.IsNullOrEmpty(record.name) ? record.name : string.Empty;
            package.Description = !string.IsNullOrEmpty(record.description) ? record.description : string.Empty;
            package.FeatureSetID = record.feature_set_id != null ? record.feature_set_id : 0;
            package.CreatedAt = record.created_at;
            return package;
        }

        public PackageType ToPackageType(string type)
        {
            var pkgType = PackageType.Organization;
            switch (type)
            {
                case "O":
                    pkgType = PackageType.Organization;
                    break;
                case "V":
                    pkgType = PackageType.VIN;
                    break;
                case "R":
                    pkgType = PackageType.ORGVIN;
                    break;
            }
            return pkgType; ;
        }


        public PackageStatus ToPackageStatus(string status)
        {

            return status == "A" ? PackageStatus.Active : PackageStatus.Inactive;
        }

        public char MapPackageType(string packageType)
        {
            var type = default(char);
            switch (packageType)
            {
                case "Organization":
                    type = 'O';
                    break;
                case "Vehicle":
                    type = 'V';
                    break;
                case "ORGVIN":
                    type = 'R';
                    break;
            }
            return type;
        }


        public string MapCharToPackageType(string type)
        {
            var ptype = string.Empty;
            switch (type)
            {
                case "O":
                    ptype = "Organization";
                    break;
                case "V":
                    ptype = "VIN";
                    break;
                case "R":
                    ptype = "ORG VIN";
                    break;
            }
            return ptype;
        }

        public string MapCharToPackageStatus(string status)
        {

            var ptype = status == "A" ? "Active" : "Inactive";
            return ptype;

        }


    }
}
