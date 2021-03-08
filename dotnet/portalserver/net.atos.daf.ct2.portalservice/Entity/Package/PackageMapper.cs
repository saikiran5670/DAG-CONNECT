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
        public PackageCreateRequest ToCreatePackage(PackagePortalRequest request)
        {

            var createPackagerequest = new PackageCreateRequest()
            {
                Id=request.Id,
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Status = ToPackageStatus(request.Status),
                Type = ToPackageType(request.Type),
                
            };
            createPackagerequest.Features.AddRange(request.Features.Select(x=>x.ToString()));
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

       


        public void ToImportPackage() { }

        //public List<string> ToFeatures() {
        
        
        //}

    }
}
