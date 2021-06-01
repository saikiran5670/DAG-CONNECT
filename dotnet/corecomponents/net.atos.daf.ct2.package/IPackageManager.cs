using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.package.entity;

namespace net.atos.daf.ct2.package
{
    public interface IPackageManager
    {

        Task<Package> Create(Package package);
        Task<Package> Update(Package package);
        Task<IEnumerable<Package>> Get(PackageFilter packageFilter);
        Task<List<Package>> Import(List<Package> packageList);
        Task<bool> Delete(int packageId);

        Task<Package> UpdatePackageState(Package package);


        Task<int> Create(FeatureSet featureSet);// required clarification on return type
                                                //  Task<int> Update(FeatureSet featureSet);// required clarification on return type
        Task<IEnumerable<FeatureSet>> GetFeatureSet(int featureSetId, char state); // required is_active parameter


    }
}