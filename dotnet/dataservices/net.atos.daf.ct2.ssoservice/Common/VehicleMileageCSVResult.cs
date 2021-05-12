using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.vehicle.entity;
//using net.atos.daf.ct2.vehicledataservice.Entity;
namespace net.atos.daf.ct2.singlesignonservice.Common
{
    public class VehicleMileageCSVResult : ContentResult //FileResult
    {
        private readonly IEnumerable<VehiclesCSV> _mileageData;
        public VehicleMileageCSVResult(IEnumerable<VehiclesCSV> mileageData) //: base("text/csv") //, string fileDownloadName
        {
            _mileageData= mileageData;
            //FileDownloadName = fileDownloadName;
        }
        public async override Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            //context.HttpContext.Response.Headers.Add("Content-Disposition", new[] { "attachment; filename=" + FileDownloadName });
            using (var streamWriter = new StreamWriter(response.Body)) {
              await streamWriter.WriteLineAsync(
                $"EvtDateTime;VIN;TachoMileage;RealMileage;RealMileageAlgorithmVersion"
              );
              foreach (var m in _mileageData)
              {
                await streamWriter.WriteLineAsync(
                  $"{m.EvtDateTime};{m.VIN};{m.TachoMileage};{m.RealMileage};{m.RealMileageAlgorithmVersion}"
                );
                await streamWriter.FlushAsync();
              }
              await streamWriter.FlushAsync();
            }
        }
    }
}
