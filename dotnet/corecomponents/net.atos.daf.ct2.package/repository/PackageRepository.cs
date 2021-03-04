using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.features;
using net.atos.daf.ct2.features.entity;
using net.atos.daf.ct2.package.entity;
using net.atos.daf.ct2.package.ENUM;
using net.atos.daf.ct2.utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public PackageRepository(IDataAccess dataAccess, IFeatureManager featureManager)
        {
            _dataAccess = dataAccess;
            _featureManager = featureManager;
        }

        public async Task<Package> Create(Package package)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@packagecode", package.Code);
                parameter.Add("@feature_set_id", package.FeatureSetID);
                parameter.Add("@name", package.Name);
                parameter.Add("@type", package.Type);
                parameter.Add("@description", package.Description);
                // parameter.Add("@is_default", Convert.ToBoolean(package.Default));
                //parameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(package.StartDate));
                //parameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(package.EndDate));
                parameter.Add("@is_active", Convert.ToBoolean(package.Status));

                string query = @"insert into master.package(packagecode,feature_set_id,name,type,description,is_active) " +
                              "values(@packagecode,@feature_set_id,@name,@type,@description,@is_active) RETURNING id";

                var id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                package.Id = id;

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
                var parameter = new DynamicParameters();
                parameter.Add("@Id", package.Id);
                parameter.Add("@packagecode", package.Code);
                parameter.Add("@feature_set_id", package.FeatureSetID);
                parameter.Add("@name", package.Name);
                parameter.Add("@type", (char)package.Type);
                parameter.Add("@description", package.Description);
                // parameter.Add("@is_default", Convert.ToBoolean(package.Default));
                //   parameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(package.StartDate));
                //  parameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(package.EndDate));
                parameter.Add("@is_active", Convert.ToBoolean(package.Status));
                string query = @"update master.package set packagecode=@packagecode, 
                                                           feature_set_id=@feature_set_id,
                                                           name=@name,
                                                           type=@type,
                                                           description=@description,                                
                                                           is_active=@is_active                                                            
                                                           where id = @Id RETURNING id";
                package.Id = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
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
                        package.FeatureSetID = 9;//this line is just for testing purpose
                       // package.FeatureSetID = CreateFeatureSet(package.Features).Result;
                        if (package.Code != null && package.FeatureSetID > 0)
                        {
                            parameter = new DynamicParameters();
                            parameter.Add("@packagecode", package.Code);
                            parameter.Add("@feature_set_id", package.FeatureSetID);
                            parameter.Add("@name", package.Name);
                            parameter.Add("@type", (char)package.Type);
                            parameter.Add("@description", package.Description);
                            //parameter.Add("@is_default", Convert.ToBoolean(package.Default));                         
                            parameter.Add("@is_active", Convert.ToBoolean(package.Status));
                            query = @"insert into master.package(packagecode,feature_set_id,name,type,description,is_active) " +
                                    "values(@packagecode,@feature_set_id,@name,@type,@description,@is_active) RETURNING id";
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

        private Task<int> CreateFeatureSet(List<string> featues)
        {
            var pkgFeatures = featues.Select(x => new Feature() { Name = x }).ToList();

            long iSessionStartedAt = UTCHandling.GetUTCFromDateTime(DateTime.Now);
            long iSessionExpireddAt = UTCHandling.GetUTCFromDateTime(DateTime.Now.AddMinutes(30));
            FeatureSet featureSet = new FeatureSet();
            featureSet.Name = "FeatureSet_" + iSessionStartedAt;
            featureSet.description = "PackageTest data";
            featureSet.Is_Active = true;
            featureSet.created_at = iSessionStartedAt;
            featureSet.created_by = 1;
            featureSet.modified_at = iSessionExpireddAt;
            featureSet.modified_by = 1;
            featureSet.Features = pkgFeatures;
            var featureId = _featureManager.AddFeatureSet(featureSet);
            return featureId;

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

                query = @"select id,packagecode,feature_set_id,name,type,description,is_active from master.package pkg where 1=1 ";

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
                        parameter.Add("@name", filter.Name + "%");
                        query = query + " and pkg.name like @name ";
                    }
                    // feature set id filter
                    if (filter.FeatureSetId > 0)
                    {
                        parameter.Add("@feature_set_id", filter.FeatureSetId);
                        query = query + " and pkg.feature_set_id = @feature_set_id ";
                    }
                    // package type filter
                    if (filter.Type != 0)// (((char)filter.Type) != ((char)PackageType.Organization))
                    {
                        parameter.Add("@type", (char)filter.Type);//, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);

                        query = query + " and pkg.type=@type";
                    }


                    // package status filter 
                    if (filter.Status != 0)
                    {
                        parameter.Add("@is_active", Convert.ToBoolean(filter.Status));

                        query = query + " and pkg.is_active=@is_active";
                    }


                    // account ids filter                    
                    if ((!string.IsNullOrEmpty(filter.PackageCodes)) && Convert.ToInt32(filter.PackageCodes.Length) > 0)
                    {
                        // Account Id list Filter
                        filter.PackageCodes = filter.PackageCodes.TrimEnd(',');
                        List<int> packagecodes = filter.PackageCodes.Split(',').Select(int.Parse).ToList();
                        parameter.Add("@packagecodes", packagecodes);
                        query = query + " and pkg.id = ANY(@packagecodes)";
                    }
                    // account group filter
                    if ((!string.IsNullOrEmpty(filter.PackageCodes)) && Convert.ToInt32(filter.PackageCodes.Length) > 0)
                    {
                        // Account Id list Filter
                        filter.PackageCodes = filter.PackageCodes.TrimEnd(',');
                        List<int> packagecodes = filter.PackageCodes.Split(',').Select(int.Parse).ToList();
                        parameter.Add("@packagecodes", packagecodes);
                        query = query + " and pkg_.id = ANY(@packagecodes)";
                    }

                    dynamic result = await _dataAccess.QueryAsync<dynamic>(query, parameter);

                    foreach (dynamic record in result)
                    {

                        packages.Add(Map(record));
                    }
                }
                return packages;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Package Map(dynamic record)
        {
            Package package = new Package();
            package.Id = record.id;
            package.Code = record.packagecode;

            package.Status = record.is_active ? PackageStatus.Active : PackageStatus.Inactive;
            package.Type = (PackageType)Convert.ToChar(record.type);
            package.Name = record.name;
            package.Description = record.description;
            package.FeatureSetID = record.feature_set_id;
            return package;
        }

        public Task<FeatureSet> Update(FeatureSet featureSet)
        {
            throw new NotImplementedException();
        }

        public Task<Feature> GetFeature(int featureId)
        {
            throw new NotImplementedException();
        }

        public Task<FeatureSet> GetFeatureSet(int featureSetId)
        {
            throw new NotImplementedException();
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
