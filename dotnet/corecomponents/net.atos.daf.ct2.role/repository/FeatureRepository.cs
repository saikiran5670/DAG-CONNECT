// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using net.atos.daf.ct2.audit;
// using net.atos.daf.ct2.role.entity;
// using net.atos.daf.ct2.role.repository;
// namespace net.atos.daf.ct2.rolerepository
// {
//     public class FeatureRepository : IFeatureRepository
//     {
//         IFeatureRepository featureRepository;
//         IAuditLog auditlog;
//         public FeatureRepository(IFeatureRepository _featureRepository,IAuditLog _auditlog)
//         {
//             featureRepository = _featureRepository;
//             auditlog=_auditlog;
//         }

//         #region Feature Type
//         public async Task<int> AddFeatureType(FeatureType featureType)
//         {
//             try
//             {
//                 int FeatureTypeId= await featureRepository.AddFeatureType(featureType);
//                 //auditlog.AddLogs(featureType.Createdby,featureType.Createdby,1,"Add Feature Type",FeatureTypeId > 0,"Feature Management", "Feature type Added With feature type Id " + FeatureTypeId.ToString());
//                 return FeatureTypeId;
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }
//         public async Task<int> UpdateFeatureType(FeatureType featureType)
//         {
//             try
//             {
//                 int FeatureTypeId= await featureRepository.UpdateFeatureType(featureType);
//                 //auditlog.AddLogs(featureType.modifiedby,featureType.modifiedby,1,"Update Feature Type", FeatureTypeId > 0,"Feature Management", "Feature type Updated With Feature type Id " + FeatureTypeId.ToString());
//                 return FeatureTypeId;
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }
//         public async Task<int> DeleteFeatureType(int FeatureTypeId, int Userid)
//         {
//             try
//             {
//                 int Featuretypeid =await featureRepository.DeleteFeatureType(FeatureTypeId, Userid);
//                 auditlog.AddLogs(Userid,Userid,1,"Delete Feature Type", Featuretypeid > 0,"Feature Management", "Feature type Deleted With Feature type Id " + Featuretypeid.ToString());
//                 return Featuretypeid;
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }
//         // public async Task<IEnumerable<FeatureType>> GetFeatureType(int FeatureTypeId)
//         // {
//         //     try
//         //     {
//         //         return await featureRepository.GetFeatureType(FeatureTypeId);
//         //     }
//         //     catch (Exception ex)
//         //     {
//         //         throw;
//         //     }
//         // }
//         public async Task<int> CheckFeatureTypeExist(string FeatureType)
//         {
//             try
//             {
//                 return await featureRepository.CheckFeatureTypeExist(FeatureType);
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         #endregion

//         #region Feature

//         // public async Task<IEnumerable<Feature>> GetFeature(int RoleFeatureId)
//         // {
//         //     try
//         //     {
//         //         return await featureRepository.GetFeature(RoleFeatureId);
//         //     }
//         //     catch (Exception ex)
//         //     {
//         //         throw;
//         //     }
//         // }

//         public async Task<int> AddFeature(Feature feature)
//         {
//             try
//             {
//                 int FeatureId= await featureRepository.AddFeature(feature);
//               //  auditlog.AddLogs(feature.createdby,feature.createdby,1,"Add Feature",FeatureId > 0,"Feature Management", "Feature Added With feature Id " + FeatureId.ToString());
//                 return FeatureId;
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         public async Task<int> UpdateFeature(Feature feature)
//         {
//             try
//             {
//                 int FeatureId= await featureRepository.UpdateFeature(feature);
//                // auditlog.AddLogs(feature.modifiedby,feature.modifiedby,1,"Update Feature",FeatureId > 0,"Feature Management", "Feature updated With feature Id " + FeatureId.ToString());
//                 return FeatureId;
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         public async Task<int> DeleteFeature(int RoleFeatureId, int UserId)
//         {
//             try
//             {
//                 int FeatureId= await featureRepository.DeleteFeature(RoleFeatureId, UserId);
//                 auditlog.AddLogs(UserId,UserId,1,"Deleted Feature",FeatureId > 0,"Feature Management", "Feature deleted With feature Id " + FeatureId.ToString());
//                 return FeatureId;
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         public async Task<int> CheckFeatureExist(string FeatureName)
//         {
//             try
//             {
//                 return await featureRepository.CheckFeatureExist(FeatureName);
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         #endregion

//         #region Feature Set

//         public async Task<int> AddFeatureSet(FeatureSet featureSet)
//         {
//             try
//             {
//                 int FeatureSetId=  await featureRepository.AddFeatureSet(featureSet);
//                // auditlog.AddLogs(featureSet.createdby,featureSet.createdby,1,"Add Feature Set",FeatureSetId > 0,"Feature Management", "Feature set added With feature set Id " + FeatureSetId.ToString());
//                 return FeatureSetId;
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         public async Task<int> UpdateFeatureSet(FeatureSet featureSet)
//         {
//             try
//             {
//                 int FeatureSetId= await featureRepository.UpdateFeatureSet(featureSet);
//                // auditlog.AddLogs(featureSet.modifiedby,featureSet.modifiedby,1,"Updated Feature Set",FeatureSetId > 0,"Feature Management", "Feature set updated With feature set Id " + FeatureSetId.ToString());
//                 return FeatureSetId;
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }


//         public async Task<int> DeleteFeatureSet(int FeatureSetId, int UserId)
//         {
//             try
//             {
//                  int FeaturesetDid= await featureRepository.DeleteFeatureSet(FeatureSetId, UserId);
//                  auditlog.AddLogs(UserId,UserId,1,"Deleted Feature Set",FeaturesetDid > 0,"Feature Management", "Feature set deleted With feature set Id " + FeaturesetDid.ToString());
//                  return FeaturesetDid;
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         public async Task<IEnumerable<FeatureSet>> GetFeatureSet(int FeatureSetId)
//         {
//             try
//             {
//                 return await featureRepository.GetFeatureSet(FeatureSetId);
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         public async Task<int> CheckFeatureSetExist(string FeatureSetName)
//         {
//             try
//             {
//                 return await featureRepository.CheckFeatureSetExist(FeatureSetName);
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         public async Task<IEnumerable<FeatureSet>> GetFeatureSetFeature(int FeatureSetId)
//         {
//             try
//             {
//                 return await featureRepository.GetFeatureSetFeature(FeatureSetId);
//             }
//             catch (Exception ex)
//             {
//                 throw;
//             }
//         }

//         #endregion

//     }
// }
