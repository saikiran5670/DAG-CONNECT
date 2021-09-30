using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.visibility.entity;
using net.atos.daf.ct2.visibility.repository;

namespace net.atos.daf.ct2.visibility
{
    public class VisibilityManager : IVisibilityManager
    {
        private readonly IVisibilityRepository _visibilityRepository;
        private readonly IVehicleManager _vehicleManager;

        public VisibilityManager(IVisibilityRepository visibilityRepository, IVehicleManager vehicleManager)
        {
            _visibilityRepository = visibilityRepository;
            _vehicleManager = vehicleManager;
        }

        public async Task<int> GetReportFeatureId(int reportId)
        {
            return await _visibilityRepository.GetReportFeatureId(reportId);
        }

        public async Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleByAccountVisibility(int accountId, int orgId, int contextOrgId, int reportFeatureId)
        {
            Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict;
            //If context switched then find vehicle visibility for the organization
            if (orgId != contextOrgId)
            {
                resultDict = await _vehicleManager.GetVisibilityVehiclesByOrganization(contextOrgId);
            }
            else
            {
                resultDict = await _vehicleManager.GetVisibilityVehicles(accountId, orgId);
            }

            // vehicle filtering based on features
            resultDict = await FilterVehiclesByfeatures(resultDict, reportFeatureId, contextOrgId);

            //return await _visibilityRepository.GetVehicleVisibilityDetails(filteredVehicles.Select(x => x.Id).ToArray(), accountId);
            return MapVehicleDetails(accountId, contextOrgId, resultDict);
        }

        public async Task<IEnumerable<VehicleDetailsAccountVisibilityForOTA>> GetVehicleByAccountVisibilityForOTA(int accountId, int orgId, int contextOrgId, int reportFeatureId)
        {
            Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict;
            //If context switched then find vehicle visibility for the organization
            if (orgId != contextOrgId)
            {
                resultDict = await _vehicleManager.GetVisibilityVehiclesByOrganization(contextOrgId);
            }
            else
            {
                resultDict = await _vehicleManager.GetVisibilityVehicles(accountId, orgId);
            }

            // vehicle filtering based on features
            resultDict = await FilterVehiclesByfeatures(resultDict, reportFeatureId, contextOrgId);

            //return await _visibilityRepository.GetVehicleVisibilityDetails(filteredVehicles.Select(x => x.Id).ToArray(), accountId);
            return MapVehicleDetailsForOTA(resultDict);
        }

        /// <summary>
        /// Filtering visible and owned vehicle as per feature id 
        /// </summary>
        /// <param name="resultDict">Visibile vehicles received from account visibility</param>
        /// <param name="reportFeatureId">Report feature Id recieved from report click</param>
        /// <param name="contextOrgId">organizatin id</param>
        /// <returns></returns>
        private async Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> FilterVehiclesByfeatures(Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict, int reportFeatureId, int contextOrgId)
        {
            var vehicles = resultDict.Values.SelectMany(x => x).Distinct(new ObjectComparer()).ToList();

            if (reportFeatureId > 0 && vehicles.Count() > 0)
            {
                var vehiclePackages = await GetSubscribedVehicleByFeature(reportFeatureId, contextOrgId);

                //Filter owned vehicles based on package features
                //If not found Org packages then filter vehicles based on subscribed vehicles
                var ownedVehicles = vehicles.Where(e => e.HasOwned == true).ToList();
                if (ownedVehicles.Count() > 0)
                {
                    if (vehiclePackages.Any(e => e.PackageType == "V"))
                    {
                        var filteredOwnedVehicleIds = ownedVehicles
                            .Where(e => vehiclePackages.FirstOrDefault(e => e.PackageType == "V").VehicleIds.Contains(e.Id)).Select(k => k.Id);

                        //Removing other vins from owned vehicles list and not allow them in the visibility
                        ownedVehicles.RemoveAll(e => !filteredOwnedVehicleIds.Contains(e.Id));
                    }
                }

                //Filter visible vehicles based on org relationship features and package features
                var visibleVehicles = vehicles.Where(e => e.HasOwned == false).ToList();

                if (visibleVehicles.Count() > 0)
                {
                    //Fetch visible relationship vehicles of having reportFeatureId in it's allowed features list
                    //Intersect those vehicles with Org+VIN package subscribed vehicles where reportFeatureId is present in the subscription
                    //Filter vehicles out those are not in relationship vehicles and subscribed vehicles.
                    if (vehiclePackages.Any(e => e.PackageType == "V"))
                    {
                        var subscriptionVehicleIds = vehiclePackages.Where(e => e.PackageType == "V").First().VehicleIds;
                        var relationshipVehicleIds = await _visibilityRepository.GetRelationshipVehiclesByFeature(reportFeatureId, contextOrgId);

                        //Fetch vehicles records to be removed from visible vehicles list
                        var filteredVisibleVehicleIds = relationshipVehicleIds.Except(subscriptionVehicleIds).ToList();
                        filteredVisibleVehicleIds.AddRange(subscriptionVehicleIds.Except(relationshipVehicleIds));

                        visibleVehicles.RemoveAll(e => filteredVisibleVehicleIds.Contains(e.Id));
                    }
                    else
                    {
                        visibleVehicles.Clear();
                    }
                }

                //Concatenate both lists
                vehicles = ownedVehicles.Concat(visibleVehicles.AsEnumerable()).ToList();

                foreach (var vg_kv in resultDict.ToList())
                {
                    var resultVehicles = vg_kv.Value;
                    resultDict[vg_kv.Key] = resultVehicles.Intersect(vehicles, new ObjectComparer()).ToList();
                }
            }

            return resultDict;
        }

