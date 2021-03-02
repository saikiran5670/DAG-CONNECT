using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.driver.entity;
using Dapper;

namespace net.atos.daf.ct2.driver
{
    public class DriverRepository : IDriverRepository
    {
         private readonly IDataAccess dataAccess;
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

         public DriverRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
        
        public Task<int> UploadDriverTemplate()
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteDriverDetails(List<DriverMaster> drivers)
        {
            await dataAccess.ExecuteAsync("UPDATE dafconnectmaster.driver SET isactive=false,updatedby=@modifiedby,updateddate=@modifieddate  WHERE drivermasterid = @Id and isactive=true", drivers);
            return 0;
        }

        // public async Task<DriverTemplate> DownloadDriverTemplate(string languageCode)
        // {
        //     var QueryStatement = @" SELECT drivertemplateid
        //                                     , name
        //                                     , templatepath
        //                                     , createddate
        //                                     , createdby
        //                                     , updateddate
        //                                     , updatedby
        //                             from dafconnectmaster.drivertemplate
        //                             where isactive=true
        //                             and (languagecode=@languageCode)";

        //     var parameter = new DynamicParameters();
        //     parameter.Add("@languagecode", languageCode);
        //     DriverTemplate DriverTemplateDetails = await dataAccess.QueryFirstOrDefault(QueryStatement, parameter);
        //     return DriverTemplateDetails;
        // }

        public async Task<IEnumerable<DriverMaster>> GetDriverList(int DriverId)
        {
             var QueryStatement = @" SELECT driver.drivermasterid
                                    , driver.driverid
                                    , driverorg.firstname
                                    , driverorg.lastname
                                    , driverorg.civility
                                    , driver.email
                                    , driver.dob
                                    , driver.languageid
                                    , driver.unitid
                                    , driver.timezoneid
                                    , driver.currencyid
                                    , driver.createddate
                                    , driver.createdby
                                    , driver.updateddate
                                    , driver.updatedby
                                    from dafconnectmaster.driver driver
                                    inner join dafconnectmaster.driverorg driverorg
                                    on driver.drivermasterid=driverorg.driverid
                                    where isactive=true
                                    and (driver.drivermasterid=@id OR @id=0)";

            var parameter = new DynamicParameters();
            parameter.Add("@id", DriverId);
            IEnumerable<DriverMaster> DriverDetails = await dataAccess.QueryAsync<DriverMaster>(QueryStatement, parameter);
            return DriverDetails;
        }

        // public async Task<string> ShowConsentForm(string languageCode,int OrganizationId)
        // {
        //     var QueryStatement = @" SELECT name
        //                             from dafconnectmaster.keyvaluecategorymaster
        //                             where isactive=true
        //                             and (languagecode=@languageCode and parentcategoryid=0) or (languagecode=@languageCode and parentcategoryid=@OrganizationId)";

        //     var parameter = new DynamicParameters();
        //     parameter.Add("@languagecode", languageCode);
        //     string ConsentMessage = await dataAccess.QueryFirstAsync(QueryStatement, parameter);
        //     return ConsentMessage;
        // }

