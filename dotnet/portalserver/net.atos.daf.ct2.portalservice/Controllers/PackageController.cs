using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.packageservice;
using net.atos.daf.ct2.portalservice.Common;

namespace net.atos.daf.ct2.portalservice.Controllers
{
   
    [ApiController]
    [Route("package")]
    public class PackageController : ControllerBase
    {


       
        private readonly ILogger<PackageController> _logger;
        private readonly PackageService.PackageServiceClient _packageClient;
        
        //private string FK_Constraint = "violates foreign key constraint";
       // private string SocketException = "Error starting gRPC call. HttpRequestException: No connection could be made because the target machine actively refused it.";

        

       
        public PackageController(PackageService.PackageServiceClient packageClient, ILogger<PackageController> logger)
        {
            _packageClient = packageClient;
            _logger = logger;
           
        }



        //[HttpPost]
        //[Route("create")]
        //public async Task<IActionResult> Create(PackageCreateRequest request)
        //{
        //    try
        //    {
        //        // Validation 
        //        if ((string.IsNullOrEmpty(request.Code)) || (string.IsNullOrEmpty(request.Name))
        //        || (request.Features.Count==0) || !EnumValidator.ValidateAccountType((char)request.Type))
        //        {
        //            return StatusCode(400, "The Package code,name,type and features are required.");
        //        }
              
              
              
        //        var packageResponse = await _packageClient.CreateAsync(request);
        //        var response = _mapper.ToAccount(accountResponse.Account);

        //        if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Failed
        //            && accountResponse.Message == "The duplicate account, please provide unique email address.")
        //        {
        //            return StatusCode(409, response);

        //            //    return StatusCode(409, object);
        //            //    //{
        //            //    //    MessageExtensions = ""
        //            //    //        Account = {

        //            //    //    }
        //            //}

        //            // Object with message and 
        //            // Duplicate within organization. 
        //            // Duplicate across organization. 
        //            // Yes (Get by id and organization.)
        //        }
        //        else if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Failed
        //            && accountResponse.Message == "There is an error creating account.")
        //        {
        //            return StatusCode(500, "There is an error creating account.");
        //        }
        //        else if (accountResponse != null && accountResponse.Code == AccountBusinessService.Responcecode.Success)
        //        {
        //            return Ok(response);
        //        }
        //        else
        //        {
        //            return StatusCode(500, "accountResponse is null");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Account Service:Create : " + ex.Message + " " + ex.StackTrace);
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





    }
}
