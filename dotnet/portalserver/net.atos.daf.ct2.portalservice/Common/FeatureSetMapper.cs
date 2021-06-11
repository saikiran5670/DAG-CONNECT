using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.featureservice;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class FeatureSetMapper
    {
        private readonly FeatureService.FeatureServiceClient _featureclient;
        public FeatureSetMapper(FeatureService.FeatureServiceClient featureclient)
        {
            _featureclient = featureclient;

        }

        public async Task<int> RetrieveFeatureSetIdByName(List<string> features)
        {
            var featureSetId = new List<int>();
            var featureSetRequest = new FetureSetRequest();
            var featureFilterRequest = new FeaturesFilterRequest();
            var featureList = await _featureclient.GetFeaturesAsync(featureFilterRequest);
            foreach (var item in features)
            {
                bool hasFeature = featureList.Features.Any(feature => feature.Name == item);
                if (hasFeature)
                {
                    foreach (var feature in featureList.Features)
                    {
                        if (feature.Name == item)
                        {
                            featureSetId.Add(feature.Id);
                        }
                    }
                }
                else
                {
                    return 0;

                }
            }

            featureSetRequest.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            featureSetId = featureSetId.Select(x => x).Distinct().ToList();
            featureSetRequest.Features.AddRange(featureSetId);

            var ObjResponse = await _featureclient.CreateFeatureSetAsync(featureSetRequest);
            return ObjResponse.FeatureSetID;

        }


        public async Task<int> UpdateFeatureSetIdByName(List<string> features, int featureSetId)
        {
            var featureSetIds = new List<int>();
            var featureSetRequest = new FetureSetRequest();
            var featureFilterRequest = new FeaturesFilterRequest();
            var featureList = await _featureclient.GetFeaturesAsync(featureFilterRequest);
            foreach (var item in features)
            {
                bool hasFeature = featureList.Features.Any(feature => feature.Name == item);
                if (hasFeature)
                {
                    foreach (var feature in featureList.Features)
                    {
                        if (feature.Name == item)
                        {
                            featureSetIds.Add(feature.Id);
                        }
                    }
                }
                else
                {
                    return 0;

                }
            }
            featureSetRequest.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            featureSetRequest.Features.Add(featureSetIds.Select(x => x).Distinct().ToArray());
            featureSetRequest.FeatureSetID = featureSetId;
            var ObjResponse = await _featureclient.UpdateFeatureSetAsync(featureSetRequest);
            return ObjResponse.FeatureSetID;

        }



        public async Task<int> RetrieveFeatureSetIdById(List<int> features)
        {
            var featureSetId = new List<int>();
            var featureSetRequest = new FetureSetRequest();
            featureSetRequest.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            featureSetId = features.Select(x => x).Distinct().ToList();
            featureSetRequest.Features.AddRange(features);
            var ObjResponse = await _featureclient.CreateFeatureSetAsync(featureSetRequest);
            return ObjResponse.FeatureSetID;

        }


        public async Task<int> UpdateFeatureSetIdById(List<int> features, int featureSetId)
        {

            var featureSetRequest = new FetureSetRequest();
            featureSetRequest.Name = "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds();
            featureSetRequest.Features.Add(features.Select(x => x).Distinct().ToArray());
            featureSetRequest.FeatureSetID = featureSetId;
            var ObjResponse = await _featureclient.UpdateFeatureSetAsync(featureSetRequest);
            return ObjResponse.FeatureSetID;

        }


        public async Task<IEnumerable<int>> GetFeatureIds(int featureSetId)
        {
            var features = new List<int>();
            var featureFilterRequest = new FeaturesFilterRequest();
            featureFilterRequest.FeatureSetID = featureSetId;
            var featureList = await _featureclient.GetFeaturesAsync(featureFilterRequest);
            features.AddRange(featureList.Features.Select(x => x.Id).ToList());
            return features;
        }

        public async Task<IEnumerable<string>> GetFeatures(int featureSetId)
        {
            var features = new List<string>();
            var featureFilterRequest = new FeaturesFilterRequest();
            featureFilterRequest.FeatureSetID = featureSetId;
            var featureList = await _featureclient.GetFeaturesAsync(featureFilterRequest);
            features.AddRange(featureList.Features.Select(x => x.Name).ToList());
            return features;
        }

    }
}
