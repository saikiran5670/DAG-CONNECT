using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using net.atos.daf.ct2.vehicle.entity;
namespace net.atos.daf.ct2.vehicledataservice.Common
{
    public class VehicleMileageCSVResult : ContentResult
    {
        private readonly IEnumerable<DtoVehicleMileage> _mileageData;
        public VehicleMileageCSVResult(IEnumerable<DtoVehicleMileage> mileageData)
        {
            _mileageData = mileageData;
        }
        public async override Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            using (var streamWriter = new StreamWriter(response.Body))
            {
                await streamWriter.WriteLineAsync(
                  $"EvtDateTime;VIN;TachoMileage;RealMileage;RealMileageAlgorithmVersion"
                );
                foreach (var m in _mileageData)
                {
                    await streamWriter.WriteLineAsync(
                      $"{m.EvtDateTime};{m.VIN};{m.TachoMileage};{m.GPSMileage};{m.RealMileageAlgorithmVersion}"
                    );
                    await streamWriter.FlushAsync();
                }
                await streamWriter.FlushAsync();
            }
        }
    }
}
