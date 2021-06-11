using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;

namespace net.atos.daf.ct2.features
{
    public class FeatureManager : IFeatureManager
    {
        readonly IFeatureRepository _featureRepository;

        // IAuditLog auditlog;
        public FeatureManager(IFeatureRepository FeatureRepository) => _featureRepository = FeatureRepository;// auditlog=_auditlog;

        public async Task<int> AddFeatureSet(FeatureSet featureSet) => await _featureRepository.AddFeatureSet(featureSet);
        public async Task<int> GetMinimumLevel(List<Feature> features) => await _featureRepository.GetMinimumLevel(features);
        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId, char state) => await _featureRepository.GetFeatureSet(FeatureSetId, state);

        public async Task<IEnumerable<Feature>> GetFeatures(int RoleId, int Organizationid, int FeatureId, int level, char? Featuretype, string Langaugecode)
        {
            return await _featureRepository.GetFeatures(RoleId, Organizationid, FeatureId, level, Featuretype, Langaugecode);
        }

        public async Task<IEnumerable<Feature>> GetFeatureIdsForFeatureSet(int GetFeatureIdsForFeatureSet, string Langaugecode)
        {
            return await _featureRepository.GetFeatureIdsForFeatureSet(GetFeatureIdsForFeatureSet, Langaugecode);
        }

        public async Task<DataAttributeSet> GetDataAttributeset(int DataAttributeSetID)
        {
            return await _featureRepository.GetDataAttributeset(DataAttributeSetID);
        }
        public async Task<bool> DeleteFeatureSet(int FeatureSetId)
        {
            return await _featureRepository.DeleteFeatureSet(FeatureSetId);
        }
        public async Task<FeatureSet> CreateFeatureSet(FeatureSet featureSet)
        {
            return await _featureRepository.CreateFeatureSet(featureSet);
        }

        public async Task<int> CreateDataattributeFeature(Feature feature)
        {
            var DataAttributeSet = await CreateDataattributeSet(feature.DataAttributeSets);
            int FeatureID = 0;
            if (DataAttributeSet.ID > 0)
            {
                FeatureID = await _featureRepository.CreateDataattributeSetFeature(feature, DataAttributeSet.ID);
            }
            return FeatureID;

        }

        public async Task<Feature> UpdateFeature(Feature feature)
        {

            var features = await _featureRepository.UpdateFeature(feature);
            return features;

        }

        public async Task<int> UpdateeDataattributeFeature(Feature feature)
        {
            var DataAttributeSet = await UpdatedataattributeSet(feature.DataAttributeSets);
            int FeatureID = 0;
            if (DataAttributeSet.ID > 0)
            {
                FeatureID = await _featureRepository.CreateDataattributeSetFeature(feature, DataAttributeSet.ID);
            }
            return FeatureID;

        }
        public async Task<DataAttributeSet> CreateDataattributeSet(DataAttributeSet dataAttributeSet)
        {
            return await _featureRepository.CreateDataattributeSet(dataAttributeSet);
        }
        public async Task<DataAttributeSet> UpdatedataattributeSet(DataAttributeSet dataAttributeSet)
        {
            return await _featureRepository.UpdatedataattributeSet(dataAttributeSet);
        }
        public async Task<bool> DeleteDataAttribute(int dataAttributeSetID)
        {
            return await _featureRepository.DeleteDataAttribute(dataAttributeSetID);
        }
        public async Task<List<DataAttributeSet>> GetDataAttributeSetDetails(int dataAttributeSetID)
        {
            return await _featureRepository.GetDataAttributeSetDetails(dataAttributeSetID);
        }
        public async Task<int> UpdateFeatureSetMapping(int UpdateFeatureSetID, int ID)
        {
            return await _featureRepository.UpdateFeatureSetMapping(UpdateFeatureSetID, ID);
        }
        public async Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet)
        {
            return await _featureRepository.UpdateFeatureSet(featureSet);
        }

        public async Task<IEnumerable<DataAttribute>> GetDataAttributes(string Langaugecode)
        {
            return await _featureRepository.GetDataAttributes(Langaugecode);
        }

        public async Task<int> DeleteFeature(int FeatureId)
        {

            return await _featureRepository.DeleteFeature(FeatureId);
        }

        public int CheckFeatureNameExist(string FeatureName, int FeatureId)
        {
            return _featureRepository.CheckFeatureNameExist(FeatureName, FeatureId);
        }

        public async Task<int> ChangeFeatureState(int FeatureID, Char State)
        {
            return await _featureRepository.ChangeFeatureState(FeatureID, State);
        }
    }
}
