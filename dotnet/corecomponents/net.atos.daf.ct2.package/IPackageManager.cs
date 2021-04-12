using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.package.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.package
{
    public interface IPackageManager{

        Task<Package> Create(Package package);
        Task<Package> Update(Package package);
        Task<IEnumerable<Package>> Get(PackageFilter packageFilter);
        Task<List<Package>> Import(List<Package> packageList);
        Task<bool> Delete(int packageId);

        Task<Package> UpdatePackageStatus(Package package);


        Task<int> Create(FeatureSet featureSet);// required clarification on return type
                                                //  Task<int> Update(FeatureSet featureSet);// required clarification on return type
        Task<IEnumerable<FeatureSet>> GetFeatureSet(int featureSetId, char state); // required is_active parameter


    }
}