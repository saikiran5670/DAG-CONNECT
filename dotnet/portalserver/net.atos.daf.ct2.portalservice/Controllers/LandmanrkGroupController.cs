using System;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.geofenceservice;
using net.atos.daf.ct2.portalservice.Common;
using net.atos.daf.ct2.portalservice.Entity.POI;
using Newtonsoft.Json;
using net.atos.daf.ct2.alertservice;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("landmarkgroup")]
    public class LandmanrkGroupController : BaseController
    {
        private readonly ILog _logger;
        private readonly GroupService.GroupServiceClient _groupServiceclient;
        private readonly AuditHelper _auditHelper;
        private readonly Entity.POI.Mapper _mapper;
        private readonly string _fK_Constraint = "violates foreign key constraint";
        private readonly AlertService.AlertServiceClient _alertServiceClient;
        public LandmanrkGroupController(GroupService.GroupServiceClient groupService, AuditHelper auditHelper, SessionHelper sessionHelper, AlertService.AlertServiceClient alertServiceClient, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor, sessionHelper)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _groupServiceclient = groupService;
            _auditHelper = auditHelper;
            _mapper = new Mapper();
            _alertServiceClient = alertServiceClient;


        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(LandmarkGroup request)
        {
            try
            {
                _logger.Info("Add Group.");
                GroupAddRequest objgroup = new GroupAddRequest();
                objgroup.OrganizationId = GetContextOrgId();
                objgroup.Name = request.Name;
                objgroup.Description = request.Description;
                objgroup.CreatedBy = request.CreatedBy;
                objgroup.State = request.State;
                if (request.OrganizationId == 0)
                {
                    return StatusCode(400, "Organization id is required");
                }
                foreach (var item in request.Poilist)
                {
                    PoiId pOI = new PoiId();
                    if (item.ID == 0)
                    {
                        return StatusCode(400, "Poi id is required");
                    }
                    pOI.Poiid = item.ID;

                    pOI.Type = _mapper.Maplandmarktype(item.Type).ToString();
                    if (pOI.Type == "None")
                    {
                        return StatusCode(400, "Invalid POI type");
                    }

                    objgroup.PoiIds.Add(pOI);
                }
                var result = await _groupServiceclient.CreateAsync(objgroup);

                if (result != null && result.Code == Responcecodes.Conflict)
                {
                    return StatusCode(409, result.Message);
                }
                else if (result != null && result.Code == Responcecodes.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "POI Component",
                    "LandmarkGroup service", Entity.Audit.AuditTrailEnum.Event_type.CREATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "Create method in Group controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                    _userDetails);
                    return Ok(result);
                }
                else
                {
                    if (result.Message.Contains(_fK_Constraint))
                    {
                        _logger.Error(result);
                        return StatusCode(500, _fK_Constraint);

                    }
                    else
                    {
                        _logger.Error(result);
                        return StatusCode(500, "Error in group create");

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Error in landmark group creation");
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(LandmarkGroup request)
        {
            //GroupAddRequest objgroup = new GroupAddRequest();
            try
            {
                if (request.Id == 0)
                {
                    return StatusCode(400, "Group ID is required");
                }
                if (request.Poilist.Count < 1)
                {
                    return StatusCode(400, "POI List is required.");
                }
                _logger.Info("Update Group.");

                GroupUpdateRequest objgroup = new GroupUpdateRequest();
                objgroup.Id = request.Id;
                objgroup.Name = request.Name;
                objgroup.Description = request.Description;
                foreach (var item in request.Poilist)
                {
                    PoiId pOI = new PoiId();
                    if (item.ID == 0)
                    {
                        return StatusCode(400, "Poi id is required");
                    }
                    pOI.Poiid = item.ID;
                    pOI.Type = _mapper.Maplandmarktype(item.Type).ToString();
                    if (pOI.Type == "None")
                    {
                        return StatusCode(400, "Invalid POI type");
                    }

                    objgroup.PoiIds.Add(pOI);
                }
                var result = await _groupServiceclient.UpdateAsync(objgroup);
                if (result != null && result.Code == Responcecodes.Conflict)
                {
                    return StatusCode(409, result.Message);
                }

                if (result != null && result.Code == Responcecodes.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "POI Component",
                    "LandmarkGroup service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "Update method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                    _userDetails);
                    return Ok(result);
                }
                else
                {
                    if (result.Message.Contains(_fK_Constraint))
                    {
                        _logger.Error(result);
                        return StatusCode(500, _fK_Constraint);
                    }
                    else
                    {
                        _logger.Error(result);
                        return StatusCode(500, "Error in group create");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Error in landmark group update");
            }
        }


        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(int GroupId, int modifiedby)
        {
            //GroupAddRequest objgroup = new GroupAddRequest();
            try
            {
                if (GroupId == 0)
                {
                    return StatusCode(400, "Group ID is required");
                }
                _logger.Info("Delete Group.");

                LandmarkIdRequest landmarkIdRequest = new LandmarkIdRequest();
                landmarkIdRequest.LandmarkId.Add(GroupId);
                landmarkIdRequest.LandmarkType = "G";

                LandmarkIdExistResponse isLandmarkavalible = await _alertServiceClient.IsLandmarkActiveInAlertAsync(landmarkIdRequest);

                if (isLandmarkavalible.IsLandmarkActive)
                {
                    return StatusCode(409, "Corridor is used in alert.");
                }

                GroupDeleteRequest objgroup = new GroupDeleteRequest();
                objgroup.Id = GroupId;
                objgroup.Modifiedby = modifiedby;


                var result = await _groupServiceclient.DeleteAsync(objgroup);

                if (result != null && result.Code == Responcecodes.Failed)
                {
                    return StatusCode(409, result.Message);
                }
                else if (result != null && result.Code == Responcecodes.Success)
                {
                    await _auditHelper.AddLogs(DateTime.Now, "POI Component",
                    "LandmarkGroup service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    "Delete method in POI controller", GroupId, GroupId, JsonConvert.SerializeObject(GroupId),
                    _userDetails);
                    return Ok(result);
                }
                else
                {
                    _logger.Error(result);
                    return StatusCode(404, "Group responce is null");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Error in landmark group delete");
            }
        }


        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get(int groupid, int organizationid)
        {
            //GroupAddRequest objgroup = new GroupAddRequest();
            try
            {
                if (groupid == 0 && organizationid == 0)
                {
                    return StatusCode(400, "Group or organization id is required");
                }
                GroupGetRequest objgroup = new GroupGetRequest();
                objgroup.GroupId = groupid;
                objgroup.OrganizationsId = GetContextOrgId();


                var result = await _groupServiceclient.GetAsync(objgroup);

                if (result != null && result.Code == Responcecodes.Failed)
                {
                    return StatusCode(409, result.Message);
                }
                else if (result != null && result.Code == Responcecodes.Success)
                {
                    //await _auditHelper.AddLogs(DateTime.Now, "POI Component",
                    //"POI service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
                    //"Create method in POI controller", request.Id, request.Id, JsonConvert.SerializeObject(request),
                    //Request);
                    if (result.Groups.Count > 0)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return StatusCode(404, "Group details not found");
                    }

                }
                else
                {
                    return StatusCode(404, "Group responce is null");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return StatusCode(500, "Error in landmark group get");
            }
        }
    }
}
