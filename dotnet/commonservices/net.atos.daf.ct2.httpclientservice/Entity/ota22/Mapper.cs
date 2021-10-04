namespace net.atos.daf.ct2.httpclientservice.Entity.ota22
{
    public class Mapper
    {
        public VehiclesStatusOverviewResponse MapVehiclesStatusOverview(net.atos.daf.ct2.httpclientfactory.entity.ota22.VehiclesStatusOverviewResponse vehiclesStatusOverviewResponses)
        {
            var returnObj = new VehiclesStatusOverviewResponse();
            returnObj.HttpStatusCode = vehiclesStatusOverviewResponses.HttpStatusCode;
            returnObj.VehiclesStatusOverview = new VehiclesStatusOverview();
            if (vehiclesStatusOverviewResponses?.VehiclesStatusOverview?.Results != null)
            {
                foreach (var item in vehiclesStatusOverviewResponses?.VehiclesStatusOverview?.Results)
                {
                    returnObj.VehiclesStatusOverview
                        .VehiclesStatusOverviewResults
                        .Add(new VehiclesStatusOverviewResults
                        {
                            Vin = item.Vin ?? string.Empty,
                            Status = item.Status ?? string.Empty,
                            Description = item.Description ?? string.Empty
                        });
                }
            }
            return returnObj;
        }

        public net.atos.daf.ct2.httpclientfactory.entity.ota22.VehiclesStatusOverviewRequest MapVehiclesStatusOverviewRequest(VehiclesStatusOverviewRequest vehiclesStatusOverviewRequest)
        {
            var returnObj = new net.atos.daf.ct2.httpclientfactory.entity.ota22.VehiclesStatusOverviewRequest();
            returnObj.Language = vehiclesStatusOverviewRequest.Language;
            returnObj.Retention = vehiclesStatusOverviewRequest.Retention;
            returnObj.Vins = new System.Collections.Generic.List<string>();
            foreach (var item in vehiclesStatusOverviewRequest.Vins)
            {
                returnObj.Vins.Add(item);
            }
            return returnObj;
        }
    }
}
