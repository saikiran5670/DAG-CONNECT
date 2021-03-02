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
                parameter.Add("@code", package.Code);
                parameter.Add("@feature_set_id", package.FeatureSetID);
                parameter.Add("@name", package.Name);
                parameter.Add("@type", (char)package.Pack_Type);
                parameter.Add("@short_description", package.ShortDescription);
                parameter.Add("@is_default", Convert.ToBoolean(package.Default));
                parameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(package.StartDate));
                parameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(package.EndDate));
                //parameter.Add("@is_active", package.Is_Active);

                string query = @"insert into master.package(code,feature_set_id,name,type,short_description,is_default,start_date,end_date) " +
                              "values(@code,@feature_set_id,@name,@type,@short_description,@is_default,@start_date,@end_date) RETURNING id";

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
                parameter.Add("@code", package.Code);
                parameter.Add("@feature_set_id", package.FeatureSetID);
                parameter.Add("@name", package.Name);
                parameter.Add("@type", (char)package.Pack_Type);
                parameter.Add("@short_description", package.ShortDescription);
                parameter.Add("@is_default", Convert.ToBoolean(package.Default));
                parameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(package.StartDate));
                parameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(package.EndDate));
                //parameter.Add("@is_active", package.Is_Active);// required to add database
                string query = @"update master.package set code=@code, 
                                                           feature_set_id=@feature_set_id,
                                                           name=@name,
                                                           type=@type,
                                                           short_description=@short_description,
                                                           is_default=@is_default,
                                                           start_date=@start_date,
                                                           end_date=@end_date                                                            
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
                var packageExits = GetPackage();

                var parameter = new DynamicParameters();
                string query = string.Empty;
                if (packageList != null)
                {
                    foreach (Package package in packageList)
                    {

                        var featureId = _featureManager.AddFeatureSet(new FeatureSet() { Features = package.Features.Cast<Feature>().ToList() });
                        if (package.Code != null && package.FeatureSetID > 0)
                        {
                            parameter = new DynamicParameters();
                            parameter.Add("@code", package.Code);
                            parameter.Add("@feature_set_id", package.FeatureSetID);
                            parameter.Add("@name", package.Name);
                            parameter.Add("@type", (char)package.Pack_Type);
                            parameter.Add("@short_description", package.ShortDescription);
                            parameter.Add("@is_default", Convert.ToBoolean(package.Default));
                            parameter.Add("@start_date", UTCHandling.GetUTCFromDateTime(package.StartDate));
                            parameter.Add("@end_date", UTCHandling.GetUTCFromDateTime(package.EndDate));
                            query = @"insert into master.package(code,feature_set_id,name,type,short_description,is_default,start_date,end_date) " +
                                    "values(@code,@feature_set_id,@name,@type,@short_description,@is_default,@start_date,@end_date) RETURNING id";
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

        public async Task<List<Package>> GetPackage()
        {
            List<Package> packageList = new List<Package>();
            try
            {
                var parameter = new DynamicParameters();
                string query = string.Empty;
                query = @"select * from master.package";
                IEnumerable<dynamic> packages = await _dataAccess.QueryAsync<dynamic>(query, parameter);
                foreach (dynamic package in packages)
                {
                    packageList.Add(package);
                }
                return packageList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

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
                //List<Account> accounts = new List<Account>();
                List<Package> packages = new List<Package>();
                string query = string.Empty;

                query = @"select id,code,feature_set_id,name,type,short_description,is_default,start_date,end_date from master.package pkg where 1=1 ";

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

                        parameter.Add("@code", filter.Code.ToLower());
                        query = query + " and LOWER(pkg.code) = @code ";
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
                    if (((char)filter.Type) != ((char)PackageType.Organization))
                    {
                        parameter.Add("@type", (char)filter.Type, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);

                        query = query + " and pkg.type=@type";
                    }


                    // package status filter 
                    if (((char)filter.Status) != ((char)PackageStatus.Active))
                    {
                        parameter.Add("@status", (char)filter.Status, DbType.AnsiStringFixedLength, ParameterDirection.Input, 1);

                        query = query + " and pkg.status=@status";
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
            package.Code = record.code;
            package.Status = record.status;
            package.Pack_Type = (PackageType)Convert.ToChar(record.type);
            package.Name = record.name;
            package.ShortDescription = record.short_description;
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
            log.Info("Delete Organization method called in repository");
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
                log.Info("Delete Organization method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(packageId));
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
