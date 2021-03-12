using System;
using System.Collections.Generic;
using AccountComponent = net.atos.daf.ct2.account;
using Preference = net.atos.daf.ct2.accountpreference;
using Group = net.atos.daf.ct2.group;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.group;

namespace net.atos.daf.ct2.accountservice
{
    public class Mapper
    {
        public AccountRequest ToAccount(AccountComponent.entity.Account account)
        {
            AccountRequest request = new AccountRequest();

            request.Id = account.Id;
            request.EmailId = account.EmailId;
            request.Salutation = account.Salutation;
            request.FirstName = account.FirstName;
            request.LastName = account.LastName;
            request.OrganizationId = account.Organization_Id;
            if (account.PreferenceId.HasValue)
                request.PreferenceId = account.PreferenceId.Value;
            if (account.BlobId.HasValue)
                request.BlobId = account.BlobId.Value;
            if (!string.IsNullOrEmpty(account.DriverId)) request.DriverId = account.DriverId;
            if (account.AccountType == AccountComponent.ENUM.AccountType.PortalAccount)
            {
                request.Type = Convert.ToString((char)AccountComponent.ENUM.AccountType.PortalAccount);
            }
            else
            {
                request.Type = Convert.ToString((char)AccountComponent.ENUM.AccountType.SystemAccount);
            }
            request.CreatedAt = (long)account.CreatedAt.Value;
            return request;
        }
        public AccountComponent.entity.Account ToAccountEntity(AccountRequest request)
        {
            var account = new AccountComponent.entity.Account();
            account.Id = request.Id;
            account.EmailId = request.EmailId;

            if (!string.IsNullOrEmpty(request.Type))
            {
                char type = Convert.ToChar(request.Type);
                if (type == 'p' || type == 'P')
                {
                    account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
                }
                else
                {
                    account.AccountType = AccountComponent.ENUM.AccountType.SystemAccount;
                }
            }
            else
            {
                account.AccountType = AccountComponent.ENUM.AccountType.PortalAccount;
            }
            account.Salutation = request.Salutation;
            account.FirstName = request.FirstName;
            account.LastName = request.LastName;
            account.Password = request.Password;
            account.Organization_Id = request.OrganizationId;
            account.DriverId = request.DriverId;
            if (request.StartDate > 0) account.StartDate = request.StartDate;
            if (request.EndDate > 0) account.EndDate = request.EndDate;            
            account.PreferenceId = request.PreferenceId;
            account.BlobId = request.BlobId;
            if(request.CreatedAt > 0)
            {
                account.CreatedAt = (long) request.CreatedAt;
            }
            else
            {
                account.CreatedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            }            
            return account;
        }
        public AccountComponent.entity.AccountBlob AccountBlob(AccountBlobRequest request)
        {
            AccountComponent.entity.AccountBlob accountBlob = new AccountComponent.entity.AccountBlob();

            accountBlob.Id = request.Id;
            accountBlob.AccountId = request.AccountId;
            accountBlob.Type = (AccountComponent.ImageType)Convert.ToChar(request.ImageType);
            if (request.Image != null)
                accountBlob.Image = request.Image.ToByteArray();
            return accountBlob;
        }
        public AccountBlobRequest AccountBlob(AccountComponent.entity.AccountBlob request)
        {
            AccountBlobRequest accountBlob = new AccountBlobRequest();
            accountBlob.Id = request.Id;
            accountBlob.AccountId = request.AccountId;
            accountBlob.ImageType = Convert.ToString((AccountComponent.ImageType)request.Type);
            if (request.Image != null)
                accountBlob.Image.CopyTo(request.Image, 0);
            return accountBlob;
        }

