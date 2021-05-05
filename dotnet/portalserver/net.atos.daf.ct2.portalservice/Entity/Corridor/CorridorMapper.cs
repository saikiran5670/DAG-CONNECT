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

            var obj = new RouteCorridorAddRequest();
            obj.Id = request.Id;
            obj.OrganizationId = request.OrganizationId;
            obj.CorridorType = Convert.ToString(request.CorridorType);
            obj.CorridorLabel = request.CorridorLabel;
            obj.StartAddress = request.StartAddress;
            obj.EndAddress = request.EndAddress;
            obj.Width = request.Width;
            obj.Trailer = request.Trailer;
            obj.TransportData = request.TransportData;
            obj.TrafficFlow = request.TrafficFlow;


            obj.Explosive = request.Explosive;
            obj.Gas = request.Gas;
            obj.Flammable = request.Flammable;
            obj.Combustible = request.Combustible;
            obj.Organic = request.organic;
            obj.Poision = request.poision;
            obj.RadioActive = request.RadioActive;
            obj.Corrosive = request.Corrosive;
            obj.PoisonousInhalation = request.PoisonousInhalation;


            obj.WaterHarm = request.WaterHarm;
            obj.Other = request.Other;
            obj.TollRoad = Convert.ToString(request.TollRoad);
            obj.Mortorway = Convert.ToString(request.Mortorway);
            obj.BoatFerries = Convert.ToString(request.BoatFerries);
            obj.RailFerries = Convert.ToString(request.RailFerries);
            obj.Tunnels = Convert.ToString(request.Tunnels);
            obj.DirtRoad = Convert.ToString(request.DirtRoad);
            obj.VehicleSizeHeight = request.VehicleSizeHeight;


            obj.VehicleSizeWidth = request.VehicleSizeWidth;
            obj.VehicleSizeLength = request.VehicleSizeLength;
            obj.VehicleSizeLimitedWeight = request.VehicleSizeLimitedWeight;
            obj.VehicleSizeWeightPerAxle = request.VehicleSizeWeightPerAxle;

            return obj;

        }
    }
}
