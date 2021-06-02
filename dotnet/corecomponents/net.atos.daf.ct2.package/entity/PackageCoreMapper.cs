using net.atos.daf.ct2.package.ENUM;

namespace net.atos.daf.ct2.package.entity
{
    public class PackageCoreMapper
    {
        public Package Map(dynamic record)
        {
            Package package = new Package();
            package.Id = record.id;
            package.Code = !string.IsNullOrEmpty(record.packagecode) ? record.packagecode : string.Empty;
            package.State = !string.IsNullOrEmpty(record.state) ? MapCharToPackageState(record.state) : string.Empty;
            // package.State = !string.IsNullOrEmpty(record.status) ? MapCharToPackageStatus(record.state) : string.Empty;
            package.Type = !string.IsNullOrEmpty(record.type) ? MapCharToPackageType(record.type) : string.Empty;
            package.Name = !string.IsNullOrEmpty(record.name) ? record.name : string.Empty;
            package.Description = !string.IsNullOrEmpty(record.description) ? record.description : string.Empty;
            package.FeatureSetID = record.feature_set_id ?? 0;
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


        public PackageState ToPackageStatus(string status)
        {

            return status == "A" ? PackageState.Active : PackageState.Inactive;
        }

        public PackageState ToPackageState(string status)
        {
            var type = PackageState.Active; ;
            switch (status)
            {
                case "A":
                    type = PackageState.Active;
                    break;
                case "I":
                    type = PackageState.Inactive;
                    break;
                case "D":
                    type = PackageState.Delete;
                    break;
            }
            return type;

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

        public string MapCharToPackageState(string state)
        {

            var ptype = string.Empty;
            switch (state)
            {
                case "A":
                    ptype = "Active";
                    break;
                case "I":
                    ptype = "Inactive";
                    break;
                case "D":
                    ptype = "Delete";
                    break;
            }
            return ptype;



        }


    }
}
