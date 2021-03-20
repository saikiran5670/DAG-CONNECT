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
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehiclerepository;
using net.atos.daf.ct2.subscription;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.account.entity;
using AccountComponent = net.atos.daf.ct2.account;
using SubscriptionComponent = net.atos.daf.ct2.subscription;
namespace net.atos.daf.ct2.organization.repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly IDataAccess dataAccess;
        private readonly IVehicleManager vehicelManager;
        private readonly IGroupManager groupManager;
        private readonly IAccountManager accountManager;
        SubscriptionComponent.ISubscriptionManager subscriptionManager;
        private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public OrganizationRepository(IDataAccess _dataAccess, IVehicleManager _vehicleManager, IGroupManager _groupManager, IAccountManager _accountManager,SubscriptionComponent.ISubscriptionManager _subscriptionManager)
        {
            dataAccess = _dataAccess;
            vehicelManager = _vehicleManager;
            groupManager = _groupManager;
            accountManager = _accountManager;
            subscriptionManager=_subscriptionManager;
        }
        public async Task<Organization> Create(Organization organization)
        {
            log.Info("Create Organization method called in repository");
            try
            {

                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@org_id", organization.OrganizationId);
                var query = @"SELECT id FROM master.organization where org_id=@org_id";
                int orgexist = await dataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);

                if (orgexist > 0)
                {
                    organization.Id = 0;
                    return organization;
                }
                else
                {

                    var parameter = new DynamicParameters();
                    parameter.Add("@OrganizationId", organization.OrganizationId);
                    parameter.Add("@OrganizationType", organization.Type);
                    parameter.Add("@Name", organization.Name);
                    parameter.Add("@AddressType", organization.AddressType);
                    parameter.Add("@AddressStreet", organization.AddressStreet);
                    parameter.Add("@AddressStreetNumber", organization.AddressStreetNumber);
                    parameter.Add("@PostalCode", organization.PostalCode);
                    parameter.Add("@City", organization.City);
                    parameter.Add("@CountryCode", organization.CountryCode);
                    parameter.Add("@ReferencedDate", organization.reference_date != null ? UTCHandling.GetUTCFromDateTime(organization.reference_date.ToString()) : (long?)null);
                    parameter.Add("@vehicle_default_opt_in", "I");
                    parameter.Add("@driver_default_opt_in", "I");
                    string queryInsert = "insert into master.organization(org_id, type, name, address_type, street, street_number, postal_code, city,country_code,reference_date,preference_id,vehicle_default_opt_in,driver_default_opt_in) " +
                                  "values(@OrganizationId, @OrganizationType, @Name, @AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode,@ReferencedDate,null,@vehicle_default_opt_in,@driver_default_opt_in) RETURNING id";

                    var orgid = await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                    organization.Id = orgid;

                    // Create dynamic account group
                    Group groupAccount = new Group();
                    groupAccount.ObjectType = ObjectType.AccountGroup;
                    groupAccount.GroupType = GroupType.Dynamic;
                    groupAccount.Argument = "";
                    groupAccount.FunctionEnum = FunctionEnum.None;
                    groupAccount.OrganizationId = orgid;
                    groupAccount.RefId = 0;
                    groupAccount.Name = "DefaultAccountGroup";
                    groupAccount.Description = "DefaultAccountGroup";
                    groupAccount.CreatedAt = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                    groupAccount = await groupManager.Create(groupAccount);

                    // Create dynamic vehicle group
                    Group groupVehicle = new Group();
                    groupVehicle.ObjectType = ObjectType.VehicleGroup;
                    groupVehicle.GroupType = GroupType.Dynamic;
                    groupVehicle.Argument = "";
                    groupVehicle.FunctionEnum = FunctionEnum.None;
                    groupVehicle.OrganizationId = orgid;
                    groupVehicle.RefId = 0;
                    groupVehicle.Name = "DefaultVehicleGroup";
                    groupVehicle.Description = "DefaultVehicleGroup";
                    groupVehicle.CreatedAt = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                    groupVehicle = await groupManager.Create(groupVehicle);

                    // Create access relationship
                    AccessRelationship accessRelationship = new AccessRelationship();
                    accessRelationship.AccountGroupId = groupAccount.Id;
                    accessRelationship.VehicleGroupId = groupVehicle.Id;
                    accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ViewOnly;
                    await accountManager.CreateAccessRelationship(accessRelationship);

                }
            }
            catch (Exception ex)
            {
                // log.Info("Create Organization method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organization));
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
                var query = @"update master.organization set is_active=false where id=@id";
                int isdelete = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
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
                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@org_id", organization.OrganizationId);
                var query = @"SELECT id FROM master.organization where org_id=@org_id";
                int orgexist = await dataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);
                if (orgexist > 0)
                {
                    organization.Id = -1;
                    return organization;
                }
                else
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@Id", organization.Id);
                    parameter.Add("@OrganizationId", organization.OrganizationId);
                    parameter.Add("@OrganizationType", organization.Type);
                    parameter.Add("@Name", organization.Name);
                    parameter.Add("@AddressType", organization.AddressType);
                    parameter.Add("@AddressStreet", organization.AddressStreet);
                    parameter.Add("@AddressStreetNumber", organization.AddressStreetNumber);
                    parameter.Add("@PostalCode", organization.PostalCode);
                    parameter.Add("@City", organization.City);
                    parameter.Add("@CountryCode", organization.CountryCode);
                    parameter.Add("@ReferencedDate", organization.reference_date != null ? UTCHandling.GetUTCFromDateTime(organization.reference_date.ToString()) : (long?)null);
                    parameter.Add("@vehicleoptin", organization.vehicle_default_opt_in);
                    parameter.Add("@driveroptin", organization.driver_default_opt_in);
                    //parameter.Add("@IsActive", organization.IsActive); 

                    var queryUpdate = @"update master.organization set org_id=@OrganizationId, type=@OrganizationType, name=@Name,
                 address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
                  postal_code=@PostalCode, city=@City,country_code=@CountryCode,reference_date=@ReferencedDate,vehicle_default_opt_in=@vehicleoptin,driver_default_opt_in=@driveroptin              
	                                 WHERE id = @Id RETURNING id;";
                    var orgid = await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameter);
                    if (orgid < 1)
                    {
                        organization.Id = 0;
                    }
                }
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
                var query = @"SELECT id, org_id, type, name, address_type, street, street_number, postal_code, city, country_code, reference_date, is_active,vehicle_default_opt_in,driver_default_opt_in
	                        FROM master.organization where id=@Id and is_active=true";
                parameter.Add("@Id", organizationId);
                IEnumerable<OrganizationResponse> OrganizationDetails = await dataAccess.QueryAsync<OrganizationResponse>(query, parameter);
                OrganizationResponse objOrganization = new OrganizationResponse();               
                foreach (var item in OrganizationDetails)
                {
                    objOrganization.Id = item.Id;
                    objOrganization.org_id = item.org_id;
                    objOrganization.type = item.type;
                    objOrganization.name = item.name;
                    objOrganization.address_type = item.address_type;
                    objOrganization.street = item.street;
                    objOrganization.street_number = item.street_number;
                    objOrganization.postal_code = item.postal_code;
                    objOrganization.city = item.city;
                    objOrganization.country_code = item.country_code;
                    objOrganization.is_active = item.is_active;
                    objOrganization.reference_date = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(item.reference_date), "America/New_York", "yyyy-MM-ddTHH:mm:ss");
                    objOrganization.vehicle_default_opt_in = item.vehicle_default_opt_in;
                    objOrganization.driver_default_opt_in = item.driver_default_opt_in;                   
                }
                if (objOrganization.Id < 1)
                {
                    objOrganization.Id = 0;
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

        public async Task<PreferenceResponse> GetPreference(int organizationId)
        {
            log.Info("Get Organization preference method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                var query = @"SELECT o.id OrganizatioId,a.id PreferenceId, c.name currency,t.name timezone ,tf.name timeformat,vd.name vehicledisplay,
                            df.name DateFormatType,lp.name landingpagedisplay,l.name LanguageName, u.name unit
                            FROM master.organization o
                            left join  master.accountpreference a on o.id=a.id
                            left join  master.currency c on c.id=a.currency_id
                            left join  master.timezone t on t.id=a.timezone_id
                            left join  master.timeformat tf on tf.id=a.time_format_id
                            left join  master.vehicledisplay vd on vd.id=a.vehicle_display_id
                            left join  master.dateformat df on df.id=a.date_format_id
                            left join  master.landingpagedisplay lp on lp.id=a.landing_page_display_id
                            left join  master.unit u on u.id=a.unit_id
                            left join  translation.language l on l.id=a.language_id
                            where o.id=@Id";
                parameter.Add("@Id", organizationId);
                IEnumerable<PreferenceResponse> PreferenceDetails = await dataAccess.QueryAsync<PreferenceResponse>(query, parameter);
                PreferenceResponse preferenceResponse = new PreferenceResponse();
                foreach (var item in PreferenceDetails)
                {
                    preferenceResponse.PreferenceId = item.PreferenceId;
                    preferenceResponse.OrganizatioId = item.OrganizatioId;
                    preferenceResponse.LanguageName = item.LanguageName;
                    preferenceResponse.Timezone = item.Timezone;
                    preferenceResponse.TimeFormat = item.TimeFormat;
                    preferenceResponse.Currency = item.Currency;
                    preferenceResponse.Unit = item.Unit;
                    preferenceResponse.VehicleDisplay = item.VehicleDisplay;
                    preferenceResponse.DateFormatType = item.DateFormatType;
                    preferenceResponse.LandingPageDisplay = item.LandingPageDisplay;

                }
                return preferenceResponse;
            }
            catch (Exception ex)
            {
                log.Info("Get Organization preference method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());
                throw ex;
            }
        }


        public async Task<CustomerRequest> UpdateCustomer(CustomerRequest customer)
        {
            log.Info("Update Customer method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@org_id", customer.CustomerID);
                var query = @"SELECT id FROM master.organization where org_id=@org_id";
                int iscustomerexist = await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                if (iscustomerexist > 0)
                {
                    Int64 referenceDateTime;
                    var parameterUpdate = new DynamicParameters();
                    parameterUpdate.Add("@org_id", customer.CustomerID);
                    parameterUpdate.Add("@Name", customer.CustomerName);
                    parameterUpdate.Add("@Type", customer.CompanyType);
                    parameterUpdate.Add("@AddressType", customer.AddressType);
                    parameterUpdate.Add("@AddressStreet", customer.Street);
                    parameterUpdate.Add("@AddressStreetNumber", customer.StreetNumber);
                    parameterUpdate.Add("@PostalCode", customer.PostalCode);
                    parameterUpdate.Add("@City", customer.City);
                    parameterUpdate.Add("@CountryCode", customer.CountryCode);                     
                    if ((customer.ReferenceDateTime != null) && (DateTime.Compare(DateTime.MinValue, customer.ReferenceDateTime) < 0))
                    {
                        referenceDateTime = UTCHandling.GetUTCFromDateTime(customer.ReferenceDateTime.ToString());
                    }
                    else
                    {
                        referenceDateTime = 0;
                    }

                    parameterUpdate.Add("@reference_date", referenceDateTime);
                    var queryUpdate = @"update master.organization set org_id=@org_id, name=@Name,type=@Type,
                 address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
                  postal_code=@PostalCode, city=@City,country_code=@CountryCode,reference_date=@reference_date                               
	                                 WHERE org_id = @org_id RETURNING id;";

                    await dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameterUpdate);

                    // Assign base package at ORG lavel if not exist
                    await subscriptionManager.Create(iscustomerexist,Convert.ToInt32(customer.OrgCreationPackage));

                }
                else
                {
                    Int64 referenceDateTime;
                    var parameterInsert = new DynamicParameters();
                    parameterInsert.Add("@org_id", customer.CustomerID);
                    parameterInsert.Add("@Name", customer.CustomerName);
                    parameterInsert.Add("@Type", customer.CompanyType);
                    parameterInsert.Add("@AddressType", customer.AddressType);
                    parameterInsert.Add("@AddressStreet", customer.Street);
                    parameterInsert.Add("@AddressStreetNumber", customer.StreetNumber);
                    parameterInsert.Add("@PostalCode", customer.PostalCode);
                    parameterInsert.Add("@City", customer.City);
                    parameterInsert.Add("@CountryCode", customer.CountryCode);
                   

                    if ((customer.ReferenceDateTime != null) && (DateTime.Compare(DateTime.MinValue, customer.ReferenceDateTime) < 0))
                    {
                        referenceDateTime = UTCHandling.GetUTCFromDateTime(customer.ReferenceDateTime.ToString());

                      //  vehicle.Termination_Date != null ? UTCHandling.GetUTCFromDateTime(vehicle.Termination_Date.ToString()) : (long?)null);

                    }
                    else
                    {
                        referenceDateTime = 0;
                    }
                    parameterInsert.Add("@vehicle_default_opt_in", "I");
                    parameterInsert.Add("@driver_default_opt_in", "U");
                   parameterInsert.Add("@reference_date", referenceDateTime);
                    string queryInsert = "insert into master.organization(org_id, name,type ,address_type, street, street_number, postal_code, city,country_code,reference_date,vehicle_default_opt_in,driver_default_opt_in) " +
                                  "values(@org_id, @Name,@Type ,@AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode,@reference_date,@vehicle_default_opt_in,@driver_default_opt_in) RETURNING id";

                   int organizationId= await dataAccess.ExecuteScalarAsync<int>(queryInsert, parameterInsert);

                    // CraeteOrganizationRelationship
                    // need to discuss here

                    // Assign base package at ORG lavel
                   await subscriptionManager.Create(iscustomerexist, Convert.ToInt32(customer.OrgCreationPackage));
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

         public async Task<int> UpdateCompany(HandOver keyHandOver)
         {
                try{
                    var parameterOrgUpdate = new DynamicParameters();
                    parameterOrgUpdate.Add("@org_id", keyHandOver.CustomerID);
                    parameterOrgUpdate.Add("@Name", keyHandOver.CustomerName);
                    parameterOrgUpdate.Add("@AddressType", keyHandOver.Type);
                    parameterOrgUpdate.Add("@AddressStreet", keyHandOver.Street);
                    parameterOrgUpdate.Add("@AddressStreetNumber", keyHandOver.StreetNumber);
                    parameterOrgUpdate.Add("@PostalCode", keyHandOver.PostalCode);
                    parameterOrgUpdate.Add("@City", keyHandOver.City);
                    parameterOrgUpdate.Add("@CountryCode", keyHandOver.CountryCode);

                    var queryOrgUpdate = @"update master.organization set org_id=@org_id,name=@Name,
                    address_type=@AddressType,street=@AddressStreet,street_number=@AddressStreetNumber,
                    postal_code=@PostalCode,city=@City,country_code=@CountryCode                 
	                                 WHERE org_id=@org_id RETURNING id;";
                 return await dataAccess.ExecuteScalarAsync<int>(queryOrgUpdate, parameterOrgUpdate);            
                }
                catch(Exception ex )
                {
                 log.Info("UpdateCompany method called in repository failed :");
                 log.Error(ex.ToString());
                 throw ex;
                }   
         }
       
        public async Task<int> InsertCompany(HandOver keyHandOver)
         {
                try{
                    var parameterOrgInsert = new DynamicParameters();
                    parameterOrgInsert.Add("@org_id", keyHandOver.CustomerID);
                    parameterOrgInsert.Add("@Name", keyHandOver.CustomerName);
                    parameterOrgInsert.Add("@AddressType", keyHandOver.Type);
                    parameterOrgInsert.Add("@AddressStreet", keyHandOver.Street);
                    parameterOrgInsert.Add("@AddressStreetNumber", keyHandOver.StreetNumber);
                    parameterOrgInsert.Add("@PostalCode", keyHandOver.PostalCode);
                    parameterOrgInsert.Add("@City", keyHandOver.City);
                    parameterOrgInsert.Add("@CountryCode", keyHandOver.CountryCode);   
                    parameterOrgInsert.Add("@vehicle_default_opt_in", "U");
                    parameterOrgInsert.Add("@driver_default_opt_in","U");

                     if (keyHandOver.ReferenceDateTime != null)
                    {
                        parameterOrgInsert.Add("@reference_date",  UTCHandling.GetUTCFromDateTime(keyHandOver.ReferenceDateTime));
                    }
                    else
                    {
                         parameterOrgInsert.Add("@reference_date",  0);
                    }

                    string queryOrgInsert = "insert into master.organization(org_id,name,address_type,street,street_number,postal_code,city,country_code,reference_date,vehicle_default_opt_in,driver_default_opt_in) " +
                                  "values(@org_id,@Name,@AddressType,@AddressStreet,@AddressStreetNumber,@PostalCode,@City,@CountryCode,@reference_date,@vehicle_default_opt_in,@driver_default_opt_in) RETURNING id";

                    return  await dataAccess.ExecuteScalarAsync<int>(queryOrgInsert, parameterOrgInsert);            
                }
                catch(Exception ex )
                {
                 log.Info("InsertCompany method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                 log.Error(ex.ToString());
                 throw ex;
                }       
         }

         public async Task<int> InsertVehicle(HandOver keyHandOver,int OrganizationId)
         {
            try{
                    bool istcuactive = true;
                    string tcuactivation = keyHandOver.TCUActivation;
                    if (tcuactivation.ToUpper() == "YES")
                    {
                        istcuactive = true;
                    }
                    else if (tcuactivation.ToUpper() == "NO")
                    {
                        istcuactive = false;
                    }
                   
                     Vehicle objvehicle=new Vehicle(); 
                     objvehicle.Organization_Id=OrganizationId;
                     objvehicle.VIN=keyHandOver.VIN;
                     objvehicle.Tcu_Id=keyHandOver.TCUID;
                     objvehicle.Is_Tcu_Register=istcuactive;
                     if (!string.IsNullOrEmpty(keyHandOver.ReferenceDateTime))
                     {
                          objvehicle.Reference_Date=Convert.ToDateTime(keyHandOver.ReferenceDateTime);
                     }
                      else{
                             objvehicle.Reference_Date=null;
                      }
                 
                    objvehicle.Oem_id=Convert.ToInt32(keyHandOver.OEMRelationship);
                    objvehicle.Oem_Organisation_id=OrganizationId;
                    objvehicle.Status_Changed_Date=System.DateTime.Now;
                    objvehicle.CreatedAt=UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                
                 // NULL FIELDS
                 objvehicle.Name=null;
                 objvehicle.License_Plate_Number=null;
                 objvehicle.Termination_Date=null;
                 objvehicle.Vid=null;
                 objvehicle.Type=objvehicle.Type;
                 objvehicle.Tcu_Serial_Number=null;
                 objvehicle.Tcu_Brand=null;
                 objvehicle.Tcu_Version=null;
                 objvehicle.VehiclePropertiesId=null;  
                 objvehicle.ModelId=null;      
                 objvehicle.Opt_In=VehicleStatusType.Inherit;
                 objvehicle.Is_Ota=false; 

                await vehicelManager.Create(objvehicle);                       
                return 1;
                 }
               catch(Exception ex )
                {
                 log.Info("InsertVehicle method called in repository failed :");
                 log.Error(ex.ToString());
                 throw ex;
                }                             
         }
        public async Task<int> UpdatetVehicle(HandOver keyHandOver,int OrganizationId)
         {
               try{
                    Vehicle objvehicle=new Vehicle();                    
                    bool istcuactive = true;                  
                    string tcuactivation = keyHandOver.TCUActivation;
                    if (tcuactivation.ToUpper() == "YES")
                    {
                        istcuactive = true;
                    }
                    else if (tcuactivation.ToUpper() == "NO")
                    {
                        istcuactive = false;
                    }
                   
                     if (!string.IsNullOrEmpty(keyHandOver.ReferenceDateTime))
                     {
                          objvehicle.Reference_Date=Convert.ToDateTime(keyHandOver.ReferenceDateTime);
                     }
                     else{
                          objvehicle.Reference_Date=null;}
                     
                      objvehicle.Is_Tcu_Register=istcuactive;
                      objvehicle.VIN=keyHandOver.VIN;
                      objvehicle.Tcu_Id=keyHandOver.TCUID;
                      objvehicle.Vid=null;
                      objvehicle.Tcu_Brand=null;
                      objvehicle.Tcu_Serial_Number=null;
                      objvehicle.Tcu_Version=null;                                      
                      objvehicle.Organization_Id=OrganizationId;                        
                      await vehicelManager.UpdateOrgVehicleDetails(objvehicle);                    
               }
               catch(Exception ex )
                {
                 log.Info("UpdatetVehicle method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                 log.Error(ex.ToString());
                 throw ex;
                }    
               return 1;          
         }

         public async Task<int> OwnerRelationship(HandOver keyHandOver,int VehicleID)
         {
            try
            {
                RelationshipMapping relationshipMapping = new RelationshipMapping();
                relationshipMapping.relationship_id = Convert.ToInt32(keyHandOver.OwnerRelationship);
                relationshipMapping.owner_org_id = Convert.ToInt32(keyHandOver.OwnerRelationship);
                relationshipMapping.target_org_id = Convert.ToInt32(keyHandOver.OwnerRelationship);
                relationshipMapping.created_org_id = Convert.ToInt32(keyHandOver.OwnerRelationship);
                relationshipMapping.vehicle_id = VehicleID;
                relationshipMapping.start_date = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                relationshipMapping.created_at = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                relationshipMapping.allow_chain = true;
                //relationshipMapping.isFirstRelation=true;  
                relationshipMapping.isFirstRelation = false;
                await CreateOwnerRelationship(relationshipMapping);
            }
            catch (Exception ex)
            {
                log.Info("UpdatetVehicle method called in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }    
               return 1;          
         }       
        public async Task<HandOver> KeyHandOverEvent(HandOver keyHandOver)
        {
           // 1. Check the VIN in exist in vehicle table.
            //2. If exist then update the vehicle details.
            //3. If not exist then create new vehicle in vehicle table.
            //4. check company exist in organization table.
            //5. If company exist then update the company details in organization table
            //6. If company not exist the create new company in organozation table.
            //7. Update the vehicle table with organizationID based on VIN (New method required):
            //   Name of other columns in vehicle table need to update 
            //8. Call CraeteOwnerRelationship(flag)
            //9. If the value of flag is true then it will end the previous relationship.
            //  end_date : today datetime
            //10. when owner changed, then update org_id in vehicle table.
            //11. Call vehicleOptOutOptin history method-   await VehicleOptInOptOutHistory(vehicle.ID);

            log.Info("KeyHandOverEvent method is called in repository :");
            try
            {             
                var parameter = new DynamicParameters();
                parameter.Add("@org_id", keyHandOver.CustomerID);
                var query = @"Select id from master.organization where org_id=@org_id";
                int iscustomerexist = await dataAccess.ExecuteScalarAsync<int>(query, parameter);                
                int isVINExist= await vehicelManager.IsVINExists(keyHandOver.VIN);

                if (iscustomerexist > 0 && isVINExist > 0)  // Update organization and vehicle
                {
                   int OrganizationId= await UpdateCompany(keyHandOver);
                   await UpdatetVehicle(keyHandOver,OrganizationId); 
                
                  // Owner Relationship Management              
                   await OwnerRelationship(keyHandOver,isVINExist);
                                     
                   return keyHandOver;                   
                }

                else if (iscustomerexist < 1 && isVINExist < 1)  // Insert organization and vehicle
                {
                   // Insert Company
                    int organizationID= await InsertCompany(keyHandOver); 

                    // Insert Vehicle                    
                    await InsertVehicle(keyHandOver,organizationID);    

                     // Owner Relationship Management    
                   int vehicleID= await vehicelManager.IsVINExists(keyHandOver.VIN);           
                   await OwnerRelationship(keyHandOver,vehicleID);
                   
                    return keyHandOver;
                }

                else if (iscustomerexist > 0 && isVINExist < 1) // Update organization and insert vehicle
                {    
                    // Update company         
                    int organizationID=await UpdateCompany(keyHandOver);

                    // insert vehicle
                    await InsertVehicle(keyHandOver,organizationID);   

                   // Owner Relationship Management   
                   int vehicleID= await vehicelManager.IsVINExists(keyHandOver.VIN);           
                   await OwnerRelationship(keyHandOver,vehicleID);
                                     
                   return keyHandOver;     
                }

                else if (iscustomerexist < 1 && isVINExist > 0) // Insert organization and update vehicle
                {
                   int organizationID= await InsertCompany(keyHandOver);
                   await UpdatetVehicle(keyHandOver,organizationID);   
                 
                     // Owner Relationship Management              
                   await OwnerRelationship(keyHandOver,isVINExist);                                    
                            
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

        // public async Task<int> CreateVehicleParty(List<Customer> customers)
        // {
        //     int count = 0;
        //     foreach (var item in customers)
        //     {
        //         await UpdateCustomer(item);
        //         count += 1;
        //     }
        //     return count;
        // }

        public async Task<int> CreateOwnerRelationship(RelationshipMapping relationshipMapping)
        {
            // 1. Check relationship exist in orgrelationshipmapping table based on VIN.
            // 2. if relationship not exist then create the relationship in orgrelationshipmapping table with configured parameters and default values
            // 3. Get configured parameter org_id and relationship_id from property file

            try
            {

                int OwnerRelationshipId = 0;
                //var parameter = new DynamicParameters();
                //parameter.Add("@vehicle_id", relationshipMapping.vehicle_id);
                //var query = @"Select id from master.orgrelationshipmapping where vehicle_id=@vehicle_id";
                //int isRelationshipExist = await dataAccess.ExecuteScalarAsync<int>(query, parameter);

                int isRelationshipExist = await IsOwnerRelationshipExist(relationshipMapping.vehicle_id);

                if (isRelationshipExist < 1 && relationshipMapping.isFirstRelation) // relationship not exist
                                                                                    // if (iscustomerexist< 1)
                {
                    var Inputparameter = new DynamicParameters();
                    Inputparameter.Add("@relationship_id", relationshipMapping.relationship_id);  // from property file
                    Inputparameter.Add("@vehicle_id", relationshipMapping.vehicle_id);
                    if (relationshipMapping.vehicle_group_id == 0)
                    {
                        Inputparameter.Add("@vehicle_group_id", null);
                    }
                    else
                    {
                        Inputparameter.Add("@vehicle_group_id", relationshipMapping.vehicle_group_id);
                    }
                    Inputparameter.Add("@owner_org_id", relationshipMapping.owner_org_id);    // from property file 
                    Inputparameter.Add("@created_org_id", relationshipMapping.created_org_id); // from property file --- first time it will same as owner_org_id
                    Inputparameter.Add("@target_org_id", relationshipMapping.target_org_id);  // from property file -- first time it will same as owner_org_id
                    Inputparameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    Inputparameter.Add("@end_date", null);   // First time -- NULL
                    Inputparameter.Add("@allow_chain", relationshipMapping.allow_chain);   // Alway true
                    Inputparameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));   // Alway true

                    var queryInsert = @"insert into master.orgrelationshipmapping(relationship_id,vehicle_id,vehicle_group_id,
                     owner_org_id,created_org_id,target_org_id,start_date,end_date,allow_chain,created_at)                     
                     values(@relationship_id,@vehicle_id,@vehicle_group_id,@owner_org_id,@created_org_id,@target_org_id,@start_date,@end_date,@allow_chain,@created_at)";

                    OwnerRelationshipId = await dataAccess.ExecuteScalarAsync<int>(queryInsert, Inputparameter);
                    return OwnerRelationshipId;
                }

                else if (isRelationshipExist > 1 && (!relationshipMapping.isFirstRelation)) // relationship exist          
                {
                    // update previuse relationship end date and insert new relationship              
                    var Updateparameter = new DynamicParameters();
                    Updateparameter.Add("@relationship_id", relationshipMapping.relationship_id);
                    Updateparameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    Updateparameter.Add("@vehicle_id", relationshipMapping.vehicle_id);
                    var queryUpdate = @"update master.orgrelationshipmapping 
                    set end_date=@end_date where relationship_id=@relationship_id and vehicle_id=@vehicle_id";
                    await dataAccess.ExecuteScalarAsync<int>(queryUpdate, Updateparameter);

                    // Insert new relationship              
                    var Inputparameter = new DynamicParameters();
                    Inputparameter.Add("@relationship_id", relationshipMapping.relationship_id);
                    Inputparameter.Add("@vehicle_id", relationshipMapping.vehicle_id);
                    if (relationshipMapping.vehicle_group_id == 0)
                    {
                        Inputparameter.Add("@vehicle_group_id", null);
                    }
                    else
                    {
                        Inputparameter.Add("@vehicle_group_id", relationshipMapping.vehicle_group_id);
                    }

                    Inputparameter.Add("@owner_org_id", relationshipMapping.owner_org_id);
                    Inputparameter.Add("@created_org_id", relationshipMapping.created_org_id);
                    Inputparameter.Add("@target_org_id", relationshipMapping.target_org_id);
                    Inputparameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    Inputparameter.Add("@end_date", null);
                    Inputparameter.Add("@allow_chain", relationshipMapping.allow_chain);
                    Inputparameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

                    var queryInsert = @"insert into master.orgrelationshipmapping (relationship_id,vehicle_id,vehicle_group_id,owner_org_id,created_org_id,
                    target_org_id,start_date,end_date,allow_chain,created_at)
                    values(@relationship_id,@vehicle_id,@vehicle_group_id,@owner_org_id,@created_org_id,@target_org_id,@start_date,@end_date,@allow_chain,@created_at)";
                    OwnerRelationshipId = await dataAccess.ExecuteScalarAsync<int>(queryInsert, Inputparameter);
                    return OwnerRelationshipId;
                }
            }
            catch (Exception ex)
            {
                log.Info("CreateOwnerRelationship method called in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }
            return 0;
        }

        public async Task<List<OrganizationResponse>> GetAll(int organizationId)      
        {
            log.Info("Get Organization method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                var query = @"SELECT id, org_id, type, name, address_type, street, street_number, postal_code, city, country_code, reference_date, is_active,vehicle_default_opt_in,driver_default_opt_in
	                        FROM master.organization org where  org.is_active=true";
                if (organizationId > 0)
                {
                    parameter.Add("@id", organizationId);
                    query = query + " and org.id=@id ";
                }
                          
                var OrganizationDetails = await dataAccess.QueryAsync<dynamic>(query, parameter);
                var objOrganization = new OrganizationResponse();
                objOrganization.OrganizationList = new List<OrganizationResponse>();
                foreach (dynamic record in OrganizationDetails)
                {

                    objOrganization.OrganizationList.Add(MapOrg(record));
                }
              
                return objOrganization.OrganizationList;
            }
            catch (Exception ex)
            {
                log.Info("Get Organization method in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                log.Error(ex.ToString());
                throw ex;
            }
        }

        private OrganizationResponse MapOrg(dynamic record)
        {
            var orgResponse = new OrganizationResponse();
            orgResponse.Id = record.id;
            orgResponse.type = record.type;
            orgResponse.name = record.name;
            orgResponse.street = record.street;
            orgResponse.address_type = record.address_type;
            orgResponse.street_number = record.street_number;
            orgResponse.postal_code = record.postal_code;
            orgResponse.city = record.city;
            orgResponse.country_code = record.country_code;
            orgResponse.org_id = record.org_id;
            orgResponse.is_active = record.is_active;
            orgResponse.reference_date = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(record.reference_date), "America/New_York", "yyyy-MM-ddTHH:mm:ss");
            orgResponse.vehicle_default_opt_in = record.vehicle_default_opt_in;
            orgResponse.driver_default_opt_in = record.driver_default_opt_in;
            return orgResponse;
        }
        // public async Task<int> CraeteOrganizationRelationship(OrganizationRelationship organizationRelationship)
        // {
        //     // 1. create  if not exist
        //     //feature_set_id  ---NULL by default
        //     //Name--need to defind patterns for this     

        //   int organizationRelationshipID = 0;       
        //  try{          
        //     var parameter = new DynamicParameters();
        //     parameter.Add("@organization_id", organizationRelationship.organization_id);
        //     var query = @"Select id from master.orgrelationship where organization_id=@organization_id";            
        //     int isRelationExist = await dataAccess.ExecuteScalarAsync<int>(query, parameter);
        //     if (isRelationExist < 1) // Organization Relationship not exist
        //     {
        //         var Inputparameter = new DynamicParameters();
        //         Inputparameter.Add("@organization_id", organizationRelationship.organization_id); 
        //         Inputparameter.Add("@feature_set_id", organizationRelationship.feature_set_id);
        //         Inputparameter.Add("@name", organizationRelationship.name);
        //         Inputparameter.Add("@description",organizationRelationship.description);  
        //         Inputparameter.Add("@code", organizationRelationship.code); 
        //         Inputparameter.Add("@is_active", organizationRelationship.is_active);  
        //         Inputparameter.Add("@level", organizationRelationship.level);
        //         Inputparameter.Add("@created_at", organizationRelationship.created_at);            

        //         var queryInsert = @"insert into master.orgrelationship(organization_id,feature_set_id,name,description,code,is_active,level,created_at)                     
        //                           values(@organization_id,@feature_set_id,@name,@description,@code,@is_active,@level,@created_at)";

        //         organizationRelationshipID = await dataAccess.ExecuteScalarAsync<int>(queryInsert, Inputparameter);
        //         return organizationRelationshipID;
        //     }            
        //     }
        //    catch (Exception ex)
        //     {
        //         log.Info("CraeteOrganizationRelationship method called in repository failed :");
        //         log.Error(ex.ToString());
        //         throw ex;
        //     }
        //     return organizationRelationshipID;           
        // }

        public async Task<List<OrganizationNameandID>> Get(OrganizationNameandID request)
        {
            log.Info("Get Organization method called in repository");
            try
            {
                List<OrganizationNameandID> objOrganizationNameandID = new List<OrganizationNameandID>();
                var parameter = new DynamicParameters();
                parameter.Add("@is_active", true);
                var query = @"SELECT id,name FROM master.organization where is_active=@is_active";
                var data = await dataAccess.QueryAsync<OrganizationNameandID>(query, parameter);
                return objOrganizationNameandID = data.Cast<OrganizationNameandID>().ToList();
            }
            catch (Exception ex)
            {
                log.Info("Get Organization method in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }
        }
        public async Task<int> IsOwnerRelationshipExist(int VehicleID)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vehicle_id", VehicleID);
                var query = @"Select id from master.orgrelationshipmapping where vehicle_id=@vehicle_id and end_date is null";
                return await dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch(Exception ex)
            {
                log.Info("IsOwnerRelationshipExist method in repository failed :");
                log.Error(ex.ToString());
                throw ex;
            }
        }
    }
}
