using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.roleservice;
using net.atos.daf.ct2.role.repository;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.role;
using net.atos.daf.ct2.features;
using Newtonsoft.Json;
using log4net;
using net.atos.daf.ct2.features.entity;
using System.Reflection;

namespace net.atos.daf.ct2.roleservice
{

    public class RoleManagementService : RoleService.RoleServiceBase
    {

        // private readonly ILogger<RoleManagementService> _logger;

        private ILog _logger;
        private readonly IRoleManagement _RoleManagement;
        private readonly IFeatureManager _FeaturesManager;
        public RoleManagementService(IRoleManagement RoleManagement, IFeatureManager FeatureManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _RoleManagement = RoleManagement;
            _FeaturesManager = FeatureManager;

        }

        public async override Task<RoleResponce> Create(RoleRequest request, ServerCallContext context)
        {
            try
            {

                RoleMaster ObjRole = new RoleMaster();
                ObjRole.Organization_Id = request.OrganizationId;
                ObjRole.Name = request.RoleName;
                ObjRole.Created_by = request.CreatedBy;
                ObjRole.Description = request.Description;
                ObjRole.Feature_set_id = 0;
                ObjRole.Level = request.Level;
                ObjRole.Code = request.Code;
                ObjRole.FeatureSet = new FeatureSet();
                ObjRole.FeatureSet.Features = new List<Feature>();
                foreach (var item in request.FeatureIds)
                {
                    ObjRole.FeatureSet.Features.Add(new Feature() { Id = item });
                }
                int Rid = _RoleManagement.CheckRoleNameExist(request.RoleName.Trim(), request.OrganizationId, 0);
                if (Rid > 0)
                {
                    return await Task.FromResult(new RoleResponce
                    {
                        Message = "Role name allready exist",
                        Code = Responcecode.Failed

                    });
                }
                var role = await _RoleManagement.CreateRole(ObjRole);

                return await Task.FromResult(new RoleResponce
                {
                    Message = "Role created with id:- " + role,
                    Code = Responcecode.Success

                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new RoleResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public async override Task<RoleResponce> Update(RoleRequest request, ServerCallContext context)
        {
            try
            {

                RoleMaster roleMaster = new RoleMaster();
                roleMaster.Name = request.RoleName;
                roleMaster.Id = request.RoleID;
                roleMaster.Updatedby = request.UpdatedBy;
                roleMaster.Description = request.Description;
                roleMaster.FeatureSet = new FeatureSet();
                //ObjRole.FeatureSet = new FeatureSet();
                roleMaster.FeatureSet.Features = new List<Feature>();
                foreach (var item in request.FeatureIds)
                {
                    roleMaster.FeatureSet.Features.Add(new Feature() { Id = item });
                }
                var role = await _RoleManagement.UpdateRole(roleMaster);

                return await Task.FromResult(new RoleResponce
                {
                    Message = "Role Updated id:- " + role,
                    Code = Responcecode.Success

                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new RoleResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public override Task<RoleResponce> Delete(RoleRequest request, ServerCallContext context)
        {
            try
            {

                var role = _RoleManagement.DeleteRole(request.RoleID, request.OrganizationId).Result;

                if (role < 0)
                {
                    return Task.FromResult(new RoleResponce
                    {
                        Message = "ROLE_IN_USE",
                        Code = Responcecode.Assigned

                    });
                }
                else
                {
                    return Task.FromResult(new RoleResponce
                    {
                        Message = "Role Updated id:- " + role,
                        Code = Responcecode.Success

                    });
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return Task.FromResult(new RoleResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public async override Task<RoleListResponce> Get(RoleFilterRequest request, ServerCallContext context)
        {
            try
            {
                RoleFilter ObjroleFilter = new RoleFilter();
                RoleListResponce ObjroleList = new RoleListResponce();

                ObjroleFilter.AccountId = request.AccountId;
                ObjroleFilter.RoleId = request.RoleId;
                ObjroleFilter.Organization_Id = request.OrganizationId;
                ObjroleFilter.State = request.Active ? "A" : "I";
                ObjroleFilter.IsGlobal = request.IsGlobal;
                ObjroleFilter.LangaugeCode = request.LangaugeCode;

                var role = _RoleManagement.GetRoles(ObjroleFilter).Result;
                foreach (var item in role)
                {
                    RoleRequest ObjResponce = new RoleRequest();
                    ObjResponce.RoleID = item.Id;
                    ObjResponce.OrganizationId = item.Organization_Id == null ? 0 : item.Organization_Id.Value;
                    ObjResponce.RoleName = item.Name;
                    ObjResponce.CreatedBy = item.Created_by;
                    ObjResponce.CreatedAt = item.Created_at;
                    //ObjResponce.= item.Is_Active;
                    ObjResponce.Description = item.Description == null ? "" : item.Description;
                    //ObjResponce.Roletype= item.Organization_Id == null ? RoleTypes.Global : RoleTypes.Regular;
                    ObjResponce.FeatureIds.Add(item.FeatureSet.Features.Select(I => I.Id).ToArray());
                    ObjResponce.Level = item.Level;
                    ObjResponce.Code = item.Code;
                    ObjroleList.Roles.Add(ObjResponce);
                }

                ObjroleList.Message = "Roles data retrieved";
                ObjroleList.Code = Responcecode.Success;
                return await Task.FromResult(ObjroleList);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new RoleListResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        //public async override Task<FeaturesListResponce> GetFeatures(FeaturesFilterRequest request,ServerCallContext context)
        //{
        //    try
        //    {
        //    FeaturesListResponce ObFeaturesList = new FeaturesListResponce();
        //    var result= await _FeaturesManager.GetFeatures(Convert.ToChar(request.Type),true);
        //         foreach (var item in result)
        //        {
        //            FeaturesRequest ObjResponce = new FeaturesRequest();
        //            ObjResponce.Id=item.Id;
        //            ObjResponce.Name = item.Name;
        //            ObjResponce.Active= item.Is_Active;
        //            ObjResponce.Description = item.Description == null ? "" : item.Description;
        //            ObjResponce.Type= item.Type.ToString();
        //            ObFeaturesList.Features.Add(ObjResponce);
        //        }
        //     return await Task.FromResult(ObFeaturesList);
        //    }
        //    catch(Exception ex)
        //    {
        //        return await Task.FromResult(new FeaturesListResponce
        //        {
        //            Message = "Exception " + ex.Message,
        //            Code = Responcecode.Failed
        //        });
        //    }
        //}


    }
}
