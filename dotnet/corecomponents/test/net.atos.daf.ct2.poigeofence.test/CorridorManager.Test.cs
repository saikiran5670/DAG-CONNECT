using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;

namespace net.atos.daf.ct2.poigeofence.test
{
    [TestClass]
    public class CorridorManagerTest
    {
        private readonly IConfiguration _config;
        private readonly IDataAccess _dataAccess;
        private readonly IDataMartDataAccess _iDataMartDataAccess;
        private readonly CorridorRepository _corridorRepository;
        private readonly ICorridorManger _iCorridorManger;

        public CorridorManagerTest()
        {
            _config = new ConfigurationBuilder().AddJsonFile("appsettings.Test.json")
                                                .Build();
            string connectionString = _config.GetConnectionString("DevAzure");
            string dataMartConnectionString = string.Empty;
            _dataAccess = new PgSQLDataAccess(connectionString);
            _iDataMartDataAccess = new PgSQLDataMartDataAccess(dataMartConnectionString);
            _corridorRepository = new CorridorRepository(_dataAccess, _iDataMartDataAccess);//respolved param missing error
            _iCorridorManger = new CorridorManger(_corridorRepository);
        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Corridor List By OrgId details using filter")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetCorridorListByOrgId()
        {
            CorridorRequest objCorridorRequest = new CorridorRequest();
            objCorridorRequest.OrganizationId = 100;//orgid 5 ,100

            var resultCorridorList = _iCorridorManger.GetCorridorList(objCorridorRequest).Result;
            Assert.IsNotNull(resultCorridorList);
            Assert.IsTrue(resultCorridorList.GridView.Count > 0);

        }
        [TestCategory("Unit-Test-Case")]
        [Description("Test for Get Corridor List By OrgId and CorriId details using filter")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void GetCorridorListByOrgIdandCorriId()
        {
            CorridorRequest objCorridorRequest = new CorridorRequest();
            objCorridorRequest.OrganizationId = 100;//orgid 5 ,100
            objCorridorRequest.CorridorId = 172; //landmark table id 109, 172

            var resultCorridorList = _iCorridorManger.GetCorridorList(objCorridorRequest).Result;
            Assert.IsNotNull(resultCorridorList);
            Assert.IsTrue(resultCorridorList.EditView != null);

        }


        [TestCategory("Unit-Test-Case")]
        [Description("Test to create ExistingTripCorridor ")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void AddExistingTripCorridorTest()
        {
            var existingTripCorridor = new ExistingTripCorridor()
            {
                Address = "Pune",
                //  CategoryId = 10,
                City = "Pune",
                Country = "India",
                Distance = 12,
                StartLatitude = 51.07,
                StartLongitude = 57.07,
                CorridorLabel = "trip Test9",
                State = "A",
                // ModifiedAt =,
                //  ModifiedBy =,
                OrganizationId = 100,
                // SubCategoryId = 8,
                //TripId = 10,
                CorridorType = "E",
                Zipcode = "411057",
                CreatedBy = 1,
                ExistingTrips = new List<ExistingTrip>() {
                                    new ExistingTrip() {
                                        Distance=10, DriverId1="d1", DriverId2="d2", EndDate=121323, EndLatitude=51.07,
                                        EndPosition="End address", EndLongitude=51.07,
                                        StartDate=11313, StartPosition="start address",
                                        StartLatitude=43.34, StartLongitude=12.34, TripId="trip13",
                                        NodePoints=new List<Nodepoint>()
                                                       { new Nodepoint()
                                                             { Latitude=12.34, TripId="trip13", Address="Node address",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             }
                                                       }
                                    },
                                    new ExistingTrip() {
                                        Distance=16, DriverId1="d2", DriverId2="d2", EndDate=121323, EndLatitude=51.07,
                                        EndPosition="End address", EndLongitude=51.07,
                                        StartDate=11313, StartPosition="start address",
                                        StartLatitude=43.34, StartLongitude=12.34, TripId="trip11",
                                        NodePoints=new List<Nodepoint>()
                                                       { new Nodepoint()
                                                             { Latitude=12.34, TripId="trip11", Address="Node address11",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             },
                                                             new Nodepoint()
                                                             { Latitude=12.34, TripId="trip11", Address="Node address11",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             }
                                                       }
                                    },
                                    new ExistingTrip() {
                                        Distance=13, DriverId1="d3", DriverId2="d2", EndDate=121323, EndLatitude=51.07,
                                        EndPosition="End address", EndLongitude=51.07,
                                        StartDate=11313, StartPosition="start address",
                                        StartLatitude=43.34, StartLongitude=12.34, TripId="trip12",
                                        NodePoints=new List<Nodepoint>()
                                                       { new Nodepoint()
                                                             { Latitude=12.34, TripId="trip12", Address="Node address12",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             },
                                                             new Nodepoint()
                                                             { Latitude=12.34, TripId="trip12", Address="Node address122",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             },
                                                             new Nodepoint()
                                                             { Latitude=12.34, TripId="trip12", Address="Node address123",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             }
                                                       }
                                    }
                                },

            };
            // objCorridorRequest.OrganizationId = 100;//orgid 5 ,100
            // objCorridorRequest.CorridorId = 172; //landmark table id 109, 172

            var resultCorridorList = _iCorridorManger.AddExistingTripCorridor(existingTripCorridor).Result;
            Assert.IsNotNull(resultCorridorList);
            Assert.IsTrue(resultCorridorList.ExistingTrips.Count > 0);

        }

        [TestCategory("Unit-Test-Case")]
        [Description("Test to update ExistingTripCorridor ")]
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void UpdateExistingTripCorridorTest()
        {
            var existingTripCorridor = new ExistingTripCorridor()
            {
                Id = 199,
                Address = "Pune",
                //  CategoryId = 10,
                City = "Pune",
                Country = "India",
                Distance = 12,
                StartLatitude = 51.07,
                StartLongitude = 57.07,
                CorridorLabel = "trip Test9",
                State = "A",
                // ModifiedAt =,
                //  ModifiedBy =,
                OrganizationId = 100,
                // SubCategoryId = 8,
                //TripId = 10,
                CorridorType = "E",
                Zipcode = "411057",
                CreatedBy = 1,
                ExistingTrips = new List<ExistingTrip>() {
                                    new ExistingTrip() { Id=12,
                                        Distance=10, DriverId1="dddfd1", DriverId2="d2", EndDate=121323, EndLatitude=51.07,
                                        EndPosition="End address", EndLongitude=51.07,
                                        StartDate=11313, StartPosition="start address",
                                        StartLatitude=43.34, StartLongitude=12.34, TripId="trip13",
                                        NodePoints=new List<Nodepoint>()
                                                       { new Nodepoint()
                                                             { Latitude=12.34, TripId="trip13", Address="Node address",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             }
                                                       }
                                    },
                                    new ExistingTrip() {Id=13,
                                        Distance=16, DriverId1="d2", DriverId2="d2", EndDate=121323, EndLatitude=51.07,
                                        EndPosition="End address", EndLongitude=51.07,
                                        StartDate=11313, StartPosition="start address",
                                        StartLatitude=43.34, StartLongitude=12.34, TripId="trip11",
                                        NodePoints=new List<Nodepoint>()
                                                       { new Nodepoint()
                                                             {
                                                           Id=100,
                                                           Latitude=12.34, TripId="trip11", Address="Node address11",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             },
                                                             new Nodepoint()
                                                             { Latitude=12.34, TripId="trip11", Address="Node address11",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             }
                                                       }
                                    },
                                    new ExistingTrip() {
                                        Id=14,
                                        Distance=13, DriverId1="d3", DriverId2="d2", EndDate=121323, EndLatitude=51.07,
                                        EndPosition="End address", EndLongitude=51.07,
                                        StartDate=11313, StartPosition="start address",
                                        StartLatitude=43.34, StartLongitude=12.34, TripId="trip12",
                                        NodePoints=new List<Nodepoint>()
                                                       { new Nodepoint()
                                                             {
                                                            Id=101,Latitude=12.34, TripId="trip12", Address="Node address12",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             },
                                                             new Nodepoint()
                                                             { Latitude=12.34, TripId="trip12", Address="Node address122",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             },
                                                             new Nodepoint()
                                                             { Latitude=12.34, TripId="trip12", Address="Node address123",
                                                               CreatedBy=1,Longitude =33.23, SequenceNumber=1, State="A"
                                                             }
                                                       }
                                    }
                                },

            };
            // objCorridorRequest.OrganizationId = 100;//orgid 5 ,100
            // objCorridorRequest.CorridorId = 172; //landmark table id 109, 172

            var resultCorridorList = _iCorridorManger.UpdateExistingTripCorridor(existingTripCorridor).Result;
            Assert.IsNotNull(resultCorridorList);
            Assert.IsTrue(resultCorridorList.ExistingTrips.Count > 0);

        }


    }
}
