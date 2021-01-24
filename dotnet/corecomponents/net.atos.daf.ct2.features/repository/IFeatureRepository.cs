using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.features.entity;

namespace net.atos.daf.ct2.features.repository
{
    public interface IFeatureRepository
    {
        

        #region Feature Set

        Task<int> AddFeatureSet(FeatureSet featureSet);
        Task<int> UpdateFeatureSet(FeatureSet featureSet);
        Task<int> DeleteFeatureSet(int FeatureSetId, int Userid);
        Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId,bool Active);
         Task<IEnumerable<Feature>> GetFeatures(char Featuretype,bool Active);
        Task<int> CheckFeatureSetExist(string FeatureSetName);
        Task<IEnumerable<FeatureSet>> GetFeatureSetFeature(int FeatureSetId);
         Task<IEnumerable<Feature> > GetFeatureIdsForFeatureSet(int feature_set_id);
        
        #endregion
    }
}
