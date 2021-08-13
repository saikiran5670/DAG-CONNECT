﻿using System.Threading.Tasks;
using net.atos.daf.ct2.rfms.entity;
using net.atos.daf.ct2.rfms.response;

namespace net.atos.daf.ct2.rfms
{
    public interface IRfmsManager
    {
        Task<RfmsVehicles> GetVehicles(string lastVin, int threshold, int accountId, int orgId);
        Task<RfmsVehiclePosition> GetVehiclePosition(RfmsVehiclePositionRequest rfmsVehiclePositionRequest);
        Task<string> GetRFMSFeatureRate(string emailId, string featureName);
        Task<RfmsVehicleStatus> GetRfmsVehicleStatus(RfmsVehicleStatusRequest rfmsVehicleStatusRequest);
    }
}