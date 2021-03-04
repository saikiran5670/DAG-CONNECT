using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VehicleBusinessService = net.atos.daf.ct2.vehicleservice;
using net.atos.daf.ct2.portalservice.Entity.Vehicle;

namespace net.atos.daf.ct2.portalservice.Controllers
{
    [ApiController]
    [Route("vehicle")]
    public class VehicleController : Controller
    {
        private readonly ILogger<VehicleController> _logger;
        private readonly VehicleBusinessService.VehicleService.VehicleServiceClient _vehicleClient;
        private readonly Mapper _mapper;
        private string FK_Constraint = "violates foreign key constraint";
        private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";

        public VehicleController(ILogger<VehicleController> logger, VehicleBusinessService.VehicleService.VehicleServiceClient vehicleClient)
        {
            _logger = logger;
            _vehicleClient = vehicleClient;
            _mapper = new Mapper();
        }


        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(VehicleCreateRequest request)
        {
            try
            {
                _logger.LogInformation("Create method in vehicle API called.");


                if (string.IsNullOrEmpty(request.Name))
                {
                    return StatusCode(401, "invalid Vehicle Name: The Vehicle Name is Empty.");
                }
                if (string.IsNullOrEmpty(request.License_Plate_Number))
                {
                    return StatusCode(401, "invalid Vehicle License Plate Number: The Vehicle License Plate Number is Empty.");
                }
                if (string.IsNullOrEmpty(request.VIN))
                {
                    return StatusCode(401, "invalid Vehicle VIN: The Vehicle VIN is Empty.");
                }

                var vehicleRequest = _mapper.ToVehicleCreate(request);
                VehicleBusinessService.VehicleCreateResponce vehicleResponse = await _vehicleClient.CreateAsync(vehicleRequest);
                var response = _mapper.ToVehicleCreate(vehicleResponse.Vehicle);

                 if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                    && vehicleResponse.Message == "There is an error creating account.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "accountResponse is null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Account Service:Create : " + ex.Message + " " + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }




        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update(VehicleRequest request)
        {
            try
            {
                _logger.LogInformation("Create method in vehicle API called.");

                // Validation 
                if (request.ID <= 0)
                {
                    return StatusCode(400, "The VehicleId is required.");
                }
                var vehicleRequest = _mapper.ToVehicle(request);
                VehicleBusinessService.VehicleResponce vehicleResponse = await _vehicleClient.UpdateAsync(vehicleRequest);
                var response = _mapper.ToVehicle(vehicleResponse.Vehicle);

                if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Failed
                     && vehicleResponse.Message == "There is an error updating vehicle.")
                {
                    return StatusCode(500, "There is an error creating account.");
                }
                else if (vehicleResponse != null && vehicleResponse.Code == VehicleBusinessService.Responcecode.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(500, "accountResponse is null");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:Create : " + ex.Message + " " + ex.StackTrace);
                // check for fk violation
                if (ex.Message.Contains(FK_Constraint))
                {
                    return StatusCode(500, "Internal Server Error.(01)");
                }
                // check for fk violation
                if (ex.Message.Contains(SocketException))
                {
                    return StatusCode(500, "Internal Server Error.(02)");
                }
                return StatusCode(500, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
