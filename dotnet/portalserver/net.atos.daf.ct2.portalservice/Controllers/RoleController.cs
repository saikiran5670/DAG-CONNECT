using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;
//using net.atos.daf.ct2.portalservice.Entity.Feature;
using net.atos.daf.ct2.portalservice.Entity.Role;
using net.atos.daf.ct2.roleservice;
using Newtonsoft.Json;
using RoleBusinessService = net.atos.daf.ct2.roleservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("role")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class RoleController : BaseController
    {
        #region Private Variable
        private readonly RoleService.RoleServiceClient _roleclient;
        private readonly Mapper _mapper;
        private readonly ILog _logger;
        private readonly string _fk_Constraint = "violates foreign key constraint";
        private readonly AuditHelper _auditHelper;

        #endregion

        #region Constructor
        public RoleController(RoleBusinessService.RoleService.RoleServiceClient roleclient, AuditHelper auditHelper, IHttpContextAccessor httpContextAccessor, SessionHelper sessionHelper) : base(httpContextAccessor, sessionHelper)
        {
            _roleclient = roleclient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _mapper = new Mapper();
            _auditHelper = auditHelper;
        }
        #endregion

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Rolerequest request)
        {
            try
            {
                //Assign context orgId
                if (request.OrganizationId > 0)
                {
                    request.OrganizationId = GetContextOrgId();
                }


                if ((string.IsNullOrEmpty(request.RoleName)))
                {
                    return StatusCode(400, "Role name  is required");
                }
                if ((string.IsNullOrEmpty(request.Code)))
                {
                    request.Code = "PLADM";
                }
                //Commenting out this check for bug no.6210
                //if (request.FeatureIds.Length == 0)
                //{
                //    return StatusCode(400, "Feature Ids are required.");
                //}
                //int Rid = _roleclient.CheckRoleNameExist(roleMaster.RoleName.Trim(), roleMaster.OrganizationId, 0);
                RoleRequest ObjRole = new RoleRequest();
                ObjRole.OrganizationId = request.OrganizationId;
                ObjRole.RoleName = request.RoleName;
                ObjRole.CreatedBy = request.Createdby;
                ObjRole.Description = request.Description;
                //ObjRole.Fea = 0;
                ObjRole.Level = request.Level;
                ObjRole.Code = request.Code;

                //ObjRole.FeatureSet.Features = new List<Feature>();
                foreach (var item in request.FeatureIds)
                {
                    ObjRole.FeatureIds.Add(item);
                }
                var role = await _roleclient.CreateAsync(ObjRole);


                await _auditHelper.AddLogs(DateTime.Now, "Role Component",
                        "Role service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                        "Create  method in Role controller", 0, request.RoleId, JsonConvert.SerializeObject(request),
                         _userDetails);

                return Ok(role);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Role Component",
                       "Role service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                       "Create  method in Role controller", 0, request.RoleId, JsonConvert.SerializeObject(request),
                        _userDetails);
                _logger.Error(null, ex);
                if (ex.Message.Contains(_fk_Constraint))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Please contact system administrator. ");
            }
        }


        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(Roleupdaterequest roleMaster)
        {

            if (string.IsNullOrEmpty(roleMaster.RoleName) || roleMaster.RoleId == 0)
            {
                return StatusCode(400, "Role name and Role Id required Roleid required");
            }
            // commenting for bug 6210
            //if (roleMaster.FeatureIds.Length == 0)
            //{
            //    return StatusCode(400, "Feature Ids required.");
            //}
            //context org id is only set when role id is different
            //Assign context orgId
            roleMaster.OrganizationId = AssignOrgContextByRoleId(roleMaster.RoleId);

            try
            {
                RoleRequest ObjRole = new RoleRequest();
                //RoleEntity.RoleMaster ObjRole = new RoleEntity.RoleMaster();
                ObjRole.RoleName = roleMaster.RoleName.Trim();
                ObjRole.Description = roleMaster.Description;
                ObjRole.RoleID = roleMaster.RoleId;
                if ((ObjRole.Description != null) && (!string.IsNullOrEmpty(ObjRole.Description)))
                    ObjRole.Description = roleMaster.Description.Trim();
                else ObjRole.Description = string.Empty;

                ObjRole.UpdatedBy = roleMaster.Updatedby;
                //ObjRole.FeatureSet = new FeatureSet();
                //ObjRole.FeatureSet.Features = new List<Feature>();
                foreach (var item in roleMaster.FeatureIds)
                {
                    ObjRole.FeatureIds.Add(item);
                }
                var role = await _roleclient.UpdateAsync(ObjRole);
                //auditlog.AddLogs(DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Create method in Role manager", roleMaster.RoleId, 0, JsonConvert.SerializeObject(roleMaster));
                _logger.Info(role.Message + "Role Master Updated");
                await _auditHelper.AddLogs(DateTime.Now, "Role Component",
                     "Role service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     "Update  method in Role controller", roleMaster.RoleId, roleMaster.RoleId, JsonConvert.SerializeObject(roleMaster),
                      _userDetails);
                return Ok(role);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Role Component",
                   "Role service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                   "Update  method in Role controller", roleMaster.RoleId, roleMaster.RoleId, JsonConvert.SerializeObject(roleMaster),
                    _userDetails);
                _logger.Error(null, ex);
                //auditlog.AddLogs(DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "Create method in Role manager", roleMaster.RoleId, 0, JsonConvert.SerializeObject(roleMaster));
                if (ex.Message.Contains("foreign key"))
                {
                    return StatusCode(400, "The foreign key violation in one of dependant data.");
                }
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(int roleId, int updatedby)
        {
            RoleRequest ObjRole = new RoleRequest();
            try
            {
                if (roleId == 0)
                {
                    return StatusCode(400, "Role id required ");
                }

                ObjRole.RoleID = roleId;
                ObjRole.UpdatedBy = updatedby;
                var role_Id = await _roleclient.DeleteAsync(ObjRole);
                if (role_Id.Code == Responcecode.Assigned)
                {
                    return StatusCode(400, role_Id);
                }
                //auditlog.AddLogs(DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Delete method in Role manager", roleId, roleId, JsonConvert.SerializeObject(roleId));
                await _auditHelper.AddLogs(DateTime.Now, "Role Component",
                     "Role service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     "Delete  method in Role controller", ObjRole.RoleID, ObjRole.RoleID, JsonConvert.SerializeObject(ObjRole),
                      _userDetails);
                return Ok(role_Id);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, "Role Component",
                    "Role service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                    "Delete  method in Role controller", ObjRole.RoleID, ObjRole.RoleID, JsonConvert.SerializeObject(ObjRole),
                     _userDetails);
                //await auditlog.AddLogs(DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.FAILED, "Create method in Role manager", 0, 0, JsonConvert.SerializeObject(roleId));
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpGet]
        [Route("get")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "It has to be fixed while clean up of Organization Id related code")]
        public async Task<IActionResult> Get(int Roleid, int? Organizationid, bool IsGlobal, string LangaugeCode)
        {
            try
            {
                if (string.IsNullOrEmpty(IsGlobal.ToString()))
                {
                    return StatusCode(400, "Is global role filter is required");
                }
                //Assign context orgId
                Organizationid = GetContextOrgId();
                RoleFilterRequest obj = new RoleFilterRequest();
                obj.RoleId = Roleid;
                obj.OrganizationId = Organizationid == null ? 0 : Convert.ToInt32(Organizationid);
                obj.IsGlobal = IsGlobal;
                if (obj.OrganizationId == 0 && IsGlobal == false && obj.RoleId <= 0)
                {
                    return StatusCode(400, "Organization id required ");
                }
                obj.LangaugeCode = (LangaugeCode == null || LangaugeCode == "") ? "EN-GB" : LangaugeCode;

                var role = await _roleclient.GetAsync(obj);
                List<Rolerequest> roleList = new List<Rolerequest>();
                foreach (var roleitem in role.Roles)
                {
                    Rolerequest Robj = new Rolerequest();
                    Robj.RoleId = roleitem.RoleID;
                    Robj.RoleName = roleitem.RoleName;
                    Robj.Description = roleitem.Description;
                    Robj.OrganizationId = roleitem.OrganizationId;
                    Robj.Level = roleitem.Level;
                    Robj.CreatedAt = roleitem.CreatedAt;
                    Robj.Code = roleitem.Code;
                    Robj.FeatureIds = roleitem.FeatureIds.ToArray();
                    roleList.Add(Robj);
                }


                return Ok(roleList);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
