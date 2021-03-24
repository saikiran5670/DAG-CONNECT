using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.package.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.package.repository
{
    public interface IPackageRepository
    {
        Task<Package> Create(Package package);
        Task<Package> Update(Package  package);
        Task<List<Package>> Get(PackageFilter filter);
        Task<bool>  Delete(int packageId);


        Task<List<Package>> Import(List<Package> packageList);
        Task<List<Package>> Export();


        //Task<Feature> GetFeature(int featureId);


        //Task<FeatureSet> Create(FeatureSet featureSet);
        //Task<FeatureSet> Update(FeatureSet featureSet);
        //Task<FeatureSet> GetFeatureSet(int featureSetId);

    }
}