        public AccountRequest ToAccountDetail(AccountComponent.entity.Account account)
        {
            AccountRequest response = new AccountRequest();

            response.Id = account.Id;
            response.EmailId = account.EmailId;
            response.Salutation = account.Salutation;
            response.FirstName = account.FirstName;
            response.LastName = account.LastName;
            response.OrganizationId = account.Organization_Id;
            if (account.PreferenceId.HasValue)
                response.PreferenceId = account.PreferenceId.Value;
            if (account.BlobId.HasValue)
                response.BlobId = account.BlobId.Value;
            if (!string.IsNullOrEmpty(account.DriverId)) response.DriverId = account.DriverId;
            if (account.AccountType == AccountComponent.ENUM.AccountType.PortalAccount)
            {
                response.Type = Convert.ToString((char)AccountComponent.ENUM.AccountType.PortalAccount);
            }
            else
            {
                response.Type = Convert.ToString((char)AccountComponent.ENUM.AccountType.SystemAccount);
            }
            response.CreatedAt = (long)account.CreatedAt.Value;
            return response;
        }
        public accountpreference.AccountPreference ToPreference(AccountPreference request)
        {
            accountpreference.AccountPreference preference = new accountpreference.AccountPreference();
            preference.Id = request.Id;
            preference.RefId = request.RefId;
            preference.PreferenceType = Preference.PreferenceType.Account;
            preference.LanguageId = request.LanguageId;
            preference.TimezoneId = request.TimezoneId;
            preference.CurrencyId = request.CurrencyId;
            preference.UnitId = request.UnitId;
            preference.VehicleDisplayId = request.VehicleDisplayId;
            preference.DateFormatTypeId = request.DateFormatId;            
            preference.TimeFormatId = request.TimeFormatId;
            preference.LandingPageDisplayId = request.LandingPageDisplayId;
            return preference;
        }
        public AccountPreference ToPreferenceEntity(Preference.AccountPreference entity)
        {
            AccountPreference request = new AccountPreference();
            request.Id = entity.Id.HasValue ? entity.Id.Value : 0;
            request.RefId = entity.RefId;
            request.LanguageId = entity.LanguageId;
            request.TimezoneId = entity.TimezoneId;
            request.CurrencyId = entity.CurrencyId;
            request.UnitId = entity.UnitId;
            request.VehicleDisplayId = entity.VehicleDisplayId;
            request.DateFormatId = entity.DateFormatTypeId;            
            request.TimeFormatId = entity.TimeFormatId;
            request.LandingPageDisplayId = entity.LandingPageDisplayId;
            return request;
        }
        // group mapping
        public Group.Group ToGroup(AccountGroupRequest request)
        {
            Group.Group entity = new Group.Group();
            entity.Id = request.Id;
            entity.Name = request.Name;            
            entity.Description = request.Description;
            entity.Argument = "";//request.Argument;
            entity.FunctionEnum = ToFunctionMethod(request.FunctionEnum);            
            entity.RefId = null;
            if (request.RefId >0) entity.RefId = request.RefId;
            entity.GroupType = ToGroupType(request.GroupType);
                      
            entity.ObjectType = group.ObjectType.AccountGroup;
            entity.OrganizationId = request.OrganizationId;
            if (request.CreatedAt > 0)
            {
                entity.CreatedAt = (long)request.CreatedAt;
            }
            else
            {
                entity.CreatedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            }
            entity.GroupRef = new List<Group.GroupRef>();            
            return entity;
        }
        public AccessRelationship ToAccessRelationShip(AccountComponent.entity.AccessRelationship entity)
        {
            AccessRelationship request = new AccessRelationship();
            request.Id = entity.Id;
            request.AccessRelationType = entity.AccessRelationType.ToString();
            request.AccountGroupId = entity.AccountGroupId;
            request.VehicleGroupId = entity.VehicleGroupId;
            return request;
        }
        public AccountGroupRequest ToAccountGroup(Group.Group entity)
        {
            AccountGroupRequest request = new AccountGroupRequest();
            request.Id = entity.Id;
            request.Name = entity.Name;
            request.Description = entity.Description;
            //request.Argument = entity.Argument;
            //request.FunctionEnum = (FunctionEnum)Enum.Parse(typeof(FunctionEnum), entity.FunctionEnum.ToString());
            //request.FunctionEnum = entity.FunctionEnum.ToString();
            request.GroupType = entity.GroupType.ToString();            
            //request.ObjectType = (ObjectType)Enum.Parse(typeof(ObjectType), entity.ObjectType.ToString());
            //request.ObjectType = entity.ObjectType.ToString();
            request.OrganizationId = entity.OrganizationId;
            request.GroupRefCount = entity.GroupRefCount;
            request.CreatedAt = entity.CreatedAt.Value;
            if (entity.GroupRef != null)
            {
                foreach (var item in entity.GroupRef)
                {
                    request.GroupRef.Add(new AccountGroupRef() { RefId = item.Ref_Id, GroupId = item.Group_Id });
                }
            }
            return request;
        }
        private FunctionEnum ToFunctionMethod(string methodName)
        {
            FunctionEnum functionMethod = FunctionEnum.None;
            if (string.IsNullOrEmpty(methodName)) return functionMethod;
            if(methodName.Length > 1) return functionMethod;
            functionMethod = (FunctionEnum)Convert.ToChar(methodName);
            return functionMethod;
        }
        private GroupType ToGroupType(string groupType)
        {
            GroupType groupTypeEnum = GroupType.None;
            if (string.IsNullOrEmpty(groupType)) return groupTypeEnum;            
            groupTypeEnum = (GroupType)Convert.ToChar(groupType);            
            return groupTypeEnum;
        }

        public Group.Group ToGroupObject(Group.GroupType groupType, Group.ObjectType objectType, string Argument, 
            Group.FunctionEnum functionEnum, int RefId, string groupName, string description, long createAt)
        {
            // create vehicle group with vehicle
            Group.Group group = new Group.Group();
            group.GroupType = groupType;
            group.ObjectType = objectType;
            group.Argument = Argument;
            group.FunctionEnum = functionEnum;
            group.RefId = RefId;
            group.Description = description;
            group.CreatedAt = createAt;            
            group.Name = groupName;
            return group;
        }

        //private AccountType SetEnumAccountType(AccountComponent.ENUM.AccountType type)
        //{
        //    AccountType accountType = AccountType.None;

        //    if (type == AccountComponent.ENUM.AccountType.None)
        //    {
        //        accountType = AccountType.None;
        //    }
        //    else if (type == AccountComponent.ENUM.AccountType.SystemAccount)
        //    {
        //        accountType = AccountType.SystemAccount;
        //    }
        //    else if (type == AccountComponent.ENUM.AccountType.PortalAccount)
        //    {
        //        accountType = AccountType.PortalAccount;
        //    }
        //    return accountType;
        //}
        // private AccountComponent.ENUM.AccountType GetEnum(int value)
        // {
        //     AccountComponent.ENUM.AccountType accountType;
        //     switch (value)
        //     {
        //         case 0:
        //             accountType = AccountComponent.ENUM.AccountType.None;
        //             break;
        //         case 1:
        //             accountType = AccountComponent.ENUM.AccountType.SystemAccount;
        //             break;
        //         case 2:
        //             accountType = AccountComponent.ENUM.AccountType.PortalAccount;
        //             break;
        //         default:
        //             accountType = AccountComponent.ENUM.AccountType.PortalAccount;
        //             break;
        //     }
        //     return accountType;
        // }
    }
}
