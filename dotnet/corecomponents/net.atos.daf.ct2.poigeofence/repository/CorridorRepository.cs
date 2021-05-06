using net.atos.daf.ct2.data;
using System;
using System.Collections.Generic;
using net.atos.daf.ct2.poigeofence.entity;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.poigeofence.repository
{
   public class CorridorRepository : ICorridorRepository
    {
        private readonly IDataAccess _dataAccess;
        private static readonly log4net.ILog log =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CorridorCoreMapper _corridorCoreMapper;

        public CorridorRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;

            _corridorCoreMapper = new CorridorCoreMapper();

        }

        public async Task<List<CorridorResponse>> GetCorridorListByOrganization(CorridorRequest objCorridorRequest)
        {
            List<CorridorResponse> objCorridorResponseList = new List<CorridorResponse>();
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
                                ,l.state as State
                                ,l.type as CorridorType
	                            ,l.created_at as Created_At
	                            ,l.created_by as CreatedBy
	                            ,l.modified_at as ModifiedAt
	                            ,l.modified_by as ModifiedBy
                        FROM       master.landmark l
                        LEFT JOIN master.nodes n on l.id = n.landmark_id
                        WHERE      l.type IN ('E','R')
                        AND        l.organization_id = @organization_id";

                parameter.Add("@organization_id", objCorridorRequest.OrganizationId);
                var data = await _dataAccess.QueryAsync<CorridorResponse>(query, parameter);
                return objCorridorResponseList = data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CorridorEditViewResponse>> GetCorridorListByOrgIdAndCorriId(CorridorRequest objCorridorRequest)
        {
            List<CorridorEditViewResponse> objCorridorEditViewResponse1 = new List<CorridorEditViewResponse>();
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
                        WHERE      l.type IN ('E','R')
                        AND        l.organization_id = @organization_id
                        AND        l.id = @id";

                parameter.Add("@organization_id", objCorridorRequest.OrganizationId);
                parameter.Add("@id", objCorridorRequest.CorridorId);
                var data = await _dataAccess.QueryAsync<CorridorEditViewResponse>(query, parameter);
                return objCorridorEditViewResponse1 = data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ViaAddressDetail>> GetCorridorViaStopById(int Id)
        {
            List<ViaAddressDetail> objViaAddressDetailList = new List<ViaAddressDetail>();
            try
            {
                string query = string.Empty; var parameter = new DynamicParameters();
                query = @"select id as CorridorViaStopId
                                 ,name as CorridorViaStopName
                                 ,latitude
                                 ,longitude
                          FROM MASTER.CORRIDORVIASTOP WHERE
                          landmark_id=@landmark_id";

                parameter.Add("@landmark_id", Id);
                var data = await _dataAccess.QueryAsync<ViaAddressDetail>(query, parameter);
                return objViaAddressDetailList = data.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
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

                    var isExist = CheckRouteCorridorIsexist(routeCorridor.CorridorLabel, routeCorridor.OrganizationId, routeCorridor.Id);
                    if (!isExist)
                    {
                        var insertIntoLandmark = @"INSERT INTO master.landmark(
                                          organization_id, category_id, name, address,city, type, Width, state, latitude, longitude, created_at, created_by)
                                            VALUES (@OrganizationId,@category_id, @CorridorLabel, @StartAddress,@City, @CorridorType, @Width, @state, @StartLatitude ,@StartLongitude, @Created_At, @Created_By)RETURNING id";


                        var insertIntoNodes = @"INSERT INTO master.nodes(
                                          landmark_id, state, latitude, longitude, created_at, created_by, address)
                                            VALUES (@LandmarkId, @state, @StartLatitude ,@StartLongitude, @Created_At, @Created_By, @EndAddress) RETURNING id";

                        var insertIntoCorridorProperties = @"INSERT INTO master.corridorproperties(
                                          landmark_id, is_transport_data, is_traffic_flow, no_of_trailers, is_explosive, is_gas, is_flammable, is_combustible, is_organic, is_poison, is_radio_active, is_corrosive, is_poisonous_inhalation, is_warm_harm, is_other, toll_road_type, motorway_type, boat_ferries_type, rail_ferries_type, tunnels_type, dirt_road_type, vehicle_height, vehicle_width, vehicle_length, vehicle_limited_weight, vehicle_weight_per_axle, created_at)
                                           VALUES (@LandmarkId, @TransportData, @TrafficFlow, @Trailer, @Explosive, @Gas, @Flammable, @Combustible, @organic, @poision, @RadioActive, @Corrosive, @PoisonousInhalation, @WaterHarm, @Other, @TollRoad, @Mortorway, @BoatFerries, @RailFerries, @Tunnels, @DirtRoad, @VehicleSizeHeight, @VehicleSizeWidth, @VehicleSizeLength, @VehicleSizeLimitedWeight, @VehicleSizeWeightPerAxle, @Created_At) RETURNING id";


                       


                        parameter.Add("@OrganizationId", routeCorridor.OrganizationId != 0 ? routeCorridor.OrganizationId : null);
                        parameter.Add("@category_id", 50);
                        parameter.Add("@City", "Pune");
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
                        parameter.Add("@organic", routeCorridor.organic);
                        parameter.Add("@poision", routeCorridor.poision);
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
                                          landmark_id, latitude, longitude, name)
                                            VALUES (@LandmarkId, @Latitude, @Longitude ,@ViaStopName) RETURNING id";

                                await _dataAccess.ExecuteScalarAsync<int>(insertIntoCorridorViaStop, parameter);

                            }

                            

                        }
                    }
                    else
                    {
                        routeCorridor.Id = -1;
                    }
                    transactionScope.Complete();

                }
            }
            catch (Exception ex)
            {
                log.Info("AddRouteCorridor method in repository failed :" + Newtonsoft.Json.JsonConvert.SerializeObject(routeCorridor.Id));
                log.Error(ex.ToString());
                // throw ex;
            }
            return routeCorridor;
        }

        private bool CheckRouteCorridorIsexist(string CorridorName, int? OrganizationId, int Id)
        {
            RouteCorridorFilter routeCorridorFilter = new RouteCorridorFilter();
            routeCorridorFilter.CorridorLabel = CorridorName;
            routeCorridorFilter.OrganizationId = OrganizationId;

            var corridores = GetRouteCorridor(routeCorridorFilter);

            var nameExistsForInsert = corridores.Result.Where(t => t.CorridorLabel == CorridorName && t.Id != Id).Count();
            if (nameExistsForInsert == 0)
                return false;
            else if (nameExistsForInsert > 0)
                return true;
            else
                return nameExistsForInsert == 0 ? false : true;
        }

        public async Task<IEnumerable<RouteCorridor>> GetRouteCorridor(RouteCorridorFilter routeCorridorFilter)
        {
            try
            {
                var parameter = new DynamicParameters();
                
                List<RouteCorridor> routeCorridors = new List<RouteCorridor>();
                string getQuery = string.Empty;

                getQuery = @"SELECT id as Id, organization_id as OrganizationId,  name as CorridorLabel, address as StartAddress, type as CorridorType, width as Width, state as State, created_at as Created_At, created_by as Created_By, modified_at as Modified_At, modified_by as Modified_By FROM master.landmark where 1=1 ";

                if (routeCorridorFilter != null)
                {
                    // id filter
                    if (routeCorridorFilter.ID > 0)
                    {
                        parameter.Add("@id", routeCorridorFilter.ID);
                        getQuery = getQuery + " and id=@id ";
                    }
                    // Category Type Filter
                    if (routeCorridorFilter.CorridorType != null)
                    {
                        parameter.Add("@type", routeCorridorFilter.CorridorType);
                        getQuery = getQuery + " and type= @type ";
                    }
                    // Category Name Filter
                    if (!string.IsNullOrEmpty(routeCorridorFilter.CorridorLabel))
                    {
                        parameter.Add("@Name", routeCorridorFilter.CorridorLabel);
                        getQuery = getQuery + " and name= @Name ";
                    }
                    if (routeCorridorFilter.OrganizationId > 0)
                    {
                        //It will return organization specific category/subcategory
                        parameter.Add("@organization_id", routeCorridorFilter.OrganizationId);
                        getQuery = getQuery + " and organization_id=@organization_id  ";
                    }
                    else
                    {
                        //only return global poi
                        getQuery = getQuery + " and organization_id is null ";
                    }
                    parameter.Add("@State", "A");
                    getQuery = getQuery + " and state= @State ";

                    getQuery = getQuery + " ORDER BY id ASC; ";
                    var result = await _dataAccess.QueryAsync<RouteCorridor>(getQuery, parameter);
                    routeCorridors = result.ToList();

                }
                return routeCorridors;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
