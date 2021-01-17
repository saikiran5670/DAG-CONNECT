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
using net.atos.daf.ct2.audit.Enum;
using  net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehiclerepository;


namespace net.atos.daf.ct2.organization.repository
{
    public class OrganizationRepository:IOrganizationRepository
    {
        private readonly IDataAccess dataAccess;
        private readonly IVehicleManager _vehicelManager;
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public OrganizationRepository(IDataAccess _dataAccess,IVehicleManager vehicleManager)
        {
            dataAccess = _dataAccess;
           _vehicelManager= vehicleManager;
        }
          
        public async Task<Organization> Create(Organization organization)       
        {
           log.Info("Create Organization method called in repository");
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
                log.Info("Create Organization method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organization));
                log.Error(ex.ToString());
                throw ex;
            }
            return organization;
        }

        public async Task<bool> Delete(int organizationId)
        {
            log.Info("Delete Organization method called in repository");            
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
                log.Info("Delete Organization method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());
                throw ex;
            }            
        }

        public async Task<Organization> Update(Organization organization)
        {
            log.Info("Update Organization method called in repository");     
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
                log.Info("Update Organization method in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());
                throw ex;
            }
            return organization;
        }
        public async Task<Organization> Get(int organizationId)
        {
            log.Info("Get Organization method called in repository");     
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
                log.Info("Get Organization method in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());
                throw ex;
            }
        }
        // public async Task<Organization> UpdateCustomer(Organization organization)
        // {
        //     log.Info("Update Customer method called in repository"); 
        //     try
        //     {
        //         var parameter = new DynamicParameters();
        //         parameter.Add("@OrganizationId", organization.OrganizationId);
        //         var query = @"master.organization where org_id=@OrganizationId";
        //         int iscustomerexist= await dataAccess.ExecuteScalarAsync<int>(query, parameter);  

        //     if (iscustomerexist>0)
        //         {
        //         var parameterUpdate = new DynamicParameters();
        //         parameterUpdate.Add("@Id",organization.Id);
        //         parameterUpdate.Add("@OrganizationId",organization.OrganizationId);
        //         parameterUpdate.Add("@OrganizationType",organization.Type);
        //         parameterUpdate.Add("@Name", organization.Name);
        //         parameterUpdate.Add("@AddressType",organization.AddressType);
        //         parameterUpdate.Add("@AddressStreet", organization.AddressStreet);
        //         parameterUpdate.Add("@AddressStreetNumber", organization.AddressStreetNumber);
        //         parameterUpdate.Add("@PostalCode", organization.PostalCode);  
        //         parameterUpdate.Add("@City", organization.City);
        //         parameterUpdate.Add("@CountryCode", organization.CountryCode);    
        //         parameterUpdate.Add("@ReferencedDate", organization.ReferencedDate);               
        //         parameterUpdate.Add("@OptOutStatus", organization.OptOutStatus);
        //         parameterUpdate.Add("@OptOutStatusChangedDate", organization.OptOutStatusChangedDate);
        //         parameterUpdate.Add("@IsActive", organization.IsActive);  

        //         var queryUpdate = @"update master.organization set type=@OrganizationType, name=@Name,
        //          address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
        //           postal_code=@PostalCode, city=@City,country_code=@CountryCode,reference_date=@ReferencedDate,
        //           optout_status=@OptOutStatus,optout_status_changed_date=@OptOutStatusChangedDate,is_active=@IsActive
	    //                              WHERE id = @Id RETURNING id;";
        //         await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameterUpdate);      
        //         }
        //     else
        //     {                     
        //         var parameterInsert = new DynamicParameters();
        //         parameterInsert.Add("@OrganizationId",organization.OrganizationId);
        //         parameterInsert.Add("@OrganizationType",organization.Type);
        //         parameterInsert.Add("@Name", organization.Name);
        //         parameterInsert.Add("@AddressType",organization.AddressType);
        //         parameterInsert.Add("@AddressStreet", organization.AddressStreet);
        //         parameterInsert.Add("@AddressStreetNumber", organization.AddressStreetNumber);
        //         parameterInsert.Add("@PostalCode", organization.PostalCode);  
        //         parameterInsert.Add("@City", organization.City);
        //         parameterInsert.Add("@CountryCode", organization.CountryCode);    
        //         parameterInsert.Add("@ReferencedDate", organization.ReferencedDate);               
        //         parameterInsert.Add("@OptOutStatus", organization.OptOutStatus);
        //         parameterInsert.Add("@OptOutStatusChangedDate", organization.OptOutStatusChangedDate);
        //         parameterInsert.Add("@IsActive", organization.IsActive);               

