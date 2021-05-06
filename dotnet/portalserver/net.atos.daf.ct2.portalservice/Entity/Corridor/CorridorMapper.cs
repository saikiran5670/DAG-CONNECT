using net.atos.daf.ct2.corridorservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.portalservice.Entity.Corridor
{
    public class CorridorMapper
    {

        public RouteCorridorAddRequest MapCorridor(CorridorRequest request)
        {
            var mapvia = new ViaDetails();
            var obj = new RouteCorridorAddRequest();
            obj.Id = request.Id;
            obj.OrganizationId = request.OrganizationId;
            obj.CorridorType = Convert.ToString(request.CorridorType);
            obj.CorridorLabel = request.CorridorLabel;
            obj.StartAddress = request.StartAddress;
            obj.StartLatitude = request.StartLatitude;
            obj.StartLongitude = request.StartLongitude;


            obj.EndAddress = request.EndAddress;
            obj.EndLatitude = request.EndLatitude;
            obj.EndLongitude = request.EndLongitude;
            obj.Width = request.Width;
            obj.Trailer = request.attribute.Trailer;
            obj.TransportData = request.TransportData;
            obj.TrafficFlow = request.TrafficFlow;


            obj.Explosive = request.attribute.Explosive;
            obj.Gas = request.attribute.Gas;
            obj.Flammable = request.attribute.Flammable;
            obj.Combustible = request.attribute.Combustible;
            obj.Organic = request.attribute.organic;
            obj.Poision = request.attribute.poision;
            obj.RadioActive = request.attribute.RadioActive;
            obj.Corrosive = request.attribute.Corrosive;
            obj.PoisonousInhalation = request.attribute.PoisonousInhalation;


            obj.WaterHarm = request.attribute.WaterHarm;
            obj.Other = request.attribute.Other;
            obj.TollRoad = Convert.ToString(request.exclusion.TollRoad);
            obj.Mortorway = Convert.ToString(request.exclusion.Mortorway);
            obj.BoatFerries = Convert.ToString(request.exclusion.BoatFerries);
            obj.RailFerries = Convert.ToString(request.exclusion.RailFerries);
            obj.Tunnels = Convert.ToString(request.exclusion.Tunnels);
            obj.DirtRoad = Convert.ToString(request.exclusion.DirtRoad);


            obj.VehicleSizeHeight = request.vehicleSize.VehicleSizeHeight;
            obj.VehicleSizeWidth = request.vehicleSize.VehicleSizeWidth;
            obj.VehicleSizeLength = request.vehicleSize.VehicleSizeLength;
            obj.VehicleSizeLimitedWeight = request.vehicleSize.VehicleSizeLimitedWeight;
            obj.VehicleSizeWeightPerAxle = request.vehicleSize.VehicleSizeWeightPerAxle;

           
            if (request != null && request.ViaAddressDetails != null)
            {
                foreach (var item in request.ViaAddressDetails)
                {
                    obj.ViaAddressDetails.Add(new ViaDetails() { ViaName = item.ViaRoutName, Latitude = item.Latitude,Longitude=item.Longitude });
                }
            }
            return obj;

        }
    }
}
