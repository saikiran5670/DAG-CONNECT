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
         Task<IEnumerable<Feature>> GetFeatures(int RoleId, int Organizationid, int FeatureId, int level, char? Featuretype);
        Task<IEnumerable<Feature> > GetFeatureIdsForFeatureSet(int GetFeatureIdsForFeatureSet);
        Task<bool> DeleteFeatureSet(int FeatureSetId);
        Task<FeatureSet> CreateFeatureSet(FeatureSet featureSet);
        Task<DataAttributeSet> CreateDataattributeSet(DataAttributeSet dataAttributeSet);
        Task<DataAttributeSet> UpdatedataattributeSet(DataAttributeSet dataAttributeSet);
        Task<bool> DeleteDataAttribute(int dataAttributeSetID);
        Task <List<DataAttributeSet>> GetDataAttributeSetDetails(int dataAttributeSetID);
        Task<int> UpdateFeatureSetMapping(int UpdateFeatureSetID, int ID);
        Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet);
        //Task<int> CreateDataattributeSetFeature(Feature feature, int InserteddataAttributeSetID);
        Task<int> CreateDataattributeFeature(Feature feature);
        Task<IEnumerable<DataAttribute>> GetDataAttributes();
    }
}
