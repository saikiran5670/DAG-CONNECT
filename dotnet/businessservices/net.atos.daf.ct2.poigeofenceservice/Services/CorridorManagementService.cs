﻿using Grpc.Core;
using log4net;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.poigeofence;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofenceservice
{
    public class CorridorManagementService: CorridorService.CorridorServiceBase
    {

        private ILog _logger;
        private readonly ICorridorManger _corridorManger;
        public CorridorManagementService(ICorridorManger corridorManger)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _corridorManger = corridorManger;

        }

        public override async Task<CorridorResponseList> GetCorridorList(CorridorRequest request, ServerCallContext context)
        {
            try
            {
                CorridorResponseList objCorridorResponseList = new CorridorResponseList();
                net.atos.daf.ct2.poigeofence.entity.CorridorRequest obj = new poigeofence.entity.CorridorRequest();
                obj.OrganizationId = request.OrganizationId;
                obj.CorridorId = request.CorridorId;
                var data = await _corridorManger.GetCorridorList(obj);
                foreach (var item in data)
                {
                    CorridorResponse objCorridorResponse = new CorridorResponse();
                    objCorridorResponse.Id = item.Id;
                    objCorridorResponse.OrganizationId = item.OrganizationId;
                    objCorridorResponse.CorridoreName = CheckNull(item.CorridoreName);
                    objCorridorResponse.StartPoint = CheckNull(item.StartPoint);
                    objCorridorResponse.StartLat = item.StartLat;
                    objCorridorResponse.StartLong = item.StartLong;
                    objCorridorResponse.EndPoint = CheckNull(item.EndPoint);
                    objCorridorResponse.EndLat = item.EndLat;
                    objCorridorResponse.EndLong = item.EndLong;
                    objCorridorResponse.Distance = item.Distance;
                    objCorridorResponse.Width = item.Width;
                    objCorridorResponse.CreatedAt = item.CreatedAt;
                    objCorridorResponse.CreatedBy = item.CreatedBy;
                    objCorridorResponse.ModifiedAt = item.ModifiedAt;
                    objCorridorResponse.ModifiedBy = item.ModifiedBy;
                    objCorridorResponseList.CorridorList.Add(objCorridorResponse);
                }
                objCorridorResponseList.Message = "CorridorList data retrieved";
                objCorridorResponseList.Code = Responsecode.Success;
                _logger.Info("GetCorridorList method in CorridorManagement service called.");
                return await Task.FromResult(objCorridorResponseList);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new CorridorResponseList
                {
                    Code = Responsecode.Failed,
                    Message = $"Exception while retrieving data from GetCorridorList : {ex.Message}"
                });
            }
        }

        string CheckNull(string value)
        {
            return string.IsNullOrEmpty(value) == true ? string.Empty : value;
        }

    }
}
