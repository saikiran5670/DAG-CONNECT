using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.data;

namespace net.atos.daf.ct2.group.test
{
    [TestClass]
    public class GroupRepositoryTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        readonly IGroupRepository _groupRepository;
        private readonly IAuditTraillib _auditlog;
        public GroupRepositoryTest()
        {
            _config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.Test.json")
            .Build();
            //Get connection string
            var connectionString = _config.GetConnectionString("DevAzure");
            //string connectionString = "Server = 127.0.0.1; Port = 5432; Database = DAFCT; User Id = postgres; Password = Admin@1978; CommandTimeout = 90; ";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _groupRepository = new GroupRepository(_dataAccess);
        }

        [TestMethod]
        public void CreateGroup()
        {
            Group group = new Group();
            group.ObjectType = ObjectType.VehicleGroup;
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
        [TestMethod]
        public void CreateVehicleGroupWithVehicle()
        {
            Group group = new Group();
            group.ObjectType = ObjectType.VehicleGroup;
            group.GroupType = GroupType.Group;
            group.Argument = "Truck";
            group.FunctionEnum = FunctionEnum.None;
            group.OrganizationId = 1;
            group.RefId = null;
            group.Name = "V1";
            group.Description = "Vehicle Group";
            var groupResult = _groupRepository.Create(group).Result;
            if (groupResult.Id > 0)
            {
                // Add vehicles in it
                groupResult.GroupRef = new List<GroupRef>();
                groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 101 });
                groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 102 });
                groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 103 });
                groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 104 });
                groupResult.GroupRef.Add(new GroupRef() { Ref_Id = 105 });
            }
            var result = _groupRepository.AddRefToGroups(groupResult.GroupRef).Result;
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateGroup()
        {
            Group group = new Group();
            group.Id = 2;
            group.ObjectType = ObjectType.AccountGroup;
            group.GroupType = GroupType.Single;
            group.Argument = "Truck";
            group.FunctionEnum = FunctionEnum.All;
            group.OrganizationId = 1;
            group.RefId = null;
            group.Name = "Account Group2";
            group.Description = "Account Group Test2 ";
            var result = _groupRepository.Update(group).Result;
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void DeleteGroup()
        {
            var result = _groupRepository.Delete(3, ObjectType.AccountGroup).Result;
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetVehicleGroupByOrganization()
        {
            Group group = new Group();
            // filter by organization
            GroupFilter filter = new GroupFilter();
            filter.OrganizationId = 1;
            filter.FunctionEnum = FunctionEnum.None;
            filter.ObjectType = ObjectType.None;
            filter.GroupType = GroupType.None;
            var result = _groupRepository.Get(filter).Result;
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void GetVehicleGroupByOrganizationWithVehicleCount()
        {
            Group group = new Group();
            // filter by organization
            GroupFilter filter = new GroupFilter();
            filter.OrganizationId = 1;
            filter.GroupRefCount = true;
            filter.FunctionEnum = FunctionEnum.None;
            filter.ObjectType = ObjectType.None;
            filter.GroupType = GroupType.None;

            var result = _groupRepository.Get(filter).Result;
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void Get()
        {
            Group group = new Group();

            // filter by organization
            GroupFilter filter = new GroupFilter();
            filter.OrganizationId = 1;
            filter.FunctionEnum = FunctionEnum.All;
            filter.ObjectType = ObjectType.AccountGroup;
            filter.GroupType = GroupType.Single;
            //filter.GroupType = GroupType.None;
            var result = _groupRepository.Get(filter).Result;

            //filter by id
            filter = new GroupFilter();
            filter.Id = 2;
            filter.FunctionEnum = FunctionEnum.None;
            filter.ObjectType = ObjectType.None;
            filter.GroupType = GroupType.None;
            result = _groupRepository.Get(filter).Result;
            // filter by organization id and id
            filter = new GroupFilter();
            filter.FunctionEnum = FunctionEnum.None;
            filter.ObjectType = ObjectType.None;
            filter.GroupType = GroupType.None;
            filter.Id = 2;
            filter.OrganizationId = 1;
            result = _groupRepository.Get(filter).Result;

            // filter by functional enum
            filter = new GroupFilter();
            filter.FunctionEnum = FunctionEnum.All;
            filter.ObjectType = ObjectType.None;
            filter.GroupType = GroupType.None;
            result = _groupRepository.Get(filter).Result;

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void AddGroupRef()
        {
            Group group = new Group();

            group.Id = 2;
            // Add vehicles in it
            group.GroupRef = new List<GroupRef>();
            group.GroupRef.Add(new GroupRef() { Ref_Id = 100 });
            group.GroupRef.Add(new GroupRef() { Ref_Id = 200 });
            group.GroupRef.Add(new GroupRef() { Ref_Id = 300 });
            group.GroupRef.Add(new GroupRef() { Ref_Id = 400 });
            group.GroupRef.Add(new GroupRef() { Ref_Id = 500 });

            var result = _groupRepository.AddRefToGroups(group.GroupRef).Result;
            Assert.IsTrue(result == true);
        }
    }
}