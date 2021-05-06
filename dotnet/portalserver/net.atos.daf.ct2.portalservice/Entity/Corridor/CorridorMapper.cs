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
            obj.Distance = request.Distance;
            obj.Trailer = request.attribute.IsTrailer;
            obj.IsTransportData = request.TransportData;
            obj.IsTrafficFlow = request.TrafficFlow;


            obj.IsExplosive = request.attribute.IsExplosive;
            obj.IsGas = request.attribute.IsGas;
            obj.IsFlammable = request.attribute.IsFlammable;
            obj.IsCombustible = request.attribute.IsCombustible;
            obj.Isorganic = request.attribute.Isorganic;
            obj.Ispoision = request.attribute.Ispoision;
            obj.IsRadioActive = request.attribute.IsRadioActive;
            obj.IsCorrosive = request.attribute.IsCorrosive;
            obj.IsPoisonousInhalation = request.attribute.IsPoisonousInhalation;


            obj.IsWaterHarm = request.attribute.IsWaterHarm;
            obj.IsOther = request.attribute.IsOther;
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