        //         string queryInsert= "insert into master.organization(org_id, type, name, address_type, street, street_number, postal_code, city,country_code,reference_date,optout_status,optout_status_changed_date,is_active) " +
        //                       "values(@OrganizationId, @OrganizationType, @Name, @AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode,@ReferencedDate, @OptOutStatus,@OptOutStatusChangedDate,@IsActive) RETURNING id";

        //         var orgid =   await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameterInsert);                
        //         organization.Id = orgid;
        //     } 
        //     }    
            
        //     catch (Exception ex)
        //     {
        //         log.Info("Update Customer method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
        //         log.Error(ex.ToString()); 
        //         throw ex;
        //     }
        //     return organization;
        //}

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            log.Info("Update Customer method called in repository"); 
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@org_id", customer.CompanyUpdatedEvent.Company.ID);
                //var query = @"Select org_id from master.organization where org_id=@org_id";
                 var query = @"SELECT org_id FROM master.organization where org_id=@org_id";
                int iscustomerexist= await dataAccess.ExecuteScalarAsync<int>(query, parameter);  
                      
        
               if (iscustomerexist>0)
                 {
                var parameterUpdate = new DynamicParameters();
                parameterUpdate.Add("@org_id", customer.CompanyUpdatedEvent.Company.ID);
                parameterUpdate.Add("@Name",  customer.CompanyUpdatedEvent.Company.Name);
                parameterUpdate.Add("@Type",  customer.CompanyUpdatedEvent.Company.type);
                parameterUpdate.Add("@AddressType", customer.CompanyUpdatedEvent.Company.Address.Type);
                parameterUpdate.Add("@AddressStreet", customer.CompanyUpdatedEvent.Company.Address.Street);
                parameterUpdate.Add("@AddressStreetNumber", customer.CompanyUpdatedEvent.Company.Address.StreetNumber);
                parameterUpdate.Add("@PostalCode", customer.CompanyUpdatedEvent.Company.Address.PostalCode);  
                parameterUpdate.Add("@City", customer.CompanyUpdatedEvent.Company.Address.City);
                parameterUpdate.Add("@CountryCode", customer.CompanyUpdatedEvent.Company.Address.CountryCode);    
              //  parameterUpdate.Add("@ReferencedDate", customer.CompanyUpdatedEvent.Company.ReferenceDateTime != null ? UTCHandling.GetUTCFromDateTime(customer.CompanyUpdatedEvent.Company.ReferenceDateTime.ToString()) : 0);    
               // reference_date=@ReferencedDate    
                var queryUpdate = @"update master.organization set name=@Name,type=@Type,
                 address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
                  postal_code=@PostalCode, city=@City,country_code=@CountryCode                               
	                                 WHERE org_id = @org_id RETURNING org_id;";
                await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameterUpdate);      
            }           
        
