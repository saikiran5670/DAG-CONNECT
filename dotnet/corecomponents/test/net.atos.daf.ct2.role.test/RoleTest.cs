using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.role.repository;
using System.Linq;
using net.atos.daf.ct2.role.entity;

namespace net.atos.daf.ct2.role.test
{
    [TestClass]
    public class RoleTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly IRoleRepository _RoleRepository;

        public RoleTest()
        {
              string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y\\97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _RoleRepository = new RoleRepository(_dataAccess);
            
        }

        [TestMethod]
        public void CreateRole()
        {
            RoleMaster ObjRole = new RoleMaster();

            ObjRole.Organization_Id =12;
            ObjRole.Name = "Role 9";
            ObjRole.createdby = 2;
            var role = _RoleRepository.CreateRole(ObjRole).Result;
            Assert.IsNotNull(role);
            Assert.IsTrue(role > 0);

        }

        [TestMethod]
        public void DeleteRole()
        {
            int roleid=1;
            int accountid=20;
            var role = _RoleRepository.DeleteRole(roleid,accountid).Result;
            Assert.IsNotNull(role);
            Assert.IsTrue(role > 0);

        }

        [TestMethod]
        public void GetRoles()
        {
            int roleid=1;
            int accountid=20;
            var role = _RoleRepository.GetRoles(roleid).Result;
            Assert.IsNotNull(role);
            Assert.IsTrue(role.Count() > 0);

        }
    }
}
