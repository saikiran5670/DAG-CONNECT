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
                parameter.Add("@ReferencedDate",organization.reference_date != null ? UTCHandling.GetUTCFromDateTime(organization.reference_date.ToString()) : (long ?)null);               
                parameter.Add("@OptOutStatus", organization.OptOutStatus);
                parameter.Add("@OptOutStatusChangedDate",organization.optout_status_changed_date != null ? UTCHandling.GetUTCFromDateTime(organization.optout_status_changed_date.ToString()) : (long ?)null); 
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
                parameter.Add("@ReferencedDate",organization.reference_date != null ? UTCHandling.GetUTCFromDateTime(organization.reference_date.ToString()) : (long ?)null);               
                parameter.Add("@OptOutStatus", organization.OptOutStatus);
                parameter.Add("@OptOutStatusChangedDate",organization.optout_status_changed_date != null ? UTCHandling.GetUTCFromDateTime(organization.optout_status_changed_date.ToString()) : (long ?)null); 
                parameter.Add("@IsActive", organization.IsActive);  

                var query = @"update master.organization set org_id=@OrganizationId, type=@OrganizationType, name=@Name,
                 address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
                  postal_code=@PostalCode, city=@City,country_code=@CountryCode,reference_date=@ReferencedDate,
                  optout_status=@OptOutStatus,optout_status_changed_date=@OptOutStatusChangedDate,is_active=@IsActive
	                                 WHERE id = @Id RETURNING id;";
                var groupid = await dataAccess.ExecuteScalarAsync<int>(query, parameter);              
            }
            catch (Exception ex)
            {
                log.Info("Update Organization method in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }
            return organization;
        }

        public async Task<OrganizationResponse> Get(int organizationId)
        {
            log.Info("Get Organization method called in repository");     
            try
            {                
                var parameter = new DynamicParameters();
                var query = @"SELECT id, org_id, type, name, address_type, street, street_number, postal_code, city, country_code, reference_date , optout_status, optout_status_changed_date, is_active
	                        FROM master.organization where id=@Id";               
                parameter.Add("@Id", organizationId);
                IEnumerable<OrganizationResponse> OrganizationDetails = await dataAccess.QueryAsync<OrganizationResponse>(query, parameter);
                OrganizationResponse objOrganization=new OrganizationResponse();
                foreach (var item in OrganizationDetails)
                    {         
                         objOrganization.Id=item.Id;
                         objOrganization.org_id=item.org_id;
                         objOrganization.type=item.type;
                         objOrganization.name=item.name;
                         objOrganization.address_type=item.address_type;
                         objOrganization.street=item.street;
                         objOrganization.street_number=item.street_number;
                         objOrganization.postal_code=item.postal_code;
                         objOrganization.city=item.city;
                         objOrganization.country_code=item.country_code;         
                         objOrganization.is_active=item.is_active;                                
                         objOrganization.reference_date=UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(item.reference_date),"America/New_York", "yyyy-MM-ddTHH:mm:ss");
                         objOrganization.optout_status_changed_date=UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(item.optout_status_changed_date),"America/New_York", "yyyy-MM-ddTHH:mm:ss");
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
        // public async Task<Organization> Get(int organizationId)
        // {
        //     log.Info("Get Organization method called in repository");     
        //     try
        //     {                
        //         var parameter = new DynamicParameters();
        //         // var query = @"SELECT id, org_id, type, name, address_type, street, street_number, postal_code, city, country_code, reference_date, optout_status, optout_status_changed_date, is_active
	    //         //             FROM master.organization where id=@Id";
        //         var query = @"SELECT o.id,c.name currency,t.name timezone ,tf.name timeformat,vd.name vehicledisplay,
        //                     df.name dateformat,lp.name landingpagedisplay,l.description Languagename,u.name unit,a.type PrefType,a.ref_id RefId,org_id OrganizationId,o.type, o.name, address_type AddressType, street AddressStreet, street_number AddressStreetNumber, postal_code PostalCode, city, country_code CountryCode, reference_date ReferencedDate , optout_status OptOutStatus, optout_status_changed_date OptOutStatusChangedDate, O.is_active IsActive
        //                     FROM master.organization o
        //                     left join  master.accountpreference a on o.id=a.ref_id
        //                     left join  master.currency c on c.id=a.currency_id
        //                     left join  master.timezone t on t.id=a.timezone_id
        //                     left join  master.timeformat tf on tf.id=a.time_format_id
        //                     left join  master.vehicledisplay vd on vd.id=a.vehicle_display_id
        //                     left join  master.dateformat df on df.id=a.date_format_id
        //                     left join  master.landingpagedisplay lp on lp.id=a.landing_page_display_id
        //                     left join  master.unit u on u.id=a.unit_id
        //                     left join  translation.language l on l.id=a.language_id
        //                     where o.id=@Id";
        //         parameter.Add("@Id", organizationId);
        //         IEnumerable<Organization> OrganizationDetails = await dataAccess.QueryAsync<Organization>(query, parameter);
        //         Organization objOrganization=new Organization();
        //         foreach (var item in OrganizationDetails)
        //             {         
        //                  objOrganization.Id=item.Id;
        //                  objOrganization.OrganizationId=item.OrganizationId;
        //                  objOrganization.Type=item.Type;
        //                  objOrganization.Name=item.Name;
        //                  objOrganization.AddressType=item.AddressType;
        //                  objOrganization.AddressStreet=item.AddressStreet;
        //                  objOrganization.AddressStreetNumber=item.AddressStreetNumber;
        //                  objOrganization.PostalCode=item.PostalCode;
        //                  objOrganization.City=item.City;
        //                  objOrganization.CountryCode=item.CountryCode;                        
        //                  objOrganization.OptOutStatus=item.OptOutStatus;                     
        //                  objOrganization.IsActive=item.IsActive;
        //                  objOrganization.Currency=item.Currency;
        //                  objOrganization.Timezone=item.Timezone;
        //                  objOrganization.Timeformat=item.Timeformat;
        //                  objOrganization.Vehicledisplay=item.Vehicledisplay;
        //                  objOrganization.Dateformat=item.Dateformat;
        //                  objOrganization.LandingpageDisplay=item.LandingpageDisplay;
        //                  objOrganization.Languagename=item.Languagename;
        //                  objOrganization.Unit=item.Unit;
        //                  objOrganization.PrefType=item.PrefType;
        //                  objOrganization.RefId=item.RefId;          
        //                  objOrganization.Referenced=Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(item.ReferencedDate,"America/New_York", "yyyy-MM-ddTHH:mm:ss"));
        //                  objOrganization.OptOutStatusDate=Convert.ToDateTime(UTCHandling.GetConvertedDateTimeFromUTC(item.OptOutStatusChangedDate,"America/New_York", "yyyy-MM-ddTHH:mm:ss"));
        //             }            
        //         return objOrganization;
        //     }
        //     catch (Exception ex)
        //     {
        //         log.Info("Get Organization method in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
        //         log.Error(ex.ToString());
        //         throw ex;
        //     }
        // }
      

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            log.Info("Update Customer method called in repository"); 
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@org_id", customer.CompanyUpdatedEvent.Company.ID);               
                var query = @"SELECT id FROM master.organization where org_id=@org_id";
                int iscustomerexist= await dataAccess.ExecuteScalarAsync<int>(query, parameter);                       
        
               if (iscustomerexist>0)
                 {
                Int64 referenceDateTime;
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
                //parameterUpdate.Add("@reference_date", customer.CompanyUpdatedEvent.Company.ReferenceDateTime != null ? UTCHandling.GetUTCFromDateTime(customer.CompanyUpdatedEvent.Company.ReferenceDateTime.ToString()) : 0);    
                 if ((customer.CompanyUpdatedEvent.Company.ReferenceDateTime != null) && (DateTime.Compare(DateTime.MinValue, customer.CompanyUpdatedEvent.Company.ReferenceDateTime)< 0))
                {
                   referenceDateTime=UTCHandling.GetUTCFromDateTime(customer.CompanyUpdatedEvent.Company.ReferenceDateTime);
                }   
                else
                {
                    referenceDateTime=0;
                }
                
                parameterUpdate.Add("@reference_date", referenceDateTime);
                var queryUpdate = @"update master.organization set org_id=@org_id, name=@Name,type=@Type,
                 address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
                  postal_code=@PostalCode, city=@City,country_code=@CountryCode,reference_date=@reference_date                               
	                                 WHERE org_id = @org_id RETURNING id;";

                await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameterUpdate);      
            }    
            else
            {                     
                Int64 referenceDateTime;
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

                if ((customer.CompanyUpdatedEvent.Company.ReferenceDateTime != null) && (DateTime.Compare(DateTime.MinValue, customer.CompanyUpdatedEvent.Company.ReferenceDateTime)< 0))
                {
                   referenceDateTime=UTCHandling.GetUTCFromDateTime(customer.CompanyUpdatedEvent.Company.ReferenceDateTime);
                }   
                else
                {
                    referenceDateTime=0;
                }
               // parameterInsert.Add("@reference_date", customer.CompanyUpdatedEvent.Company.ReferenceDateTime != null ? UTCHandling.GetUTCFromDateTime(customer.CompanyUpdatedEvent.Company.ReferenceDateTime.ToString()) : 0);                
                parameterInsert.Add("@reference_date", referenceDateTime);
                string queryInsert= "insert into master.organization(org_id, name,type ,address_type, street, street_number, postal_code, city,country_code,reference_date) " +
                              "values(@org_id, @Name,@Type ,@AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode,@reference_date) RETURNING id";

                 await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameterInsert);      
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

             log.Info("KeyHandOverEvent method is called in repository :");
               try{
                var parameterVeh = new DynamicParameters();
                parameterVeh.Add("@vinexsist", keyHandOver.KeyHandOverEvent.VIN);
                var queryVeh = @"SELECT id from master.vehicle where vin=@vinexsist";
                int isVINExist= await dataAccess.ExecuteScalarAsync<int>(queryVeh, parameterVeh);
             
                var parameter = new DynamicParameters();
                parameter.Add("@org_id", keyHandOver.KeyHandOverEvent.EndCustomer.ID);
                var query = @"Select id from master.organization where org_id=@org_id";
                int iscustomerexist= await dataAccess.ExecuteScalarAsync<int>(query, parameter); 

                if (iscustomerexist>0 && isVINExist>0)  // Update organization and vehicle
                {                    
                var parameterOrgUpdate = new DynamicParameters();
                parameterOrgUpdate.Add("@org_id",keyHandOver.KeyHandOverEvent.EndCustomer.ID);               
                parameterOrgUpdate.Add("@Name", keyHandOver.KeyHandOverEvent.EndCustomer.Name);              
                parameterOrgUpdate.Add("@AddressType",keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type);
                parameterOrgUpdate.Add("@AddressStreet", keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street);
                parameterOrgUpdate.Add("@AddressStreetNumber",keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber);
                parameterOrgUpdate.Add("@PostalCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode);  
                parameterOrgUpdate.Add("@City",keyHandOver.KeyHandOverEvent.EndCustomer.Address.City);
                parameterOrgUpdate.Add("@CountryCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode);    
                
                var queryOrgUpdate = @"update master.organization set org_id=@org_id,name=@Name,
                 address_type=@AddressType,street=@AddressStreet,street_number=@AddressStreetNumber,
                  postal_code=@PostalCode,city=@City,country_code=@CountryCode                 
	                                 WHERE org_id=@org_id RETURNING id;";

                await dataAccess.ExecuteScalarAsync<int>(queryOrgUpdate, parameterOrgUpdate);    
               
                bool istcuactive=true;
                Int64 referenceDateTime;
                string tcuactivation=keyHandOver.KeyHandOverEvent.TCUActivation;
                if(tcuactivation.ToUpper()=="YES")
                {
                  istcuactive=true;
                }
                else if(tcuactivation.ToUpper()=="NO")
                {
                    istcuactive=false;
                }

                var parameterVehUpdate = new DynamicParameters();           
                parameterVehUpdate.Add("@vin",keyHandOver.KeyHandOverEvent.VIN);
                parameterVehUpdate.Add("@tcu_id",keyHandOver.KeyHandOverEvent.TCUID);
                parameterVehUpdate.Add("@is_tcu_register",istcuactive);

                if (keyHandOver.KeyHandOverEvent.ReferenceDateTime != null)
                {
                   referenceDateTime=UTCHandling.GetUTCFromDateTime(keyHandOver.KeyHandOverEvent.ReferenceDateTime);
                }   
                else
                {
                    referenceDateTime=0;
                }
               // parameterVehUpdate.Add("@reference_date",keyHandOver.KeyHandOverEvent.ReferenceDateTime != null ? UTCHandling.GetUTCFromDateTime(keyHandOver.KeyHandOverEvent.ReferenceDateTime) : 0);
                parameterVehUpdate.Add("@reference_date",referenceDateTime);
                //(keyHandOver.KeyHandOverEvent.ReferenceDateTime != null && DateTime.Compare(DateTime.MinValue, keyHandOver.KeyHandOverEvent.ReferenceDateTime) > 0)  ? UTCHandling.GetUTCFromDateTime(customer.CompanyUpdatedEvent.Company.ReferenceDateTime.ToString()) : 0);
                var queryUpdate = @"update master.vehicle set tcu_id=@tcu_id,is_tcu_register=@is_tcu_register,reference_date=@reference_date WHERE vin=@vin RETURNING id;";
                int vehid = await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameterVehUpdate); 
                return keyHandOver;  
                //     update vehicle
                //    int vehId= await _vehicelManager.Update(keyHandOver.KeyHandOverEvent.EndCustomer.ID,keyHandOver.KeyHandOverEvent.VIN,keyHandOver.KeyHandOverEvent.TCUActivation, keyHandOver.KeyHandOverEvent.ReferenceDateTime);
               }
                    
             if (iscustomerexist<1 && isVINExist<1)  // Insert organization and vehicle
                {
                var parameterOrgInsert = new DynamicParameters();
                parameterOrgInsert.Add("@org_id",keyHandOver.KeyHandOverEvent.EndCustomer.ID);               
                parameterOrgInsert.Add("@Name", keyHandOver.KeyHandOverEvent.EndCustomer.Name);              
                parameterOrgInsert.Add("@AddressType",keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type);
                parameterOrgInsert.Add("@AddressStreet", keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street);
                parameterOrgInsert.Add("@AddressStreetNumber",keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber);
                parameterOrgInsert.Add("@PostalCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode);  
                parameterOrgInsert.Add("@City",keyHandOver.KeyHandOverEvent.EndCustomer.Address.City);
                parameterOrgInsert.Add("@CountryCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode); 
                parameterOrgInsert.Add("@reference_date", 0); 


                string queryOrgInsert= "insert into master.organization(org_id,name,address_type,street,street_number,postal_code,city,country_code,reference_date) " +
                              "values(@org_id,@Name,@AddressType,@AddressStreet,@AddressStreetNumber,@PostalCode,@City,@CountryCode,@reference_date) RETURNING id";

                var orgid =   await dataAccess.ExecuteScalarAsync<int>(queryOrgInsert, parameterOrgInsert);   

                bool istcuactive=true;
                string tcuactivation=keyHandOver.KeyHandOverEvent.TCUActivation;
                if(tcuactivation.ToUpper()=="YES")
                {
                  istcuactive=true;
                }
                else if(tcuactivation.ToUpper()=="NO")
                {
                    istcuactive=false;
                }  
                Int64 referenceDateTime;
                var parameterVehInsert = new DynamicParameters();
                parameterVehInsert.Add("@organization_id",orgid);        
                parameterVehInsert.Add("@vin",keyHandOver.KeyHandOverEvent.VIN);               
                parameterVehInsert.Add("@tcuid", keyHandOver.KeyHandOverEvent.TCUID);           
                parameterVehInsert.Add("@is_tcu_register", istcuactive);
                if (keyHandOver.KeyHandOverEvent.ReferenceDateTime != null)
                {
                   referenceDateTime=UTCHandling.GetUTCFromDateTime(keyHandOver.KeyHandOverEvent.ReferenceDateTime);
                }   
                else
                {
                    referenceDateTime=0;
                }
               // parameterVehUpdate.Add("@reference_date",keyHandOver.KeyHandOverEvent.ReferenceDateTime != null ? UTCHandling.GetUTCFromDateTime(keyHandOver.KeyHandOverEvent.ReferenceDateTime) : 0);
                parameterVehInsert.Add("@reference_date",referenceDateTime);
                
                var queryVehInsert= @"INSERT INTO master.vehicle
                                      (organization_id                                  
                                      ,vin
                                      ,tcu_id
                                      ,is_tcu_register
                                      ,reference_date)                                    
                            	VALUES(@organization_id                                     
                                      ,@vin
                                      ,@tcuid
                                      ,@is_tcu_register
                                      ,@reference_date                                                                       
                                     ) RETURNING id";
                int vehid = await dataAccess.ExecuteScalarAsync<int>(queryVehInsert, parameterVehInsert); 
                //Insert vehicle
                // int vehId= await _vehicelManager.Create(orgid,keyHandOver.KeyHandOverEvent.EndCustomer.ID,keyHandOver.KeyHandOverEvent.VIN,keyHandOver.KeyHandOverEvent.TCUActivation, keyHandOver.KeyHandOverEvent.ReferenceDateTime);

                return keyHandOver;               
              }

               else if (iscustomerexist>0 && isVINExist<1) // Update organization and insert vehicle
                {
                var parameterOrgUpdate = new DynamicParameters();
                parameterOrgUpdate.Add("@org_id",keyHandOver.KeyHandOverEvent.EndCustomer.ID);               
                parameterOrgUpdate.Add("@Name", keyHandOver.KeyHandOverEvent.EndCustomer.Name);              
                parameterOrgUpdate.Add("@AddressType",keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type);
                parameterOrgUpdate.Add("@AddressStreet", keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street);
                parameterOrgUpdate.Add("@AddressStreetNumber",keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber);
                parameterOrgUpdate.Add("@PostalCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode);  
                parameterOrgUpdate.Add("@City",keyHandOver.KeyHandOverEvent.EndCustomer.Address.City);
                parameterOrgUpdate.Add("@CountryCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode);
               
                 var queryOrgUpdate = @"update master.organization set org_id=@org_id, name=@Name,
                 address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
                  postal_code=@PostalCode,city=@City,country_code=@CountryCode                 
	                                 WHERE org_id = @org_id RETURNING id;";
               await dataAccess.ExecuteScalarAsync<int>(queryOrgUpdate, parameterOrgUpdate); 

                int orgid = await dataAccess.ExecuteScalarAsync<int>(queryOrgUpdate, parameterOrgUpdate); 
               
                bool istcuactive=true;
                string tcuactivation=keyHandOver.KeyHandOverEvent.TCUActivation;
                 Int64 referenceDateTime;
                if(tcuactivation.ToUpper()=="YES")
                {
                  istcuactive=true;
                }
                else if(tcuactivation.ToUpper()=="NO")
                {
                    istcuactive=false;
                }  

                var parameterVehInsert = new DynamicParameters();
                parameterVehInsert.Add("@organization_id",orgid);        
                parameterVehInsert.Add("@vin",keyHandOver.KeyHandOverEvent.VIN);               
                parameterVehInsert.Add("@tcuid", keyHandOver.KeyHandOverEvent.TCUID);
                if (keyHandOver.KeyHandOverEvent.ReferenceDateTime != null)
                {
                   referenceDateTime=UTCHandling.GetUTCFromDateTime(keyHandOver.KeyHandOverEvent.ReferenceDateTime);
                }   
                else
                {
                    referenceDateTime=0;
                }           
                parameterVehInsert.Add("@reference_date",referenceDateTime);
                parameterVehInsert.Add("@is_tcu_register",istcuactive );             
                                
                var queryVehInsert= @"INSERT INTO master.vehicle
                                      (organization_id                                  
                                      ,vin
                                      ,tcu_id
                                      ,is_tcu_register
                                      ,reference_date )                                    
                            	VALUES(@organization_id                                     
                                      ,@vin
                                      ,@tcuid
                                      ,@is_tcu_register
                                      ,@reference_date                                                                      
                                     ) RETURNING id";
                int vehid = await dataAccess.ExecuteScalarAsync<int>(queryVehInsert, parameterVehInsert);
                 return keyHandOver;  
                 // Insert vehicle
                 //int vehId= await _vehicelManager.Create(orgid,keyHandOver.KeyHandOverEvent.EndCustomer.ID,keyHandOver.KeyHandOverEvent.VIN,keyHandOver.KeyHandOverEvent.TCUActivation, keyHandOver.KeyHandOverEvent.ReferenceDateTime);
   
                }
                  
               else if (iscustomerexist<1 && isVINExist>0) // Insert organization and update vehicle
                {
                var parameterOrgInsert = new DynamicParameters();
                parameterOrgInsert.Add("@org_id",keyHandOver.KeyHandOverEvent.EndCustomer.ID);               
                parameterOrgInsert.Add("@Name", keyHandOver.KeyHandOverEvent.EndCustomer.Name);              
                parameterOrgInsert.Add("@AddressType",keyHandOver.KeyHandOverEvent.EndCustomer.Address.Type);
                parameterOrgInsert.Add("@AddressStreet", keyHandOver.KeyHandOverEvent.EndCustomer.Address.Street);
                parameterOrgInsert.Add("@AddressStreetNumber",keyHandOver.KeyHandOverEvent.EndCustomer.Address.StreetNumber);
                parameterOrgInsert.Add("@PostalCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.PostalCode);  
                parameterOrgInsert.Add("@City",keyHandOver.KeyHandOverEvent.EndCustomer.Address.City);
                parameterOrgInsert.Add("@CountryCode", keyHandOver.KeyHandOverEvent.EndCustomer.Address.CountryCode);    
                parameterOrgInsert.Add("@reference_date", 0);            
                string queryOrgInsert= "insert into master.organization(org_id,name, address_type, street, street_number, postal_code, city,country_code,reference_date) " +
                              "values(@org_id,@Name, @AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode,@reference_date) RETURNING id";

                int orgid =   await dataAccess.ExecuteScalarAsync<int>(queryOrgInsert, parameterOrgInsert);  
                
                bool istcuactive=true;
                string tcuactivation=keyHandOver.KeyHandOverEvent.TCUActivation;
                if(tcuactivation.ToUpper()=="YES")
                {
                  istcuactive=true;
                }
                else if(tcuactivation.ToUpper()=="NO")
                {
                    istcuactive=false;
                }  
                
                Int64 referenceDateTime;
                var parameterVehUpdate = new DynamicParameters();           
                parameterVehUpdate.Add("@vin",keyHandOver.KeyHandOverEvent.VIN);
                parameterVehUpdate.Add("@tcu_id",keyHandOver.KeyHandOverEvent.TCUID);
                parameterVehUpdate.Add("@is_tcu_register",istcuactive); 
                 if (keyHandOver.KeyHandOverEvent.ReferenceDateTime != null)
                {
                   referenceDateTime=UTCHandling.GetUTCFromDateTime(keyHandOver.KeyHandOverEvent.ReferenceDateTime);
                }   
                else
                {
                    referenceDateTime=0;
                }           
                parameterVehUpdate.Add("@reference_date",referenceDateTime);    
                var queryUpdate = @"update master.vehicle set tcu_id=@tcu_id,is_tcu_register=@is_tcu_register,reference_date=@reference_date WHERE vin = @vin RETURNING id;";
                int vehid = await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameterVehUpdate); 
 
                //update vehicle
                //int vehId= await _vehicelManager.Update(keyHandOver.KeyHandOverEvent.EndCustomer.ID,keyHandOver.KeyHandOverEvent.VIN,keyHandOver.KeyHandOverEvent.TCUActivation, keyHandOver.KeyHandOverEvent.ReferenceDateTime);
                 }     
               }
             catch (Exception ex)
            {
                log.Info("KeyHandOverEvent method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString()); 
                throw ex;
            }
            return keyHandOver;  
            }           
               
        public async Task<int> CreateVehicleParty(List<Customer> customers)
        {
             int count=0;
            foreach (var item in customers)
            {
                await UpdateCustomer(item);             
                count+=1;
            }
            return count;
        }
    }
}
