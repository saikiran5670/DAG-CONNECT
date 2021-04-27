using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.geofenceservice;
using net.atos.daf.ct2.poigeofence;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofenceservice.entity;


namespace net.atos.daf.ct2.poigeofenceservice.Services
{
    public class GroupManagementService : GroupService.GroupServiceBase
    {
        private ILog _logger;
        private readonly ILandmarkGroupManager _landmarkGroupManager;
        private readonly Mapper _mapper;

        public GroupManagementService(ILandmarkGroupManager landmarkGroupManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _landmarkGroupManager = landmarkGroupManager;
            _mapper = new Mapper();
        }

        public override async Task<GroupAddResponse> Create(GroupAddRequest request, ServerCallContext context)
        {
            GroupAddResponse response = new GroupAddResponse();
            try
            {
                _logger.Info("Add Group.");
                LandmarkGroup obj = new LandmarkGroup();
                obj.organization_id = request.OrganizationId;
                obj.name = request.Name;
                obj.description = request.Description;

                obj.created_by = request.CreatedBy;
                obj.state = request.State;
                obj.created_by = request.CreatedBy;
                obj.poilist = new List<POI>();
                foreach (var item in request.PoiIds)
                {                    
                    POI pOI = new POI();
                    pOI.Id = item.Poiid;
                    pOI.Type = item.Type;
                    obj.poilist.Add(pOI);
                }
                //Check if group allready exists
                var groupid = await _landmarkGroupManager.Exists(obj);
                if (groupid > 0)
                {
                    response.Message = "Group name allready exists";
                    response.Code = Responcecodes.Conflict;
                }
                else
                {
                    var result = await _landmarkGroupManager.CreateGroup(obj);
                    if (result != null)
                    {
                        response.Message = "Group Added : " + result.id.ToString();
                        response.Code = Responcecodes.Success;
                    }
                    else
                    {
                        response.Message = "Add group fail";
                        response.Code = Responcecodes.Failed;
                    }
                }
                

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                //response.Message = "Not Deleted";
                response.Message = ex.ToString();
                response.Code = Responcecodes.Failed;
            }
            return await Task.FromResult(response);
        }

        public override async Task<GroupUpdateResponse> Update(GroupUpdateRequest request, ServerCallContext context)
        {
            GroupUpdateResponse response = new GroupUpdateResponse();
            try
            {
                _logger.Info("Update Group.");
                LandmarkGroup obj = new LandmarkGroup();
                obj.id = request.Id;
                obj.name = request.Name;
                obj.description = request.Description;
                obj.modified_by = request.ModifiedBy;
                obj.poilist = new List<POI>();
                foreach (var item in request.PoiIds)
                {
                    POI pOI = new POI();
                    pOI.Id = item.Poiid;
                    pOI.Type = item.Type;

                    obj.poilist.Add(pOI);
                }
                //Check if group allready exists
                var groupid = await _landmarkGroupManager.Exists(obj);
                if (groupid > 0)
                {
                    response.Message = "Group name allready exists";
                    response.Code = Responcecodes.Conflict;
                }
                else
                {
                    var result = await _landmarkGroupManager.UpdateGroup(obj);
                    if (result != null)
                    {
                        response.Message = "Updated successfully : " + result.id.ToString();
                        response.Code = Responcecodes.Success;
                    }
                    else
                    {
                        response.Message = "Update group fail";
                        response.Code = Responcecodes.Failed;
                    }
                }
                

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                //response.Message = "Not Deleted";
                response.Message = ex.ToString();
                response.Code = Responcecodes.Failed;
            }
            return await Task.FromResult(response);
        }

        public override async Task<GroupDeleteResponse> Delete(GroupDeleteRequest request, ServerCallContext context)
        {
            GroupDeleteResponse response = new GroupDeleteResponse();
            try
            {
                _logger.Info("Delete Group.");
                LandmarkGroup obj = new LandmarkGroup();
                var result = await _landmarkGroupManager.DeleteGroup(request.Id,request.Modifiedby);
                if (result > 0)
                {
                    response.Message = "Deleted successfully : " + result.ToString(); 
                    response.Code = Responcecodes.Success;
                }
                else
                {
                    response.Message = "Delete group fail";
                    response.Code = Responcecodes.Failed;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                //response.Message = "Not Deleted";
                response.Message = ex.ToString();
                response.Code = Responcecodes.Failed;
            }
            return await Task.FromResult(response);
        }

        public override async Task<GroupGetResponse> Get(GroupGetRequest request, ServerCallContext context)
        {
            GroupGetResponse response = new GroupGetResponse();
            try
            {
                _logger.Info("Get Group.");
                var result = await _landmarkGroupManager.GetlandmarkGroup(request.OrganizationsId, request.GroupId);
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        Group obj = new Group();
                        obj.Id = item.id;
                        obj.OrganizationId = item.organization_id;
                        obj.Name = item.name;
                        obj.CreatedAt = item.created_at;
                        obj.ModifiedAt = item.modified_at;                        
                        obj.PoiCount = item.poiCount;
                        obj.GeofenceCount = item.geofenceCount;
                        response.Groups.Add(obj);
                    }
                    response.Message = "Get success";
                    response.Code = Responcecodes.Success;
                }
                else
                {
                    response.Message = "Get group fail";
                    response.Code = Responcecodes.Failed;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                response.Message = ex.ToString();
                response.Code = Responcecodes.Failed;
            }
            return await Task.FromResult(response);
        }
    }
}
