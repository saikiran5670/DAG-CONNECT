using System;
using net.atos.daf.ct2.corridorservice;

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
            obj.Trailer = request.Attribute.IsTrailer;
            obj.IsTransportData = request.TransportData;
            obj.IsTrafficFlow = request.TrafficFlow;

            obj.IsExplosive = request.Attribute.IsExplosive;
            obj.IsGas = request.Attribute.IsGas;
            obj.IsFlammable = request.Attribute.IsFlammable;
            obj.IsCombustible = request.Attribute.IsCombustible;
            obj.Isorganic = request.Attribute.Isorganic;
            obj.Ispoision = request.Attribute.Ispoision;
            obj.IsRadioActive = request.Attribute.IsRadioActive;
            obj.IsCorrosive = request.Attribute.IsCorrosive;
            obj.IsPoisonousInhalation = request.Attribute.IsPoisonousInhalation;


            obj.IsWaterHarm = request.Attribute.IsWaterHarm;
            obj.IsOther = request.Attribute.IsOther;
            obj.TollRoad = Convert.ToString(request.Exclusion.TollRoad);
            obj.Mortorway = Convert.ToString(request.Exclusion.Mortorway);
            obj.BoatFerries = Convert.ToString(request.Exclusion.BoatFerries);
            obj.RailFerries = Convert.ToString(request.Exclusion.RailFerries);
            obj.Tunnels = Convert.ToString(request.Exclusion.Tunnels);
            obj.DirtRoad = Convert.ToString(request.Exclusion.DirtRoad);


            obj.VehicleSizeHeight = request.VehicleSize.VehicleSizeHeight;
            obj.VehicleSizeWidth = request.VehicleSize.VehicleSizeWidth;
            obj.VehicleSizeLength = request.VehicleSize.VehicleSizeLength;
            obj.VehicleSizeLimitedWeight = request.VehicleSize.VehicleSizeLimitedWeight;
            obj.VehicleSizeWeightPerAxle = request.VehicleSize.VehicleSizeWeightPerAxle;

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
                Id = request.Id,
                OrganizationId = request.OrganizationId != null ? request.OrganizationId.Value : 0,
                CorridorType = request.CorridorType ?? "E",
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

                foreach (var node in trip.NodePoints)
                {
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
