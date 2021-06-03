using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;


namespace net.atos.daf.ct2.account.test
{
    [TestClass]
    public class AccountRepositoryTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        readonly IAccountRepository _repository;


        public AccountRepositoryTest()
        {

            _config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.Test.json")
            .Build();
            //Get connection string
            var connectionString = _config.GetConnectionString("DevAzure");
            _dataAccess = new PgSQLDataAccess(connectionString);
            _repository = new AccountRepository(_dataAccess);
        }

        [TestMethod]
        public void CreateBlob()
        {
            account.entity.AccountBlob accountBlob = new account.entity.AccountBlob();

            accountBlob.AccountId = 243;
            accountBlob.Id = 0;
            accountBlob.Type = ImageType.JPEG;
            string imageFilePath = @"C:\DAF\Repo\atos.daf.ct2.0\dotnet\corecomponents\test\net.atos.daf.ct2.account.test\mockdata\image\Profile1.JPG";
            byte[] imgdata = System.IO.File.ReadAllBytes(imageFilePath);
            accountBlob.Image = imgdata;
            var result = _repository.CreateBlob(accountBlob).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }
        [TestMethod]
        public void GetBlob()
        {
            account.entity.AccountBlob accountBlob = new account.entity.AccountBlob();
            accountBlob.Id = 7;
            accountBlob.Type = ImageType.JPEG;
            var result = _repository.GetBlob(accountBlob.Id).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }

        [TestMethod]
        public void UpdateBlob()
        {
            account.entity.AccountBlob accountBlob = new account.entity.AccountBlob();
            accountBlob.AccountId = 0;
            accountBlob.Id = 2;
            accountBlob.Type = ImageType.JPEG;
            string imageFilePath = @"C:\DAF\Repo\atos.daf.ct2.0\dotnet\corecomponents\test\net.atos.daf.ct2.account.test\mockdata\image\Profile3.JPG";
            byte[] imgdata = System.IO.File.ReadAllBytes(imageFilePath);
            accountBlob.Image = imgdata;
            var result = _repository.CreateBlob(accountBlob).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }

        // [TestMethod]
        // public void CreateAccount()
        // {
        //     Account entity = new Account();     
        //     DateTime dob = new DateTime(1978,07,11);
        //     entity.EmailId = "viranjay.singh112122@atos.net";
        //     entity.Salutation = "Mr";
        //     entity.FirstName = "Viranjay2";
        //     entity.LastName = "Singh2";
        //     entity.Dob = dob;
        //     entity.AccountType = AccountType.SystemAccount;
        //     entity.Organization_Id=1;
        //     entity.StartDate = DateTime.Now;            
        //     entity.EndDate = DateTime.Now.AddYears(2);
        //     entity.Active = true;
        //     var result = repository.Create(entity).Result;
        //     Assert.IsTrue(result != null && result.Id > 0);
        // }

        // [TestMethod]
        // public void UpdateAccount()
        // {
        //     Account entity = new Account();     

        //     DateTime dob = new DateTime(1978,07,11);
        //     entity.Id = 5;
        //     entity.EmailId = "viranjay.singh112@gmail.com";
        //     entity.Salutation = "Mr";
        //     entity.FirstName = "Viranjay";
        //     entity.LastName = "Singh";
        //     entity.Dob = dob;
        //     entity.AccountType = AccountType.SystemAccount;
        //     entity.Organization_Id=1;
        //     entity.StartDate = DateTime.Now;            
        //     entity.EndDate = DateTime.Now.AddYears(2);
        //     entity.Active = true;
        //     var result = repository.Update(entity).Result;
        //     Assert.IsTrue(result != null && result.Id > 0);
        // }

        // [TestMethod]
        // public void DeleteAccount()
        // {
        //     Account entity = new Account();     
        //     int accountid=1;
        //     int organizationid=1;             
        //     var result = repository.Delete(accountid,organizationid).Result;
        //     Assert.IsTrue(result != null);
        // }

        // [TestMethod]
        // public void GetAccountByAccountId()
        // {
        //     AccountFilter filter = new AccountFilter();     

        //     filter.Id=4;
        //     filter.AccountType = AccountType.None;            
        //     var result = repository.Get(filter).Result;
        //     Assert.IsTrue(result != null);
        // }
        // [TestMethod]
        // public void GetAccountByOrganization()
        // {
        //     AccountFilter filter = new AccountFilter();     

        //     filter.Id=0;
        //     filter.OrganizationId =1;
        //     filter.AccountType = AccountType.None;            
        //     var result = repository.Get(filter).Result;
        //     Assert.IsTrue(result != null );
        // }
        // [TestMethod]
        // public void GetAccountByType()
        // {
        //     AccountFilter filter = new AccountFilter();     

        //     filter.Id=0;
        //     filter.OrganizationId = 0;
        //     filter.AccountType = AccountType.SystemAccount;            
        //     var result = repository.Get(filter).Result;
        //     Assert.IsTrue(result != null );
        // }

        // [TestMethod]
        // public void GetAccountByAccountAndOrganization()
        // {
        //     AccountFilter filter = new AccountFilter();     

        //     filter.Id=2;
        //     filter.OrganizationId=1;
        //     filter.AccountType = AccountType.None;            
        //     var result = repository.Get(filter).Result;
        //     Assert.IsTrue(result != null);
        // }
    }
}
