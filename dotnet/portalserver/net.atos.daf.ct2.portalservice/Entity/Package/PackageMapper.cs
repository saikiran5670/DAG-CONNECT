using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.packageservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Package
{
    public class PackageMapper
    {
        private readonly FeatureService.FeatureServiceClient _featureclient;
        public PackageMapper(FeatureService.FeatureServiceClient featureclient)
        {
            _featureclient = featureclient;

        }
        public PackageCreateRequest ToCreatePackage(PackagePortalRequest request)
        {

            var createPackagerequest = new PackageCreateRequest()
            {
                Id = request.Id,
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Status = ToPackageStatus(request.Status),
                Type = ToPackageType(request.Type),

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
                var featureSetID = RetrieveFeatureSetId(x.Features).Result;
                if (featureSetID > 0)
                {
                    var pkgRequest = new PackageCreateRequest()
                    {
                        Code = x.Code,
                        FeatureSetID = featureSetID,
                        Description = x.Description,
                        Name = x.Name,
                        Status = ToPackageStatus(x.Status),
                        Type = ToPackageType(x.Type)
                    };
                    packageRequest.Packages.Add(pkgRequest);
                }
            }
            return packageRequest;

        }

        public async Task<int> RetrieveFeatureSetId(List<string> features)
        {
            var featureSetId = new List<int>();
            var featureSetRequest = new FetureSetRequest();
            var featureFilterRequest = new FeaturesFilterRequest();
            var featureList = await _featureclient.GetFeaturesAsync(featureFilterRequest);
            foreach (var item in features)
            {
                bool hasFeature = featureList.Features.Any(feature => feature.Name == item);
                if (hasFeature)
                {
                    foreach (var feature in featureList.Features)
                    {
                        if (feature.Name == item)
                        {
                            featureSetId.Add(feature.Id);
                        }
                    }
                }
                else
                {
                    return 0;

                }
            }

            featureSetRequest.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            featureSetId = featureSetId.Select(x => x).Distinct().ToList();
            featureSetRequest.Features.AddRange(featureSetId);

            var ObjResponse = await _featureclient.CreateFeatureSetAsync(featureSetRequest);
            return Convert.ToInt32(ObjResponse.Message);

        }


        public async Task<int> UpdateFeatureSetId(List<string> features,int featureSetId)
        {
            var featureSetIds = new List<int>();
            var featureSetRequest = new FetureSetRequest();
            var featureFilterRequest = new FeaturesFilterRequest();
            var featureList = await _featureclient.GetFeaturesAsync(featureFilterRequest);
            foreach (var item in features)
            {
                bool hasFeature = featureList.Features.Any(feature => feature.Name == item);
                if (hasFeature)
                {
                    foreach (var feature in featureList.Features)
                    {
                        if (feature.Name == item)
                        {
                            featureSetIds.Add(feature.Id);
                        }
                    }
                }
                else
                {
                    return 0;

                }
            }

            featureSetRequest.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds(); 
            featureSetIds = featureSetIds.Select(x => x).Distinct().ToList();
            featureSetRequest.Features.AddRange(featureSetIds);

            var ObjResponse = await _featureclient.UpdateFeatureSetAsync(featureSetRequest);
            return Convert.ToInt32(ObjResponse.Message);

        }





    }
}
