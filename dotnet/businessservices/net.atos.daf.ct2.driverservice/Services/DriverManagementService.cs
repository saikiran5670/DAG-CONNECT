using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.driver;
using net.atos.daf.ct2.driver.entity;
using net.atos.daf.ct2.driverservice.entity;

namespace net.atos.daf.ct2.driverservice
{
    public class DriverManagementService : DriverService.DriverServiceBase
    {
        private readonly IAuditTraillib _AuditTrail;
        private readonly IAuditTraillib auditlog;
        private readonly IDriverManager driverManager;

        private readonly DriverMapper _mapper;

        private ILog _logger;

        public DriverManagementService(IAuditTraillib AuditTrail, IAuditTraillib _auditlog, IDriverManager _driverManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _AuditTrail = AuditTrail;
            auditlog = _auditlog;
            driverManager = _driverManager;
            _mapper = new DriverMapper();
        }

        public override async Task<DriverDataList> Get(IdRequest request, ServerCallContext context)
        {
            try
            {
                DriverDataList response = new DriverDataList();
                var result = await driverManager.GetDriver(request.OrgID, request.DriverID);
                if (result.Count() > 0)
                {
                    foreach (driver.entity.DriverResponse entity in result)
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

        public override async Task<DriverUpdateResponse> Update(DriverUpdateRequest request, ServerCallContext context)
        {
            try
            {
                DriverUpdateResponse response = new DriverUpdateResponse();
                Driver driver = new Driver();
                driver = _mapper.ToDriverUpdateResponse(request);
                var result = await driverManager.UpdateDriver(driver);

                var objDriver = _mapper.DriverToDriverResponse(driver);
                response.Code = Responcecode.Success;
                response.Message = "Updated";
                response.Driver = objDriver;
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new DriverUpdateResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Driver get failed due to - " + ex.Message
                });
            }
        }

        public override async Task<DriverDeleteResponse> Delete(IdRequest request, ServerCallContext context)
        {
            try
            {
                DriverDeleteResponse response = new DriverDeleteResponse();
                bool result = await driverManager.DeleteDriver(request.OrgID, request.DriverID);
                if (result)
                {
                    response.Message = "Deleted";
                    response.Code = Responcecode.Success;
                }
                if (!result)
                {
                    response.Message = "Not Deleted";
                    response.Code = Responcecode.Failed;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new DriverDeleteResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Driver get failed due to - " + ex.Message
                });
            }
        }

        public override async Task<OptOutOptInResponse> UpdateOptinOptout(OptOutOptInRequest Optrequest, ServerCallContext context)
        {
            try
            {
                OptOutOptInResponse response = new OptOutOptInResponse();
                bool result = await driverManager.UpdateOptinOptout(Optrequest.OrgID, Optrequest.Optoutoptinstatus);
                if (result)
                {
                    response.Message = "Driver OptOutOptIn updated";
                    response.Code = Responcecode.Success;
                }
                if (!result)
                {
                    response.Message = "Driver OptOutOptIn Not updated";
                    response.Code = Responcecode.Failed;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new OptOutOptInResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Driver get failed due to - " + ex.Message
                });
            }
        }
        public override async Task<DriverImportData> ImportDrivers(DriverImportRequest request, ServerCallContext context)
        {
            try
            {
                DriverImportData response = new DriverImportData();
                List<Driver> lstDriver = new List<Driver>();

                foreach (DriversImport entity in request.Drivers)
                {
                    lstDriver.Add(_mapper.ToDriver(entity));
                }
                List<driver.entity.DriverImportResponse> objDrv = new List<driver.entity.DriverImportResponse>();
                objDrv = await driverManager.ImportDrivers(lstDriver, request.OrgID);
                DriverReturns objdrvReturn = new DriverReturns();

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
                    Message = "Driver get failed due to - " + ex.Message + " " + ex.StackTrace
                });
            }
        }
    }
}
