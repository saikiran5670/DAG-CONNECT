using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.organization.entity;

namespace net.atos.daf.ct2.organization.repository
{
    public class OrganizationRepository:IOrganizationRepository
    {
        private readonly IDataAccess dataAccess;
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public OrganizationRepository(IDataAccess _dataAccess)
        {
            dataAccess = _dataAccess;
        }
        public async Task<Organization> Create(Organization organization)       
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@OrganizationId",organization.OrganizationId);
                parameter.Add("@OrganizationType",organization.Type);
                parameter.Add("@Name", organization.Name);
                parameter.Add("@AddressType",organization.AddressType);
                parameter.Add("@AddressStreet", organization.AddressStreet);
                parameter.Add("@AddressStreetNumber", organization.AddressStreetNumber);
                parameter.Add("@PostalCode", organization.PostalCode);  
                parameter.Add("@City", organization.City);
                parameter.Add("@CountryCode", organization.CountryCode);    
                parameter.Add("@ReferencedDate", organization.ReferencedDate);               
                parameter.Add("@OptOutStatus", organization.OptOutStatus);
                parameter.Add("@OptOutStatusChangedDate", organization.OptOutStatusChangedDate);
                parameter.Add("@IsActive", organization.IsActive);               

                string query= "insert into master.organization(org_id, type, name, address_type, street, street_number, postal_code, city,country_code,reference_date,optout_status,optout_status_changed_date,is_active) " +
                              "values(@OrganizationId, @OrganizationType, @Name, @AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode,@ReferencedDate, @OptOutStatus,@OptOutStatusChangedDate,@IsActive) RETURNING id";

                var orgid =   await dataAccess.ExecuteScalarAsync<int>(query, parameter);                
                organization.Id = orgid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return organization;
        }

        public async Task<bool> Delete(int organizationId)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", organizationId);
                var query = @"delete from master.organization where id=@id";
                await dataAccess.ExecuteScalarAsync<int>(query, parameter);    
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public async Task<Organization> Update(Organization organization)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Id",organization.Id);
                parameter.Add("@OrganizationId",organization.OrganizationId);
                parameter.Add("@OrganizationType",organization.Type);
                parameter.Add("@Name", organization.Name);
                parameter.Add("@AddressType",organization.AddressType);
                parameter.Add("@AddressStreet", organization.AddressStreet);
                parameter.Add("@AddressStreetNumber", organization.AddressStreetNumber);
                parameter.Add("@PostalCode", organization.PostalCode);  
                parameter.Add("@City", organization.City);
                parameter.Add("@CountryCode", organization.CountryCode);    
                parameter.Add("@ReferencedDate", organization.ReferencedDate);               
                parameter.Add("@OptOutStatus", organization.OptOutStatus);
                parameter.Add("@OptOutStatusChangedDate", organization.OptOutStatusChangedDate);
                parameter.Add("@IsActive", organization.IsActive);  

                var query = @"update master.organization set type=@OrganizationType, name=@Name,
                 address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
                  postal_code=@PostalCode, city=@City,country_code=@CountryCode,reference_date=@ReferencedDate,
                  optout_status=@OptOutStatus,optout_status_changed_date=@OptOutStatusChangedDate,is_active=@IsActive
	                                 WHERE id = @Id RETURNING id;";
                var groupid = await dataAccess.ExecuteScalarAsync<int>(query, parameter);              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return organization;
        }
        public async Task<Organization> Get(int organizationId)
        {
            try
            {                
                var parameter = new DynamicParameters();
                var query = @"SELECT id, org_id, type, name, address_type, street, street_number, postal_code, city, country_code, reference_date, optout_status, optout_status_changed_date, is_active
	                        FROM master.organization where id=@Id";
                parameter.Add("@Id", organizationId);
                IEnumerable<Organization> OrganizationDetails = await dataAccess.QueryAsync<Organization>(query, parameter);
                Organization objOrganization=new Organization();
                foreach (var item in OrganizationDetails)
                    {         
                         objOrganization.Id=item.Id;
                         objOrganization.OrganizationId=item.OrganizationId;
                         objOrganization.Type=item.Type;
                         objOrganization.Name=item.Name;
                         objOrganization.AddressType=item.AddressType;
                         objOrganization.AddressStreet=item.AddressStreet;
                         objOrganization.PostalCode=item.PostalCode;
                         objOrganization.City=item.City;
                         objOrganization.CountryCode=item.CountryCode;
                         objOrganization.ReferencedDate=item.ReferencedDate;
                         objOrganization.OptOutStatus=item.OptOutStatus;
                         objOrganization.OptOutStatusChangedDate=item.OptOutStatusChangedDate;
                         objOrganization.IsActive=item.IsActive;
                    }            
                return objOrganization;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CreateVehicleParty(List<Organization> organization)
        {
            foreach (var item in organization)
            {
                string Org_Id = await dataAccess.QuerySingleAsync<string>("SELECT org_id FROM master.organization where org_id=@org_id", new { org_id = item.OrganizationId });
                if(!string.IsNullOrEmpty(Org_Id)){
                    await Create(item);
                }
                else{
                    await Update(item);
                }
            }
            return 0;
        }
    }
}