        public async Task<List<string>> InertUpdateDriverDetails(List<DriverMaster> driverdetails)
        {
            List<string> InsertedData = new List<string>();
             string driverid=string.Empty;
             string ErrorMessage=string.Empty;
            for (int i = 0; i < driverdetails.Count(); i++)
            {
                try
                {
                    DriverMaster ObjDriverMaster=new DriverMaster ();
                    driverid = driverdetails[i].DriverId.ToString();
                    string languageCode = driverdetails[i].LanguageCode.ToString();
                    string currency = driverdetails[i].Currency.ToString();
                    string timezone = driverdetails[i].TimeZone.ToString();
                    string unit = driverdetails[i].Unit.ToString();

                    IEnumerable<DriverMaster> ObjDriver = await dataAccess.QueryAsync<DriverMaster>("SELECT * FROM dafconnectmaster.driver where driverid = @driverid and isactive=true", new { driverid = driverid });
                    ObjDriverMaster.DriverId =driverdetails[i].DriverId.ToString();
                    ObjDriverMaster.OrganizationId =driverdetails[i].OrganizationId;
                    ObjDriverMaster.Email =driverdetails[i].Email.ToString();
                    ObjDriverMaster.DateOfBirth =driverdetails[i].DateOfBirth;
                    ObjDriverMaster.FirstName =driverdetails[i].FirstName.ToString();
                    ObjDriverMaster.LastName =driverdetails[i].LastName.ToString();
                    ObjDriverMaster.OptOutStatus =driverdetails[i].OptOutStatus;
                    ObjDriverMaster.OptOutLevelId=driverdetails[i].OptOutLevelId;
                    ObjDriverMaster.LanguageCodeId = await dataAccess.QueryFirstOrDefaultAsync<int>("SELECT languagemasterid FROM dafconnectmaster.languagemaster where code = @languageCode and isactive=true", new { languageCode = languageCode });
                    ObjDriverMaster.CurrencyId = await dataAccess.QueryFirstOrDefaultAsync<int>("SELECT currencyid FROM dafconnectmaster.currency where name = @currency and isactive=true", new { currency = currency });
                    ObjDriverMaster.TimeZoneId = await dataAccess.QueryFirstOrDefaultAsync<int>("SELECT timezoneid FROM dafconnectmaster.timezone where shortname = @timezone and isactive=true", new { timezone = timezone });
                    ObjDriverMaster.UnitId = await dataAccess.QueryFirstOrDefaultAsync<int>("SELECT unitid FROM dafconnectmaster.unit where name = @unit and isactive=true", new { unit = unit });
                    ObjDriverMaster.createddate=DateTime.Now;
                    ObjDriverMaster.createdby=driverdetails[i].createdby;
                    ObjDriverMaster.isactive=true;
                    if (ObjDriver.Count() > 0)
                    {
                        ObjDriverMaster.DriverMasterId=await dataAccess.ExecuteScalarAsync<int>("UPDATE dafconnectmaster.driver SET email=@Email,dob=@DateOfBirth,languageid=@LanguageCodeId,unitid=@UnitId,timezoneid=@TimeZoneId,currencyid=@CurrencyId,isactive=true,updatedby=@createdby,updateddate=@createddate  WHERE driverid = @DriverId and isactive=true RETURNING drivermasterid;", ObjDriverMaster);
                        await dataAccess.ExecuteAsync("UPDATE dafconnectmaster.driverorg SET OrganizationId=@OrganizationId,civility=@Civility,firstname=@FirstName,lastname=@LastName,optoutstatus=@OptOutStatus,optoutstatuschangeddate=@OptOutStatusChangedDate,optoutlevelid=@OptOutLevelId,isconsentgiven=@IsConsentGiven,consentchangeddate=@ConsentChangedDate,isactive=true,updatedby=@createdby,updateddate=@createddate  WHERE driverid = @DriverMasterId and isactive=true", ObjDriverMaster);
                        ErrorMessage=driverid+ "Not Updated";
                    }
                    else
                    {
                        
                        ObjDriverMaster.DriverMasterId=await dataAccess.ExecuteScalarAsync<int>("INSERT INTO dafconnectmaster.driver (driverid, email, dob, languageid, unitid, timezoneid, currencyid, isactive, createddate, createdby) VALUES(@DriverId,@Email,@DateOfBirth,@LanguageCodeId,@UnitId,@TimeZoneId,@CurrencyId,@isactive,@createddate,@CreatedBy) RETURNING drivermasterid;", ObjDriverMaster);
                        await dataAccess.ExecuteAsync("INSERT INTO dafconnectmaster.driverorg (OrganizationId,driverid, civility, firstname, lastname, optoutstatus, optoutstatuschangeddate, optoutlevelid,isconsentgiven,consentchangeddate, isactive, createddate, createdby) VALUES(@OrganizationId,@DriverMasterId,@Civility,@FirstName,@LastName,@OptOutStatus,@OptOutStatusChangedDate,@OptOutLevelId,@IsConsentGiven,@ConsentChangedDate,@isactive,@createddate,@CreatedBy)", ObjDriverMaster);
                        ErrorMessage=driverid+ "Not Inserted";
                    }
                }
                catch (Exception ex)
                {
                   InsertedData.Add(ErrorMessage);
                }

            }
            return InsertedData;
        }

