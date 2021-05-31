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
        Task<int> GetMinimumLevel(List<Feature> features);
        Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet);
        // Task<int> DeleteFeatureSet(int FeatureSetId, int Userid);
        Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId, char state);
        Task<IEnumerable<Feature>> GetFeatures(int RoleId, int Organizationid, int FeatureId, int level, char? Featuretype, string Langaugecode);
        Task<int> CheckFeatureSetExist(string FeatureSetName);
        Task<IEnumerable<FeatureSet>> GetFeatureSetFeature(int FeatureSetId);
        Task<IEnumerable<Feature>> GetFeatureIdsForFeatureSet(int feature_set_id, string Langaugecode);
        Task<DataAttributeSet> GetDataAttributeset(int DataAttributeSetID);
        Task<bool> DeleteFeatureSet(int FeatureSetId);
        Task<FeatureSet> CreateFeatureSet(FeatureSet featureSet);
        Task<DataAttributeSet> CreateDataattributeSet(DataAttributeSet dataAttributeSet);
        Task<int> CreateDataattributeSetFeature(Feature feature, int InserteddataAttributeSetID);
        Task<DataAttributeSet> UpdatedataattributeSet(DataAttributeSet dataAttributeSet);
        Task<bool> DeleteDataAttribute(int dataAttributeSetID);
        Task<List<DataAttributeSet>> GetDataAttributeSetDetails(int dataAttributeSetID);
        Task<int> UpdateFeatureSetMapping(int UpdateFeatureSetID, int ID);

        Task<IEnumerable<DataAttribute>> GetDataAttributes(string Langaugecode);
        Task<Feature> UpdateFeature(Feature feature);
        Task<int> DeleteFeature(int FeatureId);
        int CheckFeatureNameExist(string FeatureName, int FeatureId);
        Task<int> ChangeFeatureState(int FeatureID, Char State);

        #endregion
    }
}
