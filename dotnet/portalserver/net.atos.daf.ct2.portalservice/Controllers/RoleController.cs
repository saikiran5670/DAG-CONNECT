using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
//using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;
//using net.atos.daf.ct2.portalservice.Entity.Feature;
using net.atos.daf.ct2.portalservice.Entity.Role;
using net.atos.daf.ct2.roleservice;
using RoleBusinessService = net.atos.daf.ct2.roleservice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using log4net;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("role")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class RoleController : Controller
    {

        #region Private Variable
        //private readonly ILogger<RoleController> _logger;
        private readonly RoleBusinessService.RoleService.RoleServiceClient _roleclient;
        private readonly Mapper _mapper;

        private ILog _logger;
        private string FK_Constraint = "violates foreign key constraint";
        private readonly AuditHelper _auditHelper;
        private readonly SessionHelper _sessionHelper;
        private readonly HeaderObj _userDetails;
        #endregion

        #region Constructor
        public RoleController(RoleBusinessService.RoleService.RoleServiceClient roleclient, AuditHelper auditHelper, IHttpContextAccessor _httpContextAccessor, SessionHelper sessionHelper)
        {
            _roleclient = roleclient;
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _mapper = new Mapper();
            _sessionHelper = sessionHelper;
            _userDetails = _sessionHelper.GetSessionInfo(_httpContextAccessor.HttpContext.Session);
            _auditHelper = auditHelper;
            _userDetails = _auditHelper.GetHeaderData(_httpContextAccessor.HttpContext.Request);
        }
        #endregion

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Rolerequest request)
        {
            try
            {
                //Assign context orgId
                request.OrganizationId = _userDetails.contextOrgId;

                if ((string.IsNullOrEmpty(request.RoleName)))
                {
                    return StatusCode(400, "Role name  is required");
                }
                if ((string.IsNullOrEmpty(request.Code)))
                {
                    request.Code = "PLADM";
                }
                if (request.FeatureIds.Length == 0)
                {
                    return StatusCode(400, "Feature Ids are required.");
                }
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


                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Role Component",
                        "Role service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                        "Create  method in Role controller", 0, request.RoleId, JsonConvert.SerializeObject(request),
                         Request);

                return Ok(role);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Role Component",
                       "Role service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                       "Create  method in Role controller", 0, request.RoleId, JsonConvert.SerializeObject(request),
                        Request);
                _logger.Error(null, ex);
                if (ex.Message.Contains(FK_Constraint))
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

            if ((string.IsNullOrEmpty(roleMaster.RoleName)) || (roleMaster.RoleId == 0))
            {
                return StatusCode(400, "Role name and Role Id required Roleid required");
            }
            if (roleMaster.FeatureIds.Length == 0)
            {
                return StatusCode(400, "Feature Ids required.");
            }
            //context org id is only set when role id is different
            //Assign context orgId
            if (roleMaster.RoleId != _userDetails.roleId)
            {
                roleMaster.OrganizationId = _userDetails.contextOrgId;
            }
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
                //auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Create method in Role manager", roleMaster.RoleId, 0, JsonConvert.SerializeObject(roleMaster));
                _logger.Info(role.Message + "Role Master Updated");
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Role Component",
                     "Role service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     "Update  method in Role controller", roleMaster.RoleId, roleMaster.RoleId, JsonConvert.SerializeObject(roleMaster),
                      Request);
                return Ok(role);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Role Component",
                   "Role service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                   "Update  method in Role controller", roleMaster.RoleId, roleMaster.RoleId, JsonConvert.SerializeObject(roleMaster),
                    Request);
                _logger.Error(null, ex);
                //auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.FAILED, "Create method in Role manager", roleMaster.RoleId, 0, JsonConvert.SerializeObject(roleMaster));
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
                //auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Delete method in Role manager", roleId, roleId, JsonConvert.SerializeObject(roleId));
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Role Component",
                     "Role service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                     "Delete  method in Role controller", ObjRole.RoleID, ObjRole.RoleID, JsonConvert.SerializeObject(ObjRole),
                      Request);
                return Ok(role_Id);
            }
            catch (Exception ex)
            {
                await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Role Component",
                    "Role service", Entity.Audit.AuditTrailEnum.Event_type.DELETE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
                    "Delete  method in Role controller", ObjRole.RoleID, ObjRole.RoleID, JsonConvert.SerializeObject(ObjRole),
                     Request);
                //await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.FAILED, "Create method in Role manager", 0, 0, JsonConvert.SerializeObject(roleId));
                _logger.Error(null, ex);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get(int Roleid, int? Organizationid, bool IsGlobal, string LangaugeCode)
        {
            try
            {
                if (string.IsNullOrEmpty(IsGlobal.ToString()))
                {
                    return StatusCode(400, "Is global role filter is required");
                }
                //Assign context orgId
                Organizationid = _userDetails.contextOrgId;
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
