using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.packageservice;
using net.atos.daf.ct2.portalservice.Common;
using System.Linq;

namespace net.atos.daf.ct2.portalservice.Entity.Package
{
    public class PackageMapper
    {
       
        private readonly FeatureSetMapper _featureSetMapper;
        public PackageMapper(FeatureService.FeatureServiceClient featureclient)
        {           
            _featureSetMapper = new FeatureSetMapper(featureclient);
        }
        public PackageCreateRequest ToCreatePackage(PackagePortalRequest request)
        {

            var createPackagerequest = new PackageCreateRequest()
            {
                Id = request.Id,
                Code = request.Code,
                Name = request.Name,
                FeatureSetID=request.FeatureSetID,
                Description = request.Description,
                Status = request.Status,
                Type = request.Type,

            };
            createPackagerequest.Features.AddRange(request.Features.Select(x => x.ToString()));
            return createPackagerequest;

        }
        public packageservice.PackageStatus ToPackageStatus(string status)
        {

            return status == "A" ? packageservice.PackageStatus.Active : packageservice.PackageStatus.Inactive;

        }
        public packageservice.PackageType ToPackageType(string type)
        {

            var packageType = packageservice.PackageType.Organization;
            switch (type)
            {
                case "V":
                    packageType = packageservice.PackageType.Vehicle;
                    break;
                case "O":
                    packageType = packageservice.PackageType.Organization;
                    break;
            }
            return packageType;
        }




        public ImportPackageRequest ToImportPackage(PackageImportRequest request)
        {

            var packageRequest = new ImportPackageRequest();
            foreach (var x in request.packages)
            {
                var featureSetID = _featureSetMapper.RetrieveFeatureSetIdByName(x.Features).Result;
                if (featureSetID > 0)
                {
                    var pkgRequest = new PackageCreateRequest()
                    {
                        Code = x.Code,
                        FeatureSetID = featureSetID,
                        Description = x.Description,
                        Name = x.Name,
                        Status = x.Status,
                        Type = x.Type
                    };
                    packageRequest.Packages.Add(pkgRequest);
                }
            }
            return packageRequest;

        }

    }
}
