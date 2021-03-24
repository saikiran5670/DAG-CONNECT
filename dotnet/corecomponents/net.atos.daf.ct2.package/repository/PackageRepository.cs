using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.package.entity;
using net.atos.daf.ct2.utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.package.repository
{
    public class PackageRepository : IPackageRepository
    {
        private readonly IDataAccess _dataAccess;
        private readonly IFeatureManager _featureManager;
        private static readonly log4net.ILog log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly PackageCoreMapper _packageCoreMapper;

        public PackageRepository(IDataAccess dataAccess, IFeatureManager featureManager)
        {
            _dataAccess = dataAccess;
            _featureManager = featureManager;
            _packageCoreMapper = new PackageCoreMapper();
        }

        public async Task<Package> Create(Package package)
        {
            try
            {
                var isPackageCodeExist = IsPackageCodeExists(package.Code);
                if (!isPackageCodeExist)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@packagecode", package.Code);
                    parameter.Add("@feature_set_id", package.FeatureSetID);
                    parameter.Add("@name", package.Name);
                    parameter.Add("@type", Convert.ToChar(package.Type));
                    parameter.Add("@description", package.Description);
                    parameter.Add("@is_active", package.IsActive);
                    parameter.Add("@status", Convert.ToChar(package.Status));
                    parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

                    string query = @"insert into master.package(packagecode,feature_set_id,name,type,description,is_active,status,created_at) " +
                                  "values(@packagecode,@feature_set_id,@name,@type,@description,@is_active,@status,@created_at) RETURNING id";

                    var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                    package.Id = id;
                }
                else
                {
                    package.Id = -1;//to check either code exists or not
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return package;
        }
        public async Task<Package> Update(Package package)
        {
            try
            {
                var isPackageUpdate = CheckPackageCodeForUpdate(package.Code);

                if (isPackageUpdate)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("@Id", package.Id);
                    parameter.Add("@packagecode", package.Code);
                    parameter.Add("@feature_set_id", package.FeatureSetID);
                    parameter.Add("@name", package.Name);
                    parameter.Add("@type", Convert.ToChar(package.Type));
                    parameter.Add("@description", package.Description);
                    parameter.Add("@is_active", package.IsActive);
                    parameter.Add("@status", Convert.ToChar(package.Status));
                 //   parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    string query = @"update master.package set packagecode=@packagecode, 
                                                           feature_set_id=@feature_set_id,
                                                           name=@name,
                                                           type=@type,
                                                           description=@description,                                
                                                           is_active=@is_active,
                                                           status=@status                                                          
                                                           where id = @Id RETURNING id";
                    package.Id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                }
                else
                {
                    package.Id = -1;//to check either code exists or not
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return package;
        }

        public async Task<List<Package>> Import(List<Package> packageList)
        {
            var packages = new List<Package>();
            try
            {
                var packageExits = PackageExists(packageList);
                var newPackages = from package in packageList
                                  where !packageExits.Any(x => x.Code == package.Code)
                                  select package;

                var parameter = new DynamicParameters();
                string query = string.Empty;
                if (packageList != null)
                {
                    foreach (Package package in newPackages)
                    {

                        if (package.Code != null && package.FeatureSetID > 0)
                        {
                            parameter = new DynamicParameters();
                            parameter.Add("@packagecode", package.Code);
                            parameter.Add("@feature_set_id", package.FeatureSetID);
                            parameter.Add("@name", package.Name);
                            parameter.Add("@type", package.Type.Length > 1 ? _packageCoreMapper.MapPackageType(package.Type) : Convert.ToChar(package.Type));
                            parameter.Add("@description", package.Description);
                            parameter.Add("@is_active", package.IsActive);
                            parameter.Add("@status", Convert.ToChar(package.Status));
                            parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                            query = @"insert into master.package(packagecode,feature_set_id,name,type,description,is_active,status,created_at) " +
                                    "values(@packagecode,@feature_set_id,@name,@type,@description,@is_active,@status,@created_at) RETURNING id";
                            var pkgId = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                            package.Id = pkgId;
                            if (pkgId > 0)
                            {
                                packages.Add(package);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return packages;
        }

        private List<Package> PackageExists(List<Package> packageList)
        {
            var packageFilter = new PackageFilter();
            var packages = Get(packageFilter);
            var PackageExist = from package in packages.Result
                               where packageList.Any(x => x.Code == package.Code)
                               select package;
            return PackageExist.ToList();

        }
        private bool IsPackageCodeExists(string packageCode)
        {
            var packageFilter = new PackageFilter();
            var packages = Get(packageFilter);
            var codeExists = packages.Result.Any(t => t.Code == packageCode);
            return codeExists;
        }
        private bool CheckPackageCodeForUpdate(string packageCode)
        {
            var packageFilter = new PackageFilter();
            var packages = Get(packageFilter);
            var codeExists = packages.Result.Where(t => t.Code == packageCode).Count();
            return codeExists > 1 ? false : true;
        }

        public Task<FeatureSet> Create(FeatureSet featureSet)
        {
            try
            {
                throw new NotImplementedException();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<Package>> Get(PackageFilter filter)
        {
            try
            {
                var parameter = new DynamicParameters();
                List<Package> packages = new List<Package>();
                string query = string.Empty;

                query = @"select id,packagecode,feature_set_id,name,type,description,is_active,status,created_at from master.package pkg where id !=1 and is_active = true ";

                if (filter != null)
                {
                    // id filter
                    if (filter.Id > 0)
                    {
                        parameter.Add("@id", filter.Id);
                        query = query + " and pkg.id=@id ";
                    }
                    // package code filter
                    if (!string.IsNullOrEmpty(filter.Code))
                    {

                        parameter.Add("@packagecode", filter.Code.ToLower());
                        query = query + " and LOWER(pkg.packagecode) = @packagecode ";
                    }
                    // package name filter
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        parameter.Add("@name", "%" + filter.Name + "%");
                        query = query + " and LOWER(pkg.name) like @name ";
                    }
                    // feature set id filter
                    if (filter.FeatureSetId > 0)
                    {
                        parameter.Add("@feature_set_id", filter.FeatureSetId);
                        query = query + " and pkg.feature_set_id = @feature_set_id ";
                    }
                    // package type filter
                    if (!string.IsNullOrEmpty(filter.Type) && filter.Type.Length == 1)
                    {
                        parameter.Add("@type", (char)_packageCoreMapper.ToPackageType(filter.Type), DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and pkg.type=@type ";
                    }


                    // package status filter 
                    if (!string.IsNullOrEmpty(filter.Status) && filter.Status.Length == 1)
                    {
                        parameter.Add("@status", (char)_packageCoreMapper.ToPackageStatus(filter.Status), DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);
                        query = query + " and pkg.status=@status";
                    }

                    query = query + " and pkg.type in ('O','V') ORDER BY id ASC; ";
                    dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);

                    foreach (dynamic record in result)
                    {

                        packages.Add(_packageCoreMapper.Map(record));
                    }
                }
                return packages;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Delete(int packageId)
        {
            log.Info("Delete Package method called in repository");
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@id", packageId);
                var query = @"update master.package set is_active=false where id=@id";
                int isdelete = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return true;
            }
            catch (Exception ex)
            {
                log.Info("Delete Package method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(packageId));
                log.Error(ex.ToString());
                throw ex;
            }
        }
        public Task<List<Package>> Export()
        {
            throw new NotImplementedException();
        }
    }
}
