using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reportservice.entity;
using Newtonsoft.Json;
using ReportComponent = net.atos.daf.ct2.reports;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        //public override async Task<FuelBenchmarkResponse> GetFuelBenchmarks(FuelBenchmarkRequest request, ServerCallContext context)
        //{
        //    try
        //    {
        //        ReportComponent.entity.FuelBenchmark objFuelBenchmark = new ReportComponent.entity.FuelBenchmark
        //        {
        //            StartDate = request.StartDateTime,
        //            EndDate = request.EndDateTime,
        //            VIN = request.VINs.ToList()
        //        };
        //        var result = await _reportManager.GetFuelBenchmarks(objFuelBenchmark);
        //        FuelBenchmarkResponse response = new FuelBenchmarkResponse();
        //        if (result.Any())
        //        {
        //            string res = JsonConvert.SerializeObject(result);
        //            response.FuelBenchmarkDetails.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FuelBenchmarkDetails>>(res));
        //            response.Code = Responsecode.Success;
        //            response.Message = Responsecode.Success.ToString();
        //        }
        //        else
        //        {
        //            response.Code = Responsecode.NotFound;
        //            response.Message = ReportConstants.NORESULTFOUND_MSG;
        //        }
        //        return await Task.FromResult(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(null, ex);
        //        return await Task.FromResult(new FuelBenchmarkResponse
        //        {
        //            Code = Responsecode.Failed,
        //            Message = "Fuel benchmark details get failed due to - " + ex.Message
        //        });
        //    }

        //}


    }
}
