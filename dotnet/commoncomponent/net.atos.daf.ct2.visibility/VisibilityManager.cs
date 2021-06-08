using System.Collections.Generic;
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

        public Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibility(int accountId, int OrganizationId)
        {
            return _visibilityRepository.GetVehicleByAccountVisibility(accountId, OrganizationId);
        }
    }
}
