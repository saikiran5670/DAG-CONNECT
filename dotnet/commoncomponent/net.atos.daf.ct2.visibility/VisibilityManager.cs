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

            return MapVehicleDetails(accountId, contextOrgId, resultDict);
        }

        public async Task<IEnumerable<VehicleDetailsAccountVisibilityForOTA>> GetVehicleByAccountVisibilityForOTA(int accountId, int orgId, int contextOrgId, int featureId, int adminFeatureId)
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
            resultDict = await FilterVehiclesByfeatures(resultDict, featureId, contextOrgId);

            return await MapVehicleDetailsForOTA(resultDict, adminFeatureId, orgId, contextOrgId);
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
                var vehiclePackages = await _visibilityRepository.GetSubscribedVehicleByFeature(reportFeatureId, contextOrgId);

                //Filter owned vehicles based on package features
                //If not found Org packages then filter vehicles based on subscribed vehicles
                var ownedVehicles = vehicles.Where(e => e.HasOwned == true).ToList();
                if (ownedVehicles.Count() > 0)
                {
                    if (vehiclePackages.Any(e => e.PackageType == "O"))
                    {
                        //do nothing (if org type package no need to remove from owned vehicles)
                    }
                    else if (vehiclePackages.Any(e => (e.PackageType == "V" && e.HasOwned == true) || e.PackageType == "N")) // check if any v type and owned subscription available
                    {

                        var subscriptionVehicleIds = vehiclePackages.Where(e => e.HasOwned == true && e.PackageType == "V").SelectMany(e => e.VehicleIds);
                        var vinPackageVehicleIds = vehiclePackages.Where(e => e.PackageType == "N").SelectMany(e => e.VehicleIds).ToList();

                        var visibleVehiclesFromVR = ownedVehicles.Where(x => subscriptionVehicleIds.Contains(x.Id));//v1, v2, v3
                        var visibleVehiclesFromN = ownedVehicles.Where(x => vinPackageVehicleIds.Contains(x.Id));//v2, v4
                        ownedVehicles = visibleVehiclesFromVR.Union(visibleVehiclesFromN, new ObjectComparer()).ToList();

                        //Step1- take subscribed vehicle id org+vin
                        //step2- take vin type vehicle id vin
                        // step 3 Union and assigned to owned vehicle
                    }
                }

                //Filter visible vehicles based on org relationship features and package features
                var visibleVehicles = vehicles.Where(e => e.HasOwned == false).ToList();

                if (visibleVehicles.Count() > 0)
                {
                    //Fetch visible relationship vehicles of having reportFeatureId in it's allowed features list
                    //Intersect those vehicles with Org+VIN package subscribed vehicles where reportFeatureId is present in the subscription
                    //Filter vehicles out those are not in relationship vehicles and subscribed vehicles.
                    if (vehiclePackages.Any(e => (e.HasOwned == false && e.PackageType == "V") || e.PackageType == "N"))
                    {
                        //Step1- Take subscribed vehicle id org+vin
                        var subscriptionVehicleIds = vehiclePackages.Where(e => e.HasOwned == false && e.PackageType == "V").SelectMany(e => e.VehicleIds);
                        //Step2- Take vin type vehicle id vin
                        var vinPackageVehicleIds = vehiclePackages.Where(e => e.PackageType == "N").SelectMany(e => e.VehicleIds).ToList();

                        var relationshipVehicleIds = await _visibilityRepository.GetRelationshipVehiclesByFeature(reportFeatureId, contextOrgId);

                        //Fetch vehicles records from visible vehicles list from org+ vin package
                        var filteredVisibleVehicleIds = relationshipVehicleIds.Intersect(subscriptionVehicleIds);

                        //Fetch vehicles records from visible vehicles list from Vin package
                        var filteredVinPackageVisibleVehicleIds = relationshipVehicleIds.Intersect(vinPackageVehicleIds);

                        //Step3- Union and assigned to visible  vehicle
                        var visibleVehiclesFromVR = visibleVehicles.Where(x => filteredVisibleVehicleIds.Contains(x.Id));//v1, v2, v3
                        var visibleVehiclesFromN = visibleVehicles.Where(x => filteredVinPackageVisibleVehicleIds.Contains(x.Id));//v2, v4

                        visibleVehicles = visibleVehiclesFromVR.Union(visibleVehiclesFromN, new ObjectComparer()).ToList();

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

        /// <summary>
        /// Filtering visible and owned vehicle as per feature id 
        /// </summary>
        /// <param name="resultDict">Visibile vehicles received from account visibility</param>
        /// <param name="reportFeatureId">Report feature Id recieved from report click</param>
        /// <param name="contextOrgId">organizatin id</param>
        /// <returns></returns>
        private async Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> FilterVehiclesByfeaturesForAlert(Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict, int[] featureIds, int contextOrgId)
        {
            var vehicles = resultDict.Values.SelectMany(x => x).Distinct(new ObjectComparer()).ToList();

            if (vehicles.Count() > 0)
            {
                var vehiclePackages = await _visibilityRepository.GetSubscribedVehicleByFeatureForAlert(featureIds, contextOrgId);

                //Filter owned vehicles based on package features
                //If not found Org packages then filter vehicles based on subscribed vehicles
                var ownedVehicles = vehicles.Where(e => e.HasOwned == true);
                if (ownedVehicles.Count() > 0)
                {
                    if (vehiclePackages.Any(e => e.PackageType == "O"))
                    {
                        foreach (var vehicle in ownedVehicles)
                        {
                            vehicle.FeatureIds = new List<long>();
                            vehicle.FeatureIds.AddRange(vehiclePackages.Where(e => e.PackageType == "O").FirstOrDefault()?.FeatureIds ?? new long[] { });
                            vehicle.SubscriptionType = "O";
                        }
                    }

                    if (vehiclePackages.Any(e => (e.PackageType == "V" && e.HasOwned == true) || e.PackageType == "N")) // check if any v type and owned subscription available
                    {
                        //Step 1 - Take subscribed vehicle id org+vin
                        var subscriptionVehicleIds = vehiclePackages.Where(e => e.HasOwned == true && e.PackageType == "V").Select(e => e.Vehicle_Id);
                        var ownedVehiclesFromV = ownedVehicles.Where(x => subscriptionVehicleIds.Contains(x.Id));//v1, v2, v3

                        //Step 2 - Take vin type vehicle id vin
                        var vinPackageVehicleIds = vehiclePackages.Where(e => e.PackageType == "N").Select(e => e.Vehicle_Id);
                        var ownedVehiclesFromN = ownedVehicles.Where(x => vinPackageVehicleIds.Contains(x.Id));//v2, v4

                        //Step 3 - Union and assigned to owned vehicle
                        var ownedVehiclesFromNV = ownedVehiclesFromV.Union(ownedVehiclesFromN, new ObjectComparer());

                        ownedVehicles = ownedVehicles.Union(ownedVehiclesFromNV, new ObjectComparer());

                        //Step 4 - Assign subscribed features to owned vehicles
                        foreach (var vehicle in ownedVehicles)
                        {
                            vehicle.FeatureIds = vehicle.FeatureIds ?? new List<long>();
                            vehicle.FeatureIds = vehicle.FeatureIds.Union(vehiclePackages.Where(e => e.Vehicle_Id == vehicle.Id).SelectMany(x => x.FeatureIds)).ToList();
                            vehicle.SubscriptionType = ownedVehiclesFromNV.Any(e => e.Id == vehicle.Id) ? "V" : vehicle.SubscriptionType;
                        }

                        ownedVehicles = ownedVehicles.Where(x => x.FeatureIds != null && x.FeatureIds.Count() > 0);
                    }
                }

                //Filter visible vehicles based on org relationship features and package features
                var visibleVehicles = vehicles.Where(e => e.HasOwned == false).ToList();

                if (visibleVehicles.Count() > 0)
                {
                    //Fetch visible relationship vehicles of having reportFeatureId in it's allowed features list
                    //Intersect those vehicles with Org+VIN package subscribed vehicles where reportFeatureId is present in the subscription
                    //Filter vehicles out those are not in relationship vehicles and subscribed vehicles.
                    if (vehiclePackages.Any(e => (e.HasOwned == false && e.PackageType == "V") || e.PackageType == "N"))
                    {
                        //Step 1 - Take subscribed vehicle ids org+vin
                        var subscriptionVehicleIds = vehiclePackages.Where(e => e.HasOwned == false && e.PackageType == "V").Select(e => e.Vehicle_Id);

                        //Step 2 - Take relationship vehicle ids
                        var relationshipVehicles = await _visibilityRepository.GetRelationshipVehiclesByFeatureForAlert(featureIds, contextOrgId);
                        var relationshipVehicleIds = relationshipVehicles.Select(x => x.Vehicle_Id);

                        //Fetch vehicles records to be removed from visible vehicles list
                        var filteredVisibleVehicleIds = relationshipVehicleIds.Intersect(subscriptionVehicleIds);

                        //Step 3 - Take VIN type vehicle ids
                        var vinPackageVehicleIds = vehiclePackages.Where(e => e.PackageType == "N").Select(e => e.Vehicle_Id).ToList();

                        //Fetch vehicles records to be removed from visible vehicles list from Vin package
                        var filteredVinPackageVisibleVehicleIds = relationshipVehicleIds.Intersect(vinPackageVehicleIds);

                        //Step 4 - Union and assigned to visible vehicles
                        var visibleVehiclesFromVR = visibleVehicles.Where(x => filteredVisibleVehicleIds.Contains(x.Id));//v1, v2, v3
                        var visibleVehiclesFromN = visibleVehicles.Where(x => filteredVinPackageVisibleVehicleIds.Contains(x.Id));//v2, v4
                        visibleVehicles = visibleVehiclesFromVR.Union(visibleVehiclesFromN, new ObjectComparer()).ToList();

                        //Step 5 - Assign subscribed features to visible vehicles
                        foreach (var vehicle in visibleVehicles)
                        {
                            vehicle.FeatureIds = new List<long>();
                            vehicle.FeatureIds = vehiclePackages.Where(e => e.Vehicle_Id == vehicle.Id).SelectMany(x => x.FeatureIds)
                                                    .Intersect(relationshipVehicles.Where(e => e.Vehicle_Id == vehicle.Id).FirstOrDefault()?.FeatureIds ?? new long[] { }).ToList();
                            vehicle.SubscriptionType = "V";
                        }
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
                    resultDict[vg_kv.Key] = vehicles.Where(x => resultVehicles.Any(y => y.Id == x.Id)).ToList();
                }
            }

            return resultDict;
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

            return MapVehicleDetailsTemp(accountId, contextOrgId, resultDict);
        }

        public async Task<IEnumerable<VehicleDetailsAccountVisibilityForAlert>> GetVehicleByAccountVisibilityForAlert(int accountId, int orgId, int contextOrgId, int[] featureIds)
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
            resultDict = await FilterVehiclesByfeaturesForAlert(resultDict, featureIds, contextOrgId);

            return MapVehicleDetailsForAlert(accountId, contextOrgId, resultDict);
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

        public async Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> GetVisibilityVehicles(IEnumerable<int> vehicleGroupIds, int orgId)
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

        private static IEnumerable<VehicleDetailsAccountVisibilityForAlert> MapVehicleDetailsForAlert(int accountId, int contextOrgId, Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict)
        {
            List<VehicleDetailsAccountVisibilityForAlert> vehicleDetails = new List<VehicleDetailsAccountVisibilityForAlert>();
            foreach (var vg_kv in resultDict)
            {
                var vehicleGroup = vg_kv.Key;
                var visibleVehicles = vg_kv.Value;
                //if (!vehicleGroup.GroupType.Equals("S"))
                {
                    foreach (var vehicle in visibleVehicles)
                    {
                        vehicleDetails.Add(new VehicleDetailsAccountVisibilityForAlert
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
                            OrganizationId = contextOrgId,
                            SubscriptionType = vehicle.SubscriptionType,
                            FeatureIds = vehicle.FeatureIds
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
            }).Select(x => new VehicleDetailsAccountVisibilityForAlert
            {
                VehicleId = x.Key.VehicleId,
                Vin = x.Key.Vin,
                VehicleName = x.Key.VehicleName,
                RegistrationNo = x.Key.RegistrationNo,
                OrganizationId = x.Key.OrganizationId,
                VehicleGroupIds = x.Select(x => x.VehicleGroupId).ToArray(),
                VehicleGroupDetails = x.Select(x => $"{ x.VehicleGroupId }~{ x.VehicleGroupName }~{ x.GroupType }")
                .Aggregate((s1, s2) => s1 + "," + s2),
                SubscriptionType = x.Select(x => x.SubscriptionType).Any(x => x.Equals("V")) ? "V" : "O",
                FeatureIds = x.SelectMany(x => x.FeatureIds).Distinct().ToList()
            });
            return result;
        }

        private async Task<IEnumerable<VehicleDetailsAccountVisibilityForOTA>> MapVehicleDetailsForOTA(Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict, int adminFeatureId, int orgId, int contextOrgId)
        {
            List<VehicleDetailsAccountVisibilityForOTA> vehicleDetails = new List<VehicleDetailsAccountVisibilityForOTA>();

            foreach (var vg_kv in resultDict)
            {
                var vehicleGroup = vg_kv.Key;
                var visibleVehicles = vg_kv.Value;
                //if (!vehicleGroup.GroupType.Equals("S"))
                var vehiclePropertiesList = await _vehicleManager.GetVehiclePropertiesByIds(visibleVehicles.Select(s => s.Id).Distinct().ToArray());
                {
                    foreach (var vehicle in visibleVehicles)
                    {
                        var details = resultDict.Where(x => x.Value.Any(y => y.VIN.Equals(vehicle.VIN))).Select(x => x.Key);

                        //If context switch happens, make isAccessible true by default because Access Relationship does not come into picture.
                        var isAccessible = (orgId == contextOrgId) ? details.Any(x => x.AccessRelationType?.Equals("F") ?? false) : true;

                        vehicleDetails.Add(new VehicleDetailsAccountVisibilityForOTA
                        {
                            VehicleId = vehicle.Id,
                            Vin = vehicle.VIN,
                            VehicleName = vehicle.Name ?? string.Empty,
                            RegistrationNo = vehicle.RegistrationNo ?? string.Empty,
                            VehicleGroupName = vehicleGroup.Name,
                            ModelYear = vehiclePropertiesList.Where(w => w.VehicleId == vehicle.Id).FirstOrDefault().ModelYear ?? string.Empty,
                            Type = vehiclePropertiesList.Where(w => w.VehicleId == vehicle.Id).FirstOrDefault().Type ?? string.Empty,
                            HasAdminRights = vehicle.HasOwned ? (isAccessible && adminFeatureId > 0)
                                                              : (isAccessible && adminFeatureId > 0 && (vehicle.Btype_Features?.Any(x => x == adminFeatureId) ?? false))
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
                x.Type,
                x.HasAdminRights
            }).Select(x => new VehicleDetailsAccountVisibilityForOTA
            {
                VehicleId = x.Key.VehicleId,
                Vin = x.Key.Vin,
                VehicleName = x.Key.VehicleName,
                RegistrationNo = x.Key.RegistrationNo,
                ModelYear = x.Key.ModelYear,
                Type = x.Key.Type,
                HasAdminRights = x.Key.HasAdminRights,
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

        public async Task<IEnumerable<VehicleDetailsVisibiltyAndFeatureTemp>> GetSubscribedVehicleByAlertFeature(List<int> featureid, int organizationid)
        {
            return await _visibilityRepository.GetSubscribedVehicleByAlertFeature(featureid, organizationid);
        }

        public async Task<List<int>> GetAccountsForOTA(string vin)
        {
            return await _visibilityRepository.GetAccountsForOTA(vin);
        }
    }
}
