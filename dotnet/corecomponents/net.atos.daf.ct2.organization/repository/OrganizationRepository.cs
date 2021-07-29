using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using net.atos.daf.ct2.account;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using IdentitySessionComponent = net.atos.daf.ct2.identitysession;
using SubscriptionComponent = net.atos.daf.ct2.subscription;

namespace net.atos.daf.ct2.organization.repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IVehicleManager _vehicelManager;
        private readonly IGroupManager _groupManager;
        private readonly IAccountManager _accountManager;
        readonly SubscriptionComponent.ISubscriptionManager _subscriptionManager;
        readonly IdentitySessionComponent.IAccountSessionManager _accountSessionManager;
        readonly IdentitySessionComponent.IAccountTokenManager _accountTokenManager;

        private static readonly log4net.ILog _log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public OrganizationRepository(IDataAccess dataAccess, IVehicleManager vehicleManager, IGroupManager groupManager, IAccountManager accountManager, SubscriptionComponent.ISubscriptionManager subscriptionManager, IdentitySessionComponent.IAccountSessionManager accountSessionManager, IdentitySessionComponent.IAccountTokenManager accountTokenManager)
        {
            _dataAccess = dataAccess;
            _vehicelManager = vehicleManager;
            _groupManager = groupManager;
            _accountManager = accountManager;
            _subscriptionManager = subscriptionManager;
            _accountSessionManager = accountSessionManager;
            _accountTokenManager = accountTokenManager;
        }
        public async Task<Organization> Create(Organization organization)
        {
            _log.Info("Create Organization method called in repository");
            try
            {

                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@org_id", organization.OrganizationId);
                var query = @"SELECT id FROM master.organization where org_id=@org_id";
                int orgexist = await _dataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);

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
                    parameter.Add("@ReferencedDate", organization.ReferenceDate != null ? UTCHandling.GetUTCFromDateTime(organization.ReferenceDate.ToString()) : (long?)null);
                    parameter.Add("@vehicle_default_opt_in", "I");
                    parameter.Add("@driver_default_opt_in", "I");
                    string queryInsert = "insert into master.organization(org_id, type, name, address_type, street, street_number, postal_code, city,country_code,reference_date,preference_id,vehicle_default_opt_in,driver_default_opt_in) " +
                                  "values(@OrganizationId, @OrganizationType, @Name, @AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode,@ReferencedDate,null,@vehicle_default_opt_in,@driver_default_opt_in) RETURNING id";

                    var orgid = await _dataAccess.ExecuteScalarAsync<int>(queryInsert, parameter);
                    organization.Id = orgid;

                    // Create dynamic account group
                    Group groupAccount = new Group();
                    groupAccount.ObjectType = ObjectType.AccountGroup;
                    groupAccount.GroupType = GroupType.Dynamic;
                    groupAccount.Argument = "";
                    groupAccount.FunctionEnum = FunctionEnum.All;
                    groupAccount.OrganizationId = orgid;
                    groupAccount.RefId = 0;
                    groupAccount.Name = "DefaultAccountGroup";
                    groupAccount.Description = "DefaultAccountGroup";
                    groupAccount.CreatedAt = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                    groupAccount = await _groupManager.Create(groupAccount);

                    // Create dynamic vehicle group
                    Group groupVehicle = new Group();
                    groupVehicle.ObjectType = ObjectType.VehicleGroup;
                    groupVehicle.GroupType = GroupType.Dynamic;
                    groupVehicle.Argument = "";
                    groupVehicle.FunctionEnum = FunctionEnum.All;
                    groupVehicle.OrganizationId = orgid;
                    groupVehicle.RefId = 0;
                    groupVehicle.Name = "DefaultVehicleGroup";
                    groupVehicle.Description = "DefaultVehicleGroup";
                    groupVehicle.CreatedAt = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                    groupVehicle = await _groupManager.Create(groupVehicle);

                    //// Create access relationship
                    //AccessRelationship accessRelationship = new AccessRelationship();
                    //accessRelationship.AccountGroupId = groupAccount.Id;
                    //accessRelationship.VehicleGroupId = groupVehicle.Id;
                    //accessRelationship.AccessRelationType = AccountComponent.ENUM.AccessRelationType.ViewOnly;
                    //await accountManager.CreateAccessRelationship(accessRelationship);

                }
            }
            catch (Exception ex)
            {
                // log.Info("Create Organization method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organization));
                _log.Error(ex.ToString());
                throw;
            }
            return organization;
        }

        public async Task<bool> Delete(int organizationId)
        {
            _log.Info("Delete Organization method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", organizationId);
                var query = @"update master.organization set state='D' where id=@id";
                int isdelete = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception ex)
            {
                _log.Info("Delete Organization method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<Organization> Update(Organization organization)
        {
            _log.Info("Update Organization method called in repository");
            try
            {
                var parameterduplicate = new DynamicParameters();
                parameterduplicate.Add("@org_id", organization.OrganizationId);
                var query = @"SELECT id FROM master.organization where org_id=@org_id and state='A'";
                int orgexist = await _dataAccess.ExecuteScalarAsync<int>(query, parameterduplicate);
                if (orgexist > 0)
                {
                    organization.Id = -1;
                    return organization;
                }
                else
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@Id", organization.Id);
                    //parameter.Add("@OrganizationId", organization.OrganizationId);
                    //parameter.Add("@OrganizationType", organization.Type);
                    //parameter.Add("@Name", organization.Name);
                    //parameter.Add("@AddressType", organization.AddressType);
                    //parameter.Add("@AddressStreet", organization.AddressStreet);
                    //parameter.Add("@AddressStreetNumber", organization.AddressStreetNumber);
                    //parameter.Add("@PostalCode", organization.PostalCode);
                    //parameter.Add("@City", organization.City);
                    //parameter.Add("@CountryCode", organization.CountryCode);
                    //parameter.Add("@ReferencedDate", organization.reference_date != null ? UTCHandling.GetUTCFromDateTime(organization.reference_date.ToString()) : (long?)null);
                    parameter.Add("@vehicleoptin", organization.VehicleDefaultOptIn);
                    parameter.Add("@driveroptin", organization.DriverDefaultOptIn);
                    //parameter.Add("@IsActive", organization.IsActive); 

                    //   var queryUpdate = @"update master.organization set org_id=@OrganizationId, type=@OrganizationType, name=@Name,
                    //address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
                    // postal_code=@PostalCode, city=@City,country_code=@CountryCode,reference_date=@ReferencedDate,vehicle_default_opt_in=@vehicleoptin,driver_default_opt_in=@driveroptin              
                    //                 WHERE id = @Id RETURNING id;";

                    var queryUpdate = @"update master.organization set vehicle_default_opt_in=@vehicleoptin,driver_default_opt_in=@driveroptin              
	                                 WHERE id = @Id RETURNING id;";
                    var orgid = await _dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameter);
                    if (orgid < 1)
                    {
                        organization.Id = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Info("Update Organization method in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
            return organization;
        }

        public async Task<OrganizationResponse> Get(int organizationId)
        {
            _log.Info("Get Organization method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                var query = @"SELECT id Id, 
                    org_id OrgId,
                    type as Type,
                    coalesce(name, '(' || org_id || ')') as Name, 
                    address_type as AddressType, 
                    street as Street,
                    street_number as StreetNumber, 
                    postal_code as PostalCode,
                    city as City, 
                    country_code as CountryCode, 
                    reference_date as ReferenceDate, 
                    state as State,
                    vehicle_default_opt_in as VehicleDefaultOptIn ,
                    driver_default_opt_in as DriverDefaultOptIn 
                FROM master.organization where id=@Id and state='A'";
                parameter.Add("@Id", organizationId);
                IEnumerable<OrganizationResponse> OrganizationDetails = await _dataAccess.QueryAsync<OrganizationResponse>(query, parameter);
                OrganizationResponse objOrganization = new OrganizationResponse();
                foreach (var item in OrganizationDetails)
                {
                    objOrganization.Id = item.Id;
                    objOrganization.OrgId = item.OrgId;
                    objOrganization.Type = item.Type;
                    objOrganization.Name = item.Name;
                    objOrganization.AddressType = item.AddressType;
                    objOrganization.Street = item.Street;
                    objOrganization.StreetNumber = item.StreetNumber;
                    objOrganization.PostalCode = item.PostalCode;
                    objOrganization.City = item.City;
                    objOrganization.CountryCode = item.CountryCode;
                    objOrganization.State = item.State;
                    objOrganization.ReferenceDate = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(item.ReferenceDate), "UTC", "yyyy-MM-ddTHH:mm:ss");
                    objOrganization.VehicleDefaultOptIn = item.VehicleDefaultOptIn;
                    objOrganization.DriverDefaultOptIn = item.DriverDefaultOptIn;
                }
                if (objOrganization.Id < 1)
                {
                    objOrganization.Id = 0;
                }

                return objOrganization;
            }
            catch (Exception ex)
            {
                _log.Info("Get Organization method in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
        }
        public async Task<OrganizationDetailsResponse> GetOrganizationDetails(int organizationId)
        {
            _log.Info("Get Organization details method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                var query = @"SELECT
                              a.id PreferenceId,
                              o.id Id,
                              o.org_id OrgId,
                              coalesce(o.name, '') as Name,                             
                              o.city City,                             
                              o.street Street,
                              o.street_number StreetNumber,
                              o.postal_code PostalCode,
                              o.country_code CountryCode,
                              o.vehicle_default_opt_in VehicleDefaultOptIn,
                              o.driver_default_opt_in DriverDefaultOptIn,
                              c.name Currency,
                              t.name Timezone,
                              tf.name TimeFormat,
                              df.name DateFormatType,
                              l.name LanguageName,
                              u.name Unit
                            FROM master.organization o
                            left join  master.accountpreference a on o.preference_id=a.id
                            left join  master.currency c on c.id=a.currency_id
                            left join  master.timezone t on t.id=a.timezone_id
                            left join  master.timeformat tf on tf.id=a.time_format_id
                            left join  master.dateformat df on df.id=a.date_format_id
                            left join  master.unit u on u.id=a.unit_id
                            left join  translation.language l on l.id=a.language_id                                                      
                            where o.id=@Id and o.state='A'";
                parameter.Add("@Id", organizationId);
                IEnumerable<OrganizationDetailsResponse> OrgDetails = await _dataAccess.QueryAsync<OrganizationDetailsResponse>(query, parameter);
                OrganizationDetailsResponse OrgDetailsResponse = new OrganizationDetailsResponse();
                foreach (var item in OrgDetails)
                {
                    OrgDetailsResponse.Id = item.Id;
                    OrgDetailsResponse.PreferenceId = item.PreferenceId;
                    OrgDetailsResponse.OrgId = item.OrgId;
                    OrgDetailsResponse.Name = item.Name;
                    OrgDetailsResponse.City = item.City;
                    OrgDetailsResponse.CountryCode = item.CountryCode;
                    OrgDetailsResponse.Street = item.Street;
                    OrgDetailsResponse.StreetNumber = item.StreetNumber;
                    OrgDetailsResponse.PostalCode = item.PostalCode;
                    OrgDetailsResponse.VehicleDefaultOptIn = item.VehicleDefaultOptIn;
                    OrgDetailsResponse.DriverDefaultOptIn = item.DriverDefaultOptIn;

                    OrgDetailsResponse.LanguageName = item.LanguageName;
                    OrgDetailsResponse.Timezone = item.Timezone;
                    OrgDetailsResponse.TimeFormat = item.TimeFormat;
                    OrgDetailsResponse.Currency = item.Currency;
                    OrgDetailsResponse.Unit = item.Unit;
                    OrgDetailsResponse.DateFormatType = item.DateFormatType;
                }
                return OrgDetailsResponse;
            }
            catch (Exception ex)
            {
                _log.Info("Get Organization preference method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<PreferenceResponse> GetPreference(int organizationId)
        {
            _log.Info("Get Organization preference method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                var query = @"SELECT o.id OrganizationId,
                            a.id PreferenceId,
                            c.id Currency,
                            t.id Timezone,
                            tf.id TimeFormat,
                            vd.id VehicleDisplay,
                            df.id DateFormatType,
                            l.id LanguageName,
                            u.id Unit
                            FROM master.organization o
                            left join  master.accountpreference a on o.preference_id=a.id
                            left join  master.currency c on c.id=a.currency_id
                            left join  master.timezone t on t.id=a.timezone_id
                            left join  master.timeformat tf on tf.id=a.time_format_id
                            left join  master.vehicledisplay vd on vd.id=a.vehicle_display_id
                            left join  master.dateformat df on df.id=a.date_format_id
                            left join  master.unit u on u.id=a.unit_id
                            left join  translation.language l on l.id=a.language_id
                            where o.id=@Id";
                parameter.Add("@Id", organizationId);
                IEnumerable<PreferenceResponse> preferenceDetails = await _dataAccess.QueryAsync<PreferenceResponse>(query, parameter);
                PreferenceResponse preferenceResponse = new PreferenceResponse();
                foreach (var item in preferenceDetails)
                {
                    preferenceResponse.PreferenceId = item.PreferenceId;
                    preferenceResponse.OrganizationId = item.OrganizationId;
                    preferenceResponse.LanguageName = item.LanguageName;
                    preferenceResponse.Timezone = item.Timezone;
                    preferenceResponse.TimeFormat = item.TimeFormat;
                    preferenceResponse.Currency = item.Currency;
                    preferenceResponse.Unit = item.Unit;
                    preferenceResponse.VehicleDisplay = item.VehicleDisplay;
                    preferenceResponse.DateFormatType = item.DateFormatType;
                }
                return preferenceResponse;
            }
            catch (Exception ex)
            {
                _log.Info("Get Organization preference method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
        }


        public async Task<CustomerRequest> UpdateCustomer(CustomerRequest customer)
        {
            _log.Info("Update Customer method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@org_id", customer.CustomerID);
                var query = @"SELECT id FROM master.organization where org_id=@org_id and state='A'";
                int iscustomerexist = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);

                if (iscustomerexist > 0)
                {
                    Int64 referenceDateTime;
                    var parameterUpdate = new DynamicParameters();
                    parameterUpdate.Add("@org_id", customer.CustomerID);
                    //parameterUpdate.Add("@Name", customer.CustomerName);
                    if ((customer.CustomerName != null) && (customer.CustomerName.Trim().Length > 0))
                    {
                        parameterUpdate.Add("@Name", customer.CustomerName);
                    }
                    if ((customer.CompanyType != null) && (customer.CompanyType.Trim().Length > 0))
                    {
                        parameterUpdate.Add("@Type", customer.CompanyType);
                    }
                    if ((customer.AddressType != null) && (customer.AddressType.Trim().Length > 0))
                    {
                        parameterUpdate.Add("@AddressType", customer.AddressType);
                    }
                    if ((customer.Street != null) && (customer.Street.Trim().Length > 0))
                    {
                        parameterUpdate.Add("@AddressStreet", customer.Street);
                    }
                    if ((customer.StreetNumber != null) && (customer.StreetNumber.Trim().Length > 0))
                    {
                        parameterUpdate.Add("@AddressStreetNumber", customer.StreetNumber);
                    }
                    if ((customer.PostalCode != null) && (customer.PostalCode.Trim().Length > 0))
                    {
                        parameterUpdate.Add("@PostalCode", customer.PostalCode);
                    }
                    if ((customer.City != null) && (customer.City.Trim().Length > 0))
                    {
                        parameterUpdate.Add("@City", customer.City);
                    }
                    if ((customer.CountryCode != null) && (customer.CountryCode.Trim().Length > 0))
                    {
                        parameterUpdate.Add("@CountryCode", customer.CountryCode);
                    }
                    if ((customer.ReferenceDateTime != null) && (DateTime.Compare(DateTime.MinValue, customer.ReferenceDateTime) < 0))
                    {
                        referenceDateTime = UTCHandling.GetUTCFromDateTime(customer.ReferenceDateTime, "UTC");
                    }
                    else
                    {
                        referenceDateTime = 0;
                    }

                    parameterUpdate.Add("@reference_date", referenceDateTime);
                    //   var queryUpdate = @"update master.organization set org_id=@org_id, name=@Name,type=@Type,
                    //address_type=@AddressType, street=@AddressStreet, street_number=@AddressStreetNumber,
                    // postal_code=@PostalCode, city=@City,country_code=@CountryCode,reference_date=@reference_date                               
                    //                 WHERE org_id = @org_id RETURNING id;";

                    var queryUpdate = @"update master.organization set org_id=@org_id, reference_date=@reference_date";

                    if ((customer.CustomerName != null) && (customer.CustomerName.Trim().Length > 0))
                    {
                        queryUpdate = queryUpdate + @", name = @Name";
                    }
                    if ((customer.CompanyType != null) && (customer.CompanyType.Trim().Length > 0))
                    {
                        queryUpdate = queryUpdate + @", type = @Type";
                    }
                    if ((customer.AddressType != null) && (customer.AddressType.Trim().Length > 0))
                    {
                        queryUpdate = queryUpdate + @", address_type=@AddressType";
                    }
                    if ((customer.Street != null) && (customer.Street.Trim().Length > 0))
                    {
                        queryUpdate = queryUpdate + @", street=@AddressStreet";
                    }
                    if ((customer.StreetNumber != null) && (customer.StreetNumber.Trim().Length > 0))
                    {
                        queryUpdate = queryUpdate + @", street_number=@AddressStreetNumber";
                    }
                    if ((customer.PostalCode != null) && (customer.PostalCode.Trim().Length > 0))
                    {
                        queryUpdate = queryUpdate + @", postal_code=@PostalCode";
                    }
                    if ((customer.City != null) && (customer.City.Trim().Length > 0))
                    {
                        queryUpdate = queryUpdate + @", city=@City";
                    }
                    if ((customer.CountryCode != null) && (customer.CountryCode.Trim().Length > 0))
                    {
                        queryUpdate = queryUpdate + @", country_code=@CountryCode";
                    }
                    queryUpdate = queryUpdate + @" WHERE org_id = @org_id RETURNING id;";

                    await _dataAccess.ExecuteScalarAsync<int>(queryUpdate, parameterUpdate);

                    // Assign base package at ORG lavel if not exist                   
                    await _subscriptionManager.Create(iscustomerexist, Convert.ToInt32(customer.OrgCreationPackage));

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
                    parameterInsert.Add("@state", "A");
                    string queryInsert = "insert into master.organization(org_id, name,type ,address_type, street, street_number, postal_code, city,country_code,reference_date,vehicle_default_opt_in,driver_default_opt_in,state) " +
                                  "values(@org_id, @Name,@Type ,@AddressType, @AddressStreet,@AddressStreetNumber ,@PostalCode,@City,@CountryCode,@reference_date,@vehicle_default_opt_in,@driver_default_opt_in,@state) RETURNING id";

                    int organizationId = await _dataAccess.ExecuteScalarAsync<int>(queryInsert, parameterInsert);

                    // Create dynamic account group
                    await CreateDefaultGroups(organizationId);

                    // Assign base package at ORG lavel
                    await _subscriptionManager.Create(organizationId, Convert.ToInt32(customer.OrgCreationPackage));
                }
            }
            catch (Exception ex)
            {
                _log.Info("Update Customer method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
            return customer;
        }

        public async Task<int> CreateDefaultGroups(int organizationId)
        {
            try
            {
                Group groupAccount = new Group();
                groupAccount.ObjectType = ObjectType.AccountGroup;
                groupAccount.GroupType = GroupType.Dynamic;
                groupAccount.Argument = "";
                groupAccount.FunctionEnum = FunctionEnum.All;
                groupAccount.OrganizationId = organizationId;
                groupAccount.RefId = 0;
                groupAccount.Name = "DefaultAccountGroup";
                groupAccount.Description = "DefaultAccountGroup";
                groupAccount.CreatedAt = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                groupAccount = await _groupManager.Create(groupAccount);

                // Create dynamic vehicle group
                Group groupVehicle = new Group();
                groupVehicle.ObjectType = ObjectType.VehicleGroup;
                groupVehicle.GroupType = GroupType.Dynamic;
                groupVehicle.Argument = "";
                groupVehicle.FunctionEnum = FunctionEnum.All;
                groupVehicle.OrganizationId = organizationId;
                groupVehicle.RefId = 0;
                groupVehicle.Name = "DefaultVehicleGroup";
                groupVehicle.Description = "DefaultVehicleGroup";
                groupVehicle.CreatedAt = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                groupVehicle = await _groupManager.Create(groupVehicle);
            }
            catch (Exception ex)
            {
                _log.Info("CreateDefaultGroups method called in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
            return 1;
        }

        public async Task<int> UpdateCompany(HandOver keyHandOver)
        {
            try
            {
                var parameterOrgUpdate = new DynamicParameters();
                parameterOrgUpdate.Add("@org_id", keyHandOver.CustomerID);
                if ((keyHandOver.CustomerName != null) && (keyHandOver.CustomerName.Trim().Length > 0))
                {
                    parameterOrgUpdate.Add("@Name", keyHandOver.CustomerName);
                }
                if ((keyHandOver.Type != null) && (keyHandOver.Type.Trim().Length > 0))
                {
                    parameterOrgUpdate.Add("@AddressType", keyHandOver.Type);
                }
                if ((keyHandOver.Street != null) && (keyHandOver.Street.Trim().Length > 0))
                {
                    parameterOrgUpdate.Add("@AddressStreet", keyHandOver.Street);
                }
                if ((keyHandOver.StreetNumber != null) && (keyHandOver.StreetNumber.Trim().Length > 0))
                {
                    parameterOrgUpdate.Add("@AddressStreetNumber", keyHandOver.StreetNumber);
                }
                if ((keyHandOver.PostalCode != null) && (keyHandOver.PostalCode.Trim().Length > 0))
                {
                    parameterOrgUpdate.Add("@PostalCode", keyHandOver.PostalCode);
                }
                if ((keyHandOver.City != null) && (keyHandOver.City.Trim().Length > 0))
                {
                    parameterOrgUpdate.Add("@City", keyHandOver.City);
                }
                if ((keyHandOver.CountryCode != null) && (keyHandOver.CountryCode.Trim().Length > 0))
                {
                    parameterOrgUpdate.Add("@CountryCode", keyHandOver.CountryCode);
                }

                if (keyHandOver.ReferenceDateTime != null)
                {
                    parameterOrgUpdate.Add("@reference_date", UTCHandling.GetUTCFromDateTime(keyHandOver.ReferenceDateTime, "UTC"));
                }
                else
                {
                    parameterOrgUpdate.Add("@reference_date", 0);
                }

                var queryOrgUpdate = @"update master.organization set org_id=@org_id, reference_date=@reference_date";
                if ((keyHandOver.CustomerName != null) && (keyHandOver.CustomerName.Trim().Length > 0))
                {
                    queryOrgUpdate = queryOrgUpdate + @", name=@Name";
                }
                if ((keyHandOver.Type != null) && (keyHandOver.Type.Trim().Length > 0))
                {
                    queryOrgUpdate = queryOrgUpdate + @", address_type=@AddressType";
                }
                if ((keyHandOver.Street != null) && (keyHandOver.Street.Trim().Length > 0))
                {
                    queryOrgUpdate = queryOrgUpdate + @", street=@AddressStreet";
                }
                if ((keyHandOver.StreetNumber != null) && (keyHandOver.StreetNumber.Trim().Length > 0))
                {
                    queryOrgUpdate = queryOrgUpdate + @", street_number=@AddressStreetNumber";
                }
                if ((keyHandOver.PostalCode != null) && (keyHandOver.PostalCode.Trim().Length > 0))
                {
                    queryOrgUpdate = queryOrgUpdate + @", postal_code=@PostalCode";
                }
                if ((keyHandOver.City != null) && (keyHandOver.City.Trim().Length > 0))
                {
                    queryOrgUpdate = queryOrgUpdate + @", city=@City";
                }
                if ((keyHandOver.CountryCode != null) && (keyHandOver.CountryCode.Trim().Length > 0))
                {
                    queryOrgUpdate = queryOrgUpdate + @", country_code=@CountryCode";
                }

                queryOrgUpdate = queryOrgUpdate + @" WHERE org_id = @org_id RETURNING id;";

                return await _dataAccess.ExecuteScalarAsync<int>(queryOrgUpdate, parameterOrgUpdate);
            }
            catch (Exception ex)
            {
                _log.Info("UpdateCompany method called in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<int> InsertCompany(HandOver keyHandOver)
        {
            try
            {
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
                parameterOrgInsert.Add("@driver_default_opt_in", "U");
                parameterOrgInsert.Add("@state", "A");

                if (keyHandOver.ReferenceDateTime != null)
                {
                    parameterOrgInsert.Add("@reference_date", UTCHandling.GetUTCFromDateTime(keyHandOver.ReferenceDateTime, "UTC"));
                }
                else
                {
                    parameterOrgInsert.Add("@reference_date", 0);
                }

                string queryOrgInsert = "insert into master.organization(org_id,name,address_type,street,street_number,postal_code,city,country_code,reference_date,vehicle_default_opt_in,driver_default_opt_in,state) " +
                              "values(@org_id,@Name,@AddressType,@AddressStreet,@AddressStreetNumber,@PostalCode,@City,@CountryCode,@reference_date,@vehicle_default_opt_in,@driver_default_opt_in,@state) RETURNING id";

                return await _dataAccess.ExecuteScalarAsync<int>(queryOrgInsert, parameterOrgInsert);
            }
            catch (Exception ex)
            {
                _log.Info("InsertCompany method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<int> InsertVehicle(HandOver keyHandOver, int OrganizationId)
        {
            try
            {
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

                Vehicle objvehicle = new Vehicle();
                objvehicle.Organization_Id = OrganizationId;
                objvehicle.VIN = keyHandOver.VIN;
                objvehicle.Tcu_Id = keyHandOver.TCUID;
                objvehicle.Is_Tcu_Register = istcuactive;
                objvehicle.Reference_Date = keyHandOver.ReferenceDateTime;

                objvehicle.Oem_id = Convert.ToInt32(keyHandOver.OEMRelationship);
                objvehicle.Oem_Organisation_id = OrganizationId;
                objvehicle.Status_Changed_Date = System.DateTime.Now;
                objvehicle.CreatedAt = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);

                // NULL FIELDS
                objvehicle.Name = null;
                objvehicle.License_Plate_Number = null;
                objvehicle.Termination_Date = null;
                objvehicle.Vid = null;
                objvehicle.Type = objvehicle.Type;
                objvehicle.Tcu_Serial_Number = null;
                objvehicle.Tcu_Brand = null;
                objvehicle.Tcu_Version = null;
                objvehicle.VehiclePropertiesId = null;
                objvehicle.ModelId = null;
                objvehicle.Opt_In = VehicleStatusType.Inherit;
                objvehicle.Is_Ota = false;

                await _vehicelManager.Create(objvehicle);
                return 1;
            }
            catch (Exception ex)
            {
                _log.Info("InsertVehicle method called in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
        }
        public async Task<int> UpdatetVehicle(HandOver keyHandOver, int OrganizationId)
        {
            try
            {
                Vehicle objvehicle = new Vehicle();
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

                objvehicle.Reference_Date = keyHandOver.ReferenceDateTime;

                objvehicle.Is_Tcu_Register = istcuactive;
                objvehicle.VIN = keyHandOver.VIN;
                objvehicle.Tcu_Id = keyHandOver.TCUID;
                objvehicle.Vid = null;
                objvehicle.Tcu_Brand = null;
                objvehicle.Tcu_Serial_Number = null;
                objvehicle.Tcu_Version = null;
                objvehicle.Organization_Id = OrganizationId;
                await _vehicelManager.UpdateOrgVehicleDetails(objvehicle);
            }
            catch (Exception ex)
            {
                _log.Info("UpdatetVehicle method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
            return 1;
        }

        public async Task<int> OwnerRelationship(HandOver keyHandOver, int VehicleID)
        {
            try
            {
                RelationshipMapping relationshipMapping = new RelationshipMapping();
                relationshipMapping.RelationshipId = Convert.ToInt32(keyHandOver.OwnerRelationship);
                relationshipMapping.OwnerOrgId = Convert.ToInt32(keyHandOver.OEMRelationship);
                relationshipMapping.TargetOrgId = Convert.ToInt32(keyHandOver.OEMRelationship);
                relationshipMapping.CreatedOrgId = Convert.ToInt32(keyHandOver.OEMRelationship);
                relationshipMapping.VehicleId = VehicleID;
                relationshipMapping.StartDate = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                relationshipMapping.CreatedAt = UTCHandling.GetUTCFromDateTime(System.DateTime.Now);
                relationshipMapping.AllowChain = true;
                //relationshipMapping.isFirstRelation=true;  
                relationshipMapping.IsFirstRelation = true;
                await CreateOwnerRelationship(relationshipMapping);
            }
            catch (Exception ex)
            {
                _log.Info("UpdatetVehicle method called in repository failed :");
                _log.Error(ex.ToString());
                throw;
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

            _log.Info("KeyHandOverEvent method is called in repository :");
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@org_id", keyHandOver.CustomerID);
                var query = @"select coalesce((Select id from master.organization where org_id=@org_id), 0)";
                int iscustomerexist = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                int isVINExist = await _vehicelManager.IsVINExists(keyHandOver.VIN);

                if (iscustomerexist > 0 && isVINExist > 0)  // Update organization and vehicle
                {
                    int OrganizationId = await UpdateCompany(keyHandOver);
                    await UpdatetVehicle(keyHandOver, OrganizationId);

                    // Owner Relationship Management

                    keyHandOver.OEMRelationship = OrganizationId.ToString();
                    await OwnerRelationship(keyHandOver, isVINExist);

                    return keyHandOver;
                }

                else if (iscustomerexist < 1 && isVINExist < 1)  // Insert organization and vehicle
                {
                    // Insert Company
                    int organizationID = await InsertCompany(keyHandOver);
                    await CreateDefaultGroups(organizationID);

                    // Insert Vehicle                    
                    await InsertVehicle(keyHandOver, organizationID);

                    // Owner Relationship Management    
                    int vehicleID = await _vehicelManager.IsVINExists(keyHandOver.VIN);

                    keyHandOver.OEMRelationship = organizationID.ToString();
                    await OwnerRelationship(keyHandOver, vehicleID);

                    return keyHandOver;
                }

                else if (iscustomerexist > 0 && isVINExist < 1) // Update organization and insert vehicle
                {
                    // Update company         
                    int organizationID = await UpdateCompany(keyHandOver);

                    // insert vehicle
                    await InsertVehicle(keyHandOver, organizationID);

                    // Owner Relationship Management   
                    int vehicleID = await _vehicelManager.IsVINExists(keyHandOver.VIN);

                    keyHandOver.OEMRelationship = organizationID.ToString();
                    await OwnerRelationship(keyHandOver, vehicleID);

                    return keyHandOver;
                }

                else if (iscustomerexist < 1 && isVINExist > 0) // Insert organization and update vehicle
                {
                    int organizationID = await InsertCompany(keyHandOver);
                    await CreateDefaultGroups(organizationID);
                    await UpdatetVehicle(keyHandOver, organizationID);

                    // Owner Relationship Management  
                    keyHandOver.OEMRelationship = organizationID.ToString();
                    await OwnerRelationship(keyHandOver, isVINExist);
                }
            }
            catch (Exception ex)
            {
                _log.Info("KeyHandOverEvent method called in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
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

                int isRelationshipExist = await IsOwnerRelationshipExist(relationshipMapping.VehicleId);

                if (isRelationshipExist < 1 && relationshipMapping.IsFirstRelation) // relationship not exist
                                                                                    // if (iscustomerexist< 1)
                {
                    var Inputparameter = new DynamicParameters();
                    Inputparameter.Add("@relationship_id", relationshipMapping.RelationshipId);  // from property file
                    Inputparameter.Add("@vehicle_id", relationshipMapping.VehicleId);
                    if (relationshipMapping.VehicleGroupId == 0)
                    {
                        Inputparameter.Add("@vehicle_group_id", null);
                    }
                    else
                    {
                        Inputparameter.Add("@vehicle_group_id", relationshipMapping.VehicleGroupId);
                    }
                    Inputparameter.Add("@owner_org_id", relationshipMapping.OwnerOrgId);    // from property file 
                    Inputparameter.Add("@created_org_id", relationshipMapping.CreatedOrgId); // from property file --- first time it will same as owner_org_id
                    Inputparameter.Add("@target_org_id", relationshipMapping.TargetOrgId);  // from property file -- first time it will same as owner_org_id
                    Inputparameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    Inputparameter.Add("@end_date", null);   // First time -- NULL
                    Inputparameter.Add("@allow_chain", relationshipMapping.AllowChain);   // Alway true
                    Inputparameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));   // Alway true

                    var queryInsert = @"insert into master.orgrelationshipmapping(relationship_id,vehicle_id,vehicle_group_id,
                     owner_org_id,created_org_id,target_org_id,start_date,end_date,allow_chain,created_at)                     
                     values(@relationship_id,@vehicle_id,@vehicle_group_id,@owner_org_id,@created_org_id,@target_org_id,@start_date,@end_date,@allow_chain,@created_at)";

                    OwnerRelationshipId = await _dataAccess.ExecuteScalarAsync<int>(queryInsert, Inputparameter);
                    return OwnerRelationshipId;
                }

                else if (isRelationshipExist > 1 && relationshipMapping.IsFirstRelation) // relationship exist          
                {
                    // Check organization relationship with vehicle
                    //int orgRelExist = await IsOrganizationRelationshipExist(relationshipMapping);
                    //if (orgRelExist < 0)
                    //{
                    // update previuse relationship end date and insert new relationship              
                    var Updateparameter = new DynamicParameters();
                    Updateparameter.Add("@relationship_id", relationshipMapping.RelationshipId);
                    // Updateparameter.Add("@relationship_id", isRelationshipExist);
                    Updateparameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    Updateparameter.Add("@vehicle_id", relationshipMapping.VehicleId);
                    var queryUpdate = @"update master.orgrelationshipmapping 
                    set end_date=@end_date where relationship_id=@relationship_id and vehicle_id=@vehicle_id";
                    await _dataAccess.ExecuteScalarAsync<int>(queryUpdate, Updateparameter);

                    // Insert new relationship              
                    var Inputparameter = new DynamicParameters();
                    Inputparameter.Add("@relationship_id", relationshipMapping.RelationshipId);
                    Inputparameter.Add("@vehicle_id", relationshipMapping.VehicleId);
                    if (relationshipMapping.VehicleGroupId == 0)
                    {
                        Inputparameter.Add("@vehicle_group_id", null);
                    }
                    else
                    {
                        Inputparameter.Add("@vehicle_group_id", relationshipMapping.VehicleGroupId);
                    }

                    Inputparameter.Add("@owner_org_id", relationshipMapping.OwnerOrgId);
                    Inputparameter.Add("@created_org_id", relationshipMapping.CreatedOrgId);
                    Inputparameter.Add("@target_org_id", relationshipMapping.TargetOrgId);
                    Inputparameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    Inputparameter.Add("@end_date", null);
                    Inputparameter.Add("@allow_chain", relationshipMapping.AllowChain);
                    Inputparameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

                    var queryInsert = @"insert into master.orgrelationshipmapping (relationship_id,vehicle_id,vehicle_group_id,owner_org_id,created_org_id,
                    target_org_id,start_date,end_date,allow_chain,created_at)
                    values(@relationship_id,@vehicle_id,@vehicle_group_id,@owner_org_id,@created_org_id,@target_org_id,@start_date,@end_date,@allow_chain,@created_at)";
                    OwnerRelationshipId = await _dataAccess.ExecuteScalarAsync<int>(queryInsert, Inputparameter);
                    return OwnerRelationshipId;
                }
                //}
            }
            catch (Exception ex)
            {
                _log.Info("CreateOwnerRelationship method called in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
            return 0;
        }

        public async Task<List<OrganizationResponse>> GetAll(int organizationId)
        {
            _log.Info("Get Organization method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                var query = @"SELECT id, org_id, type,coalesce(name, '(' || org_id || ')') as name, address_type, street, street_number, postal_code, city, country_code, reference_date, state,vehicle_default_opt_in,driver_default_opt_in
	                        FROM master.organization org where org.state='A'";
                if (organizationId > 0)
                {
                    parameter.Add("@id", organizationId);
                    query = query + " and org.id=@id ";
                }

                var OrganizationDetails = await _dataAccess.QueryAsync<dynamic>(query, parameter);
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
                _log.Info("Get Organization method in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<IEnumerable<OrganizationContextListResponse>> GetAllOrganizationsForContext()
        {
            try
            {
                var query = @"SELECT id,coalesce(name, '(' || org_id || ')') as name
	                        FROM master.organization org where org.state='A'";

                return await _dataAccess.QueryAsync<OrganizationContextListResponse>(query, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private OrganizationResponse MapOrg(dynamic record)
        {
            var orgResponse = new OrganizationResponse();
            orgResponse.Id = record.id;
            orgResponse.Type = record.type;
            orgResponse.Name = record.name;
            orgResponse.Street = record.street;
            orgResponse.AddressType = record.address_type;
            orgResponse.StreetNumber = record.street_number;
            orgResponse.PostalCode = record.postal_code;
            orgResponse.City = record.city;
            orgResponse.CountryCode = record.country_code;
            orgResponse.OrgId = record.org_id;
            orgResponse.State = Convert.ToChar(record.state);
            orgResponse.ReferenceDate = UTCHandling.GetConvertedDateTimeFromUTC(Convert.ToInt64(record.reference_date), "UTC", "yyyy-MM-ddTHH:mm:ss");
            orgResponse.VehicleDefaultOptIn = record.vehicle_default_opt_in;
            orgResponse.DriverDefaultOptIn = record.driver_default_opt_in;
            return orgResponse;
        }

        public string MapCharToState(string state)
        {
            var ptype = string.Empty;
            switch (state)
            {
                case "A":
                    ptype = "Active";
                    break;
                case "I":
                    ptype = "Inactive";
                    break;
                case "D":
                    ptype = "Delete";
                    break;
            }
            return ptype;

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
        //         throw;
        //     }
        //     return organizationRelationshipID;           
        // }

        public async Task<List<OrganizationNameandID>> Get(OrganizationByID objOrganizationByID)
        {
            _log.Info("Get Organization method called in repository");
            try
            {
                int level = await GetLevelByRoleId(objOrganizationByID.Id, objOrganizationByID.RoleId);
                string strquery = string.Empty;
                List<OrganizationNameandID> objOrganizationNameandID = new List<OrganizationNameandID>();
                var parameter = new DynamicParameters();
                parameter.Add("@state", 'A');
                strquery = @"SELECT id, coalesce(name, '(' || org_id || ')') as name FROM master.organization where state=@state";
                switch (level)
                {
                    case 10:
                    case 20:
                        break;
                    case 30:
                    case 40:
                        parameter.Add("@id", objOrganizationByID.Id);
                        strquery = $"{strquery} and id=@id";
                        break;
                    default:
                        parameter.Add("@id", objOrganizationByID.Id);
                        strquery = $"{strquery} and id=@id";
                        break;
                        //return objOrganizationNameandID;
                }

                var data = await _dataAccess.QueryAsync<OrganizationNameandID>(strquery, parameter);
                if (data == null)
                {
                    return objOrganizationNameandID;
                }
                return data.ToList();
            }
            catch (Exception ex)
            {
                _log.Info("Get Organization method in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<int> GetLevelByRoleId(int orgId, int roleId)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@id", roleId);
            parameter.Add("@organization_id", orgId);
            var data = await _dataAccess.ExecuteScalarAsync
                             (@"select level from master.Role where id=@id and (organization_id=@organization_id or organization_id  is null)",
                            parameter);
            int level = data != null ? Convert.ToInt32(data) : 0;
            return level;
        }

        public async Task<int> IsOwnerRelationshipExist(int VehicleID)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vehicle_id", VehicleID);
                var query = @"Select id from master.orgrelationshipmapping where vehicle_id=@vehicle_id and end_date is null";
                return await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                _log.Info("IsOwnerRelationshipExist method in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
        }
        public async Task<int> IsOrganizationRelationshipExist(RelationshipMapping relationshipMapping)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@vehicle_id", relationshipMapping.VehicleId);
                parameter.Add("@created_org_id", relationshipMapping.CreatedOrgId);
                parameter.Add("@relationship_id", relationshipMapping.RelationshipId);
                var query = @"select id from master.orgrelationshipmapping where end_date is null and vehicle_id=@vehicle_id and created_org_id=@created_org_id and relationship_id=@relationship_id";
                return await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
            }
            catch (Exception ex)
            {
                _log.Info("IsOrganizationRelationshipExist method in repository failed :");
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<IEnumerable<Organization>> GetAllOrganizations(int OrganizationID)
        {
            _log.Info("GetAllOrganizations method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", OrganizationID);
                var query = @"Select distinct om.owner_org_id id,
                                coalesce(o.name, '(' || o.org_id || ')') as name
                                from master.organization o
                               left join master.orgrelationshipmapping om on om.target_org_id=o.id
                                where o.id=@id and o.state='A'";
                return await _dataAccess.QueryAsync<Organization>(query, parameter);
            }
            catch (Exception ex)
            {
                _log.Info("GetAllOrganizations method in repository failed :");// + Newtonsoft.Json.JsonConvert.SerializeObject(organizationId));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<Organization> GetOrganizationByOrgCode(string organizationCode)
        {
            try
            {

                var parameters = new DynamicParameters();
                parameters.Add("@org_id", organizationCode);
                var query = @"SELECT * FROM master.organization where org_id=@org_id";
                return await _dataAccess.QueryFirstOrDefaultAsync<Organization>(query, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
