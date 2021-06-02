using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.features.repository;
using net.atos.daf.ct2.role.entity;
using net.atos.daf.ct2.role.repository;

namespace net.atos.daf.ct2.role.test
{
    [TestClass]
    public class RoleTest
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _config;
        private readonly IRoleRepository _RoleRepository;

        private readonly IRoleManagement _RoleManagement;

        private readonly IFeatureManager _featureManagement;
        private readonly FeatureRepository _FeatureRepository;

        public RoleTest()
        {
            string connectionString = "Server=dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com;Database=dafconnectmasterdatabase;Port=5432;User Id=pgadmin@dafct-dev0-dta-cdp-pgsql;Password=W%PQ1AI}Y\\97;Ssl Mode=Require;";
            _dataAccess = new PgSQLDataAccess(connectionString);
            _RoleRepository = new RoleRepository(_dataAccess);

            _FeatureRepository = new FeatureRepository(_dataAccess);
            _featureManagement = new FeatureManager(_FeatureRepository);
            _RoleManagement = new RoleManagement(_RoleRepository, _featureManagement, _FeatureRepository);

        }

        [TestMethod]
        public void CreateRole()
        {
            RoleMaster ObjRole = new RoleMaster();

            ObjRole.Organization_Id = 12;
            ObjRole.Name = "Role 9";
            ObjRole.FeatureSet = new FeatureSet();
            ObjRole.FeatureSet.Features = new List<Feature>();
            features.entity.Feature objfeature = new features.entity.Feature();
            objfeature.Id = 4;
            features.entity.Feature objfeature1 = new features.entity.Feature();
            objfeature1.Id = 2;
            features.entity.Feature objfeature2 = new features.entity.Feature();
            objfeature2.Id = 3;
            ObjRole.FeatureSet.Features.Add(objfeature);
            ObjRole.FeatureSet.Features.Add(objfeature1);
            ObjRole.FeatureSet.Features.Add(objfeature2);
            var role = _RoleManagement.CreateRole(ObjRole).Result;
            Assert.IsNotNull(role);
            Assert.IsTrue(role > 0);

        }

        [TestMethod]
        public void DeleteRole()
        {
            int roleid = 1;
            int accountid = 20;
            var role = _RoleRepository.DeleteRole(roleid, accountid).Result;
            Assert.IsNotNull(role);
            Assert.IsTrue(role > 0);

        }

        [TestMethod]
        public void GetRoles()
        {
            RoleFilter filter = new RoleFilter();
            filter.Organization_Id = 12;
            var role = _RoleRepository.GetRoles(filter).Result;
            Assert.IsNotNull(role);
            Assert.IsTrue(role.Count() > 0);

        }

        [TestMethod]
        public void UpdateRole()
        {
            RoleMaster roleMaster = new RoleMaster();
            roleMaster.Name = "UpdateRole";
            roleMaster.Id = 5;
            roleMaster.Updatedby = 6;
            var role = _RoleRepository.UpdateRole(roleMaster).Result;
            Assert.IsNotNull(role);
            Assert.IsTrue(role > 0);

        }


        [TestMethod]
        public void AddFeatureSet()
        {
            FeatureSet set = new FeatureSet();
            set.Features = new List<features.entity.Feature>();
            features.entity.Feature objfeature = new features.entity.Feature();
            objfeature.Id = 4;
            features.entity.Feature objfeature1 = new features.entity.Feature();
            objfeature1.Id = 2;
            features.entity.Feature objfeature2 = new features.entity.Feature();
            objfeature2.Id = 3;
            set.Name = "FeatureSet04";
            // set.Createdby = 12;
            set.Description = "FSet04";
            set.Is_custom_feature_set = true;
            set.Features.Add(objfeature);
            set.Features.Add(objfeature1);
            set.Features.Add(objfeature2);
            var result = _FeatureRepository.AddFeatureSet(set).Result;
            Assert.IsNotNull(result);
            // Assert.IsTrue(result.da > 0);

        }

        [TestMethod]
        public void GetFeatureSet()
        {
            int featuresetid = 0;
            var result = _FeatureRepository.GetFeatureSet(featuresetid, 'Á').Result;
            Assert.IsNotNull(result);
            // Assert.IsTrue(result.da > 0);
        }

        [TestMethod]
        public void GetFeatures()
        {
            // var result= _FeatureRepository.GetFeatures(Type,true).Result;
            // Assert.IsNotNull(result);
            // Assert.IsTrue(result.da > 0);
        }
    }
}
