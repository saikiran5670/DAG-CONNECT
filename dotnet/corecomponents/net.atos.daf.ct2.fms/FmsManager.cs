﻿using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.fms.entity;
using net.atos.daf.ct2.fms.repository;

namespace net.atos.daf.ct2.rfms
{
    public class FmsManager : IFmsManager
    {
        readonly IFmsRepository _fmsRepository;
        public FmsManager(IFmsRepository fmsRepository)
        {
            _fmsRepository = fmsRepository;
        }
        public Task<List<VehiclePositionResponse>> GetVehiclePosition(string vin, string since)
        {
            return _fmsRepository.GetVehiclePosition(vin, since);
        }
        public Task<List<VehiclePositionResponse>> GetVehicleStatus(string vin, string since)
        {
            return _fmsRepository.GetVehiclePosition(vin, since);
        }

    }
}
