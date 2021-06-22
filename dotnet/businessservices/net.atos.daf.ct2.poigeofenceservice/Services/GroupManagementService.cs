using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Google.Protobuf;
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
        private readonly ILog _logger;
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
                obj.Organization_id = request.OrganizationId;
                obj.Name = request.Name;
                obj.Description = request.Description;

                obj.Created_by = request.CreatedBy;
                obj.State = request.State;
                obj.Created_by = request.CreatedBy;
                obj.PoiList = new List<POI>();
                foreach (var item in request.PoiIds)
                {
                    POI pOI = new POI();
                    pOI.Id = item.Poiid;
                    pOI.Type = item.Type;
                    obj.PoiList.Add(pOI);
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
                        response.Message = "Group Added : " + result.Id.ToString();
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
                obj.Id = request.Id;
                obj.Name = request.Name;
                obj.Description = request.Description;
                obj.Modified_by = request.ModifiedBy;
                obj.PoiList = new List<POI>();
                foreach (var item in request.PoiIds)
                {
                    POI pOI = new POI();
                    pOI.Id = item.Poiid;
                    pOI.Type = item.Type;

                    obj.PoiList.Add(pOI);
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
                        response.Message = "Updated successfully : " + result.Id.ToString();
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
                var result = await _landmarkGroupManager.DeleteGroup(request.Id, request.Modifiedby);
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
                        obj.Id = item.Id;
                        obj.OrganizationId = item.Organization_id;
                        obj.Name = item.Name;
                        obj.Name = item.Name;
                        obj.Description = item.Description ?? "";
                        obj.CreatedAt = item.Created_at;
                        obj.ModifiedAt = item.Modified_at;
                        obj.PoiCount = item.PoiCount;
                        obj.GeofenceCount = item.GeofenceCount;
                        if (request.GroupId > 0)
                        {
                            foreach (var grp in item.PoiList)
                            {
                                Landmarkdetails objlandmarkdetails = new Landmarkdetails();
                                objlandmarkdetails.Landmarkid = grp.Id;
                                objlandmarkdetails.Landmarkname = grp.Name;
                                objlandmarkdetails.Categoryname = grp.CategoryName;
                                objlandmarkdetails.Subcategoryname = grp.SubCategoryName;
                                objlandmarkdetails.Address = grp.Address;
                                objlandmarkdetails.Icon = ByteString.CopyFrom(grp.Icon);
                                objlandmarkdetails.Type = grp.Type.ToString();
                                obj.Landmarks.Add(objlandmarkdetails);
                            }
                        }
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
