using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.organization.entity;
using net.atos.daf.ct2.organization;
using AccountPreferenceComponent = net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using System.Collections.Generic;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.vehicle;
namespace net.atos.daf.ct2.organizationservice.Services
{
    public class OrganizationManagementService : OrganizationService.OrganizationServiceBase
    {
      //  private readonly ILogger<GreeterService> _logger;
        private readonly ILogger _logger;        
        private readonly IAuditLogRepository _IAuditLogRepository;       
        private readonly IAuditTraillib _AuditTrail;      
        private readonly IOrganizationManager organizationtmanager;
        private readonly IPreferenceManager preferencemanager;
         private readonly IVehicleManager vehicleManager;

        
        public OrganizationManagementService(ILogger<OrganizationManagementService> logger, IAuditTraillib AuditTrail, IOrganizationManager _organizationmanager,IPreferenceManager _preferencemanager,IVehicleManager _vehicleManager)
        {
            _logger = logger;
            _AuditTrail = AuditTrail;
            organizationtmanager = _organizationmanager;
            preferencemanager=_preferencemanager;
            vehicleManager=_vehicleManager;
        }
        public override Task<OrganizationResponse> Create(OrganizationCreateRequest request, ServerCallContext context)
        {
            try
            {   
                Organization organization = new Organization();
                organization.OrganizationId = request.OrganizationId;
                organization.Type = request.Type;
                organization.Name = request.Name;
                organization.AddressType = request.AddressType;
                organization.AddressStreet = request.AddressStreet;                
                organization.AddressStreetNumber = request.AddressStreetNumber;
                organization.PostalCode = request.PostalCode;
                organization.City = request.City;
                organization.CountryCode = request.CountryCode;
                organization.ReferencedDate = request.ReferencedDate;
                organization.OptOutStatus = request.OptOutStatus;
                organization.OptOutStatusChangedDate = request.OptOutStatusChangedDate;
                organization.IsActive = request.IsActive;
                
                organization = organizationtmanager.Create(organization).Result;

                 AccountPreference accountPreference=new AccountPreference();
                 accountPreference.Ref_Id=organization.Id;
                 accountPreference.PreferenceType=GetPreferenceType(request.PreferenceType);                 
                 accountPreference.Language_Id= request.LanguageId;
                 accountPreference.Timezone_Id= request.TimezoneId;
                 accountPreference.Currency_Type=GetCurrencyType(request.CurrencyType);
                 accountPreference.Unit_Type=GetUnitType(request.UnitType);
                 accountPreference.VehicleDisplay_Type=GetVehicleDisplayType(request.VehicleDisplayType);
                 accountPreference.DateFormat_Type=GetDateFormatDisplayType(request.DateFormatType);
                 accountPreference.DriverId= string.Empty;
                 accountPreference.Is_Active= request.IsActive;

                 accountPreference = preferencemanager.Create(accountPreference).Result;
                
                return Task.FromResult(new OrganizationResponse
                {
                    Message = "Organization Created " + organization.Id
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new OrganizationResponse
                {
                    Message = "Exception " + ex.Message
                });
            }
        }    
         public override Task<OrganizationResponse> Update(OrganizationUpdateRequest request, ServerCallContext context)
        {
            try
            {
                Organization organization = new Organization();
                organization.Id = request.Id;
                organization.OrganizationId = request.OrganizationId;
                organization.Type = request.Type;
                organization.Name = request.Name;
                organization.AddressType = request.AddressType;
                organization.AddressStreet = request.AddressStreet;                
                organization.AddressStreetNumber = request.AddressStreetNumber;
                organization.PostalCode = request.PostalCode;
                organization.City = request.City;
                organization.CountryCode = request.CountryCode;
                organization.ReferencedDate = request.ReferencedDate;
                organization.OptOutStatus = request.OptOutStatus;
                organization.OptOutStatusChangedDate = request.OptOutStatusChangedDate;
                organization.IsActive = request.IsActive;
                
                organization = organizationtmanager.Update(organization).Result;

                 AccountPreference accountPreference=new AccountPreference();
                 accountPreference.Ref_Id=organization.Id;
                 accountPreference.PreferenceType=GetPreferenceType(request.PreferenceType);                 
                 accountPreference.Language_Id= request.LanguageId;
                 accountPreference.Timezone_Id= request.TimezoneId;
                 accountPreference.Currency_Type=GetCurrencyType(request.CurrencyType);
                 accountPreference.Unit_Type=GetUnitType(request.UnitType);
                 accountPreference.VehicleDisplay_Type=GetVehicleDisplayType(request.VehicleDisplayType);
                 accountPreference.DateFormat_Type=GetDateFormatDisplayType(request.DateFormatType);
                 accountPreference.DriverId= string.Empty;
                 accountPreference.Is_Active= request.IsActive;
                 
                 accountPreference = preferencemanager.Update(accountPreference).Result;

                return Task.FromResult(new OrganizationResponse
                {
                    Message = "Organization Updated " + organization.OrganizationId
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new OrganizationResponse
                {
                    Message = "Exception " + ex.Message
                });
            }
        }
         public override Task<OrganizationResponse> Delete(OrganizationDeleteRequest request, ServerCallContext context)
        {
            try
            {                                 
                bool isOrgDeleted=organizationtmanager.Delete(request.Id).Result; 
                bool isPreferenceDeleted=preferencemanager.Delete(request.Id).Result;
                if(isOrgDeleted && isPreferenceDeleted) 
                {
                return Task.FromResult(new OrganizationResponse
                {
                    Message = "Organization Deleted "
                });
                }
                else{
                    return Task.FromResult(new OrganizationResponse
                {
                    Message = "Organization Not Deleted " 
                });
                }
            }
            catch (Exception ex)
            {
               return Task.FromResult(new OrganizationResponse
                {
                    Message = "Exception " + ex.Message
                });
            }            
        }

        public override Task<GetOrganizationResponce> Get(GetOrganizationRequest request, ServerCallContext context)
        {            
           Organization  organization = organizationtmanager.Get(request.Id).Result;         
           return Task.FromResult(new GetOrganizationResponce
                {                 
                    Id=organization.Id,
                    OrganizationId=organization.OrganizationId,
                    Type = organization.Type,
                    Name = organization.Name,
                    AddressType = organization.AddressType,
                    AddressStreet = organization.AddressStreet,       
                    AddressStreetNumber = organization.AddressStreetNumber,
                    PostalCode = organization.PostalCode,
                    City = organization.City,
                    CountryCode = organization.CountryCode,
                    ReferencedDate = organization.ReferencedDate,
                    OptOutStatus = organization.OptOutStatus,
                    OptOutStatusChangedDate = organization.OptOutStatusChangedDate,
                    IsActive = organization.IsActive
                });                           
        }
         private AccountPreferenceComponent.CurrencyType GetCurrencyType(int value)
        {
            AccountPreferenceComponent.CurrencyType currencyType;
            switch(value)
            {
                case 0:
                currencyType = AccountPreferenceComponent.CurrencyType.None;
                break;
                case 1:
                currencyType = AccountPreferenceComponent.CurrencyType.Euro;
                break;
                case 2:
                currencyType = AccountPreferenceComponent.CurrencyType.USDollar;
                break;
                default:
                currencyType = AccountPreferenceComponent.CurrencyType.PondSterlingr;
                break;
            }
            return currencyType;
        }

       private AccountPreferenceComponent.UnitType GetUnitType(int value)
        {
            AccountPreferenceComponent.UnitType unitType;
            switch(value)
            {
                case 0:
                unitType = AccountPreferenceComponent.UnitType.None;
                break;
                case 1:
                unitType = AccountPreferenceComponent.UnitType.Metric;
                break;
                case 2:
                unitType = AccountPreferenceComponent.UnitType.Imperial;
                break;
                default:
                unitType = AccountPreferenceComponent.UnitType.US_Imperial;
                break;
            }
            return unitType;
        }

        private AccountPreferenceComponent.VehicleDisplayType GetVehicleDisplayType(int value)
        {
            AccountPreferenceComponent.VehicleDisplayType vehicleDisplayType;
            switch(value)
            {
                case 0:
                vehicleDisplayType = AccountPreferenceComponent.VehicleDisplayType.None;
                break;
                case 1:
                vehicleDisplayType = AccountPreferenceComponent.VehicleDisplayType.Registration_Number;
                break;
                case 2:
                vehicleDisplayType = AccountPreferenceComponent.VehicleDisplayType.Name;
                break;
                default:
                vehicleDisplayType = AccountPreferenceComponent.VehicleDisplayType.VIN;
                break;
            }
            return vehicleDisplayType;
        }
        private AccountPreferenceComponent.DateFormatDisplayType GetDateFormatDisplayType(int value)
        {
            AccountPreferenceComponent.DateFormatDisplayType dateFormatDisplayType;
            switch(value)
            {
                case 0:
                dateFormatDisplayType = AccountPreferenceComponent.DateFormatDisplayType.None;
                break;
                case 1:
                dateFormatDisplayType = AccountPreferenceComponent.DateFormatDisplayType.Day_Month_Year;
                break;
                case 2:
                dateFormatDisplayType = AccountPreferenceComponent.DateFormatDisplayType.Month_Day_Year;
                break;
                default:
                dateFormatDisplayType = AccountPreferenceComponent.DateFormatDisplayType.Year_Month_Day;
                break;
            }
            return dateFormatDisplayType;
        }

         private AccountPreferenceComponent.PreferenceType GetPreferenceType(int value)
         {
            AccountPreferenceComponent.PreferenceType preferenceType;
            switch(value)
            {
                case 0:
                preferenceType = AccountPreferenceComponent.PreferenceType.None;
                break;
                case 1:
                preferenceType = AccountPreferenceComponent.PreferenceType.Account;
                break;
                default:
                preferenceType = AccountPreferenceComponent.PreferenceType.Organization;
                break;             
            }
            return preferenceType;
        }
    }
}