        public async Task<IEnumerable<VehiclePackage>> GetSubscribedVehicleByFeature(int featureid, int organizationid)
        {
            return await _visibilityRepository.GetSubscribedVehicleByFeature(featureid, organizationid);
        }

        public async Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleByAccountVisibilityTemp(int accountId, int orgId, int contextOrgId, int reportFeatureId)
        {
            Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict;
            //If context switched then find vehicle visibility for the organization
            if (orgId != contextOrgId)
            {
                resultDict = await _vehicleManager.GetVisibilityVehiclesByOrganization(contextOrgId);
            }
            else
            {
                resultDict = await _vehicleManager.GetVisibilityVehicles(accountId, orgId);
            }

            // vehicle filtering based on features
            resultDict = await FilterVehiclesByfeatures(resultDict, reportFeatureId, contextOrgId);

            //return await _visibilityRepository.GetVehicleVisibilityDetailsTemp(vehicles.Select(x => x.Id).ToArray());
            return MapVehicleDetailsTemp(accountId, contextOrgId, resultDict);
        }

        public Task<IEnumerable<VehicleDetailsFeatureAndSubsction>> GetVehicleByFeatureAndSubscription(int accountId, int orgId, int contextOrgId, int roleId,
                                                                                                string featureName = "Alert") => _visibilityRepository.GetVehicleByFeatureAndSubscription(accountId, orgId, contextOrgId, roleId, featureName);

