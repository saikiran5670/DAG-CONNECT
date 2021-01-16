using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;

namespace net.atos.daf.ct2.features
{
    public class FeatureManager:IFeatureManager
    {
        IFeatureRepository FeatureRepository;
        
        // IAuditLog auditlog;
        public FeatureManager(IFeatureRepository _FeatureRepository)
        {
            FeatureRepository = _FeatureRepository;
            // auditlog=_auditlog;
        }

         public async Task<int> AddFeatureSet(FeatureSet featureSet)
        {
            return await FeatureRepository.AddFeatureSet(featureSet);
        }

        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId,bool Active)
        {
            return await FeatureRepository.GetFeatureSet(FeatureSetId, Active);
        }

        public async Task<IEnumerable<Feature>> GetFeatures(char Featuretype,bool Active)
        {
            return await FeatureRepository.GetFeatures(Featuretype, Active);
        }
    }
}
