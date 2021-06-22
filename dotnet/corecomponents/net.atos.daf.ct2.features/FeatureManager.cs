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
        public FeatureManager(IFeatureRepository featureRepository) => _featureRepository = featureRepository;// auditlog=_auditlog;

        public async Task<int> AddFeatureSet(FeatureSet featureSet) => await _featureRepository.AddFeatureSet(featureSet);
        public async Task<int> GetMinimumLevel(List<Feature> features) => await _featureRepository.GetMinimumLevel(features);
        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int featureSetId, char state) => await _featureRepository.GetFeatureSet(featureSetId, state);

        public async Task<IEnumerable<Feature>> GetFeatures(int roleId, int organizationid, int featureId, int level, char? featureType, string langaugeCode)
        {
            return await _featureRepository.GetFeatures(roleId, organizationid, featureId, level, featureType, langaugeCode);
        }

        public async Task<IEnumerable<Feature>> GetFeatureIdsForFeatureSet(int getFeatureIdsForFeatureSet, string langaugeCode)
        {
            return await _featureRepository.GetFeatureIdsForFeatureSet(getFeatureIdsForFeatureSet, langaugeCode);
        }

        public async Task<DataAttributeSet> GetDataAttributeset(int dataAttributeSetID)
        {
            return await _featureRepository.GetDataAttributeset(dataAttributeSetID);
        }
        public async Task<bool> DeleteFeatureSet(int featureSetId)
        {
            return await _featureRepository.DeleteFeatureSet(featureSetId);
        }
        public async Task<FeatureSet> CreateFeatureSet(FeatureSet featureSet)
        {
            return await _featureRepository.CreateFeatureSet(featureSet);
        }

        public async Task<int> CreateDataattributeFeature(Feature feature)
        {
            var dataAttributeSet = await CreateDataattributeSet(feature.DataAttributeSets);
            int featureID = 0;
            if (dataAttributeSet.ID > 0)
            {
                featureID = await _featureRepository.CreateDataattributeSetFeature(feature, dataAttributeSet.ID);
            }
            return featureID;

        }

        public async Task<Feature> UpdateFeature(Feature feature)
        {

            var features = await _featureRepository.UpdateFeature(feature);
            return features;

        }

        public async Task<int> UpdateeDataattributeFeature(Feature feature)
        {
            var dataAttributeSet = await UpdatedataattributeSet(feature.DataAttributeSets);
            int featureID = 0;
            if (dataAttributeSet.ID > 0)
            {
                featureID = await _featureRepository.CreateDataattributeSetFeature(feature, dataAttributeSet.ID);
            }
            return featureID;

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
        public async Task<int> UpdateFeatureSetMapping(int updateFeatureSetID, int id)
        {
            return await _featureRepository.UpdateFeatureSetMapping(updateFeatureSetID, id);
        }
        public async Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet)
        {
            return await _featureRepository.UpdateFeatureSet(featureSet);
        }

        public async Task<IEnumerable<DataAttribute>> GetDataAttributes(string langaugeCode)
        {
            return await _featureRepository.GetDataAttributes(langaugeCode);
        }

        public async Task<int> DeleteFeature(int featureId)
        {

            return await _featureRepository.DeleteFeature(featureId);
        }

        public int CheckFeatureNameExist(string featureName, int featureId)
        {
            return _featureRepository.CheckFeatureNameExist(featureName, featureId);
        }

        public async Task<int> ChangeFeatureState(int featureID, Char state)
        {
            return await _featureRepository.ChangeFeatureState(featureID, state);
        }
    }
}
