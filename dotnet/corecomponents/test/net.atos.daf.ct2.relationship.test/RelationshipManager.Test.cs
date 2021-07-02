using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.repository;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.relationship.entity;
using net.atos.daf.ct2.relationship.repository;

namespace net.atos.daf.ct2.relationship.test
{
    [TestClass]
    public class RelationshipManagerTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly IRelationshipRepository _relationshipRepository;
        private readonly IRelationshipManager _relationshipManager;
        private readonly IAuditTraillib _auditlog;
        private readonly IAuditLogRepository _auditLogRepository;
        public RelationshipManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json").Build();
            string connectionString = "Server=dafct-lan1-d-euwe-cdp-pgsql-master.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432; User Id=pgdbadmin@dafct-lan1-d-euwe-cdp-pgsql-master;Password=9RQkJM2hwfe!;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _auditLogRepository = new AuditLogRepository(_dataAccess);
            _auditlog = new AuditTraillib(_auditLogRepository);
            _relationshipRepository = new RelationshipRepository(_dataAccess);
            _relationshipManager = new RelationshipManager(_relationshipRepository);
        }

        [TestMethod]
        public void CreateRelationship_Manager()
        {
            var relationship = new Relationship();
            relationship.OrganizationId = 1;
            relationship.Code = "C1";
            relationship.Level = 1;
            relationship.Name = "Test Data";
            relationship.Description = "Unit testing";
            relationship.FeaturesetId = 1;
            relationship.State = "A";
            var result = _relationshipManager.CreateRelationship(relationship).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }



        [TestMethod]
        public void UpdateRelationship_Manager()
        {
            var relationship = new Relationship();
            relationship.OrganizationId = 8;
            relationship.Code = "C1";
            relationship.Level = 1;
            relationship.Name = "Test Data";
            relationship.Description = "Unit testing";
            relationship.FeaturesetId = 1;
            relationship.State = "A";
            var result = _relationshipManager.UpdateRelationship(relationship).Result;
            Assert.IsTrue(result != null && result.Id > 0);
        }
        [TestMethod]
        public void DeleteRelationship_Manager()
        {
            var result = _relationshipManager.DeleteRelationship(1).Result;
            Assert.IsTrue(result == true);
        }

        [TestMethod]
        public void GetRelationship_Manager()
        {
            var relationship = new RelationshipFilter() { Id = 2 };
            var result = _relationshipManager.GetRelationship(relationship).Result;
            Assert.IsTrue(result != null);
        }
        [TestMethod]
        public void GetRelationshipLevelCode_Test()
        {

            var result = _relationshipManager.GetRelationshipLevelCode();
            Assert.IsTrue(result != null);
        }



        [TestMethod]
        public void GetRelationshipMapping_Test()
        {
            var relationship = new OrganizationRelationShip() { Created_org_id = 10 };
            var result = _relationshipManager.GetRelationshipMapping(relationship).Result;
            Assert.IsTrue(result != null);
        }

    }
}
