// // using System;
// // using System.Collections.Generic;
// // using System.Data;
// // using System.Linq;
// // using Microsoft.Extensions.Configuration;
// // using net.atos.daf.ct2.account.entity;
// // using net.atos.daf.ct2.data;
// // using Dapper;
// // using System.Threading.Tasks;

// // namespace net.atos.daf.ct2.account.repository
// // {
// //     public class AccountGroupRepository
// //     {
// //         private readonly IDataAccess dataAccess;
// //         readonly string languagecode = "EN-GB";
        
// //         public AccountGroupRepository(IDataAccess _dataAccess) 
// //         {
// //            dataAccess=_dataAccess;
// //         }
       
// //         public async Task<Account> Create(Account account)       
// //         {
// //             try
// //             {
// //                 var parameter = new DynamicParameters();
// //                 parameter.Add("@id", account.Id);
// //                 parameter.Add("@group_type", (char) group.GroupType);
// //                 parameter.Add("@argument", group.Argument);
// //                 parameter.Add("@function_enum", (char) group.FunctionEnum);
// //                 parameter.Add("@organization_id", group.OrganizationId);
// //                 parameter.Add("@ref_id", group.RefId);
// //                 parameter.Add("@name", group.Name);
// //                 parameter.Add("@description", group.Description);
// //                 string query= "insert into master.group(object_type, group_type, argument, function_enum, organization_id, ref_id, name, description) " +
// //                               "values(@object_type, @group_type, @argument, @function_enum, @organization_id, @ref_id, @name, @description) RETURNING id";

// //                 var groupid =   await dataAccess.ExecuteScalarAsync<int>(query, parameter);
                
// //                 group.Id = groupid;
// //             }
// //             catch (Exception ex)
// //             {
// //                 throw ex;
// //             }
// //             return group;
// //         }
// //     }
// // }
        

// //     //    public int AddUserGroup(AccountGroup usergroup)
// //     //    {

// //     //     try{
// //     //              var parameter = new DynamicParameters();
// //     //              parameter.Add("@organizationid", usergroup.organizationId);
// //     //              parameter.Add("@name", usergroup.Name);
// //     //              parameter.Add("@isactive", usergroup.IsActive);
// //     //              parameter.Add("@createdby",  usergroup.CreatedBy);
// //     //              parameter.Add("@createddate",  DateTime.UtcNow);
// //     //              parameter.Add("@isuserdefinedgroup",usergroup.Isuserdefinedgroup);
// //     //              int resultAddUserGroup =dataAccess.Execute("INSERT INTO dafconnectmaster.usergroup  (organizationid,name,isactive,createdby,createddate,isuserdefinedgroup) VALUES(@organizationid,@name,@isactive,@createdby,@createddate,@isuserdefinedgroup)", parameter);
// //     //              return resultAddUserGroup;
// //     //         }
// //     //         catch (Exception ex)
// //     //         {
// //     //             var err=ex.Message;
// //     //             throw ex;
// //     //         }
              
// //     //    }

// //     //    public int UpdateUserGroup(AccountGroup usergroup)
// //     //    {
// //     //     try{
// //     //              var parameter = new DynamicParameters();
// //     //              parameter.Add("@usergroupid", usergroup.UsergroupId);
// //     //              parameter.Add("@organizationid", usergroup.organizationId);
// //     //              parameter.Add("@name", usergroup.Name);
// //     //              parameter.Add("@isactive", usergroup.IsActive);
// //     //              parameter.Add("@updatedby",  usergroup.UpdatedBy);
// //     //              parameter.Add("@updateddate",  DateTime.UtcNow);                
// //     //              int resultUpdateUserGroup =dataAccess.Execute("update dafconnectmaster.usergroup set name=@name,updatedby=@updatedby,updateddate=@updateddate where @organizationid=@organizationid and usergroupid=@usergroupid", parameter);
// //     //              return resultUpdateUserGroup;
// //     //         }
// //     //         catch (Exception ex)
// //     //         {
// //     //             var err=ex.Message;
// //     //             throw ex;
// //     //         }
// //     //    }

// //     //    public int DeleteUserGroup(int usergroupId,int UpdatedBy,bool IsActive)
// //     //    {
// //     //     try{
// //     //              var parameter = new DynamicParameters();
// //     //              parameter.Add("@usergroupid", usergroupId);
// //     //              parameter.Add("@isactive", IsActive);
// //     //              parameter.Add("@updatedby",  UpdatedBy);
// //     //              parameter.Add("@updateddate",  DateTime.UtcNow);                
// //     //              int resultdeleteUserGroup =dataAccess.Execute("update dafconnectmaster.usergroup set isactive=@isactive,updatedby=@updatedby,updateddate=@updateddate where usergroupid=@usergroupid", parameter);
// //     //              return resultdeleteUserGroup;
// //     //         }
// //     //         catch (Exception ex)
// //     //         {
// //     //             var err=ex.Message;
// //     //             throw ex;
// //     //         }
// //     //    }
// //     //    public IEnumerable<AccountGroup> GetUserGroups(int organizationId,bool IsActive)
// //     //    {
// //     //     try{
// //     //              var parameter = new DynamicParameters();
// //     //              parameter.Add("@organizationId", organizationId);
// //     //              parameter.Add("@isactive", IsActive);            
// //     //              var resultGetUserGroup =dataAccess.Query<AccountGroup>("Select * from dafconnectmaster.usergroup where organizationId=@organizationId  and isactive=@isActive", parameter);
                       
