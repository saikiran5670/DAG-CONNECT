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
using net.atos.daf.ct2.featureservice;
using net.atos.daf.ct2.portalservice.Account;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.Feature;
using net.atos.daf.ct2.portalservice.Entity.Role;
using net.atos.daf.ct2.roleservice;
using RoleBusinessService = net.atos.daf.ct2.roleservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("role")]
    public class RoleController : Controller
    {

        #region Private Variable
        private readonly ILogger<RoleController> _logger;
        private readonly RoleBusinessService.RoleService.RoleServiceClient _roleclient;
        private readonly Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";

        #endregion

        #region Constructor
        public RoleController(RoleBusinessService.RoleService.RoleServiceClient Featureclient, ILogger<RoleController> logger)
        {
            _roleclient = Featureclient;
            _logger = logger;
            _mapper = new Mapper();
        }
        #endregion

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Rolerequest request)
        {
            try
            {

                if ((string.IsNullOrEmpty(request.RoleName.Trim())))
                {
                    return StatusCode(400, "Role name  is required");
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

                //ObjRole.FeatureSet.Features = new List<Feature>();
                foreach (var item in request.FeatureIds)
                {
                    ObjRole.FeatureIds.Add(item);
                }
                var role = await _roleclient.CreateAsync(ObjRole);
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError("Role Service:Create : " + ex.Message + " " + ex.StackTrace);
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

            if ((string.IsNullOrEmpty(roleMaster.RoleName.Trim())) || (roleMaster.RoleId == 0))
            {
                return StatusCode(400, "Role name and Role Id required Roleid required");
            }
            if (roleMaster.FeatureIds.Length == 0)
            {
                return StatusCode(400, "Feature Ids required.");
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
                _logger.LogInformation(role.Message + "Role Master Updated");
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
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
            try
            {
                if (roleId == 0)
                {
                    return StatusCode(400, "Role id required ");
                }
                RoleRequest ObjRole = new RoleRequest();
                ObjRole.RoleID = roleId;
                ObjRole.UpdatedBy = updatedby;
                var role_Id = await _roleclient.DeleteAsync(ObjRole);
                //auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Delete method in Role manager", roleId, roleId, JsonConvert.SerializeObject(roleId));
                return Ok(role_Id);
            }
            catch (Exception ex)
            {
                //await auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Role Component", "Role Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.FAILED, "Create method in Role manager", 0, 0, JsonConvert.SerializeObject(roleId));
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get(int Roleid, int? Organizationid, bool IsGlobal)
        {
            try
            {
                if (string.IsNullOrEmpty(IsGlobal.ToString()))
                {
                    return StatusCode(400, "Is global role filter is required");
                }

                RoleFilterRequest obj = new RoleFilterRequest();
                obj.RoleId = Roleid;
                obj.OrganizationId = Organizationid == null ? 0 : Convert.ToInt32(Organizationid);
                obj.IsGlobal = IsGlobal;
                if (obj.OrganizationId == 0 && IsGlobal == false && obj.RoleId <= 0)
                {
                    return StatusCode(400, "Organization id required ");
                }

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
                    Robj.FeatureIds = roleitem.FeatureIds.ToArray();
                    roleList.Add(Robj);
                }


                return Ok(roleList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