        public async Task<IEnumerable<VehicleDetailsVisibiltyAndFeature>> GetVehicleByVisibilityAndFeature(int accountId, int orgId, int contextOrgId, int roleId,
                                                                                                           IEnumerable<VehicleDetailsAccountVisibility> vehicleDetailsAccountVisibilty, int featureId, string featureName = "Alert")
        {
            try
            {
                var vehicleByVisibilityAndFeature = new List<VehicleDetailsVisibiltyAndFeature>();
                var vehicleByVisibility = vehicleDetailsAccountVisibilty ?? await GetVehicleByAccountVisibility(accountId, orgId, contextOrgId, featureId);

                if (!vehicleByVisibility.Any())
                {
                    return await Task.FromResult(new List<VehicleDetailsVisibiltyAndFeature>());
                }

                var vehicleByFeature = await GetVehicleByFeatureAndSubscription(accountId, orgId, contextOrgId, roleId, featureName);

                if (!vehicleByFeature.Any())
                {
                    return await Task.FromResult(new List<VehicleDetailsVisibiltyAndFeature>());
                }
                else
                {
                    foreach (var feature in vehicleByFeature)
                    {
                        if (feature.VehicleId == 0 && feature.SubscriptionType.ToLower() == "o")
                        {
                            foreach (var item in vehicleByVisibility)
                            {
                                vehicleByVisibilityAndFeature.Add(new VehicleDetailsVisibiltyAndFeature
                                {
                                    Vin = item.Vin,
                                    VehicleGroupId = item.VehicleGroupId,
                                    VehicleId = item.VehicleId,
                                    FeatureName = feature.Name,
                                    FeatureKey = feature.Name.ToLower().Contains("alerts.") == true ? feature.FeatureEnum : feature.Key,
                                    Subscribe = true
                                });
                            }
                        }
                        else
                        {
                            foreach (var item in vehicleByVisibility)
                            {
                                if (feature.VehicleId == item.VehicleId)
                                {
                                    vehicleByVisibilityAndFeature.Add(new VehicleDetailsVisibiltyAndFeature
                                    {
                                        Vin = item.Vin,
                                        VehicleGroupId = item.VehicleGroupId,
                                        VehicleId = item.VehicleId,
                                        FeatureName = feature.Name,
                                        FeatureKey = feature.Name.ToLower().Contains("alerts.") == true ? feature.FeatureEnum : feature.Key,
                                        Subscribe = true
                                    });
                                }
                            }
                        }
                    }
                }
                return await Task.FromResult(vehicleByVisibilityAndFeature);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VehicleDetailsVisibiltyAndFeatureTemp>> GetVehicleByVisibilityAndFeatureTemp(int accountId, int orgId, int contextOrgId, int roleId,
                                                                                                           string featureName = "Alert")
        {
            try
            {
                var vehicleByVisibilityAndFeature = new List<VehicleDetailsVisibiltyAndFeatureTemp>();

                var vehicleByFeature = await GetVehicleByFeatureAndSubscription(accountId, orgId, contextOrgId, roleId, featureName);

                foreach (var item in vehicleByFeature)
                {
                    vehicleByVisibilityAndFeature.Add(new VehicleDetailsVisibiltyAndFeatureTemp
                    {
                        VehicleId = item.VehicleId,
                        FeatureKey = item.Name.ToLower().Contains("alerts.") == true ? item.FeatureEnum : item.Key,
                        SubscriptionType = item.SubscriptionType
                    });
                }

                return await Task.FromResult(vehicleByVisibilityAndFeature);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<List<VisibilityVehicle>> GetVisibilityVehicles(IEnumerable<int> vehicleGroupIds, int orgId)
        {
            return await _vehicleManager.GetVisibilityVehicles(vehicleGroupIds, orgId);
        }

        private static List<VehicleDetailsAccountVisibility> MapVehicleDetails(int accountId, int contextOrgId, Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict)
        {
            List<VehicleDetailsAccountVisibility> vehicleDetails = new List<VehicleDetailsAccountVisibility>();
            foreach (var vg_kv in resultDict)
            {
                var vehicleGroup = vg_kv.Key;
                var visibleVehicles = vg_kv.Value;
                //if (!vehicleGroup.GroupType.Equals("S"))
                {
                    foreach (var vehicle in visibleVehicles)
                    {
                        vehicleDetails.Add(new VehicleDetailsAccountVisibility
                        {
                            AccountId = accountId,
                            VehicleId = vehicle.Id,
                            Vin = vehicle.VIN,
                            VehicleName = vehicle.Name ?? string.Empty,
                            RegistrationNo = vehicle.RegistrationNo ?? string.Empty,
                            ObjectType = "V",
                            VehicleGroupId = vehicleGroup.Id,
                            VehicleGroupName = vehicleGroup.Name,
                            GroupType = vehicleGroup.GroupType,
                            FunctionEnum = vehicleGroup.GroupMethod ?? string.Empty,
                            OrganizationId = contextOrgId
                        });
                    }
                }
            }

            return vehicleDetails;
        }

        private static IEnumerable<VehicleDetailsAccountVisibility> MapVehicleDetailsTemp(int accountId, int contextOrgId, Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict)
        {
            List<VehicleDetailsAccountVisibility> vehicleDetails = new List<VehicleDetailsAccountVisibility>();
            foreach (var vg_kv in resultDict)
            {
                var vehicleGroup = vg_kv.Key;
                var visibleVehicles = vg_kv.Value;
                //if (!vehicleGroup.GroupType.Equals("S"))
                {
                    foreach (var vehicle in visibleVehicles)
                    {
                        vehicleDetails.Add(new VehicleDetailsAccountVisibility
                        {
                            AccountId = accountId,
                            VehicleId = vehicle.Id,
                            Vin = vehicle.VIN,
                            VehicleName = vehicle.Name ?? string.Empty,
                            RegistrationNo = vehicle.RegistrationNo ?? string.Empty,
                            ObjectType = "V",
                            VehicleGroupId = vehicleGroup.Id,
                            VehicleGroupName = vehicleGroup.Name,
                            GroupType = vehicleGroup.GroupType,
                            FunctionEnum = vehicleGroup.GroupMethod ?? string.Empty,
                            OrganizationId = contextOrgId
                        });
                    }
                }
            }

            var result = vehicleDetails.GroupBy(x => new
            {
                x.VehicleId,
                x.VehicleName,
                x.Vin,
                x.RegistrationNo,
                x.OrganizationId
            }).Select(x => new VehicleDetailsAccountVisibility
            {
                VehicleId = x.Key.VehicleId,
                Vin = x.Key.Vin,
                VehicleName = x.Key.VehicleName,
                RegistrationNo = x.Key.RegistrationNo,
                OrganizationId = x.Key.OrganizationId,
                VehicleGroupIds = x.Select(x => x.VehicleGroupId).ToArray(),
                VehicleGroupDetails = x.Select(x => $"{ x.VehicleGroupId }~{ x.VehicleGroupName }~{ x.GroupType }")
                .Aggregate((s1, s2) => s1 + "," + s2)
            });
            return result;
        }


        private static IEnumerable<VehicleDetailsAccountVisibilityForOTA> MapVehicleDetailsForOTA(Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict)
        {
            List<VehicleDetailsAccountVisibilityForOTA> vehicleDetails = new List<VehicleDetailsAccountVisibilityForOTA>();
            foreach (var vg_kv in resultDict)
            {
                var vehicleGroup = vg_kv.Key;
                var visibleVehicles = vg_kv.Value;
                //if (!vehicleGroup.GroupType.Equals("S"))
                {
                    foreach (var vehicle in visibleVehicles)
                    {
                        vehicleDetails.Add(new VehicleDetailsAccountVisibilityForOTA
                        {
                            VehicleId = vehicle.Id,
                            Vin = vehicle.VIN,
                            VehicleName = vehicle.Name ?? string.Empty,
                            RegistrationNo = vehicle.RegistrationNo ?? string.Empty,
                            VehicleGroupName = vehicleGroup.Name,
                            ModelYear = string.Empty,
                            Type = string.Empty
                        });
                    }
                }
            }

            var result = vehicleDetails.GroupBy(x => new
            {
                x.VehicleId,
                x.VehicleName,
                x.Vin,
                x.RegistrationNo,
                x.ModelYear,
                x.Type
            }).Select(x => new VehicleDetailsAccountVisibilityForOTA
            {
                VehicleId = x.Key.VehicleId,
                Vin = x.Key.Vin,
                VehicleName = x.Key.VehicleName,
                RegistrationNo = x.Key.RegistrationNo,
                ModelYear = x.Key.ModelYear,
                Type = x.Key.Type,
                VehicleGroupNames = x.Select(x => x.VehicleGroupName)
                .Aggregate((s1, s2) => s1 + "," + s2)
            });
            return result;
        }
        internal class ObjectComparer : IEqualityComparer<VisibilityVehicle>
        {
            public bool Equals(VisibilityVehicle x, VisibilityVehicle y)
            {
                if (object.ReferenceEquals(x, y))
                {
                    return true;
                }
                if (x is null || y is null)
                {
                    return false;
                }
                return x.Id == y.Id && x.VIN == y.VIN;
            }

            public int GetHashCode([DisallowNull] VisibilityVehicle obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                int idHashCode = obj.Id.GetHashCode();
                int vinHashCode = obj.VIN == null ? 0 : obj.VIN.GetHashCode();
                return idHashCode ^ vinHashCode;
            }
        }
    }
}
