using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.features.entity;

namespace net.atos.daf.ct2.features
{
    public interface IFeatureManager
    {
        Task<int> AddFeatureSet(FeatureSet featureSet);
        Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId,bool Active);
         Task<IEnumerable<Feature>> GetFeatures(char Featuretype,bool Active);
    }
}
