using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;
using net.atos.daf.ct2.package.entity;
using net.atos.daf.ct2.package.repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.package
{
    public class PackageManager : IPackageManager
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IFeatureManager _featureManager;

        public PackageManager(IPackageRepository packageRepository,
                              IFeatureManager featureManager)
        {
            _packageRepository = packageRepository;
            _featureManager = featureManager;

        }
        public async Task<Package> Create(Package package)
        {
            return await _packageRepository.Create(package);
        }
        public async Task<bool> Delete(int packageId)
        {
            return await _packageRepository.Delete(packageId);
        }
        public async Task<List<Package>> Import(List<Package> packageList)
        {
            return await _packageRepository.Import(packageList);
        }
        public async Task<Package> Update(Package package) 
        {
            return await _packageRepository.Update(package);
        }
        public async Task<List<Package>> Export()
        {
            return await _packageRepository.Export();
        }

        public async Task<IEnumerable<Package>> Get(PackageFilter packageFilter )
        {
            return await _packageRepository.Get(packageFilter);
        }
        public async Task<int> Create(FeatureSet featureSet) //as per LLD return set sholud be featureset
        {
            return await _featureManager.AddFeatureSet(featureSet);
        }
        //public async Task<Feature> GetFeature(int featureId)
        //{
        //    return await _featureManager.GetFeatures(featureId,1);
        //}

        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int featureSetId, bool is_active) // required is_active parameter
        {
            return await _featureManager.GetFeatureSet(featureSetId, is_active);
        }

      

       

        //public async Task<int> Update(FeatureSet featureSet)
        //{
        //    return await _featureManager.UpdateFeatureSet(featureSet);
        //}
    }
}
