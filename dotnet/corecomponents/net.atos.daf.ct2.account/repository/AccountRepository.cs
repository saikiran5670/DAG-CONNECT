using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.account.entity;
using net.atos.daf.ct2.data;
using Dapper;
using System.Threading.Tasks;
using System.Transactions;

namespace net.atos.daf.ct2.account.repository
{
    public class AccountRepository:IAccountRepository
    {
        private readonly IDataAccess dataAccess;
        readonly string languagecode = "EN-GB";        
        public AccountRepository(IDataAccess _dataAccess) 
        {
           //_config = new ConfigurationBuilder()
           //.AddJsonFile("appsettings.Test.json")
           //.Build();
           // Get connection string
           //var connectionString = _config.GetConnectionString("DevAzure");
           //dataAccess = new PgSQLDataAccess(connectionString);
           dataAccess=_dataAccess;
        }
       public async Task<int>  AddUser(AccountDetails user)
       {
             int UserId=0;
             int userorg=0;
             int userpreference=0;
             var parameter = new DynamicParameters();
             parameter.Add("@userid", user.UserPrimaryDetails.UserID);
             parameter.Add("@organizationid", user.UserPrimaryDetails.OrganizationId);
             parameter.Add("@isactive",  user.UserPrimaryDetails.IsActive);
             parameter.Add("@createddate",  DateTime.UtcNow);
             parameter.Add("@createdby",  user.UserPrimaryDetails.CreateBy); 
             
             using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
             { 
                  try 
                  {       
                    UserId =  dataAccess.QuerySingle<int>("Call  dafconnectmaster.adduser(@emailid, @salutation, @firstname, @lastname, @dob, @password, @usertypeid, @isactive, @CreateBy, @updatedby)",
                    user.UserPrimaryDetails);
                    userorg=0;
                    userpreference=0;
                    if(UserId>0)
                    {
                        user.UserPrimaryDetails.UserID=UserId;
                        var parameterorg = new DynamicParameters();
                        parameterorg.Add("@userid", user.UserPrimaryDetails.UserID);
                        parameterorg.Add("@organizationid", user.UserPrimaryDetails.OrganizationId);
                        parameterorg.Add("@isactive",  user.UserPrimaryDetails.IsActive);
                        parameterorg.Add("@createddate",  DateTime.UtcNow);
                        parameterorg.Add("@createdby",  user.UserPrimaryDetails.CreateBy);                         
                        userorg=  dataAccess.QuerySingle<int>("INSERT INTO dafconnectmaster.userorg (userid, organizationid, isactive, createddate, createdby) VALUES(@userid, @organizationid, @isactive, @createddate, @createdby) RETURNING userorgid",parameterorg);
                        if(userorg > 0)
                        {
                            user.UserPreferences.UserorgId=userorg;
                           var parameterpref = new DynamicParameters();
                            parameterpref.Add("@userorgid", user.UserPreferences.UserorgId);
                            parameterpref.Add("@languagemasterid", user.UserPreferences.LanguageMasterId);
                            parameterpref.Add("@timezoneid", user.UserPreferences.Timezoneid);
                            parameterpref.Add("@currencyid",  user.UserPreferences.CurrencyId);
                            parameterpref.Add("@unitid",  user.UserPreferences.UnitId);
                            parameterpref.Add("@vehicledisplayid",  user.UserPreferences.VehicleDisplayId);
                            parameterpref.Add("@dateformatid",  user.UserPreferences.DateFormatId);
                            parameterpref.Add("@isactive",  user.UserPreferences.IsActive);
                            parameterpref.Add("@createddate",  DateTime.UtcNow);
                            parameterpref.Add("@createdby",  user.UserPreferences.CreatedBy);             
                            userpreference= await dataAccess.ExecuteScalarAsync<int>("INSERT INTO dafconnectmaster.usergeneralpreference (userorgid, languagemasterid, timezoneid, currencyid, unitid, vehicledisplayid, dateformatid, isactive, createddate, createdby) VALUES(@userorgid, @languagemasterid, @timezoneid, @currencyid, @unitid, @vehicledisplayid, @dateformatid, @isactive, @createddate, @createdby) RETURNING usergeneralpreferenceid",parameterpref);
                                
                        }
                        else
                         {
                            transactionScope.Complete();
                            return -1;
                         }

                    }
                    transactionScope.Complete();
                    
                 }
                 catch (Exception ex)
                 {
                        throw ex;
                 }
             }
                    if(userpreference>0)
                    return UserId;    
                    else  
                    return -1;
       }
       public async Task<int> AddUserorg(AccountDetails user)
       {
           try
           {
            var parameter = new DynamicParameters();
             parameter.Add("@userid", user.UserPrimaryDetails.UserID);
             parameter.Add("@organizationid", user.UserPrimaryDetails.OrganizationId);
             parameter.Add("@isactive",  user.UserPrimaryDetails.IsActive);
             parameter.Add("@createddate",  DateTime.UtcNow);
             parameter.Add("@createdby",  user.UserPrimaryDetails.CreateBy);                         
            return await dataAccess.QuerySingleAsync<int>("INSERT INTO dafconnectmaster.userorg (userid, organizationid, isactive, createddate, createdby) VALUES(@userid, @organizationid, @isactive, @createddate, @createdby) RETURNING userorgid",parameter);
           }
           catch(Exception ex)
           {
               throw ex;
           }                                  
       }
       public async Task<int> AddUserPreferences(AccountGenralPreferences userpreferences)
       {
           try
           {
            var parameter = new DynamicParameters();
             parameter.Add("@userorgid", userpreferences.UserorgId);
             parameter.Add("@languagemasterid", userpreferences.LanguageMasterId);
             parameter.Add("@timezoneid", userpreferences.Timezoneid);
             parameter.Add("@currencyid",  userpreferences.CurrencyId);
             parameter.Add("@unitid",  userpreferences.UnitId);
             parameter.Add("@vehicledisplayid",  userpreferences.VehicleDisplayId);
             parameter.Add("@dateformatid",  userpreferences.DateFormatId);
             parameter.Add("@isactive",  userpreferences.IsActive);
             parameter.Add("@createddate",  DateTime.UtcNow);
             parameter.Add("@createdby",  userpreferences.CreatedBy);             
            return await dataAccess.QuerySingleAsync<int>("INSERT INTO dafconnectmaster.usergeneralpreference (userorgid, languagemasterid, timezoneid, currencyid, unitid, vehicledisplayid, dateformatid, isactive, createddate, createdby) VALUES(@userorgid, @languagemasterid, @timezoneid, @currencyid, @unitid, @vehicledisplayid, @dateformatid, @isactive, @createddate, @createdby) RETURNING usergeneralpreferenceid",parameter);
           }
           catch(Exception ex)
           {
               throw ex;
           }                                  
       }
       public async Task<IEnumerable<Account>> GetUserDetails(int userid)
       {
                 var parameter = new DynamicParameters();
                 parameter.Add("@userid", userid);                          
                 return await dataAccess.QueryAsync<Account>("Select * from dafconnectmaster.user where userid=@userid ", parameter);     
                            
       }
       public async Task<int> DeleteUser(int userid,int loggeduser,bool IsActive)
        {            
                var parameter = new DynamicParameters();
                parameter.Add("@loggeduser", loggeduser);
                parameter.Add("@deleteuserid", userid);
                parameter.Add("@isactivate",IsActive);
                parameter.Add("@val", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                return await dataAccess.ExecuteScalarAsync<int>(@"CALL dafconnectmaster.deleteuser(@loggeduser, @deleteuserid,@isactivate,@val)", parameter);
                    
        }
        public async Task<int> UpdateUser(string firstname,string lastname,int updatedby,int userid)
        {            
                var parameter = new DynamicParameters();
                parameter.Add("@firstname", firstname);
                parameter.Add("@lastname", lastname);
                parameter.Add("@updatedby",updatedby);
                parameter.Add("@updateddate", DateTime.UtcNow);
                parameter.Add("@userid",userid);
                int resultDeleteuser = await dataAccess.ExecuteScalarAsync<int>("update dafconnectmaster.user set firstname = @firstname, lastname=@lastname , updatedby=@updatedby, updateddate= @updateddate where userid= @userid returning userid", parameter);
                return resultDeleteuser;     
        }
       public async Task<IEnumerable<Account>> GetUsers(int UsertypeID,bool IsActive)
       {
            try
            {
                 var parameter = new DynamicParameters();
                 parameter.Add("@usertypeid", UsertypeID);
                 parameter.Add("@isactive", IsActive);            
                 var resultGetUsers = await dataAccess.QueryAsync<Account>("Select * from dafconnectmaster.user where usertypeid=@usertypeid  and isactive=@isActive", parameter);                 
                 return resultGetUsers;
            }
            catch (Exception ex)
            {
                var err=ex.Message;
                throw ex;
            }
       }

       public async Task<bool> CheckEmailExist(string emailid)
       {
            try
            {
                 var parameter = new DynamicParameters();
                 parameter.Add("@emailid", emailid);                            
                 var resultGetUsers = await dataAccess.QueryAsync<Account>("Select * from dafconnectmaster.user where emailid=@emailid", parameter);                 
                 return resultGetUsers.Count() > 0;
            }
            catch (Exception ex)
            {
                var err=ex.Message;
                throw ex;
            }
       }
       public async Task<int> AddUserRoles(AccountRoleMapping userRoleMapping)
       {
           try
           {
            var parameter = new DynamicParameters();
             parameter.Add("@userorgid", userRoleMapping.Userorgid);
             parameter.Add("@rolemasterid", userRoleMapping.rolemasterid);
             parameter.Add("@isactive",  true);
             parameter.Add("@createddate",  DateTime.UtcNow);
             parameter.Add("@createdby",  userRoleMapping.CreatedBy); 
             parameter.Add("@rolestartdate",  userRoleMapping.RoleStartdate);
             parameter.Add("@roleenddate",  userRoleMapping.RoleEnddate);             
            return await dataAccess.ExecuteScalarAsync<int>("INSERT INTO dafconnectmaster.userorgrolemapping (userorgid, rolemasterid, isactive, createddate, createdby,  rolestartdate, roleenddate) VALUES(@userorgid, @rolemasterid, @isactive, @createddate, @createdby, @rolestartdate, @roleenddate) RETURNING userorgrolemappingid",parameter);
           }
           catch(Exception ex)
           {
               throw ex;
           }
       }

    //    public async Task<IEnumerable<AccountRoleMapping>> GetUserRoles(int Userorgid,bool IsActive)
    //    {
    //         try
    //         {
    //              var parameter = new DynamicParameters();
    //              parameter.Add("@Userorgid", Userorgid);
    //              parameter.Add("@isactive", IsActive);            
    //              var resultUserRoles = await dataAccess.QueryAsync<UserRoleMapping>("Select * from dafconnectmaster.userorgrolemapping where Userorgid=@Userorgid  and isactive=@isActive", parameter);                 
    //              return resultUserRoles;
    //         }
    //         catch (Exception ex)
    //         {
    //             var err=ex.Message;
    //             throw ex;
    //         }
    //    }


       public async Task<int> DeleteUserRole(int userorgrolemappingid,int UpdatedBy,bool IsActive)
       {
        try{
                 var parameter = new DynamicParameters();
                 parameter.Add("@userorgrolemappingid", userorgrolemappingid);
                 parameter.Add("@isactive", IsActive);
                 parameter.Add("@updatedby",  UpdatedBy);
                 parameter.Add("@updateddate",  DateTime.UtcNow);                
                 int resultdeleteUserGroup = await dataAccess.ExecuteScalarAsync<int>("update dafconnectmaster.userorgrolemapping set isactive=@isactive,updatedby=@updatedby,updateddate=@updateddate where userorgrolemappingid=@userorgrolemappingid RETURNING userorgrolemappingid", parameter);
                 return resultdeleteUserGroup;
            }
            catch (Exception ex)
            {
                var err=ex.Message;
                throw ex;
            }
       }
                
    }
}
