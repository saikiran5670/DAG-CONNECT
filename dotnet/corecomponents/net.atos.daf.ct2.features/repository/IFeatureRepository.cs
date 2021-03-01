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
        Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet);
       // Task<int> DeleteFeatureSet(int FeatureSetId, int Userid);
        Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId,bool Active);
        Task<IEnumerable<Feature>> GetFeatures(int RoleId, int Organizationid, char? Featuretype);
        Task<int> CheckFeatureSetExist(string FeatureSetName);
        Task<IEnumerable<FeatureSet>> GetFeatureSetFeature(int FeatureSetId);
        Task<IEnumerable<Feature> > GetFeatureIdsForFeatureSet(int feature_set_id);
        Task<bool> DeleteFeatureSet(int FeatureSetId);
        Task<FeatureSet> CreateFeatureSet(FeatureSet featureSet);
        Task<DataAttributeSet> CreateDataattributeSet(DataAttributeSet dataAttributeSet);
        Task<DataAttributeSet> UpdatedataattributeSet(DataAttributeSet dataAttributeSet);
        Task<bool> DeleteDataAttribute(int dataAttributeSetID);
        Task<List<DataAttributeSet>> GetDataAttributeSetDetails(int dataAttributeSetID);
        Task<int> UpdateFeatureSetMapping(int UpdateFeatureSetID, int ID);

        #endregion
    }
}
