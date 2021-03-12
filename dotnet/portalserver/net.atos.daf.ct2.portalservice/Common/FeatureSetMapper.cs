using net.atos.daf.ct2.featureservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Common
{
    public class FeatureSetMapper
    {
        private readonly FeatureService.FeatureServiceClient _featureclient;
        public FeatureSetMapper(FeatureService.FeatureServiceClient featureclient)
        {
            _featureclient = featureclient;

        }
       
        public async Task<int> RetrieveFeatureSetId(List<string> features)
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


        public async Task<int> UpdateFeatureSetId(List<string> features, int featureSetId)
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

    }
}
