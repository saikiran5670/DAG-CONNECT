using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace net.atos.daf.ct2.group.test
{
    [TestClass]
    public class GroupManagerTest
    {
        // private readonly IDataAccess _dataAccess;
        // private readonly IConfiguration _config;
        // readonly IGroupRepository _groupRepository;        
        // private readonly IGroupManager _groupManager;
        // private readonly IAuditTraillib _auditlog;
        //  private readonly IAuditLogRepository _auditLogRepository;
        // public GroupManagerTest()
        // {
        //      _config = new ConfigurationBuilder()
        //      .AddJsonFile("appsettings.Test.json")
        //     .Build();
        //     //Get connection string
        //     var connectionString = _config.GetConnectionString("DevAzure");            
        //     _dataAccess = new PgSQLDataAccess(connectionString);
        //      _auditLogRepository=new AuditLogRepository(_dataAccess);
        //     _auditlog= new AuditTraillib(_auditLogRepository);
        //     _groupRepository = new GroupRepository(_dataAccess);
        //     _groupManager = new GroupManager(_groupRepository,_auditlog);

        // }
        // [TestMethod]
        // public void CreateGroup_Manager()
        // {
        //      Group group = new Group();     
        //     group.ObjectType = ObjectType.VehicleGroup;
        //     group.GroupType = GroupType.Group;
        //     group.Argument = "Truck";
        //     group.FunctionEnum = FunctionEnum.None;
        //     group.OrganizationId = 1;
        //     group.RefId = null;
        //     group.Name = "AccountGroup_Manager_UTC";
        //     group.Description = "AccountGroup Manager UTC";
        //     var result = _groupManager.Create(group).Result;
        //     Assert.IsTrue(result != null && result.Id > 0);
        // }

        // [TestMethod]
        // public void GetVehicleGroupByOrganization()
        // {
        //     Group group = new Group();
        //     // filter by organization
        //     GroupFilter filter = new GroupFilter();
        //     filter.OrganizationId = 1;
        //     filter.FunctionEnum = FunctionEnum.None;
        //     filter.ObjectType = ObjectType.None;
        //     filter.GroupType = GroupType.None;
        //     var result = _groupManager.Get(filter).Result;
        //     Assert.IsTrue(result != null);
        // }
    }
}
