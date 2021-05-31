using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;

namespace net.atos.daf.ct2.featureservice
{
    public class FeatureManagementService : FeatureService.FeatureServiceBase
    {
        //private readonly ILogger<FeatureManagementService> _logger;
        private readonly IFeatureManager _FeaturesManager;
        private ILog _logger;
        public FeatureManagementService(IFeatureManager FeatureManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
                _logger.Info("Feature Set created with id." + ObjResponse.FeatureSetID);

                return await Task.FromResult(new FeatureSetResponce
                {
                    Message = featureset.FeatureSetID.ToString(),
                    Code = Responcecode.Success,
                    FeatureSetID = featureset.FeatureSetID
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
                FeaturesListResponce features = new FeaturesListResponce();
                if (featurefilterRequest.FeatureSetID != 0)
                {
                    var listfeatures = await _FeaturesManager.GetFeatureIdsForFeatureSet(featurefilterRequest.FeatureSetID, featurefilterRequest.LangaugeCode);
                    foreach (var item in listfeatures)
                    {
                        FeatureRequest ObjResponce = new FeatureRequest();
                        ObjResponce.Id = item.Id;
                        ObjResponce.Name = item.Value == null ? item.Name : item.Value;
                        ObjResponce.State = item.state;
                        ObjResponce.Key = item.Key == null ? "" : item.Key;
                        ObjResponce.Type = item.Type.ToString();
                        ObjResponce.Level = item.Level;
                        features.Features.Add(ObjResponce);
                    }

                    return await Task.FromResult(features);
                }
                else
                {
                    var feature = await _FeaturesManager.GetFeatures(featurefilterRequest.RoleID, featurefilterRequest.OrganizationID, featurefilterRequest.FeatureID, featurefilterRequest.Level, '0', featurefilterRequest.LangaugeCode);
                    foreach (var item in feature)
                    {
                        FeatureRequest ObjResponce = new FeatureRequest();
                        ObjResponce.Id = item.Id;
                        ObjResponce.Name = item.Value == null ? item.Name : item.Value;
                        //ObjResponce.Status = item.Is_Active;
                        //ObjResponce.State = (FeatureState)Enum.Parse(typeof(FeatureState), item.state.ToString().ToUpper());
                        ObjResponce.State = item.state;
                        ObjResponce.Key = item.Key == null ? "" : item.Key;

                        if (item.Type.ToString() == "D")
                        {
                            try
                            {
                                ObjResponce.DataAttribute = new DataAttributeSetRequest();
                                var DataAttributeSet = await _FeaturesManager.GetDataAttributeset(item.Data_attribute_Set_id);
                                ObjResponce.DataAttribute.Name = DataAttributeSet.Name;
                                ObjResponce.DataAttribute.IsExclusive = DataAttributeSet.Is_exlusive;
                                ObjResponce.DataAttribute.Description = DataAttributeSet.Description == null ? "" : DataAttributeSet.Description;
                                ObjResponce.DataAttribute.DataAttributeSetId = DataAttributeSet.ID;
                                ObjResponce.Description = DataAttributeSet.Description == null ? "" : DataAttributeSet.Description;
                                //ObjResponce.DataAttribute.DataAttributeIDs = new 
                                foreach (var items in DataAttributeSet.DataAttributes)
                                {
                                    ObjResponce.DataAttribute.DataAttributeIDs.Add(items.ID);
                                }
                                ObjResponce.DataAttribute.Name = DataAttributeSet.Name;
                            }
                            catch (Exception)
                            {


                            }

                        }

                        ObjResponce.Type = item.Type.ToString();
                        ObjResponce.Level = item.Level;
                        features.Features.Add(ObjResponce);
                    }

                    return await Task.FromResult(features);
                }

            }
            catch (Exception ex)
            {
                return await Task.FromResult(new FeaturesListResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });

            }
        }

