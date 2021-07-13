using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using net.atos.daf.ct2.data;
using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.poigeofence.repository
{
    public class CorridorRepository : ICorridorRepository
    {
        private readonly IDataAccess _dataAccess;
        private static readonly log4net.ILog _log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CorridorCoreMapper _corridorCoreMapper;

        public CorridorRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;

            _corridorCoreMapper = new CorridorCoreMapper();

        }

        public async Task<List<CorridorResponse>> GetCorridorListByOrganization(CorridorRequest objCorridorRequest)
        {
            try
            {
                string query = string.Empty; var parameter = new DynamicParameters();
                query = @"select l.id 
								,l.organization_id as OrganizationId
								,l.name as CorridoreName
								,l.address as StartPoint
								,l.latitude as StartLat
								,l.longitude as StartLong
								,n.address as EndPoint
								,n.latitude as EndLat
								,n.longitude as EndLong
								,l.distance as Distance
								,l.width as Width
								,l.state as State
								,l.type as CorridorType
								,l.created_at as CreatedAt
								,l.created_by as CreatedBy
								,l.modified_at as ModifiedAt
								,l.modified_by as ModifiedBy
						FROM       master.landmark l
						LEFT JOIN master.nodes n on l.id = n.landmark_id
						WHERE      l.organization_id = @organization_id
						AND        l.type = 'R'
                        AND        l.state <> 'D'";

                parameter.Add("@organization_id", objCorridorRequest.OrganizationId);
                var data = await _dataAccess.QueryAsync<CorridorResponse>(query, parameter);
                List<CorridorResponse> objCorridorResponseList;
                return objCorridorResponseList = data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CorridorEditViewResponse> GetCorridorListByOrgIdAndCorriId(CorridorRequest objCorridorRequest)
        {
            try
            {
                string query = string.Empty; var parameter = new DynamicParameters();
                query = @"select l.id 
								,l.organization_id as OrganizationId
								,l.name as CorridoreName
								,l.address as StartPoint
								,l.latitude as StartLat
								,l.longitude as StartLong
                                ,l.type as CorridorType
								,n.address as EndPoint
								,n.latitude as EndLat
								,n.longitude as EndLong
								,l.distance as Distance
								,l.width as Width
								,l.created_at as CreatedAt
								,l.created_by as CreatedBy
								,l.modified_at as ModifiedAt
								,l.modified_by as ModifiedBy
								,cp.id as CorridorPropertiesId
								, cp.landmark_id
								, cp.is_transport_data as IsTransportData
								, cp.is_traffic_flow as IsTrafficFlow
								, cp.no_of_trailers as NoOfTrailers
								, cp.is_explosive as IsExplosive
								, cp.is_gas as IsGas
								, cp.is_flammable as IsFlammable
								, cp.is_combustible as IsCombustible
								, cp.is_organic as IsOrganic
								, cp.is_poison as IsPoision
								, cp.is_radio_active as IsRadioActive
								, cp.is_corrosive as IsCorrosive
								, cp.is_poisonous_inhalation as IsPoisonousInhalation
								, cp.is_warm_harm as IsWaterHarm
								, cp.is_other as IsOther
								, cp.toll_road_type as TollRoadType
								, cp.motorway_type as Mortorway
								, cp.boat_ferries_type as BoatFerriesType
								, cp.rail_ferries_type as RailFerriesType
								, cp.tunnels_type as TunnelsType
								, cp.dirt_road_type as DirtRoadType
								, cp.vehicle_height as VehicleHeight
								, cp.vehicle_width as vehicleWidth
								, cp.vehicle_length as vehicleLength
								, cp.vehicle_limited_weight as vehicleLimitedWeight
								, cp.vehicle_weight_per_axle as vehicleWeightPerAxle
								, cp.created_at as CreatedAtForCP
								, cp.modified_at as ModifiedAtForCP
						FROM       master.landmark l
						LEFT JOIN master.nodes n on l.id = n.landmark_id
						LEFT JOIN master.corridorproperties cp on l.id = cp.landmark_id
						WHERE      l.id = @id
						AND        l.organization_id = @organization_id
						AND        l.type IN ('E','R')
                        AND        l.state <> 'D'";

                parameter.Add("@organization_id", objCorridorRequest.OrganizationId);
                parameter.Add("@id", objCorridorRequest.CorridorId);
                var data = await _dataAccess.QueryAsync<CorridorEditViewResponse>(query, parameter);
                CorridorEditViewResponse objCorridorEditViewResponse1;
                return objCorridorEditViewResponse1 = data.FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ViaAddressDetail>> GetCorridorViaStopById(int id)
        {
            try
            {
                string query = string.Empty; var parameter = new DynamicParameters();
                query = @"select id as CorridorViaStopId
								 ,name as CorridorViaStopName
								 ,latitude
								 ,longitude
						  FROM MASTER.CORRIDORVIASTOP WHERE
						  landmark_id=@landmark_id and state = 'A'";

                parameter.Add("@landmark_id", id);
                var data = await _dataAccess.QueryAsync<ViaAddressDetail>(query, parameter);
                List<ViaAddressDetail> objViaAddressDetailList;
                return objViaAddressDetailList = data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RouteCorridor> AddRouteCorridor(RouteCorridor routeCorridor)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    RouteCorridor obj = new RouteCorridor();
                    var parameter = new DynamicParameters();


                    var insertIntoLandmark = @"INSERT INTO master.landmark(
                                          organization_id, name, address, type,distance, Width, state, latitude, longitude, created_at, created_by)
                                            VALUES (@OrganizationId, @CorridorLabel, @StartAddress, @CorridorType,@Distance, @Width, @state, @StartLatitude ,@StartLongitude, @Created_At, @Created_By)RETURNING id";


                    var insertIntoNodes = @"INSERT INTO master.nodes(
                                          landmark_id, state, latitude, longitude, created_at, created_by, address)
                                            VALUES (@LandmarkId, @state, @EndLatitude ,@EndLongitude, @Created_At, @Created_By, @EndAddress) RETURNING id";

                    var insertIntoCorridorProperties = @"INSERT INTO master.corridorproperties(
                                          landmark_id, is_transport_data, is_traffic_flow, no_of_trailers, is_explosive, is_gas, is_flammable, is_combustible, is_organic, is_poison, is_radio_active, is_corrosive, is_poisonous_inhalation, is_warm_harm, is_other, toll_road_type, motorway_type, boat_ferries_type, rail_ferries_type, tunnels_type, dirt_road_type, vehicle_height, vehicle_width, vehicle_length, vehicle_limited_weight, vehicle_weight_per_axle, created_at)
                                           VALUES (@LandmarkId, @TransportData, @TrafficFlow, @Trailer, @Explosive, @Gas, @Flammable, @Combustible, @organic, @poision, @RadioActive, @Corrosive, @PoisonousInhalation, @WaterHarm, @Other, @TollRoad, @Mortorway, @BoatFerries, @RailFerries, @Tunnels, @DirtRoad, @VehicleSizeHeight, @VehicleSizeWidth, @VehicleSizeLength, @VehicleSizeLimitedWeight, @VehicleSizeWeightPerAxle, @Created_At) RETURNING id";


                    parameter.Add("@OrganizationId", routeCorridor.OrganizationId != 0 ? routeCorridor.OrganizationId : null);
                    parameter.Add("@Distance", routeCorridor.Distance);
                    parameter.Add("@CorridorType", routeCorridor.CorridorType);
                    parameter.Add("@CorridorLabel", routeCorridor.CorridorLabel);

                    parameter.Add("@StartAddress", routeCorridor.StartAddress);
                    parameter.Add("@StartLatitude", routeCorridor.StartLatitude);
                    parameter.Add("@StartLongitude", routeCorridor.StartLongitude);

                    parameter.Add("@EndAddress", routeCorridor.EndAddress);
                    parameter.Add("@EndLatitude", routeCorridor.EndLatitude);
                    parameter.Add("@EndLongitude", routeCorridor.EndLongitude);

                    parameter.Add("@Width", routeCorridor.Width);
                    parameter.Add("@TransportData", routeCorridor.TransportData);
                    parameter.Add("@TrafficFlow", routeCorridor.TrafficFlow);
                    parameter.Add("@Trailer", routeCorridor.Trailer);
                    parameter.Add("@Explosive", routeCorridor.Explosive);
                    parameter.Add("@Gas", routeCorridor.Gas);

                    parameter.Add("@Flammable", routeCorridor.Flammable);
                    parameter.Add("@Combustible", routeCorridor.Combustible);
                    parameter.Add("@organic", routeCorridor.Organic);
                    parameter.Add("@poision", routeCorridor.Poision);
                    parameter.Add("@RadioActive", routeCorridor.RadioActive);
                    parameter.Add("@Corrosive", routeCorridor.Corrosive);
                    parameter.Add("@PoisonousInhalation", routeCorridor.PoisonousInhalation);
                    parameter.Add("@WaterHarm", routeCorridor.WaterHarm);
                    parameter.Add("@Other", routeCorridor.Other);

                    parameter.Add("@TollRoad", routeCorridor.TollRoad);
                    parameter.Add("@Mortorway", routeCorridor.Mortorway);
                    parameter.Add("@BoatFerries", routeCorridor.BoatFerries);
                    parameter.Add("@RailFerries", routeCorridor.RailFerries);
                    parameter.Add("@Tunnels", routeCorridor.Tunnels);
                    parameter.Add("@DirtRoad", routeCorridor.DirtRoad);
                    parameter.Add("@VehicleSizeHeight", routeCorridor.VehicleSizeHeight);
                    parameter.Add("@VehicleSizeWidth", routeCorridor.VehicleSizeWidth);
                    parameter.Add("@VehicleSizeLength", routeCorridor.VehicleSizeLength);
                    parameter.Add("@VehicleSizeLimitedWeight", routeCorridor.VehicleSizeLimitedWeight);
                    parameter.Add("@VehicleSizeWeightPerAxle", routeCorridor.VehicleSizeWeightPerAxle);

                    parameter.Add("@Created_At", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                    parameter.Add("@Created_By", routeCorridor.Created_By);
                    parameter.Add("@state", "A");


                    var id = await _dataAccess.ExecuteScalarAsync<int>(insertIntoLandmark, parameter);
                    if (id > 0)
                    {
                        routeCorridor.Id = id;
                        parameter.Add("@LandmarkId", routeCorridor.Id);

                        await _dataAccess.ExecuteScalarAsync<int>(insertIntoNodes, parameter);

                        await _dataAccess.ExecuteScalarAsync<int>(insertIntoCorridorProperties, parameter);

                        ViaRoute routeObj = new ViaRoute();
                        foreach (var item in routeCorridor.ViaRoutDetails)
                        {
                            var temp = new ViaRoute();
                            temp.ViaStopName = item.ViaStopName;
                            temp.Latitude = item.Latitude;
                            temp.Longitude = item.Longitude;

                            parameter.Add("@Latitude", temp.Latitude);
                            parameter.Add("@Longitude", temp.Longitude);
                            parameter.Add("@ViaStopName", temp.ViaStopName);

                            var insertIntoCorridorViaStop = @"INSERT INTO master.corridorviastop(
                                          landmark_id, latitude, longitude, name, state)
                                            VALUES (@LandmarkId, @Latitude, @Longitude ,@ViaStopName, @state) RETURNING id";

                            await _dataAccess.ExecuteScalarAsync<int>(insertIntoCorridorViaStop, parameter);

                        }

                    }

                    transactionScope.Complete();

                }
            }
            catch (Exception ex)
            {
                _log.Info("AddRouteCorridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(routeCorridor.Id));
                _log.Error(ex.ToString());
                // throw;
            }
            return routeCorridor;
        }

        public async Task<bool> CheckRouteCorridorIsexist(string corridorName, int? organizationId, int id, char type)
        {
            var parameterduplicate = new DynamicParameters();
            parameterduplicate.Add("@organization_id", organizationId);
            parameterduplicate.Add("@name", corridorName);
            parameterduplicate.Add("@type", type);
            var queryduplicate = @"SELECT id FROM master.landmark where state in ('A','I')  and type = @type and name=@name and organization_id=@organization_id;";

            int corridorExist = await _dataAccess.ExecuteScalarAsync<int>(queryduplicate, parameterduplicate);

            return corridorExist == 0 ? false : true;
        }
        public async Task<bool> CheckCorridorexistByIdName(string corridorName, int? organizationId, int id, char type)
        {
            var parameterduplicate = new DynamicParameters();
            parameterduplicate.Add("@organization_id", organizationId);
            parameterduplicate.Add("@name", corridorName);
            parameterduplicate.Add("@type", type);
            parameterduplicate.Add("@id", id);
            var queryduplicate = @"SELECT id FROM master.landmark where id=@id and organization_id=@organization_id and name=@name and type = @type and state in ('A','I');";

            int corridorExist = await _dataAccess.ExecuteScalarAsync<int>(queryduplicate, parameterduplicate);

            return corridorExist != 0;
        }

        public async Task<ExistingTripCorridor> AddExistingTripCorridor(ExistingTripCorridor existingTripCorridor)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();

            try
            {

                var isExist = CheckRouteCorridorIsexist(existingTripCorridor.CorridorLabel, existingTripCorridor.OrganizationId, existingTripCorridor.Id, Convert.ToChar(existingTripCorridor.CorridorType)).Result;

                if (isExist)
                {
                    existingTripCorridor.Id = -1;// Corridor is already exist with same name.
                    return existingTripCorridor;
                }

                var insertIntoLandmark = @"INSERT INTO master.landmark(
										             organization_id, name, address,city, country, zipcode, type,latitude, longitude, distance,width, state, created_at, created_by)
											VALUES (@organization_id, @corridorLabel, @address,@city, @country, @zipcode, @corridorType, @latitude, @longitude, @distance,@width,
                                                    @state, @created_at, @created_by)RETURNING id";

                var parameter = new DynamicParameters();
                parameter.Add("@organization_id", existingTripCorridor.OrganizationId != 0 ? existingTripCorridor.OrganizationId : null);
                parameter.Add("@corridorLabel", existingTripCorridor.CorridorLabel);
                parameter.Add("@address", existingTripCorridor.Address);
                parameter.Add("@city", existingTripCorridor.City);
                parameter.Add("@country", existingTripCorridor.Country);
                parameter.Add("@zipcode", existingTripCorridor.Zipcode);
                parameter.Add("@corridorType", MapLandmarkTypeToChar(existingTripCorridor.CorridorType));
                parameter.Add("@latitude", existingTripCorridor.StartLatitude);
                parameter.Add("@longitude", existingTripCorridor.StartLongitude);
                parameter.Add("@distance", existingTripCorridor.Distance);
                parameter.Add("@width", existingTripCorridor.Width);
                parameter.Add("@state", 'A');
                parameter.Add("@created_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@created_by", existingTripCorridor.CreatedBy);
                parameter.Add("@Created_At", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@Created_By", existingTripCorridor.CreatedBy);
                parameter.Add("@state", "A");


                var id = await _dataAccess.ExecuteScalarAsync<int>(insertIntoLandmark, parameter);
                if (id > 0)
                {
                    existingTripCorridor.Id = id;
                    var tripDetails = await AddTripsCorridor(existingTripCorridor);
                }
                transactionScope.Commit();

            }
            catch (Exception ex)
            {
                transactionScope.Rollback();
                _log.Info("AddExistingTripCorridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(existingTripCorridor.Id));
                _log.Error(ex.ToString());

            }
            finally
            {
                _dataAccess.Connection.Close();
            }

            return existingTripCorridor;
        }
        private async Task<List<ExistingTrip>> AddTripsCorridor(ExistingTripCorridor existingTripCorridor)
        {
            var tripList = new List<ExistingTrip>();
            try
            {
                foreach (var existingTrip in existingTripCorridor.ExistingTrips)
                {
                    existingTrip.LandmarkId = existingTripCorridor.Id;
                    var insertIntoCorridorTrips = @"INSERT INTO master.corridortrips(
										  landmark_id, trip_id, start_date, end_date, driver_id1, driver_id2, start_latitude, 
										  start_longitude, end_latitude,end_longitude, start_position, end_position, distance,state)
										  VALUES (@LandmarkId,@TripId, @StartDate, @EndDate, @DriverId1, @DriverId2, @StartLatitude, 
										  @StartLongitude, @EndLatitude,@EndLongitude, @StartPosition, @EndPosition, @Distance,'A') RETURNING id";

                    if (existingTrip.LandmarkId > 0)
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("@LandmarkId", existingTrip.LandmarkId);
                        parameter.Add("@TripId", existingTrip.TripId);

                        parameter.Add("@StartDate", existingTrip.StartDate);

                        parameter.Add("@EndDate", existingTrip.EndDate);

                        parameter.Add("@DriverId1", existingTrip.DriverId1);

                        parameter.Add("@DriverId2", existingTrip.DriverId2);

                        parameter.Add("@StartLatitude", existingTrip.StartLatitude);

                        parameter.Add("@StartLongitude", existingTrip.StartLongitude);

                        parameter.Add("@EndLatitude", existingTrip.EndLatitude);

                        parameter.Add("@EndLongitude", existingTrip.EndLongitude);

                        parameter.Add("@StartPosition", existingTrip.StartPosition);

                        parameter.Add("@EndPosition", existingTrip.EndPosition);

                        parameter.Add("@Distance", existingTrip.Distance);
                        var id = await _dataAccess.ExecuteScalarAsync<int>(insertIntoCorridorTrips, parameter);
                        existingTrip.Id = Convert.ToInt32(id);
                        if (existingTrip.Id > 0)
                        {
                            var inseredNodesDetails = await InsertToNodes(existingTrip.NodePoints, existingTrip.LandmarkId, existingTrip.TripId);
                        }

                        tripList.Add(existingTrip);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                // throw;
            }
            return tripList;


        }


        private async Task<List<Nodepoint>> InsertToNodes(List<Nodepoint> nodePoints, int landmarkId, string tripId)
        {
            var tripNodes = new List<Nodepoint>();

            try
            {

                foreach (var nodePoint in nodePoints)
                {
                    nodePoint.LandmarkId = landmarkId;
                    nodePoint.TripId = tripId; // parent trip id for all nodes
                    var insertIntoNodes = @"INSERT INTO master.nodes(
								        landmark_id,seq_no,latitude,longitude, state, created_at, created_by, address,trip_id)
										VALUES (@LandmarkId,@SequenceNumber,@Latitude,@Longitude, @State, @Created_At, @Created_By,
                                        @Address,@TripId) RETURNING id";


                    if (nodePoint.LandmarkId > 0)
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("@LandmarkId", nodePoint.LandmarkId);
                        parameter.Add("@TripId", nodePoint.TripId);
                        parameter.Add("@SequenceNumber", nodePoint.SequenceNumber);
                        parameter.Add("@Latitude", nodePoint.Latitude);
                        parameter.Add("@Longitude", nodePoint.Longitude);
                        parameter.Add("@State", "A");
                        parameter.Add("@Created_At", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                        parameter.Add("@Created_By", nodePoint.CreatedBy);
                        parameter.Add("@Address", nodePoint.Address);
                        var result = await _dataAccess.ExecuteScalarAsync<int>(insertIntoNodes, parameter);
                        nodePoint.Id = Convert.ToInt32(result);
                        tripNodes.Add(nodePoint);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            return tripNodes;


        }



        private async Task<bool> DeleteTrips(List<int> tripIds)
        {
            bool result;
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@ids", tripIds);
                var query = @"update master.corridortrips set state='D' where  id =any(@ids)  RETURNING id";
                int isdelete = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (isdelete > 0)
                    result = true;
                else
                    result = false;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        public async Task<bool> DeleteNodes(List<int> nodeIds)
        {
            bool result;
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@ids", nodeIds);
                var query = @"update master.nodes set state='D' where  id =any(@ids)  RETURNING id";
                int isdelete = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                if (isdelete > 0)
                    result = true;
                else
                    result = false;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<ExistingTripCorridor> UpdateExistingTripCorridor(ExistingTripCorridor existingTripCorridor)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();

            try
            {
                // using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                // {
                var updateIntoLandmark = @"update master.landmark set 
                                                    organization_id=@organization_id,
                                                    name=@corridorLabel, 
                                                    address=@address,
                                                    city=@city,
                                                    country=@country, 
                                                    zipcode=@zipcode, 
                                                    type=@corridorType,
                                                    latitude=@latitude,
                                                    longitude=@longitude, 
                                                    distance=@distance,
                                                    width=@width,                                                          
                                                    modified_at=@modified_at,
                                                    modified_by =@modified_by
                                                    where id = @Id RETURNING id";




                var parameter = new DynamicParameters();
                parameter.Add("@Id", existingTripCorridor.Id);
                parameter.Add("@organization_id", existingTripCorridor.OrganizationId != 0 ? existingTripCorridor.OrganizationId : null);
                parameter.Add("@corridorLabel", existingTripCorridor.CorridorLabel);
                parameter.Add("@address", existingTripCorridor.Address);
                parameter.Add("@city", existingTripCorridor.City);
                parameter.Add("@country", existingTripCorridor.Country);
                parameter.Add("@zipcode", existingTripCorridor.Zipcode);
                parameter.Add("@corridorType", MapLandmarkTypeToChar(existingTripCorridor.CorridorType));
                parameter.Add("@latitude", existingTripCorridor.StartLatitude);
                parameter.Add("@longitude", existingTripCorridor.StartLongitude);
                parameter.Add("@distance", existingTripCorridor.Distance);
                parameter.Add("@width", existingTripCorridor.Width);
                parameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                parameter.Add("@modified_by", existingTripCorridor.ModifiedBy);


                var id = await _dataAccess.ExecuteScalarAsync<int>(updateIntoLandmark, parameter);
                if (id > 0)
                {
                    existingTripCorridor.Id = id;
                    var tripDetailsDeleted = await DeleteTripsCorridor(existingTripCorridor);
                    if (tripDetailsDeleted)
                    {

                        var result = await AddTripsCorridor(existingTripCorridor);
                    }
                }


                transactionScope.Commit();

            }
            catch (Exception ex)
            {
                transactionScope.Rollback();
                _log.Info("UpdateExistingTripCorridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(existingTripCorridor.Id));
                _log.Error(ex.ToString());

            }
            finally
            {
                _dataAccess.Connection.Close();
            }

            return existingTripCorridor;
        }



        private async Task<bool> DeleteTripsCorridor(ExistingTripCorridor existingTripCorridor)
        {
            var deleteTripList = false;
            try
            {
                var landmarkId = existingTripCorridor.Id;

                var query = @"SELECT id,trip_id FROM master.corridortrips where state in ('A','I')  and landmark_id=@landmark_id;";

                var tripparameter = new DynamicParameters();
                tripparameter.Add("@landmark_id", landmarkId);

                var result = await _dataAccess.QueryAsync<(int id, string trip_id)>(query, tripparameter);
                var tripDetails = result.ToList();

                if (tripDetails != null && tripDetails.Count > 0)
                {

                    foreach (var trip in tripDetails)
                    {
                        var tripId = new List<int>();
                        tripId.Add(trip.id);
                        var deleteTrip = await DeleteTrips(tripId);
                        if (deleteTrip)
                        {
                            var nodeparameter = new DynamicParameters();
                            nodeparameter.Add("@landmark_id", landmarkId);
                            nodeparameter.Add("@trip_id", trip.trip_id);
                            var nodequery = @"SELECT id,trip_id FROM master.nodes where state in ('A','I') and landmark_id=@landmark_id  and trip_id=@trip_id;";
                            var noderesult = await _dataAccess.QueryAsync<(int id, string trip_id)>(nodequery, nodeparameter);
                            var nodeDetails = noderesult.ToList();
                            var nodeIds = nodeDetails.Select(x => x.id).ToList();
                            var deleteNodes = await DeleteNodes(nodeIds);
                            deleteTripList = deleteNodes ? true : false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
            return deleteTripList;


        }




        private char MapLandmarkTypeToChar(string type)
        {
            char ptype = 'N';
            switch (type)
            {
                case "None":
                    ptype = 'N';
                    break;
                case "POI":
                    ptype = 'P';
                    break;
                case "CircularGeofence":
                    ptype = 'C';
                    break;
                case "PolygonGeofence":
                    ptype = 'O';
                    break;
                case "Corridor":
                    ptype = 'R';
                    break;
                case "Route":
                    ptype = 'U';
                    break;
                case "E":
                    ptype = 'E';
                    break;
            }
            return ptype;
        }

        #region GetExistingTripCorridore
        public async Task<List<CorridorEditViewResponse>> GetExistingtripCorridorListByOrgIdAndCorriId(CorridorRequest objCorridorRequest)
        {
            try
            {
                string query = string.Empty; var parameter = new DynamicParameters();
                query = @"select l.id 
                                ,l.organization_id as OrganizationId
	                            ,l.name as CorridoreName
	                            ,l.address as StartPoint
	                            ,l.latitude as StartLat
	                            ,l.longitude as StartLong
	                            ,n.address as EndPoint
	                            ,n.latitude as EndLat
	                            ,n.longitude as EndLong
	                            ,l.distance as Distance
	                            ,l.distance as Width
	                            ,l.created_at as CreatedAt
	                            ,l.created_by as CreatedBy
	                            ,l.modified_at as ModifiedAt
	                            ,l.modified_by as ModifiedBy
								
                        FROM       master.landmark l
                        LEFT JOIN master.nodes n on l.id = n.landmark_id						
                        WHERE      l.type = 'E'  
                        AND        l.organization_id = @organization_id
                        AND        l.id = @id";
                //getting type R records only to avoid existing trip nodes mismatch
                parameter.Add("@organization_id", objCorridorRequest.OrganizationId);
                parameter.Add("@id", objCorridorRequest.CorridorId);
                var data = await _dataAccess.QueryAsync<CorridorEditViewResponse>(query, parameter);
                List<CorridorEditViewResponse> objCorridorEditViewResponse1;
                return objCorridorEditViewResponse1 = data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CorridorResponse>> GetExistingTripCorridorListByOrganization(CorridorRequest objCorridorRequest)
        {
            try
            {
                string query = string.Empty; var parameter = new DynamicParameters();
                query = @"select l.id 
                                ,l.organization_id as OrganizationId
	                            ,l.name as CorridoreName
	                            ,l.address as StartPoint
	                            ,l.latitude as StartLat
	                            ,l.longitude as StartLong
                                ,n.address as EndPoint
	                            ,n.latitude as EndLat
	                            ,n.longitude as EndLong
	                            ,l.distance as Distance
	                            ,l.width as Width
                                ,l.state as State
                                ,l.type as CorridorType
	                            ,l.created_at as CreatedAt
	                            ,l.created_by as CreatedBy
	                            ,l.modified_at as ModifiedAt
	                            ,l.modified_by as ModifiedBy
                        FROM       master.landmark l
                        LEFT JOIN master.nodes n on l.id = n.landmark_id
                        WHERE      l.type = 'E' and l.state in ('A', 'I')
                        AND        l.organization_id = @organization_id";

                parameter.Add("@organization_id", objCorridorRequest.OrganizationId);
                var data = await _dataAccess.QueryAsync<CorridorResponse>(query, parameter);
                List<CorridorResponse> objCorridorResponseList;
                return objCorridorResponseList = data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ExistingTrip> GetExistingtripListByCorridorId(int corridoreid)
        {
            try
            {
                string query = string.Empty; var parameter = new DynamicParameters();
                query = @"SELECT id, 
                        landmark_id as LandmarkId, 
                        trip_id as TripId, 
                        start_date as StartDate, 
                        end_date as EndDate, 
                        driver_id1 as DriverId1, 
                        driver_id2 as DriverId2, 
                        start_latitude as StartLatitude, 
                        start_longitude as StartLongitude, 
                        end_latitude as EndLatitude, 
                        end_longitude as EndLongitude, 
                        start_position as StartPosition, 
                        end_position as EndPosition, 
                        distance
	                    FROM master.corridortrips
                        WHERE   state = 'A' and  landmark_id = @landmark_id";

                parameter.Add("@landmark_id", corridoreid);
                var data = _dataAccess.Query<ExistingTrip>(query, parameter);
                List<ExistingTrip> objCorridorResponseList;
                return objCorridorResponseList = data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Nodepoint> GetTripNodes(string tripid, int landmarkid)
        {
            try
            {
                string query = string.Empty; var parameter = new DynamicParameters();
                query = @"SELECT id,
                            landmark_id as LandmarkId,
                            seq_no as SequenceNumber,
                            latitude,
                            longitude,
                            state,
                            created_at,
                            created_by,
                            modified_at,
                            modified_by,
                            address,
                            trip_id as TripId
                            FROM master.nodes
                            where state= 'A' and trip_id = @trip_id and landmark_id=@landmark_id";

                parameter.Add("@trip_id", tripid);
                parameter.Add("@landmark_id", landmarkid);
                var data = _dataAccess.Query<Nodepoint>(query, parameter);
                List<Nodepoint> objCorridorNodes;
                return objCorridorNodes = data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        public async Task<CorridorID> DeleteCorridor(int corridorId)
        {
            _log.Info("Delete Corridor method called in repository");
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    CorridorID corridorID = new CorridorID();
                    var parameter = new DynamicParameters();

                    var deleteCorridor = @"UPDATE master.landmark SET  
                                               state=@State 
                                   WHERE id = @ID RETURNING id ";

                    var deleteViaStop = @"UPDATE master.corridorviastop
                                          SET  state= @State
                                            where landmark_id= @ID";

                    parameter.Add("@ID", corridorId);
                    parameter.Add("@State", "D");

                    var id = await _dataAccess.ExecuteScalarAsync<int>(deleteCorridor, parameter);
                    corridorID.Id = id;
                    if (corridorID.Id > 0)
                    {
                        await _dataAccess.ExecuteScalarAsync<int>(deleteViaStop, parameter);
                    }

                    transactionScope.Complete();
                    return corridorID;
                }
            }
            catch (Exception ex)
            {
                _log.Info("Delete Corridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(corridorId));
                _log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<int> GetAssociateAlertbyId(int id)
        {
            try
            {
                string query = string.Empty; var parameter = new DynamicParameters();
                query = @"select count(*)
                          from master.alertlandmarkref
                          where ref_id= @landmark_id and landmark_type=@Type and state =@State";

                parameter.Add("@landmark_id", id);
                parameter.Add("@Type", "R");
                parameter.Add("@State", "A");
                var data = await _dataAccess.ExecuteScalarAsync<int>(query, parameter);
                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RouteCorridor> UpdateRouteCorridor(RouteCorridor routeCorridor)
        {
            _dataAccess.Connection.Open();
            var transactionScope = _dataAccess.Connection.BeginTransaction();
            try
            {
                var updateCorridorparameter = new DynamicParameters();

                StringBuilder queryForUpdateCorridor = new StringBuilder();
                queryForUpdateCorridor.Append("UPDATE master.landmark set modified_at=@modified_at");
                //UpdateCorridorparameter.Add("@OrganizationId", routeCorridor.OrganizationId);
                if (!string.IsNullOrEmpty(routeCorridor.CorridorLabel))
                {
                    updateCorridorparameter.Add("@name", routeCorridor.CorridorLabel);
                    queryForUpdateCorridor.Append(", name=@name");
                }
                if (!string.IsNullOrEmpty(routeCorridor.StartAddress))
                {
                    updateCorridorparameter.Add("@address", routeCorridor.StartAddress);
                    queryForUpdateCorridor.Append(", address=@address");
                }
                //if (!string.IsNullOrEmpty(routeCorridor.CorridorType))
                //{
                //    parameter.Add("@type", routeCorridor.CorridorType);
                //    queryForUpdateCorridor.Append(", type=@type");
                //}
                if (routeCorridor.Distance > 0)
                {
                    updateCorridorparameter.Add("@distance", routeCorridor.Distance);
                    queryForUpdateCorridor.Append(", distance=@distance");
                }
                if (routeCorridor.Width > 0)
                {
                    updateCorridorparameter.Add("@Width", routeCorridor.Width);
                    queryForUpdateCorridor.Append(", Width=@Width");
                }
                if (!string.IsNullOrEmpty(routeCorridor.State))
                {
                    updateCorridorparameter.Add("@state", routeCorridor.State);
                    queryForUpdateCorridor.Append(", state=@state");
                }
                if (routeCorridor.StartLatitude > 0)
                {
                    updateCorridorparameter.Add("@latitude", routeCorridor.StartLatitude);
                    queryForUpdateCorridor.Append(", latitude=@latitude");
                }
                if (routeCorridor.StartLongitude > 0)
                {
                    updateCorridorparameter.Add("@longitude", routeCorridor.StartLongitude);
                    queryForUpdateCorridor.Append(", longitude=@longitude");
                }

                if (routeCorridor.Modified_By > 0)
                {
                    updateCorridorparameter.Add("@modified_by", routeCorridor.Modified_By);
                    queryForUpdateCorridor.Append(", modified_by=@modified_by");
                }

                updateCorridorparameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));
                //queryForUpdateCorridor.Append(", modified_at=@modified_at");
                updateCorridorparameter.Add("@id", routeCorridor.Id);
                queryForUpdateCorridor.Append(" where id=@id RETURNING id");

                //var id = await _dataAccess.ExecuteScalarAsync<int>(queryForUpdateCorridor.ToString(), parameter);
                //if (id > 0)
                //    routeCorridor.Id = id;
                //else
                //    routeCorridor.Id = 0;


                //var insertIntoNodes = @"INSERT INTO master.nodes(
                //                      landmark_id, latitude, longitude, created_at, created_by, address)
                //                        VALUES (@LandmarkId, @StartLatitude ,@StartLongitude, @Created_At, @Created_By, @EndAddress) RETURNING id";

                StringBuilder queryForUpdateCorridorNodes = new StringBuilder();
                queryForUpdateCorridorNodes.Append("UPDATE master.nodes set modified_at=@modified_at");
                var updateCorridorNodesparameter = new DynamicParameters();
                if (routeCorridor.EndLatitude > 0)
                {
                    updateCorridorNodesparameter.Add("@latitude", routeCorridor.EndLatitude);
                    queryForUpdateCorridorNodes.Append(", latitude=@latitude");
                }
                if (routeCorridor.EndLongitude > 0)
                {
                    updateCorridorNodesparameter.Add("@longitude", routeCorridor.EndLongitude);
                    queryForUpdateCorridorNodes.Append(", longitude=@longitude");
                }
                if (!string.IsNullOrEmpty(routeCorridor.EndAddress))
                {
                    updateCorridorNodesparameter.Add("@address", routeCorridor.EndAddress);
                    queryForUpdateCorridorNodes.Append(", address=@address");
                }

                queryForUpdateCorridorNodes.Append(", modified_by=@modified_by");
                updateCorridorNodesparameter.Add("@modified_by", routeCorridor.Modified_By);

                //queryForUpdateCorridorNodes.Append(", modified_at=@modified_at");
                updateCorridorNodesparameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

                queryForUpdateCorridorNodes.Append(" where landmark_id=@landmark_id RETURNING id");
                updateCorridorNodesparameter.Add("@landmark_id", routeCorridor.Id);

                //var insertIntoCorridorProperties = @"INSERT INTO master.corridorproperties(
                //                      landmark_id, is_transport_data, is_traffic_flow, no_of_trailers, is_explosive, is_gas, is_flammable, is_combustible, is_organic, is_poison, is_radio_active, is_corrosive, is_poisonous_inhalation, is_warm_harm, is_other, toll_road_type, motorway_type, boat_ferries_type, rail_ferries_type, tunnels_type, dirt_road_type, vehicle_height, vehicle_width, vehicle_length, vehicle_limited_weight, vehicle_weight_per_axle, created_at)
                //                       VALUES (@LandmarkId, @TransportData, @TrafficFlow, @Trailer, @Explosive, @Gas, @Flammable, @Combustible, @organic, @poision, @RadioActive, @Corrosive, @PoisonousInhalation, @WaterHarm, @Other, @TollRoad, @Mortorway, @BoatFerries, @RailFerries, @Tunnels, @DirtRoad, @VehicleSizeHeight, @VehicleSizeWidth, @VehicleSizeLength, @VehicleSizeLimitedWeight, @VehicleSizeWeightPerAxle, @Created_At) RETURNING id";

                StringBuilder queryToUpdateCorridorProperties = new StringBuilder();
                queryToUpdateCorridorProperties.Append("UPDATE master.corridorproperties set modified_at=@modified_at");
                var updateCorridorPropertiesparameter = new DynamicParameters();

                updateCorridorPropertiesparameter.Add("@is_transport_data", routeCorridor.TransportData);
                queryToUpdateCorridorProperties.Append(", is_transport_data=@is_transport_data");

                updateCorridorPropertiesparameter.Add("@is_traffic_flow", routeCorridor.TrafficFlow);
                queryToUpdateCorridorProperties.Append(", is_traffic_flow=@is_traffic_flow");
                if (routeCorridor.Trailer > 0)
                {
                    updateCorridorPropertiesparameter.Add("@no_of_trailers", routeCorridor.Trailer);
                    queryToUpdateCorridorProperties.Append(", no_of_trailers=@no_of_trailers");
                }
                updateCorridorPropertiesparameter.Add("@is_explosive", routeCorridor.Explosive);
                queryToUpdateCorridorProperties.Append(", is_explosive=@is_explosive");

                updateCorridorPropertiesparameter.Add("@is_gas", routeCorridor.Gas);
                queryToUpdateCorridorProperties.Append(", is_gas=@is_gas");

                updateCorridorPropertiesparameter.Add("@is_flammable", routeCorridor.Flammable);
                queryToUpdateCorridorProperties.Append(", is_flammable=@is_flammable");

                updateCorridorPropertiesparameter.Add("@is_combustible", routeCorridor.Combustible);
                queryToUpdateCorridorProperties.Append(", is_combustible=@is_combustible");

                updateCorridorPropertiesparameter.Add("@is_organic", routeCorridor.Organic);
                queryToUpdateCorridorProperties.Append(", is_organic=@is_organic");

                updateCorridorPropertiesparameter.Add("@is_poison", routeCorridor.Poision);
                queryToUpdateCorridorProperties.Append(", is_poison=@is_poison");

                updateCorridorPropertiesparameter.Add("@is_radio_active", routeCorridor.RadioActive);
                queryToUpdateCorridorProperties.Append(", is_radio_active=@is_radio_active");

                updateCorridorPropertiesparameter.Add("@is_corrosive", routeCorridor.Corrosive);
                queryToUpdateCorridorProperties.Append(", is_corrosive=@is_corrosive");

                updateCorridorPropertiesparameter.Add("@is_poisonous_inhalation", routeCorridor.PoisonousInhalation);
                queryToUpdateCorridorProperties.Append(", is_poisonous_inhalation=@is_poisonous_inhalation");

                updateCorridorPropertiesparameter.Add("@is_warm_harm", routeCorridor.WaterHarm);
                queryToUpdateCorridorProperties.Append(", is_warm_harm=@is_warm_harm");

                updateCorridorPropertiesparameter.Add("@is_other", routeCorridor.Other);
                queryToUpdateCorridorProperties.Append(", is_other=@is_other");

                //Char type added validation at model level so if is not needed here
                updateCorridorPropertiesparameter.Add("@toll_road_type", routeCorridor.TollRoad);
                queryToUpdateCorridorProperties.Append(", toll_road_type=@toll_road_type");

                updateCorridorPropertiesparameter.Add("@motorway_type", routeCorridor.Mortorway);
                queryToUpdateCorridorProperties.Append(", motorway_type=@motorway_type");

                updateCorridorPropertiesparameter.Add("@boat_ferries_type", routeCorridor.BoatFerries);
                queryToUpdateCorridorProperties.Append(", boat_ferries_type=@boat_ferries_type");

                updateCorridorPropertiesparameter.Add("@rail_ferries_type", routeCorridor.RailFerries);
                queryToUpdateCorridorProperties.Append(", rail_ferries_type=@rail_ferries_type");

                updateCorridorPropertiesparameter.Add("@tunnels_type", routeCorridor.Tunnels);
                queryToUpdateCorridorProperties.Append(", tunnels_type=@tunnels_type");

                updateCorridorPropertiesparameter.Add("@dirt_road_type", routeCorridor.DirtRoad);
                queryToUpdateCorridorProperties.Append(", dirt_road_type=@dirt_road_type");


                if (routeCorridor.VehicleSizeHeight > 0)
                {
                    updateCorridorPropertiesparameter.Add("@vehicle_height", routeCorridor.VehicleSizeHeight);
                    queryToUpdateCorridorProperties.Append(", vehicle_height=@vehicle_height");
                }
                if (routeCorridor.VehicleSizeWidth > 0)
                {
                    updateCorridorPropertiesparameter.Add("@vehicle_width", routeCorridor.VehicleSizeWidth);
                    queryToUpdateCorridorProperties.Append(", vehicle_width=@vehicle_width");
                }
                if (routeCorridor.VehicleSizeLength > 0)
                {
                    updateCorridorPropertiesparameter.Add("@vehicle_length", routeCorridor.VehicleSizeLength);
                    queryToUpdateCorridorProperties.Append(", vehicle_length=@vehicle_length");
                }
                if (routeCorridor.VehicleSizeLimitedWeight > 0)
                {
                    updateCorridorPropertiesparameter.Add("@vehicle_limited_weight", routeCorridor.VehicleSizeLimitedWeight);
                    queryToUpdateCorridorProperties.Append(", vehicle_limited_weight=@vehicle_limited_weight");
                }
                if (routeCorridor.VehicleSizeWeightPerAxle > 0)
                {
                    updateCorridorPropertiesparameter.Add("@vehicle_weight_per_axle", routeCorridor.VehicleSizeWeightPerAxle);
                    queryToUpdateCorridorProperties.Append(", vehicle_weight_per_axle=@vehicle_weight_per_axle");
                }

                updateCorridorPropertiesparameter.Add("@modified_at", UTCHandling.GetUTCFromDateTime(DateTime.Now.ToString()));

                queryToUpdateCorridorProperties.Append(" where landmark_id=@landmark_id RETURNING id");
                updateCorridorPropertiesparameter.Add("@landmark_id", routeCorridor.Id);


                var id = await _dataAccess.ExecuteScalarAsync<int>(queryForUpdateCorridor.ToString(), updateCorridorparameter);
                if (id > 0)
                {
                    routeCorridor.Id = id;
                    //parameter.Add("@landmark_id", routeCorridor.Id);

                    int nodeId = await _dataAccess.ExecuteScalarAsync<int>(queryForUpdateCorridorNodes.ToString(), updateCorridorNodesparameter);

                    int corridorProperties = await _dataAccess.ExecuteScalarAsync<int>(queryToUpdateCorridorProperties.ToString(), updateCorridorPropertiesparameter);

                    ViaRoute routeObj = new ViaRoute();
                    var updateViaStopparameter = new DynamicParameters();
                    updateViaStopparameter.Add("@state", "I");
                    updateViaStopparameter.Add("@landmark_id", routeCorridor.Id);
                    string querytoUpdateViaStop = @"UPDATE master.corridorviastop set state=@state 
                                           where landmark_id=@landmark_id";
                    await _dataAccess.ExecuteAsync(querytoUpdateViaStop, updateViaStopparameter);
                    foreach (var item in routeCorridor.ViaRoutDetails)
                    {
                        var temp = new ViaRoute();
                        temp.ViaStopName = item.ViaStopName;
                        temp.Latitude = item.Latitude;
                        temp.Longitude = item.Longitude;
                        var updateViaRoutparameter = new DynamicParameters();
                        updateViaRoutparameter.Add("@latitude", temp.Latitude);
                        updateViaRoutparameter.Add("@longitude", temp.Longitude);
                        updateViaRoutparameter.Add("@name", temp.ViaStopName);
                        updateViaRoutparameter.Add("@state", "A");
                        updateViaRoutparameter.Add("@landmark_id", routeCorridor.Id);
                        string queryInsertViaStop = string.Empty;
                        queryInsertViaStop = @"INSERT INTO master.corridorviastop(
                                          landmark_id, latitude, longitude, name, state)
                                            VALUES (@landmark_id, @latitude, @longitude ,@name, @state)";

                        await _dataAccess.ExecuteAsync(queryInsertViaStop, updateViaRoutparameter);
                    }
                }
                transactionScope.Commit();
            }
            catch (Exception ex)
            {
                _log.Info($"UpdateRouteCorridor method in repository failed : {Newtonsoft.Json.JsonConvert.SerializeObject(routeCorridor.Id)}");
                _log.Error(ex.ToString());
                transactionScope.Rollback();
                throw;
            }
            finally
            {
                _dataAccess.Connection.Close();
            }
            return routeCorridor;
        }
    }
}
