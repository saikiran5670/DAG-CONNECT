using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using Microsoft.Extensions.Configuration; 
using net.atos.daf.ct2.audit;
namespace net.atos.daf.ct2.group.test
{
    [TestClass]
    public class GroupRepositoryTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        readonly IGroupRepository _groupRepository;        
        private readonly IAuditLog _auditlog;
        public GroupRepositoryTest()
        {
             string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAFCT; User Id = postgres; Password = Admin@1978; CommandTimeout = 90; ";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _groupRepository = new GroupRepository(_dataAccess);
        }

       [TestMethod]
        public void CreateGroup()
        {
            Group group = new Group();        
            
            group.ObjType = ObjectType.VehicleGroup;
            group.GroupType = GroupType.Group;
            group.Argument = "Truck";
            group.FunctionEnum = FunctionEnum.None;
            group.OrganizationId = 1;
            group.RefId = null;
            group.Name = "Account Group12";
            group.Description = "Account Group Test12 ";
            var result = _groupRepository.Create(group).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }
        
    }
}
