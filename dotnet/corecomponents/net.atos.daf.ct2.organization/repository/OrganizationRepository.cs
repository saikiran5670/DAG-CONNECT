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
    //    public async Task<IEnumerable<Organization>> Get(string organizationId)
    //     {
    //         try
    //         {
    //             var parameter = new DynamicParameters();
    //             var query = @"SELECT id, org_id, type, name, address_type, street, street_number, postal_code, city, country_code, reference_date, optout_status, optout_status_changed_date, is_active
	//                         FROM master.organization where org_id=@organizationId";
    //             parameter.Add("@organizationId", organizationId);
    //             var organization = await dataAccess.QueryAsync<Organization>(query, parameter);
    //             return organization.ToList();
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }
        public async Task<IEnumerable<Organization>> Get(int organizationId)
        {
            try
            {                
                var parameter = new DynamicParameters();
                var query = @"SELECT id, org_id, type, name, address_type, street, street_number, postal_code, city, country_code, reference_date, optout_status, optout_status_changed_date, is_active
	                        FROM master.organization where id=@Id";
                parameter.Add("@Id", organizationId);
                IEnumerable<Organization> OrganizationDetails = await dataAccess.QueryAsync<Organization>(query, parameter);
                return OrganizationDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
