using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.driver.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.driver
{
    public class DriverRepository : IDriverRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _dataMartdataAccess;
        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DriverRepository(IDataAccess dataAccess, IDataMartDataAccess DataMartdataAccess)
        {
            _dataAccess = dataAccess;
            _dataMartdataAccess = DataMartdataAccess;
        }

        public Task<int> UploadDriverTemplate()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DriverResponse>> GetDriver(int organizationId, int driverId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@id", driverId);
                parameter.Add("@state", "A");

                var queryStatement = @" SELECT id, organization_id,driver_id_ext, first_name FirstName, last_name LastName, email Email, status Status, state State,opt_in OptIn,modified_at ModifiedAt,modified_by ModifiedBy,created_at CreatedAt
                                    from master.driver where organization_id=@organization_id and (id=@id OR @id=0) and state=@state";

                return await _dataAccess.QueryAsync<DriverResponse>(queryStatement, parameter);
            }
            catch (Exception ex)
            {
                _log.Info("Delete get method in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<DriverLookup> GetDriver(int organizationId, string driverId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@id", driverId);
                parameter.Add("@state", "A");

                var queryStatement = @"SELECT first_name as FirstName, last_name as LastName
                                    from master.driver where organization_id=@organization_id and driver_id_ext=@id and state=@state";

                return await _dataAccess.QueryFirstOrDefaultAsync<DriverLookup>(queryStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<DriverLookupResponse> GetDriver(string driverId, string email)
        {
            try
            {
                var response = new DriverLookupResponse();
                var parameter = new DynamicParameters();
                parameter.Add("@DriverId", driverId);
                parameter.Add("@Email", email);

                var queryStatement =
                        @"SELECT first_name FirstName, last_name LastName, organization_id as OrganizationId, org.name as OrganizationName
                            FROM master.driver drv inner join master.organization org on org.id=drv.organization_id
                            WHERE driver_id_ext = @DriverId and email = @Email";

                response.DriverLookup = await _dataAccess.QueryAsync<DriverLookup>(queryStatement, parameter);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckIfDriverExists(string driverId, int organisationId, string email)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@DriverId", driverId);
                parameter.Add("@Email", email);
                parameter.Add("@OrganisationId", organisationId);

                var queryStatement =
                        @"SELECT EXISTS (SELECT 1
                            FROM master.driver drv inner join master.organization org on org.id=drv.organization_id
                            WHERE driver_id_ext = @DriverId and email = @Email and drv.organization_id = @OrganisationId)";

                return await _dataAccess.ExecuteScalarAsync<bool>(queryStatement, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Driver> UpdateDriver(Driver driver)
        {
            try
            {
                var orgOptInStatus = string.Empty;
                if (driver.OptIn == "I" || driver.OptIn == "U")
                {
                    driver.Status = driver.OptIn;
                }
                else if (driver.OptIn == "H")
                {
                    var parameterOpt = new DynamicParameters();
                    parameterOpt.Add("@id", driver.Organization_id);
                    var queryOptIn = @"select driver_default_opt_in from master.organization where id=@id and state='A'";
                    orgOptInStatus = await _dataAccess.ExecuteScalarAsync<string>(queryOptIn, parameterOpt);
                    driver.Status = orgOptInStatus;
                }
                var parameter = new DynamicParameters();
                parameter.Add("@id", driver.Id);
                parameter.Add("@organization_id", driver.Organization_id);
                parameter.Add("@email", driver.Email.ToString());
                parameter.Add("@first_name", driver.FirstName.ToString());
                parameter.Add("@last_name", driver.LastName.ToString());
                parameter.Add("@opt_in", driver.OptIn);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(System.DateTime.Now));
                parameter.Add("@modified_by", driver.ModifiedBy);
                parameter.Add("@status", driver.Status);

                var queryUpdate = @"update master.driver set  first_name=@first_name, last_name=@last_name, email=@email,opt_in=@opt_in,status=@status,
             modified_at=@modified_at,modified_by=@modified_by WHERE id= @id and organization_id=@organization_id and state='A' RETURNING id;";
                int drvID = await _dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameter);

                DriverDatamart driverdatamart = new DriverDatamart();
                driverdatamart.DriverID = driver.Driver_id_ext;
                driverdatamart.FirstName = driver.FirstName;
                driverdatamart.LastName = driver.LastName;
                driverdatamart.OrganizationId = driver.Organization_id;
                await CreateAndUpdateDriverInDataMart(driverdatamart);

                return driver;
            }
            catch (Exception ex)
            {
                _log.Info("Driver update method in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
        }


        public async Task<bool> DeleteDriver(int organizationId, int driverid)
        {
            _log.Info("Delete driver method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@id", driverid);
                var query = @"update master.driver set state='D' where id=@id and organization_id=@organization_id";
                int isdelete = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception ex)
            {
                _log.Info("Delete driver method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
        }
        public async Task<bool> UpdateOptinOptout(int organizationId, string optoutStatus)
        {
            _log.Info("Delete driver method called in repository");
            try
            {
                var orgOptInStatus = string.Empty;
                var status = string.Empty;
                if (optoutStatus == "I" || optoutStatus == "U")
                {
                    status = optoutStatus;
                }
                else if (optoutStatus == "H")
                {
                    var parameterOpt = new DynamicParameters();
                    parameterOpt.Add("@id", organizationId);
                    var queryOptIn = @"select driver_default_opt_in from master.organization where id=@id and state='A'";
                    orgOptInStatus = await _dataAccess.ExecuteScalarAsync<string>(queryOptIn, parameterOpt);
                    status = orgOptInStatus;
                }
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@opt_in", optoutStatus);
                parameter.Add("@status", status);
                var query = @"update master.driver set status=@status, opt_in=@opt_in where organization_id=@organization_id and state='A'";
                int isUpdated = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception ex)
            {
                _log.Info("UpdateOptinOptout driver method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<List<DriverImportResponse>> ImportDrivers(List<Driver> drivers, int orgid)
        {
            List<string> InsertedData = new List<string>();
            // string driverid = string.Empty;
            // string ErrorMessage = string.Empty;
            string orgOptInStatus;
            // Dictionary<string,string> dicMessage=new Dictionary<string, string> ();

            List<DriverImportResponse> lstdrivers = new List<DriverImportResponse>();
            DriverImportResponse objDriver = new DriverImportResponse();
            try
            {
                string status = "H";
                var parameterOpt = new DynamicParameters();
                parameterOpt.Add("@id", orgid);
                var queryOptIn = @"select driver_default_opt_in from master.organization where id=@id and state='A'";
                orgOptInStatus = await _dataAccess.ExecuteScalarAsync<string>(queryOptIn, parameterOpt);

                if (!string.IsNullOrEmpty(orgOptInStatus))
                {
                    if (orgOptInStatus == "H" || orgOptInStatus == "I")
                    {
                        status = "I";
                    }
                    else if (orgOptInStatus == "U")
                    {
                        status = "U";
                    }
                }

                foreach (var item in drivers)
                {
                    objDriver = new DriverImportResponse();
                    try
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("@organization_id", orgid);
                        //  parameter.Add("@driver_id_ext", item.Driver_id_ext.Substring(0, item.Driver_id_ext.Length - 3));
                        parameter.Add("@first_name", item.FirstName);
                        parameter.Add("@last_name", item.LastName);
                        parameter.Add("@email", item.Email);
                        parameter.Add("@status", status);
                        parameter.Add("@opt_in", orgOptInStatus);
                        parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(System.DateTime.Now));
                        parameter.Add("@modified_by", item.ModifiedBy);
                        parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(System.DateTime.Now));

                        var parameterduplicate = new DynamicParameters();

                        string newDriverId = item.Driver_id_ext;
                        string driverID = item.Driver_id_ext;
                        driverID = driverID.Substring(0, driverID.Length - 3);
                        parameterduplicate.Add("@driver_id_ext", driverID);
                        parameterduplicate.Add("@organization_id", orgid);

                        var query = @"SELECT id FROM master.driver where LENGTH(driver_id_ext) =19 and SUBSTRING(driver_id_ext ,0, LENGTH(driver_id_ext) -2)=@driver_id_ext and state='A' and organization_id=@organization_id";
                        int ObjDriverExist = await _dataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);

                        if (ObjDriverExist > 0)
                        {
                            parameter.Add("@oldDriverID", ObjDriverExist);
                            parameter.Add("@newDriverID", newDriverId);

                            var queryUpdate = @"update master.driver set driver_id_ext=@newDriverID, first_name=@first_name, last_name=@last_name,email=@email,opt_in=@opt_in,modified_at=@modified_by,created_at=@created_at
                                        WHERE state='A' and id=@oldDriverID and organization_id=@organization_id RETURNING id;";
                            var id = await _dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameter);
                            if (id > 0)
                            {
                                objDriver.ReturnMessage = "Updated";
                                objDriver.Status = "PASS";
                            }
                            else
                            {
                                objDriver.ReturnMessage = "IsNotActive";
                                objDriver.Status = "FAIL";
                            }
                        }
                        else
                        {
                            parameter.Add("@driver_id_ext", item.Driver_id_ext);
                            var queryInsert = @"insert into master.driver(organization_id,driver_id_ext, first_name, last_name,email,status, opt_in,modified_at,modified_by,created_at,state) values(@organization_id,@driver_id_ext, @first_name, @last_name,@email ,@status, @opt_in,@modified_at,@modified_by,@created_at,'A')";
                            await _dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                            objDriver.ReturnMessage = "Inserted";
                            objDriver.Status = "PASS";
                        }

                        DriverDatamart driverdatamart = new DriverDatamart();
                        driverdatamart.DriverID = item.Driver_id_ext;
                        driverdatamart.FirstName = item.FirstName;
                        driverdatamart.LastName = item.LastName;
                        driverdatamart.OrganizationId = orgid;
                        await CreateAndUpdateDriverInDataMart(driverdatamart);
                    }
                    catch (Exception ex)
                    {
                        objDriver.ReturnMessage = ex.Message;
                        objDriver.Status = "FAIL";
                    }

                    objDriver.DriverID = item.Driver_id_ext;
                    objDriver.FirstName = item.FirstName;
                    objDriver.LastName = item.LastName;
                    objDriver.Email = item.Email;

                    lstdrivers.Add(objDriver);
                }
            }
            catch (Exception ex)
            {
                objDriver.ReturnMessage = ex.Message;
                objDriver.Status = "FAIL";
                lstdrivers.Add(objDriver);
                // dicMessage.Add(ErrorMessage,ex.Message);
            }
            return lstdrivers;
        }

        public async Task<DriverDatamart> CreateAndUpdateDriverInDataMart(DriverDatamart driver)
        {
            try
            {
                var parameterduplicate = new DynamicParameters();
                string drvID = driver.DriverID;
                drvID = drvID.Substring(0, drvID.Length - 3);
                parameterduplicate.Add("@driver_id", drvID);
                parameterduplicate.Add("@organization_id", driver.OrganizationId);

                var query = @"SELECT id FROM master.driver where LENGTH(driver_id) =19 and SUBSTRING(driver_id ,0, LENGTH(driver_id) -2)=@driver_id and organization_id=@organization_id";
                int driverDataMartID = await _dataMartdataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);
                var QueryStatement = "";

                var parameter = new DynamicParameters();
                parameter.Add("@driver_id", driver.DriverID);
                parameter.Add("@first_name", driver.FirstName);
                parameter.Add("@last_name", driver.LastName);
                parameter.Add("@organization_id", driver.OrganizationId);

                if (driverDataMartID == 0)
                {
                    // parameter.Add("@driver_id", driver.DriverID);

                    QueryStatement = @"INSERT INTO master.driver
                                      (
                                        driver_id
                                       ,first_name
                                       ,last_name
                                       ,organization_id 
                                       ) 
                            	VALUES(
                                        @driver_id
                                       ,@first_name
                                       ,@last_name
                                       ,@organization_id                                      
                                      ) RETURNING id";
                }
                else if (driverDataMartID > 0)
                {
                    parameter.Add("@id", driverDataMartID);
                    parameter.Add("@newDriverID", driver.DriverID);

                    //string driverIDNew = driver.DriverID;
                    //driverIDNew = driverIDNew.Substring(0, driverIDNew.Length - 3); 
                    //parameter.Add("@newDriverID", driverIDNew);

                    QueryStatement = @" UPDATE master.driver
                                    SET
                                     driver_id=@driver_id
                                    ,first_name=@first_name
                                    ,last_name=@last_name
                                    ,organization_id=@organization_id                                    
                                     WHERE id = @id and organization_id=@organization_id
                                     RETURNING id;";
                }
                int driverID = await _dataMartdataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);
                return driver;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Provisioning Data Service

        public async Task<ProvisioningDriver> GetCurrentDriver(ProvisioningDriverDataServiceRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@VIN", request.VIN);
                parameters.Add("@StartTimestamp", request.StartTimestamp);
                parameters.Add("@EndTimestamp", request.EndTimestamp);

                StringBuilder query =
                    new StringBuilder(@"select distinct driver1_id as DriverId from 
                                            (select driver1_id, end_time_stamp 
                                            from livefleet.livefleet_current_trip_statistics where VIN = @VIN");

                if (request.StartTimestamp.HasValue && request.EndTimestamp.HasValue)
                {
                    query.Append(" and start_time_stamp >= @StartTimestamp and (end_time_stamp <= @EndTimestamp or end_time_stamp IS NULL)");
                }
                else if (request.StartTimestamp.HasValue && !request.EndTimestamp.HasValue)
                {
                    query.Append(" and start_time_stamp >= @StartTimestamp and (end_time_stamp IS NULL or 1=1)");
                }
                else if (!request.StartTimestamp.HasValue && request.EndTimestamp.HasValue)
                {
                    query.Append(" and end_time_stamp <= @EndTimestamp or end_time_stamp IS NULL");
                }

                query.Append(" order by end_time_stamp desc) tmp");

                var provisioningDrivers = await _dataMartdataAccess.QueryAsync<ProvisioningDriver>(query.ToString(), parameters);

                if (provisioningDrivers != null && provisioningDrivers.Count() > 0)
                {
                    foreach (var provisioningDriver in provisioningDrivers)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@DriverId", provisioningDriver.DriverId);
                        parameters.Add("@OrgId", request.OrgId);

                        string queryDriver = @"select acc.email as Account, COALESCE(acc.first_name, '') as FirstName, COALESCE(acc.last_name, '') as LastName, drv.driver_id_ext as DriverId
                                            from master.driver drv inner join master.account acc on drv.email = acc.email
                                            where drv.driver_id_ext = @DriverId and drv.organization_id = @OrgId
                                            and drv.state='A' and acc.state='A'";
                        var driverAccount = await _dataAccess.QueryFirstOrDefaultAsync<ProvisioningDriver>(queryDriver, parameters);
                        if (driverAccount != null)
                            return driverAccount;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProvisioningDriver>> GetDriverList(ProvisioningDriverDataServiceRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();
                IEnumerable<ProvisioningDriver> driverAccounts = new List<ProvisioningDriver>();
                parameters.Add("@VINs", request.VINs);
                parameters.Add("@StartTimestamp", request.StartTimestamp);
                parameters.Add("@EndTimestamp", request.EndTimestamp);

                StringBuilder query =
                    new StringBuilder(@"select distinct driver1_id as DriverId from 
                                            (select driver1_id, end_time_stamp 
                                            from livefleet.livefleet_current_trip_statistics where VIN = ANY(@VINs)");

                if (request.StartTimestamp.HasValue && request.EndTimestamp.HasValue)
                {
                    query.Append(" and start_time_stamp >= @StartTimestamp and (end_time_stamp <= @EndTimestamp or end_time_stamp IS NULL)");
                }
                else if (request.StartTimestamp.HasValue && !request.EndTimestamp.HasValue)
                {
                    query.Append(" and start_time_stamp >= @StartTimestamp and (end_time_stamp IS NULL or 1=1)");
                }
                else if (!request.StartTimestamp.HasValue && request.EndTimestamp.HasValue)
                {
                    query.Append(" and end_time_stamp <= @EndTimestamp or end_time_stamp IS NULL");
                }

                query.Append(" order by end_time_stamp desc) tmp");

                var provisioningDrivers = await _dataMartdataAccess.QueryAsync<ProvisioningDriver>(query.ToString(), parameters);

                if (provisioningDrivers != null && provisioningDrivers.Count() > 0)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@DriverIds", provisioningDrivers.Select(x => x.DriverId).ToArray());
                    parameters.Add("@OrgId", request.OrgId);

                    string queryDriver = @"select acc.email as Account, COALESCE(acc.first_name, '') as FirstName, COALESCE(acc.last_name, '') as LastName, drv.driver_id_ext as DriverId 
                                            from master.driver drv inner join master.account acc on drv.email = acc.email
                                            where drv.driver_id_ext = ANY(@DriverIds) and drv.organization_id = @OrgId
                                            and drv.state='A' and acc.state='A'";
                    driverAccounts = await _dataAccess.QueryAsync<ProvisioningDriver>(queryDriver, parameters);
                }

                return driverAccounts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
