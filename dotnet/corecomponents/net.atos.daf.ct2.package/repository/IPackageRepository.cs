using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.package.entity;

namespace net.atos.daf.ct2.package.repository
{
    public interface IPackageRepository
    {
        Task<Package> Create(Package package);
        Task<Package> Update(Package package);
        Task<List<Package>> Get(PackageFilter filter);
        Task<bool> Delete(int packageId);


        Task<List<Package>> Import(List<Package> packageList);
        Task<Package> UpdatePackageState(Package package);
    }
}