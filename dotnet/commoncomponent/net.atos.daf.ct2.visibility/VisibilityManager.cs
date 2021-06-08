using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.visibility.entity;
using net.atos.daf.ct2.visibility.repository;

namespace net.atos.daf.ct2.visibility
{
    public class VisibilityManager : IVisibilityManager
    {
        private readonly IVisibilityRepository _visibilityRepository;

        public VisibilityManager(IVisibilityRepository visibilityRepository)
        {
            _visibilityRepository = visibilityRepository;
        }

        public Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibility(int accountId, int organizationId) => _visibilityRepository.GetVehicleByAccountVisibility(accountId, organizationId);
        public Task<IEnumerable<VehicleDetailsFeatureAndSubsction>> GetVehicleByFeatureAndSubscription(int accountId, int organizationId, int roleId,
                                                                                                string featureName = "Alert") => _visibilityRepository.GetVehicleByFeatureAndSubscription(accountId, organizationId, roleId, featureName);

        public async Task<IEnumerable<VehicleDetailsVisibiltyAndFeature>> GetVehicleByVisibilityAndFeature(int accountId, int organizationId, int roleId,
                                                                                                           string featureName = "Alert")
        {
            try
            {
                var vehicleByVisibilityAndFeature = new List<VehicleDetailsVisibiltyAndFeature>();
                var vehicleByVisibility = await GetVehicleByAccountVisibility(accountId, organizationId);

                if (!vehicleByVisibility.Any())
                {
                    return await Task.FromResult(new List<VehicleDetailsVisibiltyAndFeature>());
                }

                var vehicleByFeature = await GetVehicleByFeatureAndSubscription(accountId, organizationId, roleId, featureName);

                if (!vehicleByFeature.Any())
                {
                    foreach (var item in vehicleByVisibility)
                    {
                        vehicleByVisibilityAndFeature.Add(new VehicleDetailsVisibiltyAndFeature
                        {
                            VehicleGroupId = item.VehicleGroupId,
                            VehicleGroupName = item.VehicleGroupName,
                            VehicleId = item.VehicleId,
                            VehicleName = item.VehicleName,
                            Vin = item.Vin,
                            RegistrationNo = item.RegistrationNo,
                            FeatureName = string.Empty,
                            FeatureKey = string.Empty,
                            Subscribe = false
                        });
                    }
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
                                    VehicleGroupId = item.VehicleGroupId,
                                    VehicleGroupName = item.VehicleGroupName,
                                    VehicleId = item.VehicleId,
                                    VehicleName = item.VehicleName,
                                    Vin = item.Vin,
                                    RegistrationNo = item.RegistrationNo,
                                    FeatureName = feature.Name,
                                    FeatureKey = feature.FeatureEnum,
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
                                        VehicleGroupId = item.VehicleGroupId,
                                        VehicleGroupName = item.VehicleGroupName,
                                        VehicleId = item.VehicleId,
                                        VehicleName = item.VehicleName,
                                        Vin = item.Vin,
                                        RegistrationNo = item.RegistrationNo,
                                        FeatureName = feature.Name,
                                        FeatureKey = feature.FeatureEnum,
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
    }
}
