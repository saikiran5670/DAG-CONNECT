using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace atos.net.daf.ct2.accountpreference.test
{
    [TestClass]
    public class AccountPreference_Test
    {
        // private readonly IDataAccess _dataAccess;
        // private readonly IConfiguration _config;
        // readonly IAccountPreferenceRepository _repository;        
        // // private readonly IAuditLog _auditlog;
        // public AccountPreference_Test()
        // {
        //     _config = new ConfigurationBuilder()
        //      .AddJsonFile("appsettings.Test.json")
        //     .Build();
        //     //Get connection string
        //     var connectionString = _config.GetConnectionString("DevAzure");            
        //     _dataAccess = new PgSQLDataAccess(connectionString);
        //     _repository = new AccountPreferenceRepository(_dataAccess);
        // }

        // [TestMethod]
        // public void CreateOrganizationPreference()
        // {
        //     AccountPreference entity = new AccountPreference();     
        //     //entity.Id = 0;
        //     entity.Ref_Id  = 1;
        //     entity.PreferenceType = PreferenceType.Organization;
        //     entity.Language_Id = 1;
        //     entity.Timezone_Id = 1;
        //     entity.Currency_Type = CurrencyType.Euro;
        //     entity.Unit_Type = UnitType.Imperial;
        //     entity.VehicleDisplay_Type = VehicleDisplayType.Name;
        //     entity.DateFormat_Type = DateFormatDisplayType..Day_Month_Year;
        //     entity.DriverId = string.Empty;
        //     var result = _repository.Create(entity).Result;
        //     Assert.IsTrue(result != null && result.Id > 0);
        // }
        //  [TestMethod]
        // public void UpdateOrganizationAccountPreference()
        // {
        //     AccountPreference entity = new AccountPreference();     
        //     entity.Id = 2;
        //     entity.Ref_Id  = 1;
        //     entity.PreferenceType = PreferenceType.Organization;
        //     entity.Language_Id = 1;
        //     entity.Timezone_Id = 1;
        //     entity.Currency_Type = CurrencyType.Euro;
        //     entity.Unit_Type = UnitType.Imperial;
        //     entity.VehicleDisplay_Type = VehicleDisplayType.Name;
        //     entity.DateFormat_Type = DateFormatDisplayType.Day_Month_Year;
        //     entity.DriverId = string.Empty;
        //     var result = _repository.Update(entity).Result;
        //     Assert.IsTrue(result != null && result.Id > 0);
        // }
        // [TestMethod]
        // public void CreateAccountPreference()
        // {
        //     AccountPreference entity = new AccountPreference();     
        //     //entity.Id = 0;
        //     entity.Ref_Id  = 2;
        //     entity.PreferenceType = PreferenceType.Account;
        //     entity.Language_Id = 1;
        //     entity.Timezone_Id = 1;
        //     entity.Currency_Type = CurrencyType.Euro;
        //     entity.Unit_Type = UnitType.Imperial;
        //     entity.VehicleDisplay_Type = VehicleDisplayType.Name;
        //     entity.DateFormat_Type = DateFormatDisplayType.Day_Month_Year;
        //     entity.DriverId = string.Empty;
        //     var result = _repository.Create(entity).Result;
        //     Assert.IsTrue(result != null && result.Id > 0);
        // }
        // [TestMethod]
        // public void UpdateAccountPreference()
        // {
        //     AccountPreference entity = new AccountPreference();     
        //     //entity.Id = 0;
        //     entity.Ref_Id  = 2;
        //     entity.PreferenceType = PreferenceType.Account;
        //     entity.Language_Id = 1;
        //     entity.Timezone_Id = 1;
        //     entity.Currency_Type = CurrencyType.Euro;
        //     entity.Unit_Type = UnitType.Imperial;
        //     entity.VehicleDisplay_Type = VehicleDisplayType.Name;
        //     entity.DateFormat_Type = DateFormatDisplayType.Day_Month_Year;
        //     entity.DriverId = string.Empty;
        //     var result = _repository.Update(entity).Result;
        //     Assert.IsTrue(result != null && result.Id > 0);
        // }

        // [TestMethod]
        // public void DeleteOrganizationPreference()
        // {
        //     var preferenceId  = 2;            
        //     var result = _repository.Delete(preferenceId).Result;
        //     Assert.IsTrue(result);
        // }
    }
}
