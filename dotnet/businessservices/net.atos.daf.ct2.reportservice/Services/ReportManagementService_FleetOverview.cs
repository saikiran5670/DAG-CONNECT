﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reportservice.entity;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        #region Fleet Overview 

        public override async Task<FleetOverviewFilterResponse> GetFleetOverviewFilter(FleetOverviewFilterIdRequest request, ServerCallContext context)
        {
            try
            {
                var response = new FleetOverviewFilterResponse();
                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);

                if (vehicleDetailsAccountVisibilty.Any())
                {

                    var res = JsonConvert.SerializeObject(vehicleDetailsAccountVisibilty);
                    response.AssociatedVehicleRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AssociatedVehicleRequest>>(res)
                        );

                    var vehicleByVisibilityAndFeature
                                                = await _visibilityManager
                                                    .GetVehicleByVisibilityAndFeature(request.AccountId, request.OrganizationId,
                                                                                       request.RoleId, vehicleDetailsAccountVisibilty,
                                                                                       ReportConstants.FLEETOVERVIEW_FEATURE_NAME);

                    res = JsonConvert.SerializeObject(vehicleByVisibilityAndFeature);
                    response.FleetOverviewVGFilterResponse.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetOverviewVGFilterRequest>>(res)
                        );
                }

                var alertLevel = await _reportManager.GetAlertLevelList();
                var resalertLevel = JsonConvert.SerializeObject(alertLevel);
                response.ALFilterResponse.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FilterResponse>>(resalertLevel)
                    );

                var alertCategory = await _reportManager.GetAlertCategoryList();
                var resAlertCategory = JsonConvert.SerializeObject(alertCategory);
                response.ACFilterResponse.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AlertCategoryFilterResponse>>(resAlertCategory)
                    );

                var healthStatus = await _reportManager.GetHealthStatusList();
                var resHealthStatus = JsonConvert.SerializeObject(healthStatus);
                response.HSFilterResponse.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FilterResponse>>(resHealthStatus)
                    );

                var otherFilter = await _reportManager.GetOtherFilter();
                var resOtherFilter = JsonConvert.SerializeObject(otherFilter);
                response.OFilterResponse.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FilterResponse>>(resOtherFilter)
                    );

                response.Message = ReportConstants.FLEETOVERVIEW_FILTER_SUCCESS_MSG;
                response.Code = Responsecode.Success;
                _logger.Info("Get method in report service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new FleetOverviewFilterResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = ex.Message
                });
            }
        }

        #endregion 

    }
}