        //public async override
        public async override Task<DataAttributeResponceList> GetDataAttributes(DataAtributeRequest Request, ServerCallContext context)
        {
            try
            {
                var listfeatures = await _FeaturesManager.GetDataAttributes(Request.LangaugeCode);
                DataAttributeResponceList Dataresponce = new DataAttributeResponceList();
                foreach (var item in listfeatures)
                {
                    DataAttributeResponce responce = new DataAttributeResponce();
                    responce.Id = item.ID;
                    responce.Name = item.Value == null ? item.Name : item.Value;
                    responce.Description = item.Description == null ? "" : item.Description;
                    responce.Key = item.Key == null ? "" : item.Key;
                    Dataresponce.Responce.Add(responce);
                }
                Dataresponce.Code = Responcecode.Success;
                return Dataresponce;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async override Task<FeatureResponce> Create(FeatureRequest featureRequest, ServerCallContext context)
        {
            try
            {
                Feature FeatureObj = new Feature();
                var FeatureExist = _FeaturesManager.CheckFeatureNameExist(featureRequest.Name, 0);
                if (FeatureExist > 0)
                {
                    return await Task.FromResult(new FeatureResponce
                    {
                        Message = "Feature name allready exists",
                        Code = Responcecode.Success,
                        FeatureID = 0

                    });
                }
                FeatureObj.Name = featureRequest.Name.Trim();
                FeatureObj.Level = featureRequest.Level;

                FeatureObj.Description = featureRequest.Description;
                FeatureObj.DataAttributeSets = new DataAttributeSet();
                FeatureObj.DataAttributeSets.Name = featureRequest.DataAttribute.Name;
                FeatureObj.DataAttributeSets.Description = featureRequest.DataAttribute.Description;
                //FeatureObj.DataAttributeSets.Is_exlusive = featureRequest.DataAttribute.AttributeType.ToString();
                FeatureObj.FeatureState = (StatusType)Enum.Parse(typeof(StatusType), featureRequest.State.ToString().ToUpper());
                FeatureObj.DataAttributeSets.Is_exlusive = featureRequest.DataAttribute.IsExclusive;
                FeatureObj.DataAttributeSets.DataAttributes = new List<DataAttribute>();
                foreach (var item in featureRequest.DataAttribute.DataAttributeIDs)
                {
                    DataAttribute objDataAttribute = new DataAttribute();
                    objDataAttribute.ID = item;
                    FeatureObj.DataAttributeSets.DataAttributes.Add(objDataAttribute);
                }
                var result = await _FeaturesManager.CreateDataattributeFeature(FeatureObj);
                return await Task.FromResult(new FeatureResponce
                {
                    Message = "Feature Created Successfully",
                    Code = Responcecode.Success,
                    FeatureID = result

                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new FeatureResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        public async override Task<FeatureResponce> Update(FeatureRequest featureRequest, ServerCallContext context)
        {
            try
            {
                Feature FeatureObj = new Feature();
                FeatureObj.Name = featureRequest.Name.Trim();
                var FeatureExist = _FeaturesManager.CheckFeatureNameExist(featureRequest.Name, featureRequest.Id);
                if (FeatureExist > 0)
                {
                    return await Task.FromResult(new FeatureResponce
                    {
                        Message = "Feature name allready exists",
                        Code = Responcecode.Success,
                        FeatureID = 0

                    });
                }
                FeatureObj.Level = featureRequest.Level;
                FeatureObj.Key = featureRequest.Key;
                //FeatureObj.Is_Active = featureRequest.State;
                FeatureObj.Description = featureRequest.Description;
                FeatureObj.DataAttributeSets = new DataAttributeSet();
                FeatureObj.DataAttributeSets.Name = featureRequest.DataAttribute.Name;
                FeatureObj.DataAttributeSets.ID = featureRequest.DataAttribute.DataAttributeSetId;
                FeatureObj.DataAttributeSets.Description = featureRequest.DataAttribute.Description;
                FeatureObj.DataAttributeSets.Is_exlusive = featureRequest.DataAttribute.IsExclusive;
                FeatureObj.FeatureState = (StatusType)Enum.Parse(typeof(StatusType), featureRequest.State.ToString().ToUpper());
                FeatureObj.DataAttributeSets.DataAttributes = new List<DataAttribute>();
                foreach (var item in featureRequest.DataAttribute.DataAttributeIDs)
                {
                    DataAttribute objDataAttribute = new DataAttribute();
                    objDataAttribute.ID = item;
                    FeatureObj.DataAttributeSets.DataAttributes.Add(objDataAttribute);
                }
                var result = await _FeaturesManager.UpdateFeature(FeatureObj);
                return await Task.FromResult(new FeatureResponce
                {
                    Message = "Feature updated Successfully",
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new FeatureResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }


        public async override Task<FeatureSetResponce> UpdateFeatureSet(FetureSetRequest featureSetRequest, ServerCallContext context)
        {
            try
            {
                _logger.Info("UpdateFeatureSet method in Feature API called.");


                FeatureSet ObjResponse = new FeatureSet();
                FeatureSet featureset = new FeatureSet();
                featureset.FeatureSetID = featureSetRequest.FeatureSetID;
                featureset.Name = featureSetRequest.Name; // "FeatureSet_" + DateTimeOffset.Now.ToUnixTimeSeconds()
                featureset.description = featureSetRequest.Description;
                featureset.status = featureSetRequest.Active == true ? StatusType.ACTIVE : StatusType.INACTIVE;

                featureset.modified_by = featureSetRequest.ModifiedBy;
                featureset.Features = new List<Feature>();
                foreach (var item in featureSetRequest.Features)
                {
                    Feature f = new Feature();
                    f.Id = item;
                    featureset.Features.Add(f);
                }


                ObjResponse = await _FeaturesManager.UpdateFeatureSet(featureset);
                featureset.FeatureSetID = ObjResponse.FeatureSetID;
                _logger.Info("Feature Set created with id." + ObjResponse.FeatureSetID);

                //await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Feature Component", "Feature Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Update method in Feature manager", ObjResponse.FeatureSetID, ObjResponse.FeatureSetID, JsonConvert.SerializeObject(ObjResponse.FeatureSetID));
                return await Task.FromResult(new FeatureSetResponce
                {
                    Message = featureset.FeatureSetID.ToString() + " Updated successfully",
                    Code = Responcecode.Success,
                    FeatureSetID = featureset.FeatureSetID

                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new FeatureSetResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }


        public async override Task<FeatureSetResponce> DeleteFeatureSet(FetureSetRequest featureSetRequest, ServerCallContext context)
        {
            try
            {
                _logger.Info("DeleteFeatureSet method in Feature API called.");

                bool IsFeatureSetIDDeleted = await _FeaturesManager.DeleteFeatureSet(featureSetRequest.FeatureSetID);

                //await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Feature Component", "Feature Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "DeleteFeatureSet method in Feature manager", FeatureSetId, FeatureSetId, JsonConvert.SerializeObject(FeatureSetId));
                return await Task.FromResult(new FeatureSetResponce
                {
                    Message = featureSetRequest.FeatureSetID.ToString() + " Deleted successfully",
                    Code = Responcecode.Success,
                    FeatureSetID = featureSetRequest.FeatureSetID

                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new FeatureSetResponce
                {
                    Message = featureSetRequest.FeatureSetID.ToString() + " Delete failed",
                    Code = Responcecode.Failed,
                    FeatureSetID = featureSetRequest.FeatureSetID

                });

            }
        }

        public async override Task<FeatureResponce> Delete(FeatureRequest featureSetRequest, ServerCallContext context)
        {
            try
            {
                _logger.Info("DeleteFeatureSet method in Feature API called.");

                var FeatureId = await _FeaturesManager.DeleteFeature(featureSetRequest.Id);

                //await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Feature Component", "Feature Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "DeleteFeatureSet method in Feature manager", FeatureSetId, FeatureSetId, JsonConvert.SerializeObject(FeatureSetId));
                if (FeatureId > 0)
                {
                    return await Task.FromResult(new FeatureResponce
                    {
                        Message = featureSetRequest.Id.ToString() + " Deleted successfully",
                        Code = Responcecode.Success,
                        FeatureID = featureSetRequest.Id

                    });
                }
                else
                {
                    return await Task.FromResult(new FeatureResponce
                    {
                        Message = featureSetRequest.Id.ToString() + " Not a valid feature Id",
                        Code = Responcecode.Failed,
                        FeatureID = featureSetRequest.Id

                    });
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new FeatureResponce
                {
                    Message = featureSetRequest.Id.ToString() + " Delete failed",
                    Code = Responcecode.Failed,
                    FeatureID = featureSetRequest.Id

                });

            }
        }

        public async override Task<FeatureStateResponce> ChangeFeatureState(FeatureStateRequest featureSetRequest, ServerCallContext context)
        {
            try
            {
                _logger.Info("Feature State method in Feature API called.");

                var FeatureId = await _FeaturesManager.ChangeFeatureState(featureSetRequest.Featureid, Convert.ToChar(featureSetRequest.FeatureState));

                //await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Feature Component", "Feature Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "DeleteFeatureSet method in Feature manager", FeatureSetId, FeatureSetId, JsonConvert.SerializeObject(FeatureSetId));
                if (FeatureId > 0)
                {
                    return await Task.FromResult(new FeatureStateResponce
                    {
                        Message = featureSetRequest.Featureid.ToString() + " Changed successfully",
                        Code = Responcecode.Success

                    });
                }
                else
                {
                    return await Task.FromResult(new FeatureStateResponce
                    {
                        Message = featureSetRequest.Featureid.ToString() + " Not a valid feature Id",
                        Code = Responcecode.Failed


                    });
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new FeatureStateResponce
                {
                    Message = featureSetRequest.Featureid.ToString() + " Feature state change failed",
                    Code = Responcecode.Failed

                });

            }
        }
    }
}