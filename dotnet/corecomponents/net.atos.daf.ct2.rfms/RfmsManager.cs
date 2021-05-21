using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.repository;
using net.atos.daf.ct2.rfms.responce;

namespace net.atos.daf.ct2.rfms
{
    public class RfmsManager : IRfmsManager
    {
        IRfmsRepository rfmsRepository;
        IAuditTraillib auditlog;

        public RfmsManager(IRfmsRepository _rfmsRepository, IAuditTraillib _auditlog)
        {
            rfmsRepository = _rfmsRepository;
            auditlog = _auditlog;
        }

         public async Task<RfmsVehicles> Get(RfmsVehicleRequest rfmsVehicleRequest)
        {
            return await rfmsRepository.Get(rfmsVehicleRequest);
        }

         public async Task<RfmsVehiclePositionRequest> Get(RfmsVehiclePositionRequest rfmsVehiclePositionRequest)
        {

            return await rfmsRepository.Get(rfmsVehiclePositionRequest);
        }

    }
}
