using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.features.entity;

namespace net.atos.daf.ct2.features
{
    public interface IFeatureManager
    {
        Task<int> AddFeatureSet(FeatureSet featureSet);
        Task<int> GetMinimumLevel(List<Feature> features);
        Task<IEnumerable<FeatureSet>> GetFeatureSet(int featureSetId, char state);
        Task<IEnumerable<Feature>> GetFeatures(int roleId, int organizationid, int featureId, int level, char? featureType, string langaugeCode);
        Task<IEnumerable<Feature>> GetFeatureIdsForFeatureSet(int getFeatureIdsForFeatureSet, string langaugeCode);
        Task<DataAttributeSet> GetDataAttributeset(int dataAttributeSetID);
        Task<bool> DeleteFeatureSet(int featureSetId);
        Task<FeatureSet> CreateFeatureSet(FeatureSet featureSet);
        Task<DataAttributeSet> CreateDataattributeSet(DataAttributeSet dataAttributeSet);
        Task<DataAttributeSet> UpdatedataattributeSet(DataAttributeSet dataAttributeSet);
        Task<bool> DeleteDataAttribute(int dataAttributeSetID);
        Task<List<DataAttributeSet>> GetDataAttributeSetDetails(int dataAttributeSetID);
        Task<int> UpdateFeatureSetMapping(int updateFeatureSetID, int id);
        Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet);
        //Task<int> CreateDataattributeSetFeature(Feature feature, int InserteddataAttributeSetID);
        Task<int> CreateDataattributeFeature(Feature feature);
        Task<Feature> UpdateFeature(Feature feature);
        Task<IEnumerable<DataAttribute>> GetDataAttributes(string langaugeCode);
        Task<int> DeleteFeature(int featureId);

        int CheckFeatureNameExist(string featureName, int featureId);
        Task<int> ChangeFeatureState(int featureID, Char state);
    }
}
