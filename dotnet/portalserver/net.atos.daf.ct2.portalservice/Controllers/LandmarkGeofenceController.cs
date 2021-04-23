using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.poigeofenceservice;
using net.atos.daf.ct2.portalservice.Common;
using System.Threading.Tasks;
using net.atos.daf.ct2.portalservice.Entity.POI;
using System;
using net.atos.daf.ct2.portalservice.Entity.Geofence;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("geofence")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class LandmarkGeofenceController : ControllerBase
    {
        private readonly PoiGeofenceService.PoiGeofenceServiceClient _poiGeofenceServiceClient;
        private readonly AuditHelper _auditHelper;
        private readonly Mapper _mapper;
        public LandmarkGeofenceController(PoiGeofenceService.PoiGeofenceServiceClient poiGeofenceServiceClient, AuditHelper auditHelper)
        {
            _poiGeofenceServiceClient = poiGeofenceServiceClient;
            _auditHelper = auditHelper;
            _mapper = new Mapper();
        }

        #region Geofence

        //[HttpPut]
        //[Route("update")]
        //public async Task<IActionResult> CreateGeofence(Geofence request)
        //{

        //    try
        //    {
        //       // _logger.Info("Update method in vehicle API called.");

        //        // Validation 
        //        if (string.IsNullOrEmpty(request.Name))
        //        {
        //            return StatusCode(400, "The Geofence name is required.");
        //        }
        //        var geofenceRequest = new poigeofenceservice.GeofenceRequest();
        //        geofenceRequest = _mapper.ToGeofenceRequest(request);
        //        poigeofenceservice.GeofenceResponse geofenceResponse = await _poiGeofenceServiceClient.CreateGeofenceAsync(geofenceRequest);
        //        ///var response = _mapper.ToVehicle(vehicleResponse.Vehicle);

        //        if (geofenceResponse != null && geofenceResponse.Code == geofenceResponse.Responcecode.Failed
        //             && geofenceResponse.Message == "There is an error updating vehicle.")
        //        {
        //            return StatusCode(500, "There is an error creating account.");
        //        }
        //        else if (geofenceResponse != null && geofenceResponse.Code == geofenceResponse.Responcecode.Conflict)
        //        {
        //            return StatusCode(409, geofenceResponse.Message);
        //        }
        //        else if (geofenceResponse != null && geofenceResponse.Code == geofenceResponse.Responcecode.Success)
        //        {


        //            await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
        //          "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.SUCCESS,
        //          "Update  method in Vehicle controller", request.ID, request.ID, JsonConvert.SerializeObject(request),
        //           Request);

        //            return Ok(response);
        //        }
        //        else
        //        {
        //            return StatusCode(500, "vehicleResponse is null");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        await _auditHelper.AddLogs(DateTime.Now, DateTime.Now, "Vehicle Component",
        //         "Vehicle service", Entity.Audit.AuditTrailEnum.Event_type.UPDATE, Entity.Audit.AuditTrailEnum.Event_status.FAILED,
        //         "Update  method in Vehicle controller", request.ID, request.ID, JsonConvert.SerializeObject(request),
        //          Request);
        //        //_logger.Error(null, ex);
        //        // check for fk violation
        //        if (ex.Message.Contains(FK_Constraint))
        //        {
        //            return StatusCode(500, "Internal Server Error.(01)");
        //        }
        //        // check for fk violation
        //        if (ex.Message.Contains(SocketException))
        //        {
        //            return StatusCode(500, "Internal Server Error.(02)");
        //        }
        //        return StatusCode(500, ex.Message + " " + ex.StackTrace);
        //    }
        //}



        #endregion

    }
}
