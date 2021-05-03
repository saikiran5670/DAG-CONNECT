using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.audit;
using System.Collections.Generic;
using Grpc.Core;
using net.atos.daf.ct2.driver.entity;
using net.atos.daf.ct2.driver;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.driverservice.entity;
using System.Linq;
using log4net;
using System.Reflection;

namespace net.atos.daf.ct2.driverservice
{
    public class DriverManagementService:DriverService.DriverServiceBase
    {
                 
        private readonly IAuditTraillib _AuditTrail;      
        private readonly IAuditTraillib auditlog;     
        private readonly IDriverManager driverManager;        
        
        private readonly DriverMapper _mapper;

        private ILog _logger;

        public DriverManagementService( IAuditTraillib AuditTrail,IAuditTraillib _auditlog,IDriverManager _driverManager)
        {
           _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
           _AuditTrail = AuditTrail;         
           auditlog = _auditlog;
           driverManager=_driverManager;
           _mapper = new DriverMapper();
        }

        public override async Task<DriverDataList> Get(IdRequest request, ServerCallContext context)
        {
             try{
                _logger.Info("Get Drivers .");
                DriverDataList response = new DriverDataList();
                var result = await driverManager.GetDriver(request.OrgID,request.DriverID);
                if (result.Count() > 0)
                {
                    foreach (net.atos.daf.ct2.driver.entity.DriverResponse entity in result)
                    {
                        response.Driver.Add(_mapper.ToDriverResponse(entity));
                    }
                    response.Code = Responcecode.Success;
                    response.Message = "Get";
                }
                else
                {
                    response.Code = Responcecode.NotFound;
                    response.Message = "Driver not found.";
                }
                return await Task.FromResult(response);   
             }
             catch (Exception ex)
              {
                _logger.Error(null, ex);
                return await Task.FromResult(new DriverDataList
                {
                    Code = Responcecode.Failed,
                    Message = "Driver get faile due to - " + ex.Message
                    //Driver = null
                });
            } 
        }

        public override async Task<net.atos.daf.ct2.driverservice.DriverUpdateResponse> Update(net.atos.daf.ct2.driverservice.DriverUpdateRequest request, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.driverservice.DriverUpdateResponse response=new net.atos.daf.ct2.driverservice.DriverUpdateResponse();
               net.atos.daf.ct2.driverservice.DriverUpdateRequest objDriver=new  DriverUpdateRequest(); 
               driver.entity.Driver driver=new driver.entity.Driver();
               _logger.Info("Update Drivers ."); 
               driver= _mapper.ToDriverUpdateResponse(request);
               var result = await driverManager.UpdateDriver(driver);   
               
               objDriver= _mapper.DriverToDriverResponse(driver);  
               response.Code = Responcecode.Success;
               response.Message = "Updated";
               response.Driver= objDriver;
               return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
               _logger.Error(null, ex);
                return await Task.FromResult(new DriverUpdateResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Driver get faile due to - " + ex.Message
                });
            }
        }

        public override async Task<net.atos.daf.ct2.driverservice.DriverDeleteResponse> Delete(IdRequest request, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.driverservice.DriverDeleteResponse response=new driverservice.DriverDeleteResponse();
                _logger.Info("Delete Drivers ."); 
                bool result = await driverManager.DeleteDriver(request.OrgID,request.DriverID);  
                if(result)
                {
                        response.Message = "Deleted";
                        response.Code=Responcecode.Success;
                }
                 if(!result)
                 {
                        response.Message = "Not Deleted";  
                        response.Code=Responcecode.Failed; 
                 }                     
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
               _logger.Error(null, ex);
                return await Task.FromResult(new DriverDeleteResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Driver get faile due to - " + ex.Message
            });
            }
        }

        public override async Task<net.atos.daf.ct2.driverservice.OptOutOptInResponse> UpdateOptinOptout(OptOutOptInRequest Optrequest, ServerCallContext context)
        {
            try
            { 
                net.atos.daf.ct2.driverservice.OptOutOptInResponse response=new driverservice.OptOutOptInResponse();
                _logger.Info("OptOutOptIn Drivers ."); 
                bool result = await driverManager.UpdateOptinOptout(Optrequest.OrgID,Optrequest.Optoutoptinstatus);  
                if(result)
                {
                        response.Message = "Driver OptOutOptIn updated";
                        response.Code=Responcecode.Success;
                }
                 if(!result)
                 {
                        response.Message = "Driver OptOutOptIn Not updated"; 
                        response.Code=Responcecode.Failed; 
                 }                     
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
               _logger.Error(null, ex);
                return await Task.FromResult(new OptOutOptInResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Driver get faile due to - " + ex.Message
                });
            }
        }
        public override async Task<DriverImportData> ImportDrivers(DriverImportRequest request, ServerCallContext context)
        {
            try {     
            net.atos.daf.ct2.driverservice.DriverImportData response=new net.atos.daf.ct2.driverservice.DriverImportData();
               
                _logger.Info("Drivers import."); 

                List<Driver> lstDriver =new List<Driver>();
                    
                foreach (DriversImport entity in request.Drivers)
                {
                   lstDriver.Add(_mapper.ToDriver(entity));                  
                }
              List<driver.entity.DriverImportResponse> objDrv=new List<driver.entity.DriverImportResponse>();
              // Dictionary<string,string> importMessage=new Dictionary<string, string>();
               objDrv= await driverManager.ImportDrivers(lstDriver,request.OrgID);
               net.atos.daf.ct2.driverservice.DriverReturns objdrvReturn=new net.atos.daf.ct2.driverservice.DriverReturns();

                foreach (driver.entity.DriverImportResponse entity in objDrv)
                {
                    response.Driver.Add(_mapper.ToDriverImportResponse(entity));                  
                }      
               return await Task.FromResult(response);
        }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new DriverImportData
                {
                    Code = Responcecode.Failed,
                    Message = "Driver get faile due to - " + ex.Message + " " + ex.StackTrace
                });
            }
        }
    }
}
