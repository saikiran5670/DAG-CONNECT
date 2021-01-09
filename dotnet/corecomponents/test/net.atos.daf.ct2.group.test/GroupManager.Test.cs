using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using Microsoft.Extensions.Configuration; 
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using System.Collections.Generic;


namespace net.atos.daf.ct2.group.test
{
     [TestClass]
    public class GroupManagerTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        readonly IGroupRepository _groupRepository;        
        private readonly IGroupManager _groupManager;
        private readonly IAuditLog _auditlog;
         private readonly IAuditLogRepository _auditLogRepository;
        public GroupManagerTest()
        {
             _config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.Test.json")
            .Build();
            //Get connection string
            var connectionString = _config.GetConnectionString("Dev");
            //string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAFCT; User Id = postgres; Password = Admin@1978; CommandTimeout = 90; ";
            _dataAccess = new PgSQLDataAccess(connectionString);
             _auditLogRepository=new AuditLogRepository(_dataAccess);
            _auditlog= new AuditLog(_auditLogRepository);
            _groupRepository = new GroupRepository(_dataAccess);
            _groupManager = new GroupManager(_groupRepository,_auditlog);

        }
        [TestMethod]
        public void CreateGroup_Manager()
        {
             Group group = new Group();     
            group.ObjType = ObjectType.VehicleGroup;
            group.GroupType = GroupType.Group;
            group.Argument = "Truck";
            group.FunctionEnum = FunctionEnum.None;
            group.OrganizationId = 1;
            group.RefId = null;
            group.Name = "AccountGroup_Manager_UTC";
            group.Description = "AccountGroup Manager UTC";
            var result = _groupManager.Create(group).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }
    }
}
