using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.accountpreference;
using net.atos.daf.ct2.data;

namespace atos.net.daf.ct2.accountpreference.test
{
    [TestClass]
    public class AccountPreference_Test
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        readonly IAccountPreferenceRepository _repository;
        // private readonly IAuditLog _auditlog;
        public AccountPreference_Test()
        {
            _config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.Test.json")
            .Build();
            //Get connection string
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _repository = new AccountPreferenceRepository(_dataAccess);
        }

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
        [TestMethod]
        public void CreateAccountPreference()
        {
            AccountPreference entity = new AccountPreference();
            entity.Id = 0;
            entity.RefId = 2;
            entity.LanguageId = 1;
            entity.TimezoneId = 1;
            entity.CurrencyId = 1;
            entity.UnitId = 1;
            entity.VehicleDisplayId = 1;
            entity.DateFormatTypeId = 1;
            entity.TimeFormatId = 1;
            entity.LandingPageDisplayId = 1;
            entity.PreferenceType = PreferenceType.Account;
            entity.IconId = 0;
            entity.IconByte = "iVBORw0KGgoAAAANSUhEUgAAAOEAAADhCAMAAAAJbSJIAAAANlBMVEV3xGP///9rwFP6/fpwwlrd8Nl0w1/Y7dTx+e9ywlyRzoKTz4WJy3nV69H0+vNhvEfs9+rS68zjIg6FAAAA+0lEQVR4nO3cO07DABBAQeM4hgDhc//L0jpBYCEZnCdm6i32dVvtMAAAAAAAAAAAAAAAAABfmDe2d8+1+eluY+PeSVfmx40DDwr/mkKFCvenUOE/LLy5m2Z4Pt5/73yx/8t5bXzau+iTacXrYVl4HNfm9+75ufGyMFiwRmGfwj6FfQr7FPYp7FPYp7BPYZ/CPoV9CvsU9insU9insE9hn8I+hX0K+xT2KexT2KewT2Gfwj6FfQr7FPYp7FPYp7BPYZ/CPoV9CvsU9insU9insE9hn8I+hX0K+xT2Keyb3h4W3k977/MLTks39zYfAAAAAAAAAAAAAAAAKj4AsBsT01q1nkgAAAAASUVORK5CYII=";
            var result = _repository.Create(entity).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }
        [TestMethod]
        public void UpdateAccountPreference()
        {
            AccountPreference entity = new AccountPreference();
            entity.Id = 0;//put created id 
            entity.RefId = 2;
            entity.LanguageId = 1;
            entity.TimezoneId = 1;
            entity.CurrencyId = 1;
            entity.UnitId = 1;
            entity.VehicleDisplayId = 1;
            entity.DateFormatTypeId = 1;
            entity.TimeFormatId = 1;
            entity.LandingPageDisplayId = 1;
            entity.PreferenceType = PreferenceType.Account;
            entity.IconId = 0;
            entity.IconByte = "iVBORw0KGgoAAAANSUhEUgAAAOEAAADhCAMAAAAJbSJIAAAANlBMVEV3xGP///9rwFP6/fpwwlrd8Nl0w1/Y7dTx+e9ywlyRzoKTz4WJy3nV69H0+vNhvEfs9+rS68zjIg6FAAAA+0lEQVR4nO3cO07DABBAQeM4hgDhc//L0jpBYCEZnCdm6i32dVvtMAAAAAAAAAAAAAAAAABfmDe2d8+1+eluY+PeSVfmx40DDwr/mkKFCvenUOE/LLy5m2Z4Pt5/73yx/8t5bXzau+iTacXrYVl4HNfm9+75ufGyMFiwRmGfwj6FfQr7FPYp7FPYp7BPYZ/CPoV9CvsU9insU9insE9hn8I+hX0K+xT2KexT2KewT2Gfwj6FfQr7FPYp7FPYp7BPYZ/CPoV9CvsU9insU9insE9hn8I+hX0K+xT2Keyb3h4W3k977/MLTks39zYfAAAAAAAAAAAAAAAAKj4AsBsT01q1nkgAAAAASUVORK5CYII=";
            var result = _repository.Update(entity).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }

        [TestMethod]
        public void DeleteAccountPreference()
        {
            AccountPreference entity = new AccountPreference();
            entity.PreferenceType = PreferenceType.Account;
            var preferenceId = 2;
            var result = _repository.Delete(preferenceId, entity.PreferenceType).Result;
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetAccountPreference()
        {
            //var preferenceId = 2;
            //var result = _repository.Get(preferenceId).Result;
            //Assert.IsTrue(result != null && result.Id > 0);
        }

        // [TestMethod]
        // public void DeleteOrganizationPreference()
        // {
        //     var preferenceId  = 2;            
        //     var result = _repository.Delete(preferenceId).Result;
        //     Assert.IsTrue(result);
        // }
    }
}