// //     //              return resultGetUserGroup;
// //     //         }
// //     //         catch (Exception ex)
// //     //         {
// //     //             var err=ex.Message;
// //     //             throw ex;
// //     //         }
// //     //    }
// //     //    public IEnumerable<AccountGroup> GetUserGroupDetails(int UserGroupID,int organizationId)
// //     //    {
// //     //     try{
// //     //              var parameter = new DynamicParameters();
// //     //              parameter.Add("@organizationid", organizationId);
// //     //              parameter.Add("@usergroupid", UserGroupID);
                     
// //     //              var resultGetUserGroup =dataAccess.Query<AccountGroup>("Select * from dafconnectmaster.usergroup where usergroupid=@UserGroupID and organizationid=@organizationid", parameter);
                       
// //     //              return resultGetUserGroup;
// //     //         }
// //     //         catch (Exception ex)
// //     //         {
// //     //             var err=ex.Message;
// //     //             throw ex;
// //     //         }
// //     //    }

// //     //    public async Task<int> AddUserGroupRoles(AccountGroupRole userRoleMapping)
// //     //    {
// //     //        try
// //     //        {
// //     //         var parameter = new DynamicParameters();
// //     //         // parameter.Add("@usergroupid", userRoleMapping.UserGroupID);
// //     //          parameter.Add("@usergroupid", userRoleMapping.UserGroupID);
// //     //          parameter.Add("@rolemasterid", userRoleMapping.rolemasterid);
// //     //          parameter.Add("@isactive",  true);
// //     //          parameter.Add("@createddate",  DateTime.UtcNow);
// //     //          parameter.Add("@createdby",  userRoleMapping.CreatedBy); 
// //     //          parameter.Add("@rolestartdate",  userRoleMapping.RoleStartdate);
// //     //          parameter.Add("@roleenddate",  userRoleMapping.RoleEnddate);             
// //     //         return await dataAccess.ExecuteScalarAsync<int>("INSERT INTO dafconnectmaster.usergrouprolemapping (usergroupid, rolemasterid, isactive, createddate, createdby,  rolestartdate, roleenddate) VALUES(@usergroupid, @rolemasterid, @isactive, @createddate, @createdby, @rolestartdate, @roleenddate) RETURNING usergrouprolemappingid",parameter);
// //     //        }
// //     //        catch(Exception ex)
// //     //        {
// //     //            throw ex;
// //     //        }
// //     //    }

// //     //    public async Task<IEnumerable<AccountGroupRole>> GetUserGroupRoles(int UsergroupId,bool IsActive)
// //     //    {
// //     //         try
// //     //         {
// //     //              var parameter = new DynamicParameters();
// //     //              parameter.Add("@usergroupid", UsergroupId);
// //     //              parameter.Add("@isactive", IsActive);            
// //     //              var resultUserGroupRoles = await dataAccess.QueryAsync<AccountGroupRole>("Select * from dafconnectmaster.usergrouprolemapping where usergroupid=@usergroupid  and isactive=@isActive", parameter);                 
// //     //              return resultUserGroupRoles;
// //     //         }
// //     //         catch (Exception ex)
// //     //         {
// //     //             var err=ex.Message;
// //     //             throw ex;
// //     //         }
// //     //    }

// //     //    public async Task<int> DeleteUserGroupRole(int UserGroupRolemappingid,int UpdatedBy,bool IsActive)
// //     //    {
// //     //     try{
// //     //              var parameter = new DynamicParameters();
// //     //              parameter.Add("@usergrouprolemappingid", UserGroupRolemappingid);
// //     //              parameter.Add("@isactive", IsActive);
// //     //              parameter.Add("@updatedby",  UpdatedBy);
// //     //              parameter.Add("@updateddate",  DateTime.UtcNow);                
// //     //              int resultdeleteUserGroup = await dataAccess.ExecuteScalarAsync<int>("update dafconnectmaster.usergrouprolemapping set isactive=@isactive,updatedby=@updatedby,updateddate=@updateddate where usergrouprolemappingid=@usergrouprolemappingid RETURNING UserGroupRolemappingid", parameter);
// //     //              return resultdeleteUserGroup;
// //     //         }
// //     //         catch (Exception ex)
// //     //         {
// //     //             var err=ex.Message;
// //     //             throw ex;
// //     //         }
// //     //    }
   
