using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;

namespace net.atos.daf.ct2.features
{
    public class FeatureManager : IFeatureManager
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
        public async Task<int> GetMinimumLevel(List<Feature> features)
        {
            return await FeatureRepository.GetMinimumLevel(features);
        }
        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId, char state)
        {
            return await FeatureRepository.GetFeatureSet(FeatureSetId, state);
        }

        public async Task<IEnumerable<Feature>> GetFeatures(int RoleId, int Organizationid, int FeatureId, int level, char? Featuretype, string Langaugecode)
        {
            return await FeatureRepository.GetFeatures(RoleId, Organizationid, FeatureId, level, Featuretype, Langaugecode);
        }

        public async Task<IEnumerable<Feature>> GetFeatureIdsForFeatureSet(int GetFeatureIdsForFeatureSet, string Langaugecode)
        {
            return await FeatureRepository.GetFeatureIdsForFeatureSet(GetFeatureIdsForFeatureSet, Langaugecode);
        }

        public async Task<DataAttributeSet> GetDataAttributeset(int DataAttributeSetID)
        {
            return await FeatureRepository.GetDataAttributeset(DataAttributeSetID);
        }
        public async Task<bool> DeleteFeatureSet(int FeatureSetId)
        {
            return await FeatureRepository.DeleteFeatureSet(FeatureSetId);
        }
        public async Task<FeatureSet> CreateFeatureSet(FeatureSet featureSet)
        {
            return await FeatureRepository.CreateFeatureSet(featureSet);
        }

        public async Task<int> CreateDataattributeFeature(Feature feature)
        {
            var DataAttributeSet = await CreateDataattributeSet(feature.DataAttributeSets);
            int FeatureID = 0;
            if (DataAttributeSet.ID > 0)
            {
                FeatureID = await FeatureRepository.CreateDataattributeSetFeature(feature, DataAttributeSet.ID);
            }
            return FeatureID;

        }

        public async Task<Feature> UpdateFeature(Feature feature)
        {

            var features = await FeatureRepository.UpdateFeature(feature);
            return features;

        }

        public async Task<int> UpdateeDataattributeFeature(Feature feature)
        {
            var DataAttributeSet = await UpdatedataattributeSet(feature.DataAttributeSets);
            int FeatureID = 0;
            if (DataAttributeSet.ID > 0)
            {
                FeatureID = await FeatureRepository.CreateDataattributeSetFeature(feature, DataAttributeSet.ID);
            }
            return FeatureID;

        }
        public async Task<DataAttributeSet> CreateDataattributeSet(DataAttributeSet dataAttributeSet)
        {
            return await FeatureRepository.CreateDataattributeSet(dataAttributeSet);
        }
        public async Task<DataAttributeSet> UpdatedataattributeSet(DataAttributeSet dataAttributeSet)
        {
            return await FeatureRepository.UpdatedataattributeSet(dataAttributeSet);
        }
        public async Task<bool> DeleteDataAttribute(int dataAttributeSetID)
        {
            return await FeatureRepository.DeleteDataAttribute(dataAttributeSetID);
        }
        public async Task<List<DataAttributeSet>> GetDataAttributeSetDetails(int dataAttributeSetID)
        {
            return await FeatureRepository.GetDataAttributeSetDetails(dataAttributeSetID);
        }
        public async Task<int> UpdateFeatureSetMapping(int UpdateFeatureSetID, int ID)
        {
            return await FeatureRepository.UpdateFeatureSetMapping(UpdateFeatureSetID, ID);
        }
        public async Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet)
        {
            return await FeatureRepository.UpdateFeatureSet(featureSet);
        }

        public async Task<IEnumerable<DataAttribute>> GetDataAttributes(string Langaugecode)
        {
            return await FeatureRepository.GetDataAttributes(Langaugecode);
        }

        public async Task<int> DeleteFeature(int FeatureId)
        {

            return await FeatureRepository.DeleteFeature(FeatureId);
        }

        public int CheckFeatureNameExist(string FeatureName, int FeatureId)
        {
            return FeatureRepository.CheckFeatureNameExist(FeatureName, FeatureId);
        }

        public async Task<int> ChangeFeatureState(int FeatureID, Char State)
        {
            return await FeatureRepository.ChangeFeatureState(FeatureID, State);
        }
    }
}
