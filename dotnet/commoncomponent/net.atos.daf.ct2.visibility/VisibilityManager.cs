using System.Collections.Generic;
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

        public async Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibility(int accountId, int orgId, int contextOrgId)
        {
            List<VisibilityVehicle> vehicles;
            //If context switched then find vehicle visibility for the organization
            if (orgId != contextOrgId)
            {
                vehicles = await _vehicleManager.GetVisibilityVehiclesByOrganization(contextOrgId);
            }
            else
            {
                vehicles = await _vehicleManager.GetVisibilityVehicles(accountId, orgId);
            }
            // vehicle filtering based on features
            // owned vehicle filtering
            //feature id condition 
            if (false)
            {
                var subscriptionVehicle = await GetSubcribedVehicleByFeature(30, orgId);
                if (!subscriptionVehicle.Any(e => e.PackageType == 'O'))
                {
                    // getting owned + subscribed vehicle fro visible vehicle list
                    var vehicleVisible = vehicles.Where(e => e.HasOwned = true
                     && subscriptionVehicle.FirstOrDefault().VehicleIds.Contains(e.Id)).Select(k => k.Id);
                    //Removing other vins from owned
                    vehicles.RemoveAll(e => e.HasOwned = true && !vehicleVisible.Contains(e.Id));
                }

            }
            // visible vehicle filtering - todo
            return await _visibilityRepository.GetVehicleVisibilityDetails(vehicles.Select(x => x.Id).ToArray(), accountId);
        }

        public async Task<IEnumerable<VehiclePackage>> GetSubcribedVehicleByFeature(int featureid, int organizationid)
        {
            return await _visibilityRepository.GetSubcribedVehicleByFeature(featureid, organizationid);
        }

        public async Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibilityTemp(int accountId, int orgId, int contextOrgId)
        {
            List<VisibilityVehicle> vehicles;
            //If context switched then find vehicle visibility for the organization
            if (orgId != contextOrgId)
            {
                vehicles = await _vehicleManager.GetVisibilityVehiclesByOrganization(contextOrgId);
            }
            else
            {
                vehicles = await _vehicleManager.GetVisibilityVehicles(accountId, orgId);
            }

            return await _visibilityRepository.GetVehicleVisibilityDetailsTemp(vehicles.Select(x => x.Id).ToArray());
        }

        public Task<IEnumerable<VehicleDetailsFeatureAndSubsction>> GetVehicleByFeatureAndSubscription(int accountId, int orgId, int contextOrgId, int roleId,
                                                                                                string featureName = "Alert") => _visibilityRepository.GetVehicleByFeatureAndSubscription(accountId, orgId, contextOrgId, roleId, featureName);

        public async Task<IEnumerable<VehicleDetailsVisibiltyAndFeature>> GetVehicleByVisibilityAndFeature(int accountId, int orgId, int contextOrgId, int roleId,
                                                                                                           IEnumerable<VehicleDetailsAccountVisibilty> vehicleDetailsAccountVisibilty, string featureName = "Alert")
        {
            try
            {
                var vehicleByVisibilityAndFeature = new List<VehicleDetailsVisibiltyAndFeature>();
                var vehicleByVisibility = vehicleDetailsAccountVisibilty ?? await GetVehicleByAccountVisibility(accountId, orgId, contextOrgId);

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
    }
}
