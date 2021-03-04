using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.features;
using Newtonsoft.Json;
using net.atos.daf.ct2.features.entity;

namespace net.atos.daf.ct2.featureservice
{
    public class FeatureManagementService : FeatureServiceTest.FeatureServiceTestBase
    {
        private readonly ILogger<FeatureManagementService> _logger;
        private readonly IFeatureManager _FeaturesManager;
        public FeatureManagementService(ILogger<FeatureManagementService> logger, IFeatureManager FeatureManager)
        {
            _logger = logger;

            _FeaturesManager = FeatureManager;
        }

        public async override Task<FeatureSetResponce> CreateFeatureSet(FetureSetRequest featureSetRequest, ServerCallContext context)
        {
            try
            {

                FeatureSet ObjResponse = new FeatureSet();
                FeatureSet featureset = new FeatureSet();
                featureset.Name = featureSetRequest.Name; // "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds()
                featureset.description = featureSetRequest.Key;
                //featureset.status = featureSetRequest.;
                featureset.created_by = featureSetRequest.CreatedBy;

                featureset.Features = new List<Feature>();
                foreach (var item in featureSetRequest.Features)
                {
                    Feature featureObj = new Feature();
                    featureObj.Id = item;
                    featureset.Features.Add(featureObj);
                }

                ObjResponse = await _FeaturesManager.CreateFeatureSet(featureset);
                featureset.FeatureSetID = ObjResponse.FeatureSetID;
                _logger.LogInformation("Feature Set created with id." + ObjResponse.FeatureSetID);

                return await Task.FromResult(new FeatureSetResponce
                {
                    Message = featureset.FeatureSetID.ToString(),
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new FeatureSetResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public async override Task<FeaturesListResponce> GetFeatures(FeaturesFilterRequest featurefilterRequest, ServerCallContext context)
        {
            try
            {
                FeaturesListResponce features= new FeaturesListResponce();
                if (featurefilterRequest.FeatureSetID != 0 )
                {
                    var listfeatures = await _FeaturesManager.GetFeatureIdsForFeatureSet(featurefilterRequest.FeatureSetID);
                    foreach (var item in listfeatures)
                    {
                        FeatureRequest ObjResponce = new FeatureRequest();
                        ObjResponce.Id = item.Id;
                        ObjResponce.Name = item.Name;
                        ObjResponce.Active = item.Is_Active;
                        ObjResponce.Key = item.Key == null ? "" : item.Key;
                        ObjResponce.Type = item.Type.ToString();
                        features.Features.Add(ObjResponce);
                    }

                    return await Task.FromResult(features);
                }
                else
                {
                    var feature = await _FeaturesManager.GetFeatures(featurefilterRequest.RoleID, featurefilterRequest.OrganizationID, 'D');
                    foreach (var item in feature)
                    {
                        FeatureRequest ObjResponce = new FeatureRequest();
                        ObjResponce.Id = item.Id;
                        ObjResponce.Name = item.Name;
                        ObjResponce.Active = item.Is_Active;
                        ObjResponce.Key = item.Key == null ? "" : item.Key;
                        ObjResponce.Type = item.Type.ToString();
                        features.Features.Add(ObjResponce);
                    }

                    return await Task.FromResult(features);
                }

            }
            catch(Exception ex)
            {
                return await Task.FromResult(new FeaturesListResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });

            }
        }


        //public async override Task<FeatureSetResponce> CreateDataattributeSet(FetureSetRequest featureSetRequest, ServerCallContext context)
        //{

        //}
     }
}