            else
            {                     
                var parameterInsert = new DynamicParameters();
                parameterInsert.Add("@org_id", customer.CompanyUpdatedEvent.Company.ID);
                parameterInsert.Add("@Name",  customer.CompanyUpdatedEvent.Company.Name);
                parameterInsert.Add("@Type",  customer.CompanyUpdatedEvent.Company.type);
                parameterInsert.Add("@AddressType", customer.CompanyUpdatedEvent.Company.Address.Type);
                parameterInsert.Add("@AddressStreet", customer.CompanyUpdatedEvent.Company.Address.Street);
                parameterInsert.Add("@AddressStreetNumber", customer.CompanyUpdatedEvent.Company.Address.StreetNumber);
                parameterInsert.Add("@PostalCode", customer.CompanyUpdatedEvent.Company.Address.PostalCode);  
                parameterInsert.Add("@City", customer.CompanyUpdatedEvent.Company.Address.City);
                parameterInsert.Add("@CountryCode", customer.CompanyUpdatedEvent.Company.Address.CountryCode);    
               //  parameterUpdate.Add("@ReferencedDate", customer.CompanyUpdatedEvent.Company.ReferenceDateTime != null ? UTCHandling.GetUTCFromDateTime(customer.CompanyUpdatedEvent.Company.ReferenceDateTime.ToString()) : 0);                
               
                string queryInsert= "insert into master.organization(org_id, name,type ,address_type, street, street_number, postal_code, city,country_code,reference_date) " +
                              "values(@org_id, @Name,@Type ,@AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode,44444455) RETURNING org_id";

                var orgid =   await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameterInsert);                
               // organization.Id = orgid;
            } 
           }        
            catch (Exception ex)
            {
                log.Info("Update Customer method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString()); 
                throw ex;
            }
            return customer;
        }


        public async Task<KeyHandOver> KeyHandOverEvent(KeyHandOver keyHandOver)
        {
             // first check organization is exist or not
            // if exist then update the details in organization and if VIN is exist then update in vehicle table
            // if not exist then first create organization and map the organizationid to VIN 
            // if organization and VIN both already exist in system then update thier details
         
           // KeyHandOverMapping myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
           
            log.Info("KeyHandOverEvent method called in repository"); 
            //bool isSuccess=false;
            try
            {
                //int vehId= await _vehicelManager.Update(keyHandOver.KeyHandOverEvent.EndCustomer.ID,keyHandOver.KeyHandOverEvent.VIN,keyHandOver.KeyHandOverEvent.TCUActivation, keyHandOver.KeyHandOverEvent.ReferenceDateTime);
                int vehId=0;


                // var parameterOrg = new DynamicParameters();
                // parameterOrg.Add("@org_id", keyHandOver.KeyHandOverEvent.EndCustomer.ID);
                // var queryOrg = @"SELECT org_id from master.organization where org_id=@org_id";
                // int isCustomerExist= await dataAccess.ExecuteScalarAsync<int>(queryOrg, parameterOrg); 
                 
                // var parameterVeh = new DynamicParameters();
                // parameterVeh.Add("@vin", keyHandOver.KeyHandOverEvent.VIN);
                // var queryVeh = @"SELECT vin from master.vehicle where vin=@vin";
                // int isVINExist= await dataAccess.ExecuteScalarAsync<int>(queryVeh, parameterVeh);  
                // return keyHandOver;
      
            //    if (isCustomerExist>0 && isVINExist>0)  // Update organization and vehicle
            //     {                    
            //     var parameterOrgUpdate = new DynamicParameters();
            //     parameterOrgUpdate.Add("@org_id",keyHandOver.KeyHandOverEvent.EndCustomer.ID);               
            //     parameterOrgUpdate.Add("@Name", keyHandOver.KeyHandOverEvent.EndCustomer.Name);              
            //     parameterOrgUpdate.Add("@AddressType",keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type);
            //     parameterOrgUpdate.Add("@AddressStreet", keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street);
            //     parameterOrgUpdate.Add("@AddressStreetNumber",keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber);
            //     parameterOrgUpdate.Add("@PostalCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode);  
            //     parameterOrgUpdate.Add("@City",keyHandOver.KeyHandOverEvent.EndCustomer.Address.City);
            //     parameterOrgUpdate.Add("@CountryCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode);    
            //    // parameterOrgUpdate.Add("@ReferencedDate", keyHandOver.ReferenceDateTime);   

            //     var queryOrgUpdate = @"update master.organization set name=@Name,
            //      address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
            //       postal_code=@PostalCode,city=@City,country_code=@CountryCode                 
	        //                          WHERE org_id = @org_id RETURNING id;";

               // await dataAccess.ExecuteScalarAsync<int>(queryOrgUpdate, parameterOrgUpdate);    

                //update vehicle
               // int vehId= await _vehicelManager.Update(keyHandOver.KeyHandOverEvent.EndCustomer.ID,keyHandOver.KeyHandOverEvent.VIN,keyHandOver.KeyHandOverEvent.TCUActivation, keyHandOver.KeyHandOverEvent.ReferenceDateTime);
                
            
            // string vin="V22";
            // string tcuId="V22Manager";
            // string tcuactivation="1";
            // string referenceDateTime="04-04-2019";
           // var resultUpdatevehicle = _vehicelManager.Update(vin,tcuId,tcuactivation,referenceDateTime).Result;

            // }                
            //     else if (isCustomerExist<0 && isVINExist<0)  // Insert organization and vehicle
            //     {
            //     var parameterOrgInsert = new DynamicParameters();
            //     parameterOrgInsert.Add("@org_id",keyHandOver.KeyHandOverEvent.EndCustomer.ID);               
            //     parameterOrgInsert.Add("@Name", keyHandOver.KeyHandOverEvent.EndCustomer.Name);              
            //     parameterOrgInsert.Add("@AddressType",keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type);
            //     parameterOrgInsert.Add("@AddressStreet", keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street);
            //     parameterOrgInsert.Add("@AddressStreetNumber",keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber);
            //     parameterOrgInsert.Add("@PostalCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode);  
            //     parameterOrgInsert.Add("@City",keyHandOver.KeyHandOverEvent.EndCustomer.Address.City);
            //     parameterOrgInsert.Add("@CountryCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode);    
            //    // parameterOrgUpdate.Add("@ReferencedDate", keyHandOver.ReferenceDateTime);   
                          
            //     string queryOrgInsert= "insert into master.organization(org_id,type, name, address_type, street, street_number, postal_code, city,country_code) " +
            //                   "values(@org_id, 'Dealer', @Name, @AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode) RETURNING id";

            //     var orgid =   await dataAccess.ExecuteScalarAsync<int>(queryOrgInsert, parameterOrgInsert);     
              
            //     // Insert vehicle
            //      int vehId= await _vehicelManager.Create(orgid,keyHandOver.KeyHandOverEvent.EndCustomer.ID,keyHandOver.KeyHandOverEvent.VIN,keyHandOver.KeyHandOverEvent.TCUActivation, keyHandOver.KeyHandOverEvent.ReferenceDateTime);

            //     return keyHandOver;               
            //    }       

            //     else if (isCustomerExist>0 && isVINExist<0) // Update organization and insert vehicle
            //     {
            //      var parameterOrgUpdate = new DynamicParameters();
            //     parameterOrgUpdate.Add("@org_id",keyHandOver.KeyHandOverEvent.EndCustomer.ID);               
            //     parameterOrgUpdate.Add("@Name", keyHandOver.KeyHandOverEvent.EndCustomer.Name);              
            //     parameterOrgUpdate.Add("@AddressType",keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type);
            //     parameterOrgUpdate.Add("@AddressStreet", keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street);
            //     parameterOrgUpdate.Add("@AddressStreetNumber",keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber);
            //     parameterOrgUpdate.Add("@PostalCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode);  
            //     parameterOrgUpdate.Add("@City",keyHandOver.KeyHandOverEvent.EndCustomer.Address.City);
            //     parameterOrgUpdate.Add("@CountryCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode);    
            //    // parameterOrgUpdate.Add("@ReferencedDate", keyHandOver.ReferenceDateTime);   

            //     var queryOrgUpdate = @"update master.organization set name=@Name,
            //      address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
            //       postal_code=@PostalCode,city=@City,country_code=@CountryCode                 
	        //                          WHERE org_id = @org_id RETURNING id;";

            //     await dataAccess.ExecuteScalarAsync<int>(queryOrgUpdate, parameterOrgUpdate); 

            //     var orgid = await dataAccess.ExecuteScalarAsync<int>(queryOrgUpdate, parameterOrgUpdate); 

            //      // Insert vehicle
            //      int vehId= await _vehicelManager.Create(orgid,keyHandOver.KeyHandOverEvent.EndCustomer.ID,keyHandOver.KeyHandOverEvent.VIN,keyHandOver.KeyHandOverEvent.TCUActivation, keyHandOver.KeyHandOverEvent.ReferenceDateTime);
   
            //     }
                  
            //    else if (isCustomerExist>0 && isVINExist<0) // Insert organization and update vehicle
            //     {
            //     var parameterOrgInsert = new DynamicParameters();
            //     parameterOrgInsert.Add("@org_id",keyHandOver.KeyHandOverEvent.EndCustomer.ID);               
            //     parameterOrgInsert.Add("@Name", keyHandOver.KeyHandOverEvent.EndCustomer.Name);              
            //     parameterOrgInsert.Add("@AddressType",keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type);
            //     parameterOrgInsert.Add("@AddressStreet", keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street);
            //     parameterOrgInsert.Add("@AddressStreetNumber",keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber);
            //     parameterOrgInsert.Add("@PostalCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode);  
            //     parameterOrgInsert.Add("@City",keyHandOver.KeyHandOverEvent.EndCustomer.Address.City);
            //     parameterOrgInsert.Add("@CountryCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode);    
            //    // parameterOrgUpdate.Add("@ReferencedDate", keyHandOver.ReferenceDateTime);   
                          
            //     string queryOrgInsert= "insert into master.organization(org_id,type, name, address_type, street, street_number, postal_code, city,country_code) " +
            //                   "values(@org_id, 'Dealer', @Name, @AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode) RETURNING id";

            //     var orgid =   await dataAccess.ExecuteScalarAsync<int>(queryOrgInsert, parameterOrgInsert);  
 
            //     //update vehicle
            //     int vehId= await _vehicelManager.Update(keyHandOver.KeyHandOverEvent.EndCustomer.ID,keyHandOver.KeyHandOverEvent.VIN,keyHandOver.KeyHandOverEvent.TCUActivation, keyHandOver.KeyHandOverEvent.ReferenceDateTime);
                 
            //     }        
                return keyHandOver;   
            }       
           catch (Exception ex)
            {
                log.Info("KeyHandOverEvent method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());                
                return keyHandOver;
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
