using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.role.entity;

namespace net.atos.daf.ct2.role.repository
{
    public interface IFeatureRepository
    {
    #region Feature Type
        
        Task<IEnumerable<FeatureType>> GetFeatureType(int FeatureTypeId);
        Task<int> AddFeatureType(FeatureType featureType);
        Task<int> UpdateFeatureType(FeatureType featureType);
        Task<int> DeleteFeatureType(int FeatureTypeId,int Userid);
        Task<int> CheckFeatureTypeExist(string FeatureType);
        
        #endregion

        #region Feature
        
        Task<IEnumerable<Feature>> GetFeature(int RoleFeatureId);
        Task<int> AddFeature(Feature feature);
        Task<int> UpdateFeature(Feature feature);
        Task<int> DeleteFeature(int RoleFeatureId,int Userid);
        Task<int> CheckFeatureExist(string FeatureName);

        #endregion

        #region Feature Set
        Task<int> AddFeatureSet(FeatureSet featureSet);  
        Task<int> UpdateFeatureSet(FeatureSet featureSet);
        Task<int> DeleteFeatureSet(int FeatureSetId,int Userid);
        Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId);
        Task<int> CheckFeatureSetExist(string FeatureSetName);
        Task<IEnumerable<FeatureSet>> GetFeatureSetFeature(int FeatureSetId);
        
        #endregion

    }
}
