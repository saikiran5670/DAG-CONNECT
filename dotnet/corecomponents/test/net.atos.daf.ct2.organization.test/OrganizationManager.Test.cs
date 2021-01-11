using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using Microsoft.Extensions.Configuration; 
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.organization.repository;
using System.Collections.Generic;

namespace net.atos.daf.ct2.organization.test
{
    // [TestClass]
    // public class OrganizationManagerTest
    // {  private readonly IDataAccess _dataAccess;
    //     private readonly IConfiguration _config;
    //     readonly IOrganizationRepository _organizationRepository;        
    //     private readonly IOrganizationManager _organizationManager;
    //     private readonly IAuditTraillib _auditlog;
    //      private readonly IAuditLogRepository _auditLogRepository;
    //     public OrganizationManagerTest()
    //     {
    //          _config = new ConfigurationBuilder()
    //          .AddJsonFile("appsettings.Test.json")
    //         .Build();
    //         //Get connection string
    //         var connectionString = _config.GetConnectionString("Dev");
    //         //string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAFCT; User Id = postgres; Password = Admin@1978; CommandTimeout = 90; ";
    //         _dataAccess = new PgSQLDataAccess(connectionString);
    //         _auditLogRepository=new AuditLogRepository(_dataAccess);
    //         _auditlog= new AuditTraillib(_auditLogRepository);
    //         _organizationRepository = new OrganizationRepository(_dataAccess);
    //         _organizationManager = new OrganizationManager(_organizationRepository,_auditlog);
    //     }
    // }
}
