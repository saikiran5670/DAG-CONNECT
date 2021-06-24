using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.role;
using net.atos.daf.ct2.role.entity;

namespace net.atos.daf.ct2.roleservice
{

    public class RoleManagementService : RoleService.RoleServiceBase
    {
        private readonly ILog _logger;
        private readonly IRoleManagement _roleManagement;

        public RoleManagementService(IRoleManagement roleManagement)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _roleManagement = roleManagement;
        }

        public async override Task<RoleResponce> Create(RoleRequest request, ServerCallContext context)
        {
            try
            {
                RoleMaster objRole = new RoleMaster();
                objRole.Organization_Id = request.OrganizationId;
                objRole.Name = request.RoleName;
                objRole.Created_by = request.CreatedBy;
                objRole.Description = request.Description;
                objRole.Feature_set_id = 0;
                objRole.Level = request.Level;
                objRole.Code = request.Code;
                objRole.FeatureSet = new FeatureSet();
                objRole.FeatureSet.Features = new List<Feature>();
                foreach (var item in request.FeatureIds)
                {
                    objRole.FeatureSet.Features.Add(new Feature() { Id = item });
                }
                int rid = _roleManagement.CheckRoleNameExist(request.RoleName.Trim(), request.OrganizationId, 0);
                if (rid > 0)
                {
                    return await Task.FromResult(new RoleResponce
                    {
                        Message = "Role name allready exist",
                        Code = Responcecode.Failed

                    });
                }
                var role = await _roleManagement.CreateRole(objRole);

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
                var role = await _roleManagement.UpdateRole(roleMaster);

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

        public async override Task<DeleteRoleResponce> Delete(RoleRequest request, ServerCallContext context)
        {
            try
            {

                var assignedrole = await _roleManagement.IsRoleAssigned(request.RoleID);
                DeleteRoleResponce responce = new DeleteRoleResponce();
                foreach (var item in assignedrole)
                {
                    responce.Role.Add(new AssignedRole
                    {
                        FirstName = item.Firstname,
                        LastName = item.Lastname,
                        Salutation = item.Salutation,
                        AccountId = item.Accountid,
                        Roleid = item.Roleid
                    });
                }
                int role = 0;
                if (responce.Role.Count() == 0)
                {
                    role = _roleManagement.DeleteRole(request.RoleID, request.OrganizationId).Result;
                    return await Task.FromResult(new DeleteRoleResponce
                    {
                        Message = role.ToString(),
                        Code = Responcecode.Success
                    });
                }
                else
                {
                    responce.Message = "Role_in_use";
                    responce.Code = Responcecode.Assigned;
                    return await Task.FromResult(responce);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new DeleteRoleResponce
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
                RoleFilter objroleFilter = new RoleFilter();
                RoleListResponce objroleList = new RoleListResponce();

                objroleFilter.AccountId = request.AccountId;
                objroleFilter.RoleId = request.RoleId;
                objroleFilter.Organization_Id = request.OrganizationId;
                objroleFilter.State = request.Active ? "A" : "I";
                objroleFilter.IsGlobal = request.IsGlobal;
                objroleFilter.LangaugeCode = request.LangaugeCode;

                var role = _roleManagement.GetRoles(objroleFilter).Result;
                foreach (var item in role)
                {
                    RoleRequest objResponce = new RoleRequest();
                    objResponce.RoleID = item.Id;
                    objResponce.OrganizationId = item.Organization_Id == null ? 0 : item.Organization_Id.Value;
                    objResponce.RoleName = item.Name;
                    objResponce.CreatedBy = item.Created_by;
                    objResponce.CreatedAt = item.Created_at;
                    //ObjResponce.= item.Is_Active;
                    objResponce.Description = item.Description ?? "";
                    //ObjResponce.Roletype= item.Organization_Id == null ? RoleTypes.Global : RoleTypes.Regular;
                    objResponce.FeatureIds.Add(item.FeatureSet.Features.Select(I => I.Id).ToArray());
                    objResponce.Level = item.Level;
                    objResponce.Code = item.Code;
                    objroleList.Roles.Add(objResponce);
                }

                objroleList.Message = "Roles data retrieved";
                objroleList.Code = Responcecode.Success;
                return await Task.FromResult(objroleList);
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
