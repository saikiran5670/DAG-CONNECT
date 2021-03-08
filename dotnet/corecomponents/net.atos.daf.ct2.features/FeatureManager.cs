using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;

namespace net.atos.daf.ct2.features
{
    public class FeatureManager:IFeatureManager
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

        public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId,bool Active)
        {
            return await FeatureRepository.GetFeatureSet(FeatureSetId, Active);
        }

        public async Task<IEnumerable<Feature>> GetFeatures(int RoleId, int Organizationid, char? Featuretype)
        {
            return await FeatureRepository.GetFeatures(RoleId,Organizationid, Featuretype);
        }

        public async Task<IEnumerable<Feature> > GetFeatureIdsForFeatureSet(int GetFeatureIdsForFeatureSet)
        {
            return await FeatureRepository.GetFeatureIdsForFeatureSet(GetFeatureIdsForFeatureSet);
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
        public async Task <List<DataAttributeSet>> GetDataAttributeSetDetails(int dataAttributeSetID)
        {
            return await FeatureRepository.GetDataAttributeSetDetails(dataAttributeSetID);
        }
        public async Task<int> UpdateFeatureSetMapping(int UpdateFeatureSetID, int ID)
        {
            return await FeatureRepository.UpdateFeatureSetMapping(UpdateFeatureSetID,ID);
        }
        public async Task<FeatureSet> UpdateFeatureSet(FeatureSet featureSet)
        {
            return await FeatureRepository.UpdateFeatureSet(featureSet);
        }

        public async Task<IEnumerable<DataAttribute>> GetDataAttributes()
        {
            return await FeatureRepository.GetDataAttributes();
        }
    }
}
