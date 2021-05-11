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

            obj.ModifiedBy = request.Modified_By;

            if (request != null && request.ViaAddressDetails != null)
            {
                foreach (var item in request.ViaAddressDetails)
                {
                    obj.ViaAddressDetails.Add(new ViaDetails() { ViaName = item.ViaRoutName, Latitude = item.Latitude, Longitude = item.Longitude });
                }
            }
            return obj;

        }
        public DeleteCorridorRequest MapId(DeleteCorridorIdRequest request)
        {
            var obj = new DeleteCorridorRequest();
            obj.CorridorID = request.Id;
            return obj;
        }
        string CheckNull(string param)
        {
            return !string.IsNullOrEmpty(param) ? param : string.Empty;
        }
        public ExistingTripCorridorRequest MapExistingTripCorridorRequest(ExistingTripCorridor request)
        {
            var ExistingTripCorridorRequest = new ExistingTripCorridorRequest()
            {

                OrganizationId = request.OrganizationId != null ? request.OrganizationId.Value : 0,
                CorridorType = request.CorridorType != null ? request.CorridorType : "E",
                CorridorLabel = CheckNull(request.CorridorLabel),
                Address = CheckNull(request.Address),
                StartLatitude = request.StartLatitude,
                StartLongitude = request.StartLongitude,
                Width = request.Width,
                Distance = request.Distance,
                City = CheckNull(request.City),
                Country = CheckNull(request.Country),
                Description = CheckNull(request.Description),
                Zipcode = CheckNull(request.Zipcode),
                CreatedBy = request.CreatedBy,
                State = "A"
            };
            foreach (var trip in request.ExistingTrips)
            {
                var existingTrip = new corridorservice.ExistingTrip()
                {
                    Distance = trip.Distance,
                    DriverId1 = CheckNull(trip.DriverId1),
                    DriverId2 = CheckNull(trip.DriverId2),
                    EndDate = trip.EndDate,
                    EndLatitude = trip.EndLatitude,
                    EndPosition = trip.EndPosition,
                    EndLongitude = trip.EndLongitude,
                    StartDate = trip.StartDate,
                    StartPosition = trip.StartPosition,
                    StartLatitude = trip.StartLatitude,
                    StartLongitude = trip.StartLongitude,
                    TripId = CheckNull(trip.TripId)
                };

                foreach (var node in trip.NodePoints) {
                    var tripNode = new TripNodes()
                    {
                        Latitude = node.Latitude,
                        TripId = node.TripId,
                        Address = CheckNull(node.Address),
                        CreatedBy = node.CreatedBy,
                        Longitude = node.Longitude,
                        SequenceNumber = node.SequenceNumber,
                        State = "A"

                    };
                    existingTrip.NodePoints.Add(tripNode);
                }

                ExistingTripCorridorRequest.ExistingTrips.Add(existingTrip);

            }

            return ExistingTripCorridorRequest;


        }

    }
}