        public async Task<List<string>> ImportDrivers(List <Driver> drivers)
        {
            List<string> InsertedData = new List<string>();
            string driverid=string.Empty;
            string ErrorMessage=string.Empty;
            try
            {
            foreach (var item in drivers)
            {
                        var parameter = new DynamicParameters();
                         parameter.Add("@organization_id",item.Organization_id);
                         parameter.Add("@driver_id_ext",item.Driver_id_ext);
                         parameter.Add("@salutation",item.Salutation);
                         parameter.Add("@first_name",item.FirstName);
                         parameter.Add("@last_name",item.LastName);
                         parameter.Add("@dob",item.DateOfBith);
                         parameter.Add("@status",item.Status);
                         parameter.Add("@is_active",item.IsActive);
                   
                        var parameterduplicate = new DynamicParameters();
                        parameterduplicate.Add("@driver_id_ext", item.Driver_id_ext);             
                        var query = @"SELECT id FROM master.driver where driver_id_ext=@driver_id_ext";
                        int ObjDriverExist= await dataAccess.ExecuteScalarAsync<int>(query, parameter);    
  
                    if (ObjDriverExist > 0)
                    {
                       var queryUpdate = @"update master.driver set salutation=@salutation, first_name=@first_name, last_name=@last_name, dob=@dob, status=@status, is_active=@is_active
	                                 WHERE driver_id_ext = @driver_id_ext RETURNING id;";
                                      await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameter);                      
                        ErrorMessage=item.Driver_id_ext + "Not Updated";
                    }
                    else
                    {             
                        var queryInsert =@"insert into master.driver(organization_id,driver_id_ext, salutation, first_name, last_name, dob, status, is_active) values(@organization_id,@driver_id_ext, @salutation, @first_name, @last_name, @dob, @status, @is_active)";         
                        await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);     
                        ErrorMessage=item.Driver_id_ext + "Not Inserted";
                    }              
            }
            }      
                catch (Exception ex)
                {
                   InsertedData.Add(ErrorMessage);
                }            
            return InsertedData;
        }

        public async Task<IEnumerable<Driver>> GetAllDrivers(int OrganizatioId)
        {
             var QueryStatement = @" SELECT  id, organization_id,driver_id_ext, salutation, first_name, last_name, dob, status, is_active
                                    from master.driver where organization_id=organization_id and is_active=true";      
            var parameter = new DynamicParameters();
            parameter.Add("@organization_id", OrganizatioId);
            IEnumerable<Driver> drivers = await dataAccess.QueryAsync<Driver>(QueryStatement, parameter);
            return drivers;
        }
        public async Task<IEnumerable<Driver>> GetDriver(int OrganizatioId,int driverId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@organization_id", OrganizatioId);
            parameter.Add("@id", driverId);

             var QueryStatement = @" SELECT  id, organization_id,driver_id_ext, salutation, first_name, last_name, dob, status, is_active
                                    from master.driver where organization_id=@organization_id and id=@id and is_active=true";      
           
            return await dataAccess.QueryAsync<Driver>(QueryStatement, parameter);            
        }
        public async Task<Driver> UpdateDriver(Driver driver)
        {
             var parameter = new DynamicParameters();
                         parameter.Add("@id",driver.Id);
                         parameter.Add("@organization_id",driver.Organization_id);
                         parameter.Add("@driver_id_ext",driver.Driver_id_ext);
                         parameter.Add("@salutation",driver.Salutation);
                         parameter.Add("@first_name",driver.FirstName);
                         parameter.Add("@last_name",driver.LastName);
                         parameter.Add("@dob",driver.DateOfBith);
                         parameter.Add("@status",driver.Status);
                         parameter.Add("@is_active",driver.IsActive);
            var queryUpdate = @"update master.driver set salutation=@salutation, first_name=@first_name, last_name=@last_name, dob=@dob, status=@status, is_active=@is_active
	                                 WHERE id = @id and is_active=true RETURNING id;";
                                              
            parameter.Add("@organization_id", driver.Organization_id);           
            return await dataAccess.ExecuteScalarAsync<Driver>(queryUpdate, parameter);                       
        }
        public async Task<bool> DeleteDriver(int organizationId,int driverid)
        {
            log.Info("Delete driver method called in repository");            
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", organizationId);
                parameter.Add("@id", driverid);
                var query = @"update master.driver set is_active=false where id=@id and organization_id=@organization_id";
                int isdelete= await dataAccess.ExecuteScalarAsync<int>(query, parameter);    
                if(isdelete>0)
                {
                    return true;  
                }
                else
                {
                    return false;   
                }      
            }
            catch (Exception ex)
            {
                log.Info("Delete driver method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());
                throw ex;
            }            
        }
        public async Task<bool> UpdateOptinOptout(int organizationId, Boolean optoutStatus)
        {
            log.Info("Delete driver method called in repository");            
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", organizationId);
                var query = @"update master.driver set status=optoutStatus where organization_id=@organization_id and is_active=true";
                int isUpdated= await dataAccess.ExecuteScalarAsync<int>(query, parameter);    
                if(isUpdated>0)
                {
                    return true;  
                }
                else
                {
                    return false;   
                }      
            }
            catch (Exception ex)
            {
                log.Info("UpdateOptinOptout driver method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());
                throw ex;
            }            
        }
        
    }
}
