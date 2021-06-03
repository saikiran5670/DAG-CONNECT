using System.Linq;
using net.atos.daf.ct2.corridorservice;
using net.atos.daf.ct2.poigeofence.entity;

namespace net.atos.daf.ct2.poigeofenceservice.entity
{
    public class CorridorMapper
    {

        public ExistingTripCorridor ToExistingTripCorridorEntity(ExistingTripCorridorRequest request)
        {
            var etcObj = new poigeofence.entity.ExistingTripCorridor()
            {

                Id = request.Id,
                OrganizationId = request.OrganizationId,
                CorridorType = request.CorridorType,// Convert.ToChar(request.CorridorType);
                CorridorLabel = request.CorridorLabel,
                Address = request.Address,
                StartLatitude = request.StartLatitude,
                StartLongitude = request.StartLongitude,
                Width = request.Width,
                Distance = request.Distance,
                City = request.City,
                Country = request.Country,
                Description = request.Description,
                Zipcode = request.Zipcode,
                CreatedBy = request.CreatedBy,
                State = "A",
                ExistingTrips = request.ExistingTrips.Select(x => new poigeofence.entity.ExistingTrip()
                {
                    Distance = x.Distance,
                    DriverId1 = x.DriverId1,
                    DriverId2 = x.DriverId2,
                    EndDate = x.EndDate,
                    EndLatitude = x.EndLatitude,
                    EndPosition = x.EndPosition,
                    EndLongitude = x.EndLongitude,
                    StartDate = x.StartDate,
                    StartPosition = x.StartPosition,
                    StartLatitude = x.StartLatitude,
                    StartLongitude = x.StartLongitude,
                    TripId = x.TripId,
                    NodePoints = x.NodePoints.Select(node => new poigeofence.entity.Nodepoint()
                    {
                        Latitude = node.Latitude,
                        TripId = x.TripId,
                        Address = node.Address,
                        CreatedBy = node.CreatedBy,
                        Longitude = node.Longitude,
                        SequenceNumber = node.SequenceNumber,
                        State = "A"
                    }).ToList()
                }).ToList()
            };
            return etcObj;


        }



    }
}
