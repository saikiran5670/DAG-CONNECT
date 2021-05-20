using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.driver.entity;
using Dapper;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.driver
{
    public class DriverRepository : IDriverRepository
    {
        private readonly IDataAccess dataAccess;
        private readonly IDataMartDataAccess dataMartdataAccess;
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DriverRepository(IDataAccess _dataAccess, IDataMartDataAccess _DataMartdataAccess)
        {
            dataAccess = _dataAccess;
            dataMartdataAccess = _DataMartdataAccess;             
        }

        public Task<int> UploadDriverTemplate()
        {
            throw new NotImplementedException();
        }      

       
        public async Task<IEnumerable<DriverResponse>> GetDriver(int OrganizatioId, int driverId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", OrganizatioId);
                parameter.Add("@id", driverId);
                parameter.Add("@state", "A");

                var QueryStatement = @" SELECT id, organization_id,driver_id_ext, first_name FirstName, last_name LastName, email, status, state,opt_in,modified_at ,modified_by,created_at
                                    from master.driver where organization_id=@organization_id and (id=@id OR @id=0) and state=@state";

                return await dataAccess.QueryAsync<DriverResponse>(QueryStatement, parameter);
            }
            catch (Exception ex)
            {
                log.Info("Delete get method in repository failed :");
                log.Error(ex.ToString());
                throw;
            }
        }


        public async Task<Driver> UpdateDriver(Driver driver)
        {
            try
            {
                var orgOptInStatus = string.Empty;
                if (driver.opt_in == "I" || driver.opt_in == "U")
                {
                    driver.Status = driver.opt_in;
                }
                else if (driver.opt_in == "H")
                {
                    var parameterOpt = new DynamicParameters();
                    parameterOpt.Add("@id", driver.Organization_id);
                    var queryOptIn = @"select driver_default_opt_in from master.organization where id=@id and state='A'";
                    orgOptInStatus = await dataAccess.ExecuteScalarAsync<string>(queryOptIn, parameterOpt);
                    driver.Status = orgOptInStatus;
                }
                var parameter = new DynamicParameters();
                parameter.Add("@id", driver.Id);
                parameter.Add("@organization_id", driver.Organization_id);
                parameter.Add("@email", driver.email.ToString());
                parameter.Add("@first_name", driver.first_name.ToString());
                parameter.Add("@last_name", driver.last_name.ToString());
                parameter.Add("@opt_in", driver.opt_in);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(System.DateTime.Now));
                parameter.Add("@modified_by", driver.modified_by);
                parameter.Add("@status", driver.Status);

                var queryUpdate = @"update master.driver set  first_name=@first_name, last_name=@last_name, email=@email,opt_in=@opt_in,status=@status,
             modified_at=@modified_at,modified_by=@modified_by WHERE id= @id and organization_id=@organization_id and state='A' RETURNING id;";
                int drvID = await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameter);
               
                DriverDatamart driverdatamart = new DriverDatamart();
                driverdatamart.DriverID = driver.Driver_id_ext;
                driverdatamart.FirstName = driver.first_name;
                driverdatamart.LastName = driver.last_name;
                driverdatamart.OrganizationId = driver.Organization_id;
                await CreateAndUpdateDriverInDataMart(driverdatamart);

                return driver;
            }
            catch (Exception ex)
            {
                log.Info("Driver update method in repository failed :");
                log.Error(ex.ToString());
                throw;
            }
        }


        public async Task<bool> DeleteDriver(int organizationId, int driverid)
        {
            log.Info("Delete driver method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@id", driverid);
                var query = @"update master.driver set state='D' where id=@id and organization_id=@organization_id";
                int isdelete = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception ex)
            {
                log.Info("Delete driver method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());
                throw;
            }
        }
        public async Task<bool> UpdateOptinOptout(int organizationId, string optoutStatus)
        {
            log.Info("Delete driver method called in repository");
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
                    orgOptInStatus = await dataAccess.ExecuteScalarAsync<string>(queryOptIn, parameterOpt);
                    status = orgOptInStatus;
                }
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@opt_in", optoutStatus);
                parameter.Add("@status", status);
                var query = @"update master.driver set status=@status, opt_in=@opt_in where organization_id=@organization_id and state='A'";
                int isUpdated = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception ex)
            {
                log.Info("UpdateOptinOptout driver method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<List<DriverImportResponse>> ImportDrivers(List<Driver> drivers, int orgid)
        {
            List<string> InsertedData = new List<string>();
            string driverid = string.Empty;
            string ErrorMessage = string.Empty;
            string orgOptInStatus = string.Empty;
            // Dictionary<string,string> dicMessage=new Dictionary<string, string> ();

            List<DriverImportResponse> lstdrivers = new List<DriverImportResponse>();
            DriverImportResponse objDriver = new DriverImportResponse();
            try
            {
                string status = "H";
                var parameterOpt = new DynamicParameters();
                parameterOpt.Add("@id", orgid);
                var queryOptIn = @"select driver_default_opt_in from master.organization where id=@id and state='A'";
                orgOptInStatus = await dataAccess.ExecuteScalarAsync<string>(queryOptIn, parameterOpt);

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
                        parameter.Add("@first_name", item.first_name);
                        parameter.Add("@last_name", item.last_name);
                        parameter.Add("@email", item.email);
                        parameter.Add("@status", status);
                        parameter.Add("@opt_in", orgOptInStatus);
                        parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(System.DateTime.Now));
                        parameter.Add("@modified_by", item.modified_by);
                        parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(System.DateTime.Now));

                        var parameterduplicate = new DynamicParameters();

                        string newDriverId= item.Driver_id_ext;
                        string driverID = item.Driver_id_ext;
                        driverID = driverID.Substring(0, driverID.Length - 3);
                        parameterduplicate.Add("@driver_id_ext", driverID);
                        parameterduplicate.Add("@organization_id", orgid);

                        var query = @"SELECT id FROM master.driver where LENGTH(driver_id_ext) =19 and SUBSTRING(driver_id_ext ,0, LENGTH(driver_id_ext) -2)=@driver_id_ext and state='A' and organization_id=@organization_id";
                        int ObjDriverExist = await dataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);

                        if (ObjDriverExist > 0)
                        {
                            parameter.Add("@oldDriverID", ObjDriverExist);
                            parameter.Add("@newDriverID", newDriverId);

                            var queryUpdate = @"update master.driver set driver_id_ext=@newDriverID, first_name=@first_name, last_name=@last_name,email=@email,opt_in=@opt_in,modified_at=@modified_by,created_at=@created_at
                                        WHERE state='A' and id=@oldDriverID and organization_id=@organization_id RETURNING id;";
                            var id = await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameter);                           
                            if(id > 0)
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
                            await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);                            
                            objDriver.ReturnMessage = "Inserted";
                            objDriver.Status = "PASS";
                        }

                        DriverDatamart driverdatamart = new DriverDatamart();
                        driverdatamart.DriverID = item.Driver_id_ext;
                        driverdatamart.FirstName = item.first_name;
                        driverdatamart.LastName = item.last_name;
                        driverdatamart.OrganizationId = orgid;
                        await CreateAndUpdateDriverInDataMart(driverdatamart);
                    }
                    catch (Exception ex)
                    {
                        objDriver.ReturnMessage = ex.Message;
                        objDriver.Status = "FAIL";
                    }

                    objDriver.DriverID = item.Driver_id_ext;
                    objDriver.FirstName = item.first_name;
                    objDriver.LastName = item.last_name;
                    objDriver.Email = item.email;

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
                int driverDataMartID = await dataMartdataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);
                var QueryStatement = "";

                var parameter = new DynamicParameters();
                parameter.Add("@driver_id", driver.DriverID);
                parameter.Add("@first_name",driver.FirstName);
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
                else if (driverDataMartID >0)
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
                int driverID = await dataMartdataAccess.ExecuteScalarAsync<int>(QueryStatement, parameter);               
                return driver;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
