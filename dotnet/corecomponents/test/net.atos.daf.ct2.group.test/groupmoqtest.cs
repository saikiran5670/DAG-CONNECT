using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.group.test
{
    [TestClass]
    public class groupmoqtest
    {
         Mock<IGroupRepository> _iGroupRepository;
         Mock<IAuditTraillib> _auditlog;
        GroupManager _groupManager;
        public groupmoqtest()
        {
            _iGroupRepository = new Mock<IGroupRepository>();
            _auditlog = new Mock<IAuditTraillib>();
            _groupManager = new GroupManager(_iGroupRepository.Object, _auditlog.Object);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Create")]
        [TestMethod]
        public async Task CreateTest()
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
            Group actual =new Group();
            _iGroupRepository.Setup(s=>s.Create(It.IsAny<Group>())).ReturnsAsync(actual);
            var result = await _groupManager.Create(group);
            Assert.AreEqual(result, actual);
        } 
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Update")]
        [TestMethod]
        public async Task UpdateTest()
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
            Group actual =new Group();
            _iGroupRepository.Setup(s=>s.Update(It.IsAny<Group>())).ReturnsAsync(actual);
            var result = await _groupManager.Update(group);
            Assert.AreEqual(result, actual);
        } 
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Delete")]
        [TestMethod]
        public async Task DeleteTest()
        {            
          long groupid = 25326334745555;
          

            //Group actual =new Group();
            _iGroupRepository.Setup(s=>s.Delete(It.IsAny<long>(), It.IsAny<ObjectType>())).ReturnsAsync(true);
            var result = await _groupManager.Delete(groupid, ObjectType.None);
            Assert.AreEqual(result, true);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get")]
        [TestMethod]
        public async Task GetTest()
        {            
            GroupFilter groupFilter = new GroupFilter();
           // group.ObjectType = ObjectType.VehicleGroup;
            //group.GroupType = GroupType.Group;
            //group.Argument = "Truck";
           // group.FunctionEnum = FunctionEnum.None;
           // group.OrganizationId = 1;
           // group.RefId = null;
           // group.Name = "Account Group12";
           // group.Description = "Account Group Test12 ";
            var actual =new List<Group>();
            _iGroupRepository.Setup(s=>s.Get(It.IsAny<GroupFilter>())).ReturnsAsync(actual);
            var result = await _groupManager.Get(groupFilter);
            Assert.AreEqual(result, actual);
        } 
        [TestCategory("Unit-Test-Case")]
        [Description("Test for UpdateRef")]
        [TestMethod]
        public async Task UpdateRefTest()
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
           // Group actual =new Group();
            _iGroupRepository.Setup(s=>s.UpdateRef(It.IsAny<Group>())).ReturnsAsync(true);
            var result = await _groupManager.UpdateRef(group);
            Assert.AreEqual(result, true);
        } 
             [TestCategory("Unit-Test-Case")]
        [Description("Test for GetRef")]
        [TestMethod]
        public async Task GetRefRefTest()
        {            
            //Group group = new Group();
            int groupid = 34;
            List<GroupRef> actual =new List<GroupRef>();
            _iGroupRepository.Setup(s=>s.GetRef(It.IsAny<int>())).ReturnsAsync(actual);
            var result = await _groupManager.GetRef(groupid);
            Assert.AreEqual(result, actual);
        } 
        [TestCategory("Unit-Test-Case")]
        [Description("Test for RemoveRefByRefId")]
        [TestMethod]
        public async Task RemoveRefByRefIdTest()
        {         
            int refId = 45;   
            //Group group = new Group();
            //group.ObjectType = ObjectType.VehicleGroup;
           // group.GroupType = GroupType.Group;
            //group.Argument = "Truck";
            //group.FunctionEnum = FunctionEnum.None;
           // group.OrganizationId = 1;
           // group.RefId = null;
           // group.Name = "Account Group12";
           // group.Description = "Account Group Test12 ";
           //// Group actual =new Group();
            _iGroupRepository.Setup(s=>s.RemoveRefByRefId(It.IsAny<int>())).ReturnsAsync(true);
            var result = await _groupManager.RemoveRefByRefId(refId);
            Assert.AreEqual(result, true);
        } 

        [TestCategory("Unit-Test-Case")]
        [Description("Test for AddRefToGroups")]
        [TestMethod]
        public async Task AddRefToGroupsTest()
        {         
          List<GroupRef> groupRef = new List<GroupRef>();
            //Group group = new Group();
            //group.ObjectType = ObjectType.VehicleGroup;
           // group.GroupType = GroupType.Group;
            //group.Argument = "Truck";
            //group.FunctionEnum = FunctionEnum.None;
           // group.OrganizationId = 1;
           // group.RefId = null;
           // group.Name = "Account Group12";
           // group.Description = "Account Group Test12 ";
           //// Group actual =new Group();
            _iGroupRepository.Setup(s=>s.AddRefToGroups(It.IsAny<List<GroupRef>>())).ReturnsAsync(true);
            var result = await _groupManager.AddRefToGroups(groupRef);
            Assert.AreEqual(result, true);
        } 
    }
}
